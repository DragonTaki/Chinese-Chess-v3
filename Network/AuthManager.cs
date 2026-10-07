/* ----- ----- ----- ----- */
// AuthManager.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/11/01
// Update Date: 2026/10/07
// Version: v1.1
/* ----- ----- ----- ----- */

using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

using Engine.Diagnostics;

using NetClient;

namespace Chinese_Chess_v3.Network
{
    /// <summary>
    /// Runs the two-step login (version check, then credentials) for one connection attempt.
    /// Subscribed to <see cref="NetConnection.EnvelopeReceived"/> only until the attempt finishes
    /// or is abandoned (the connection goes away, or <see cref="Dispose"/>), so a reconnect never
    /// leaves an old instance reacting to the new connection's packets.
    /// </summary>
    public sealed class AuthManager : IDisposable
    {
        /// <summary>The AuthResponse data of an accepted login (anything else is the failure's reason code).</summary>
        public const string AuthSuccessString = "Taki";

        private const string CredentialsRequest = "Please provide username/password";

        private readonly NetConnection _connection;
        private readonly string _version;
        private readonly string _email;
        private readonly string _password;
        private readonly object _lock = new();

        // Continuations run asynchronously: the attempt can be abandoned from inside
        // NetConnection's disconnect, and the awaiting caller must not resume on that stack.
        private readonly TaskCompletionSource<AuthResult> _completion = new(TaskCreationOptions.RunContinuationsAsynchronously);
        private CancellationTokenRegistration _abandonedRegistration;

        // Steps of the two-step authentication (version check, then credentials)
        private enum AuthStep
        {
            None,
            VersionSent,
            CredentialsSent,
            Completed
        }

        private AuthStep _currentStep = AuthStep.None;

        /// <summary>Prepares one login on <paramref name="connection"/>.</summary>
        /// <param name="connection">The open connection to log in on.</param>
        /// <param name="version">The client version the server checks first.</param>
        /// <param name="email">The account's email (sent as the username).</param>
        /// <param name="password">The account's password.</param>
        public AuthManager(NetConnection connection, string version, string email, string password)
        {
            _connection = connection ?? throw new ArgumentNullException(nameof(connection));
            _version = version ?? throw new ArgumentNullException(nameof(version));
            _email = email ?? throw new ArgumentNullException(nameof(email));
            _password = password ?? throw new ArgumentNullException(nameof(password));
        }

        /// <summary>
        /// Sends the version and answers the server's credentials request; completes with the
        /// server's verdict, or with <see cref="AuthResult.ReasonDisconnected"/> when
        /// <paramref name="abandoned"/> fires (or <see cref="Dispose"/> is called) first.
        /// Call once.
        /// </summary>
        /// <param name="abandoned">Cancelled when the connection goes away before the login finished.</param>
        /// <returns>The login's result.</returns>
        public Task<AuthResult> RunAsync(CancellationToken abandoned)
        {
            lock (_lock)
            {
                if (_currentStep != AuthStep.None)
                    throw new InvalidOperationException("The login has already been run.");
                _currentStep = AuthStep.VersionSent;
            }

            _connection.EnvelopeReceived += HandleEnvelope;  // Subscribe to incoming packets (the server's replies)
            _abandonedRegistration = abandoned.Register(() => Finish(AuthResult.Failure(AuthResult.ReasonDisconnected)));

            var versionObj = new
            {
                type = "version",
                senderId = _connection.ClientId.ToString(),
                version = _version
            };
            Send(JsonSerializer.Serialize(versionObj));

            return _completion.Task;
        }

        private void HandleEnvelope(Envelope envelope)
        {
            Packet packet = Packet.FromEnvelope(envelope);

            AuthStep step;
            lock (_lock)
                step = _currentStep;

            if (step == AuthStep.VersionSent)
            {
                // Wait for the server to ask for username/password
                if (packet.Type == PacketType.AuthRequest && packet.Data?.Trim() == CredentialsRequest)
                {
                    if (DebugOptions.ConsoleTrace)
                        Console.WriteLine("[AuthManager] Server requests credentials.");

                    lock (_lock)
                    {
                        if (_currentStep != AuthStep.VersionSent)
                            return;
                        _currentStep = AuthStep.CredentialsSent;
                    }

                    // Step 2: send username/password
                    var credentialsObj = new
                    {
                        type = "credentials",
                        senderId = _connection.ClientId.ToString(),
                        username = _email,
                        password = _password
                    };
                    Send(JsonSerializer.Serialize(credentialsObj));
                }
                else if (packet.Type == PacketType.AuthResponse)
                {
                    // Rejected at the version check (e.g. VersionMismatch)
                    Finish(AuthResult.Failure(packet.Data));
                }
            }
            else if (step == AuthStep.CredentialsSent && packet.Type == PacketType.AuthResponse)
            {
                Finish(packet.Data == AuthSuccessString
                    ? AuthResult.Success(packet.Token)
                    : AuthResult.Failure(packet.Data));
            }
        }

        private void Send(string data)
        {
            var authPacket = Packet.Create(
                type: PacketType.AuthRequest,
                senderId: _connection.ClientId.ToString(),
                roomId: "",
                data: data
            );
            // A failed send closes the connection, which abandons this login.
            _connection.Send(authPacket.ToEnvelope());
        }

        /// <summary>Completes the login once and stops listening.</summary>
        private void Finish(AuthResult result)
        {
            lock (_lock)
            {
                if (_currentStep == AuthStep.Completed)
                    return;
                _currentStep = AuthStep.Completed;
            }

            _connection.EnvelopeReceived -= HandleEnvelope;
            _abandonedRegistration.Dispose();
            _completion.TrySetResult(result);

            if (DebugOptions.ConsoleTrace)
                Console.WriteLine($"[AuthManager] Auth completed → {result}");
        }

        /// <summary>
        /// Stops listening for packets and, if the login hasn't finished, completes it with
        /// <see cref="AuthResult.ReasonDisconnected"/>, so a caller awaiting it doesn't wait forever.
        /// </summary>
        public void Dispose() => Finish(AuthResult.Failure(AuthResult.ReasonDisconnected));
    }
}

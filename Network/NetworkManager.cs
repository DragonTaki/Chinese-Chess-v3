/* ----- ----- ----- ----- */
// NetworkManager.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/11/01
// Update Date: 2026/10/07
// Version: v1.1
/* ----- ----- ----- ----- */

using System;
using System.Threading.Tasks;

using NetClient;
using NetClient.Transport;

namespace Chinese_Chess_v3.Network
{
    /// <summary>
    /// The client's connection to the chess server: a Net-Client <see cref="NetConnection"/> with
    /// the chess protocol on top (packet kinds as <see cref="PacketType"/>, the heartbeat packet,
    /// the login). One long-lived instance.
    /// </summary>
    public class NetworkManager
    {
        /// <summary>The client version the server checks at login.</summary>
        public const string AppVersion = "v1.0.0";

        private readonly NetConnection _connection;

        /// <summary>Creates a disconnected manager for the server at <paramref name="host"/>:<paramref name="port"/>.</summary>
        /// <param name="host">The server's host name or IP address.</param>
        /// <param name="port">The server's TCP port.</param>
        public NetworkManager(string host = "127.0.0.1", int port = 8080)
        {
            var options = new NetConnectionOptions
            {
                HeartbeatType = PacketTypeNames.ToName(PacketType.Heartbeat),
                HeartbeatInterval = TimeSpan.FromSeconds(1),
                HeartbeatTimeout = TimeSpan.FromSeconds(8),
            };
            _connection = new NetConnection(new TcpStreamFactory(host, port), options, new ConsoleNetLog());
            _connection.EnvelopeReceived += envelope => OnPacketReceived?.Invoke(Packet.FromEnvelope(envelope));
            _connection.Disconnected += () => OnDisconnected?.Invoke();
        }

        /// <summary>A random id for this client instance (the sender id it puts in its packets).</summary>
        public Guid ClientId => _connection.ClientId;

        /// <summary>Whether the connection is open (also while logging in).</summary>
        public bool IsConnected => _connection.IsConnected;

        /// <summary>The result of the latest login attempt (including one re-run by <see cref="Reconnect"/>), or null before any.</summary>
        public AuthResult LastAuthResult { get; private set; }

        /// <summary>A packet arrived (heartbeats excluded; login replies included). Raised on a network thread.</summary>
        public event Action<Packet> OnPacketReceived;

        /// <summary>The connection (or the attempt to open it) ended. Raised on a network thread.</summary>
        public event Action OnDisconnected;

        /// <summary>
        /// Opens the connection without logging in. Does nothing while a connection is open or
        /// opening.
        /// </summary>
        /// <returns>True when the connection opened.</returns>
        public Task<bool> ConnectAsync() => _connection.ConnectAsync();

        /// <summary>
        /// Opens the connection and logs in with the account (version check, then credentials);
        /// the heartbeat starts once the server accepts. <see cref="Reconnect"/> logs in again
        /// with the same account. The credentials are kept in memory only, for that.
        /// </summary>
        /// <param name="email">The account's email.</param>
        /// <param name="password">The account's password.</param>
        /// <returns>The login's result (<see cref="AuthResult.ReasonConnectFailed"/> when the server
        /// could not be reached, <see cref="AuthResult.ReasonAlreadyConnected"/> while a connection is open).</returns>
        public async Task<AuthResult> LoginAsync(string email, string password)
        {
            ArgumentNullException.ThrowIfNull(email);
            ArgumentNullException.ThrowIfNull(password);

            if (_connection.IsConnected)
                return AuthResult.Failure(AuthResult.ReasonAlreadyConnected);

            AuthResult result = null;
            await _connection.ConnectAsync(async (connection, abandoned) =>
            {
                using var auth = new AuthManager(connection, AppVersion, email, password);
                AuthResult attempt = await auth.RunAsync(abandoned);
                result = attempt;
                LastAuthResult = attempt;
                return attempt.Ok;
            });

            // Null: the login never ran (the server was unreachable, or another attempt was already opening).
            return result ?? AuthResult.Failure(AuthResult.ReasonConnectFailed);
        }

        /// <summary>Closes the connection and opens it again (logging in again if the last connection did).</summary>
        public void Reconnect() => _connection.Reconnect();

        /// <summary>Closes the connection. Does nothing when disconnected.</summary>
        public void Disconnect() => _connection.Disconnect();

        /// <summary>Sends one packet.</summary>
        /// <param name="packet">The packet.</param>
        /// <returns>True when it was written; false when not connected or the write failed.</returns>
        public bool Send(Packet packet)
        {
            ArgumentNullException.ThrowIfNull(packet);
            return _connection.Send(packet.ToEnvelope());
        }
    }
}

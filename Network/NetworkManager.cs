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
    /// the login). One long-lived instance. The transport is replaceable (TLS to the server by
    /// default, see <see cref="ServerEndpoint"/>; plain TCP for LAN play).
    /// </summary>
    public class NetworkManager
    {
        /// <summary>The client version the server checks at login.</summary>
        public const string AppVersion = "v1.0.0";

        /// <summary>How often a heartbeat is sent outside a game.</summary>
        public static readonly TimeSpan LobbyHeartbeatInterval = TimeSpan.FromSeconds(1);

        /// <summary>
        /// How often a heartbeat is sent while playing (from StartGame until EndGame): the server
        /// drops a player silent for 1 second (its InGameHeartbeatInterval is 300 ms).
        /// </summary>
        public static readonly TimeSpan InGameHeartbeatInterval = TimeSpan.FromMilliseconds(300);

        /// <summary>How long without anything received before the connection counts as lost (the server sends a heartbeat every 3 seconds).</summary>
        public static readonly TimeSpan HeartbeatTimeout = TimeSpan.FromSeconds(8);

        private readonly NetConnection _connection;

        /// <summary>Creates a disconnected manager for the built-in server (<see cref="ServerEndpoint"/>: TLS, address and trust from the environment overrides).</summary>
        public NetworkManager()
            : this(ServerEndpoint.CreateTransport())
        {
        }

        /// <summary>Creates a disconnected manager over <paramref name="transport"/>.</summary>
        /// <param name="transport">Opens the connection (e.g. a <see cref="TlsStreamFactory"/>; a <see cref="TcpStreamFactory"/> for LAN play).</param>
        public NetworkManager(IStreamFactory transport)
        {
            ArgumentNullException.ThrowIfNull(transport);
            var options = new NetConnectionOptions
            {
                HeartbeatType = PacketTypeNames.ToName(PacketType.Heartbeat),
                HeartbeatInterval = LobbyHeartbeatInterval,
                HeartbeatTimeout = HeartbeatTimeout,
            };
            _connection = new NetConnection(transport, options, new ConsoleNetLog());
            _connection.EnvelopeReceived += envelope => OnPacketReceived?.Invoke(Packet.FromEnvelope(envelope));
            _connection.Disconnected += () => OnDisconnected?.Invoke();
        }

        /// <summary>A random id for this client instance (the sender id it puts in its packets).</summary>
        public Guid ClientId => _connection.ClientId;

        /// <summary>The endpoint, for diagnostics.</summary>
        public string Endpoint => _connection.Endpoint;

        /// <summary>Whether the connection is open (also while logging in).</summary>
        public bool IsConnected => _connection.IsConnected;

        /// <summary>The result of the latest login attempt (including one re-run by <see cref="Reconnect"/>), or null before any.</summary>
        public AuthResult LastAuthResult { get; private set; }

        /// <summary>
        /// A login finished, accepted or not: the one of <see cref="LoginAsync"/> and the one each
        /// <see cref="Reconnect"/> runs again. Raised on a network thread, before the heartbeat
        /// starts and before <see cref="LoginAsync"/> returns.
        /// </summary>
        public event Action<AuthResult> LoginCompleted;

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
                try
                {
                    LoginCompleted?.Invoke(attempt);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("[Network] LoginCompleted handler error: " + ex.Message);
                }
                return attempt.Ok;
            });

            // Null: the login never ran (the server was unreachable, or another attempt was already opening).
            return result ?? AuthResult.Failure(AuthResult.ReasonConnectFailed);
        }

        /// <summary>Closes the connection and opens it again (logging in again if the last connection did).</summary>
        public void Reconnect() => _connection.Reconnect();

        /// <summary>Closes the connection. Does nothing when disconnected.</summary>
        public void Disconnect() => _connection.Disconnect();

        /// <summary>
        /// Changes how often a heartbeat is sent (<see cref="LobbyHeartbeatInterval"/>,
        /// <see cref="InGameHeartbeatInterval"/>); applies at once and to later connections.
        /// </summary>
        /// <param name="interval">The new interval (positive).</param>
        public void SetHeartbeatInterval(TimeSpan interval) => _connection.SetHeartbeatInterval(interval);

        /// <summary>How often a heartbeat is sent now.</summary>
        public TimeSpan CurrentHeartbeatInterval => _connection.HeartbeatInterval;

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

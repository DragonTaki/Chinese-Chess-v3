/* ----- ----- ----- ----- */
// OnlineChannel.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/07
// Update Date: 2026/10/07
// Version: v1.0
/* ----- ----- ----- ----- */

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Chinese_Chess_v3.Network.Protocol;

namespace Chinese_Chess_v3.Network
{
    /// <summary>
    /// The typed online API over a <see cref="NetworkManager"/>: requests as methods (rooms, ready,
    /// moves, resign), the server's packets as typed events. It also keeps the heartbeat at the
    /// pace the server expects: <see cref="NetworkManager.InGameHeartbeatInterval"/> from
    /// <see cref="GameStarted"/> until <see cref="GameEnded"/> or a disconnect, otherwise
    /// <see cref="NetworkManager.LobbyHeartbeatInterval"/>.
    /// <para>
    /// Threads: every event is raised on a network thread, never the UI's. Packet events
    /// (<see cref="RoomList"/> to <see cref="ServerError"/>) come from the connection's reader, one
    /// at a time in arrival order; <see cref="LoggedIn"/> / <see cref="LoginFailed"/> from the
    /// login (also the one a reconnect runs again); <see cref="Disconnected"/> from whichever thread
    /// closed the connection. A UI marshals them to its own thread. A handler that throws is logged
    /// and does not affect the connection.
    /// </para>
    /// The server is authoritative: nothing here decides a move's legality, a clock or a result;
    /// the client only shows what the events report.
    /// </summary>
    public sealed class OnlineChannel : IDisposable
    {
        private readonly NetworkManager _network;
        private int _seq;
        private int _inGame;  // 1 between GameStarted and GameEnded / disconnect
        private bool _disposed;

        /// <summary>Wraps <paramref name="network"/> (subscribes to it until <see cref="Dispose"/>).</summary>
        /// <param name="network">The connection to the server.</param>
        public OnlineChannel(NetworkManager network)
        {
            _network = network ?? throw new ArgumentNullException(nameof(network));
            _network.OnPacketReceived += HandlePacket;
            _network.OnDisconnected += HandleDisconnected;
            _network.LoginCompleted += HandleLoginCompleted;
        }

        /// <summary>The underlying connection.</summary>
        public NetworkManager Network => _network;

        /// <summary>Whether a game is running for this client (between <see cref="GameStarted"/> and <see cref="GameEnded"/>).</summary>
        public bool IsInGame => Volatile.Read(ref _inGame) == 1;

        // ----- Events -----

        /// <summary>The server accepted the login (also after a reconnect).</summary>
        public event Action LoggedIn;

        /// <summary>The login failed: the server's reason code (e.g. InvalidCredentials, VersionMismatch) or a client-side one (<see cref="AuthResult"/>).</summary>
        public event Action<string> LoginFailed;

        /// <summary>The waiting rooms (answer to <see cref="RequestRoomList"/>).</summary>
        public event Action<IReadOnlyList<RoomStateDto>> RoomList;

        /// <summary>The current state of the room this client is in (after create, join, leave of anyone, ready, game start / end).</summary>
        public event Action<RoomStateDto> RoomState;

        /// <summary>A game started (or was resumed after a new login).</summary>
        public event Action<StartGameDto> GameStarted;

        /// <summary>A move was applied (anyone's; refusals are <see cref="ActionRejected"/>).</summary>
        public event Action<GameUpdateDto> GameUpdated;

        /// <summary>One of this client's actions was refused: its seq and the reason (e.g. NotYourTurn, IllegalMove).</summary>
        public event Action<int, string> ActionRejected;

        /// <summary>The server's clocks (every few seconds, and when a player goes away).</summary>
        public event Action<ClocksDto> TimerSynced;

        /// <summary>The game ended.</summary>
        public event Action<EndGameDto> GameEnded;

        /// <summary>The server refused a request: the error code (e.g. RoomNotFound, InvalidSettings) and the detail ("" when none).</summary>
        public event Action<string, string> ServerError;

        /// <summary>The connection ended.</summary>
        public event Action Disconnected;

        // ----- Requests -----

        /// <summary>Connects and logs in (see <see cref="NetworkManager.LoginAsync"/>); also raises <see cref="LoggedIn"/> or <see cref="LoginFailed"/>.</summary>
        /// <param name="email">The account's email.</param>
        /// <param name="password">The account's password.</param>
        /// <returns>The login's result.</returns>
        public async Task<AuthResult> LoginAsync(string email, string password)
        {
            AuthResult result = await _network.LoginAsync(email, password).ConfigureAwait(false);
            // A login that never ran (unreachable, already connected) raised nothing yet.
            if (!result.Ok && (result.Reason == AuthResult.ReasonConnectFailed || result.Reason == AuthResult.ReasonAlreadyConnected))
                Raise(LoginFailed, result.Reason);
            return result;
        }

        /// <summary>Closes the connection.</summary>
        public void Disconnect() => _network.Disconnect();

        /// <summary>Asks for the waiting rooms (answered by <see cref="RoomList"/>).</summary>
        /// <returns>True when the request was sent.</returns>
        public bool RequestRoomList() => Send(PacketType.RoomList, "");

        /// <summary>Creates a room and takes its first seat (answered by <see cref="RoomState"/> or <see cref="ServerError"/>).</summary>
        /// <param name="settings">The room's settings.</param>
        /// <returns>True when the request was sent.</returns>
        public bool CreateRoom(RoomSettingsDto settings)
        {
            ArgumentNullException.ThrowIfNull(settings);
            return Send(PacketType.CreateRoom, ProtocolJson.Serialize(settings));
        }

        /// <summary>Takes a free seat in a room (answered by <see cref="RoomState"/> or <see cref="ServerError"/>).</summary>
        /// <param name="roomId">The room number.</param>
        /// <returns>True when the request was sent.</returns>
        public bool JoinRoom(string roomId)
        {
            ArgumentNullException.ThrowIfNull(roomId);
            return Send(PacketType.JoinRoom, ProtocolJson.Serialize(new JoinRoomDto { RoomId = roomId }));
        }

        /// <summary>Leaves the room (during a game this resigns it).</summary>
        /// <returns>True when the request was sent.</returns>
        public bool LeaveRoom() => Send(PacketType.LeaveRoom, "");

        /// <summary>Sets this player's ready mark; the game starts when every seat is taken and ready.</summary>
        /// <param name="ready">Ready or not.</param>
        /// <returns>True when the request was sent.</returns>
        public bool SetReady(bool ready) => Send(PacketType.Ready, ProtocolJson.Serialize(new ReadyDto { Ready = ready }));

        /// <summary>
        /// Asks to move the piece on (<paramref name="fromX"/>, <paramref name="fromY"/>) to
        /// (<paramref name="toX"/>, <paramref name="toY"/>) (FEN coordinates, see
        /// <see cref="PieceDto"/>). Answered by <see cref="GameUpdated"/> with the returned seq,
        /// or <see cref="ActionRejected"/>.
        /// </summary>
        /// <returns>The action's seq, or -1 when it could not be sent.</returns>
        public int SendMove(int fromX, int fromY, int toX, int toY)
        {
            int seq = Interlocked.Increment(ref _seq);
            var action = new GameActionDto { Seq = seq, Type = GameActionDto.TypeMove, From = new[] { fromX, fromY }, To = new[] { toX, toY } };
            return Send(PacketType.GameAction, ProtocolJson.Serialize(action)) ? seq : -1;
        }

        /// <summary>Resigns the game (answered by <see cref="GameEnded"/>, or <see cref="ActionRejected"/> when there is none).</summary>
        /// <returns>The action's seq, or -1 when it could not be sent.</returns>
        public int Resign()
        {
            int seq = Interlocked.Increment(ref _seq);
            var action = new GameActionDto { Seq = seq, Type = GameActionDto.TypeResign };
            return Send(PacketType.GameAction, ProtocolJson.Serialize(action)) ? seq : -1;
        }

        /// <summary>Stops listening to the connection (it stays open).</summary>
        public void Dispose()
        {
            if (_disposed)
                return;
            _disposed = true;
            _network.OnPacketReceived -= HandlePacket;
            _network.OnDisconnected -= HandleDisconnected;
            _network.LoginCompleted -= HandleLoginCompleted;
        }

        // ----- Internals -----

        private bool Send(PacketType type, string data)
            => _network.Send(Packet.Create(type, _network.ClientId.ToString(), "", data));

        private void HandleLoginCompleted(AuthResult result)
        {
            if (result.Ok)
                Raise(LoggedIn);
            else
                Raise(LoginFailed, result.Reason);
        }

        private void HandleDisconnected()
        {
            SetInGame(false);
            Raise(Disconnected);
        }

        private void HandlePacket(Packet packet)
        {
            switch (packet.Type)
            {
                case PacketType.RoomList:
                    if (Parse(packet, out List<RoomStateDto> rooms))
                        Raise(RoomList, (IReadOnlyList<RoomStateDto>)rooms);
                    break;
                case PacketType.RoomState:
                    if (Parse(packet, out RoomStateDto room))
                        Raise(RoomState, room);
                    break;
                case PacketType.StartGame:
                    if (Parse(packet, out StartGameDto start))
                    {
                        // Faster first: the server drops a player silent for 1 s from now on.
                        SetInGame(true);
                        Raise(GameStarted, start);
                    }
                    break;
                case PacketType.GameUpdate:
                    if (Parse(packet, out GameUpdateDto update))
                    {
                        if (update.IsRejected)
                            Raise(ActionRejected, update.Seq, update.Rejected);
                        else
                            Raise(GameUpdated, update);
                    }
                    break;
                case PacketType.TimerSync:
                    if (Parse(packet, out ClocksDto clocks))
                        Raise(TimerSynced, clocks);
                    break;
                case PacketType.EndGame:
                    if (Parse(packet, out EndGameDto end))
                    {
                        SetInGame(false);
                        Raise(GameEnded, end);
                    }
                    break;
                case PacketType.Error:
                    SplitError(packet.Data, out string code, out string detail);
                    Raise(ServerError, code, detail);
                    break;
                default:
                    // Login replies (AuthManager's), the welcome, chat...: not this API's.
                    break;
            }
        }

        /// <summary>Switches the heartbeat pace when the game state changes.</summary>
        private void SetInGame(bool inGame)
        {
            int value = inGame ? 1 : 0;
            if (Interlocked.Exchange(ref _inGame, value) == value)
                return;
            _network.SetHeartbeatInterval(inGame ? NetworkManager.InGameHeartbeatInterval : NetworkManager.LobbyHeartbeatInterval);
        }

        /// <summary>Splits an Error packet's data "Code: detail" (the detail is "" when there is none).</summary>
        /// <param name="data">The data.</param>
        /// <param name="code">The error code.</param>
        /// <param name="detail">The detail.</param>
        public static void SplitError(string data, out string code, out string detail)
        {
            data ??= "";
            int colon = data.IndexOf(": ", StringComparison.Ordinal);
            if (colon < 0)
            {
                code = data.Trim();
                detail = "";
            }
            else
            {
                code = data[..colon].Trim();
                detail = data[(colon + 2)..].Trim();
            }
        }

        private static bool Parse<T>(Packet packet, out T value)
        {
            if (ProtocolJson.TryDeserialize(packet.Data, out value))
                return true;
            Console.WriteLine($"[Network] Unreadable {packet.Type} payload, ignored: {packet.Data}");
            return false;
        }

        private static void Raise(Action handler) => Guard(() => handler?.Invoke(), "event");

        private static void Raise<T>(Action<T> handler, T arg) => Guard(() => handler?.Invoke(arg), typeof(T).Name);

        private static void Raise<T1, T2>(Action<T1, T2> handler, T1 arg1, T2 arg2) => Guard(() => handler?.Invoke(arg1, arg2), typeof(T1).Name);

        private static void Guard(Action call, string what)
        {
            try
            {
                call();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Network] OnlineChannel handler error ({what}): {ex.Message}");
            }
        }
    }
}

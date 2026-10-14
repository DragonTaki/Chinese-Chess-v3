/* ----- ----- ----- ----- */
// LanSession.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/12
// Update Date: 2026/10/13
// Version: v1.2
/* ----- ----- ----- ----- */

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

using Chinese_Chess_v3.Game.Core;
using Chinese_Chess_v3.Game.Core.Pieces;
using Chinese_Chess_v3.Game.Core.Players;
using Chinese_Chess_v3.Network.Lan;
using Chinese_Chess_v3.Network.Protocol;

using Engine.Timing;

namespace Chinese_Chess_v3.Game.Application.Online
{
    /// <summary>
    /// Account-free LAN rooms. The host owns a separate Core game, validates both players'
    /// intents, and publishes the same DTOs the existing remote board consumes. Public commands
    /// and events run on the main thread; peer events are posted through the main-thread queue.
    /// </summary>
    public sealed class LanSession : IDisposable
    {
        private readonly IMainThreadQueue _mainThread;
        private readonly LanPeer _peer = new();
        private readonly LanDiscovery _discovery = new();
        private CancellationTokenSource _lifetime;
        private GameManager _authority;
        private string _nickname;
        private string _roomId;
        private string _gameId;
        private int _generation;
        private int _seq;
        private int _lastHostSeq;
        private int _lastGuestSeq;
        private int _hostSide;
        private int _ticks;
        private bool _joined;
        private bool _disposed;

        private sealed record Hello
        {
            public string Protocol { get; init; }
            public string RoomId { get; init; }
            public string Nickname { get; init; }
        }

        public LanSession(IMainThreadQueue mainThread)
        {
            _mainThread = mainThread ?? throw new ArgumentNullException(nameof(mainThread));
            _peer.Connected += () => Post(OnConnected);
            _peer.Received += message => Post(() => OnReceived(message));
            _peer.Disconnected += () => Post(OnDisconnected);
            _peer.Failed += error => Post(() => Failed?.Invoke(error.Message));
            _discovery.Failed += error => Post(() => Failed?.Invoke(error.Message));
        }

        public bool IsHost { get; private set; }
        public RoomStateDto Room { get; private set; }
        public StartGameDto Game { get; private set; }
        public string LocalId => IsHost ? "host" : "guest";
        public bool IsConnected => _peer.IsConnected && _joined;

        public event Action<RoomStateDto> RoomChanged;
        public event Action<StartGameDto> GameStarted;
        public event Action<GameUpdateDto> GameUpdated;
        public event Action<ClocksDto> TimerSynced;
        public event Action<EndGameDto> GameEnded;
        public event Action Disconnected;
        public event Action<string> Failed;

        /// <summary>Creates a room on this machine, using a nickname without logging in.</summary>
        public void Create(string nickname, RoomSettingsDto settings)
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
            nickname = LanProtocol.ValidateNickname(nickname);
            ValidateSettings(settings);
            Stop();
            IsHost = true;
            _nickname = nickname;
            _roomId = RandomNumberGenerator.GetInt32(100000, 1000000).ToString(CultureInfo.InvariantCulture);
            _lifetime = new CancellationTokenSource();
            Room = new RoomStateDto
            {
                RoomId = _roomId,
                HostId = "host",
                Settings = settings with { Rules = settings.Rules == null ? null : new Dictionary<string, bool>(settings.Rules) },
                State = RoomNames.StateWaiting,
                Seats = new List<SeatDto> { new() { Id = "host", Name = nickname }, null },
            };
            try
            {
                _peer.Host();
                Advertise();
                _ = TickAsync(_generation, _lifetime.Token);
            }
            catch { Stop(); throw; }
            RoomChanged?.Invoke(Room);
        }

        /// <summary>Connects to a selected nearby room and exchanges nicknames.</summary>
        public async Task JoinAsync(string nickname, LanHost host, CancellationToken token = default)
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
            nickname = LanProtocol.ValidateNickname(nickname);
            ArgumentNullException.ThrowIfNull(host);
            if (string.IsNullOrWhiteSpace(host.RoomId))
                throw new ArgumentException("The selected host has no room identifier.", nameof(host));
            Stop();
            IsHost = false;
            _nickname = nickname;
            _roomId = host.RoomId;
            _lifetime = new CancellationTokenSource();
            await _peer.JoinAsync(host.Endpoint, token);
        }

        public Task<IReadOnlyList<LanHost>> DiscoverAsync(CancellationToken token = default) =>
            LanDiscovery.FindAsync(TimeSpan.FromSeconds(2), token);

        /// <summary>Changes waiting-room settings on the host and clears both ready marks.</summary>
        public bool UpdateSettings(RoomSettingsDto settings)
        {
            if (!IsHost || Room == null || Game != null)
                return false;
            ValidateSettings(settings);
            Room = Room with
            {
                Settings = settings with { Rules = settings.Rules == null ? null : new Dictionary<string, bool>(settings.Rules) },
                Seats = Room.Seats.Select(seat => seat == null ? null : seat with { Ready = false }).ToList(),
            };
            PublishRoom();
            return true;
        }

        /// <summary>Marks a seat ready; the host starts when both seats are ready.</summary>
        public bool SetReady(bool ready)
        {
            if (Room == null || Game != null || !_joined)
                return false;
            if (!IsHost)
                return _peer.Send(LanMessage.Create("Ready", new ReadyDto { Ready = ready }));
            ApplyReady(0, ready);
            return true;
        }

        public int SendMove(int fromX, int fromY, int toX, int toY) => SendAction(new GameActionDto
        {
            Seq = ++_seq,
            Type = GameActionDto.TypeMove,
            From = new[] { fromX, fromY },
            To = new[] { toX, toY },
        });

        public int Resign() => SendAction(new GameActionDto { Seq = ++_seq, Type = GameActionDto.TypeResign });

        /// <summary>Resigns an active game and flushes its packets before closing the room.</summary>
        public void Leave()
        {
            int generation = _generation;
            if (Game != null)
                Resign();
            _ = FinishLeaveAsync(generation);
        }

        private async Task FinishLeaveAsync(int generation)
        {
            using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(2));
            try { await _peer.FlushAsync(timeout.Token); }
            catch (OperationCanceledException) { }
            _mainThread.Post(() =>
            {
                if (generation != _generation)
                    return;
                Stop();
                RoomChanged?.Invoke(null);
            });
        }

        /// <summary>Leaves and closes all LAN sockets. Hosting does not create an external server process.</summary>
        public void Stop()
        {
            _generation++;
            _lifetime?.Cancel();
            _lifetime?.Dispose();
            _lifetime = null;
            _discovery.StopAdvertising();
            _peer.Stop();
            _authority = null;
            _joined = false;
            _seq = _lastHostSeq = _lastGuestSeq = _ticks = 0;
            Room = null;
            Game = null;
        }

        public void Dispose()
        {
            if (_disposed)
                return;
            Stop();
            _disposed = true;
            _peer.Dispose();
            _discovery.Dispose();
        }

        private void OnConnected()
        {
            _joined = false;
            _discovery.StopAdvertising();
            if (!IsHost)
                _peer.Send(LanMessage.Create("Hello", new Hello
                {
                    Protocol = LanProtocol.Identity,
                    RoomId = _roomId,
                    Nickname = _nickname,
                }));
            int generation = _generation;
            _ = Task.Delay(TimeSpan.FromSeconds(5)).ContinueWith(_ => _mainThread.Post(() =>
            {
                if (generation == _generation && !_joined && _peer.IsConnected)
                    _peer.DisconnectPeer();
            }), TaskScheduler.Default);
        }

        private void OnReceived(LanMessage message)
        {
            try
            {
                if (IsHost)
                {
                    switch (message.Type)
                    {
                        case "Hello":
                            if (_joined || Room == null || Game != null)
                                return;
                            Hello hello = message.Read<Hello>();
                            if (hello == null || hello.Protocol != LanProtocol.Identity || hello.RoomId != _roomId)
                            {
                                _peer.DisconnectPeer();
                                return;
                            }
                            string nickname = LanProtocol.ValidateNickname(hello.Nickname);
                            _joined = true;
                            Room = Room with { Seats = new List<SeatDto>
                            {
                                Room.Seats[0] with { Ready = false }, new() { Id = "guest", Name = nickname },
                            } };
                            PublishRoom();
                            break;
                        case "Ready" when _joined && Game == null:
                            ReadyDto ready = message.Read<ReadyDto>();
                            if (ready != null)
                                ApplyReady(1, ready.Ready);
                            break;
                        case "GameAction" when _joined:
                            GameActionDto action = message.Read<GameActionDto>();
                            if (action != null)
                                ApplyAction(action, host: false);
                            break;
                    }
                    return;
                }

                switch (message.Type)
                {
                    case "RoomState":
                        RoomStateDto room = message.Read<RoomStateDto>();
                        if (room?.RoomId != _roomId || room.Seats?.Count != 2 || room.Seats[1]?.Id != "guest")
                            throw new ArgumentException("The host sent an invalid room.");
                        ValidateSettings(room.Settings);
                        Room = room;
                        _joined = true;
                        RoomChanged?.Invoke(Room);
                        break;
                    case "StartGame" when _joined:
                        Game = message.Read<StartGameDto>();
                        if (Game == null || Game.YourSide is < 1 or > 2 || Game.Position == null)
                            throw new ArgumentException("The host sent an invalid game.");
                        GameStarted?.Invoke(Game);
                        break;
                    case "GameUpdate" when Game != null:
                        GameUpdateDto update = message.Read<GameUpdateDto>();
                        if (update != null)
                            GameUpdated?.Invoke(update);
                        break;
                    case "TimerSync" when Game != null:
                        ClocksDto clocks = message.Read<ClocksDto>();
                        if (clocks != null)
                            TimerSynced?.Invoke(clocks);
                        break;
                    case "EndGame" when Game != null:
                        EndGameDto end = message.Read<EndGameDto>();
                        if (end?.GameId == Game.GameId)
                        {
                            Game = null;
                            GameEnded?.Invoke(end);
                        }
                        break;
                }
            }
            catch (Exception ex) when (ex is JsonException or ArgumentException or InvalidOperationException)
            {
                Failed?.Invoke(ex.Message);
                _peer.DisconnectPeer();
            }
        }

        private void ApplyReady(int seat, bool ready)
        {
            var seats = new List<SeatDto>(Room.Seats);
            seats[seat] = seats[seat] with { Ready = ready };
            Room = Room with { Seats = seats };
            PublishRoom();
            if (seats.All(s => s != null && s.Ready))
                StartGame();
        }

        private void StartGame()
        {
            _hostSide = Room.Settings.FirstMover switch
            {
                RoomNames.FirstMoverHost => 1,
                RoomNames.FirstMoverGuest => 2,
                _ => Random.Shared.Next(1, 3),
            };
            _gameId = Guid.NewGuid().ToString("N");
            var start = new StartGameDto
            {
                GameId = _gameId,
                Kind = Room.Settings.Kind,
                Rules = Room.Settings.Rules,
                Timer = Room.Settings.Timer,
                YourSide = _hostSide,
                Players = _hostSide == 1 ? new List<SeatDto>(Room.Seats) : new List<SeatDto> { Room.Seats[1], Room.Seats[0] },
            };
            _authority = new GameManager(OnlineRoomSettings.RulesFrom(start));
            _authority.SetPlayerNames(start.Players[0].Name, start.Players[1].Name, "");
            _lastHostSeq = _lastGuestSeq = 0;
            _seq = 0;
            Game = start with { Position = Position(), Clocks = Clocks() };
            Room = Room with { State = RoomNames.StatePlaying };
            PublishRoom();
            if (!_peer.Send(LanMessage.Create("StartGame", Game with { YourSide = 3 - _hostSide })))
            {
                OnDisconnected();
                return;
            }
            GameStarted?.Invoke(Game);
        }

        private int SendAction(GameActionDto action)
        {
            if (Game == null || !_joined)
                return -1;
            if (IsHost)
                ApplyAction(action, host: true);
            else if (!_peer.Send(LanMessage.Create("GameAction", action)))
                return -1;
            return action.Seq;
        }

        private void ApplyAction(GameActionDto action, bool host)
        {
            ref int lastSeq = ref (host ? ref _lastHostSeq : ref _lastGuestSeq);
            string rejected = null;
            int side = host ? _hostSide : 3 - _hostSide;
            if (action.Seq <= lastSeq || action.Seq <= 0)
                rejected = "InvalidSequence";
            else
            {
                lastSeq = action.Seq;
                if (_authority == null || Game == null || _authority.IsGameOver)
                    rejected = "NoGame";
                else
                {
                    _authority.UpdateTimers();
                    if (_authority.IsGameOver)
                    {
                        EndGame(_authority.Winner, _authority.Result.Reason.ToString());
                        rejected = "NoGame";
                    }
                    else if (action.Type == GameActionDto.TypeResign)
                    {
                        _authority.Resign((PlayerSide)side);
                        EndGame(_authority.Winner, "Resign");
                        return;
                    }
                    else if (action.Type != GameActionDto.TypeMove || !Square(action.From) || !Square(action.To))
                        rejected = "InvalidAction";
                    else if ((int)_authority.CurrentTurn != side)
                        rejected = "NotYourTurn";
                    else if (!_authority.TryMove(action.From[0], action.From[1], action.To[0], action.To[1]))
                        rejected = "IllegalMove";
                }
            }
            if (rejected != null)
            {
                var refusal = new GameUpdateDto { Seq = action.Seq, Rejected = rejected };
                if (host)
                    GameUpdated?.Invoke(refusal);
                else
                    _peer.Send(LanMessage.Create("GameUpdate", refusal));
                return;
            }

            var move = _authority.LastMove;
            var update = new GameUpdateDto
            {
                Seq = action.Seq,
                Ply = _authority.Moves.Count,
                Mover = side,
                From = action.From,
                To = action.To,
                Position = Position(),
                Check = move.GivesCheck,
                Clocks = Clocks(),
                Record = new MoveRecordDto { Kind = RoomNames.KindTraditional, Notation = move.Notation, Captured = Piece(move.Captured) },
            };
            _peer.Send(LanMessage.Create("GameUpdate", update));
            GameUpdated?.Invoke(update);
            if (_authority.IsGameOver)
                EndGame(_authority.Winner, _authority.Result.Reason.ToString());
        }

        private void OnDisconnected()
        {
            _joined = false;
            if (IsHost && Room != null)
            {
                if (Game != null)
                    EndGame((PlayerSide)_hostSide, "Disconnect");
                Room = Room with
                {
                    State = RoomNames.StateWaiting,
                    Seats = new List<SeatDto> { Room.Seats[0] with { Ready = false }, null },
                };
                Advertise();
                RoomChanged?.Invoke(Room);
            }
            else
            {
                if (Game != null)
                {
                    // A guest cannot invent a winner after losing contact with the authority.
                    var end = new EndGameDto { GameId = Game.GameId, Winner = 0, Reason = "Disconnect" };
                    Game = null;
                    GameEnded?.Invoke(end);
                }
                Room = null;
                RoomChanged?.Invoke(null);
            }
            Disconnected?.Invoke();
        }

        private void EndGame(PlayerSide winner, string reason)
        {
            var end = new EndGameDto { GameId = _gameId, Winner = (int)winner, Reason = reason };
            Game = null;
            _authority = null;
            _peer.Send(LanMessage.Create("EndGame", end));
            GameEnded?.Invoke(end);
            Room = Room with
            {
                State = RoomNames.StateWaiting,
                Seats = Room.Seats.Select(s => s == null ? null : s with { Ready = false }).ToList(),
            };
            PublishRoom();
        }

        private void PublishRoom()
        {
            _peer.Send(LanMessage.Create("RoomState", Room));
            RoomChanged?.Invoke(Room);
        }

        private void Advertise() => _discovery.Advertise(_roomId, _nickname, _peer.Port);

        private async Task TickAsync(int generation, CancellationToken token)
        {
            using var timer = new PeriodicTimer(TimeSpan.FromMilliseconds(100));
            try
            {
                while (await timer.WaitForNextTickAsync(token))
                    _mainThread.Post(() =>
                    {
                        if (generation != _generation || _authority == null || Game == null)
                            return;
                        _authority.UpdateTimers();
                        if (_authority.IsGameOver)
                        {
                            EndGame(_authority.Winner, _authority.Result.Reason.ToString());
                            return;
                        }
                        if (++_ticks % 10 == 0)
                        {
                            ClocksDto clocks = Clocks();
                            _peer.Send(LanMessage.Create("TimerSync", clocks));
                            TimerSynced?.Invoke(clocks);
                        }
                    });
            }
            catch (OperationCanceledException) when (token.IsCancellationRequested) { }
        }

        private PositionDto Position() => new()
        {
            ToMove = (int)_authority.CurrentTurn,
            Pieces = _authority.Board.GetAllPieces().Where(p => !p.CurrentInfo.IsDead).Select(p => Piece(p.CurrentInfo)).ToList(),
        };

        private ClocksDto Clocks() => new()
        {
            ToMove = (int)_authority.CurrentTurn - 1,
            Away = new List<bool> { false, false },
            Sides = new[] { _authority.Player1, _authority.Player2 }.Select(p => new ClockSideDto
            {
                UsedMs = (long)p.Timer.CurrentTotalTime.TotalMilliseconds,
                StepMs = (long)p.Timer.CurrentStepTime.TotalMilliseconds,
            }).ToList(),
        };

        private static PieceDto Piece(PieceInfo piece) => piece == null ? null : new PieceDto
        {
            Type = piece.Type.ToString(), Color = piece.Color.ToString(), Side = (int)piece.Side,
            X = piece.X, Y = piece.Y, FaceUp = piece.IsFaceUp,
        };

        private static bool Square(int[] square) => square?.Length == 2 && square[0] >= 0 && square[0] < 9 && square[1] >= 0 && square[1] < 10;

        private static void ValidateSettings(RoomSettingsDto settings)
        {
            ArgumentNullException.ThrowIfNull(settings);
            if (settings.Kind != RoomNames.KindTraditional || settings.Mode != RoomNames.ModeCustom)
                throw new ArgumentException("Only traditional custom LAN rooms are supported.", nameof(settings));
            if (settings.FirstMover is not (RoomNames.FirstMoverHost or RoomNames.FirstMoverGuest or RoomNames.FirstMoverRandom))
                throw new ArgumentException("Unknown first mover.", nameof(settings));
            TimerSettingsDto timer = settings.Timer;
            if (timer == null || timer.Mode is not (RoomNames.TimerCountUp or RoomNames.TimerCountDown)
                || timer.TotalMinutes is < 1 or > 600 || timer.StepSeconds is < 1 or > 3600 || timer.IncrementSeconds is < 0 or > 600)
                throw new ArgumentException("Invalid LAN timer settings.", nameof(settings));
            string[] rules = { "canGeneralSeeGeneral", "canGeneralLeavePalace", "canAdvisorLeavePalace", "canElephantEyeBlocked", "canHorseLegHobbled", "canCaptureOwnPiece" };
            if (settings.Rules != null && settings.Rules.Keys.Any(key => !rules.Contains(key)))
                throw new ArgumentException("Unknown LAN rule switch.", nameof(settings));
        }

        private void Post(Action action)
        {
            int generation = _generation;
            _mainThread.Post(() =>
            {
                if (!_disposed && generation == _generation)
                    action();
            });
        }
    }
}

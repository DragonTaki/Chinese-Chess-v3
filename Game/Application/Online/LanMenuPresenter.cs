/* ----- ----- ----- ----- */
// LanMenuPresenter.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/13
// Update Date: 2026/10/13
// Version: v1.1
/* ----- ----- ----- ----- */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

using Chinese_Chess_v3.Game.Application.Settings;
using Chinese_Chess_v3.Game.Application.Texts;
using Chinese_Chess_v3.Game.Configs;
using Chinese_Chess_v3.Game.Core;
using Chinese_Chess_v3.Game.Core.Players;
using Chinese_Chess_v3.Network.Lan;
using Chinese_Chess_v3.Network.Protocol;

using Engine.Localization;
using Engine.Logging;
using Engine.Timing;

namespace Chinese_Chess_v3.Game.Application.Online
{
    /// <summary>A nearby host's menu row and its join command.</summary>
    public sealed record LanRoomRow(string Key, string Text, Action Join);

    /// <summary>Nickname, nearby rooms, host-only waiting-room settings and seats inside the Multiplayer panel.</summary>
    public sealed class LanMenuPresenter : IDisposable
    {
        private static readonly OnlineFirstMover[] FirstMovers = { OnlineFirstMover.Random, OnlineFirstMover.Host, OnlineFirstMover.Guest };
        private readonly LanSession _lan;
        private readonly LanGameBridge _bridge;
        private readonly OnlineSession _online;
        private readonly IMainThreadQueue _mainThread;
        private CancellationTokenSource _search;
        private Timer _refresh;
        private IReadOnlyList<LanHost> _hosts = Array.Empty<LanHost>();
        private IReadOnlyList<SettingsMenuSection> _sections;
        private int _generation;
        private bool _joining;
        private bool _leaving;
        private bool _disposed;
        private IReadOnlyList<SeatDto> _players = Array.Empty<SeatDto>();

        public LanMenuPresenter(LanSession lan, LanGameBridge bridge, OnlineSession online,
            IMainThreadQueue mainThread, PlayerNameSettings names)
        {
            _lan = lan ?? throw new ArgumentNullException(nameof(lan));
            _bridge = bridge ?? throw new ArgumentNullException(nameof(bridge));
            _online = online ?? throw new ArgumentNullException(nameof(online));
            _mainThread = mainThread ?? throw new ArgumentNullException(nameof(mainThread));
            ArgumentNullException.ThrowIfNull(names);
            Nickname = names.PlayerName;
            _lan.RoomChanged += OnRoom;
            _lan.GameEnded += OnEnd;
            _lan.GameStarted += OnStart;
            _lan.Failed += OnFailure;
            _lan.Disconnected += OnDisconnect;
            _bridge.RoomRequested += ShowRoom;
        }

        public string Nickname { get; set; }
        public string RoomAddress { get; set; } = "";
        public string RoomAddressLabel => LanTexts.RoomAddress;
        public bool CanEditSettings => HasRoom && _lan.IsHost && _lan.Game == null;
        public bool IsShown { get; private set; }
        public bool IsSearching => _search != null;
        public bool IsBusy => _joining || _leaving || IsSearching;
        public bool HasRoom => _lan.Room != null;
        public bool CanCreate => !HasRoom && !_joining && !_leaving;
        public string ErrorSentence { get; private set; }
        public string LastResultSentence { get; private set; }
        public string TitleText => LanTexts.Title;
        public string NicknameLabel => LanTexts.Nickname;
        public string SearchText => LanTexts.Search;
        public string CreateText => OnlineTexts.CreateRoom;
        public string JoinText => OnlineTexts.Join;
        public string NearbyTitle => LanTexts.NearbyRooms;
        public string NewRoomTitle => LanTexts.RoomSettings;
        public string EmptyText => _hosts.Count == 0 && !IsSearching ? LanTexts.NoRooms : null;
        public string StatusSentence => IsSearching ? LanTexts.Searching : _joining ? OnlineTexts.Connecting : null;
        public string RoomTitle => LanTexts.RoomNumber(_lan.Room?.RoomId ?? "", _lan.Room?.Seats?[0]?.Name ?? "");
        public string RoomSummary => _lan.Room == null ? null : OnlineTexts.ModeName(OnlineMode.CustomRoom)
            + Environment.NewLine + string.Join(Environment.NewLine, OnlineRoomSettings.Summary(_lan.Room.Settings));
        public string ReadyText => MySeat?.Ready == true ? OnlineTexts.CancelReady : OnlineTexts.Ready;
        public bool CanReady => HasRoom && _lan.IsConnected && _lan.Game == null && !IsBusy;
        public string LeaveText => OnlineTexts.LeaveRoom;
        private SeatDto MySeat => _lan.Room?.Seats?.FirstOrDefault(seat => seat?.Id == _lan.LocalId);

        public PlayerSettings RoomSettingsValues { get; } = new PlayerSettings();
        public RuleSettings RoomRules => RoomSettingsValues.Rules.RulesFor(GameKind.Traditional);
        public OnlineFirstMover FirstMover { get; private set; } = OnlineFirstMover.Random;

        public IReadOnlyList<SettingsMenuSection> SettingsSections => _sections ??= CreateSections();
        public IReadOnlyList<LanRoomRow> Rooms => _hosts.Select(host => new LanRoomRow(
            host.RoomId + "@" + host.Endpoint, LanTexts.FoundRoom(host.RoomId, host.Nickname, host.Endpoint.ToString()), () => Join(host))).ToArray();
        public string SeatsText => _lan.Room?.Seats == null ? null : string.Join(Environment.NewLine, _lan.Room.Seats.Select(seat =>
            seat == null ? OnlineTexts.WaitingForPlayer : OnlineTexts.SeatLine(seat.Name,
                seat.Id == _lan.Room.HostId ? Lang.JoinList(new[] { OnlineTexts.Host, seat.Ready ? OnlineTexts.IsReady : OnlineTexts.NotReady })
                : seat.Ready ? OnlineTexts.IsReady : OnlineTexts.NotReady)));

        public event Action Changed;

        /// <summary>Starts automatic discovery only while this page is visible and outside a room.</summary>
        public void SetShown(bool shown)
        {
            IsShown = shown;
            StopRefresh();
            if (shown && !HasRoom)
            {
                Refresh();
                _refresh = new Timer(_ => _mainThread.Post(() =>
                {
                    if (IsShown && !HasRoom)
                        Refresh();
                }), null, TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(5));
            }
            if (!shown)
                CancelSearch();
        }

        public void Refresh()
        {
            if (HasRoom || IsBusy || _disposed)
                return;
            ErrorSentence = null;
            _search = new CancellationTokenSource();
            int generation = ++_generation;
            Task<IReadOnlyList<LanHost>> task = _lan.DiscoverAsync(_search.Token);
            Changed?.Invoke();
            task.ContinueWith(result => _mainThread.Post(() =>
            {
                if (_disposed || generation != _generation)
                    return;
                _search?.Dispose();
                _search = null;
                if (result.Status == TaskStatus.RanToCompletion)
                    _hosts = result.Result;
                else if (!result.IsCanceled)
                    OnFailure(result.Exception?.GetBaseException().Message ?? "Discovery failed.");
                Changed?.Invoke();
            }), TaskScheduler.Default);
        }

        public void Create()
        {
            if (_joining || _leaving || HasRoom || !ValidNickname())
                return;
            CancelSearch();
            ErrorSentence = LastResultSentence = null;
            try
            {
                _lan.Create(Nickname, OnlineRoomSettings.Create(GameKind.Traditional, new RuleSettings(), OnlineFirstMover.Random, false));
            }
            catch (Exception ex) when (ex is System.IO.IOException or System.Net.Sockets.SocketException or ArgumentException)
            {
                OnFailure(ex.Message);
            }
            Changed?.Invoke();
        }

        public void Join(LanHost host)
        {
            if (_joining || _leaving || HasRoom || !ValidNickname())
                return;
            CancelSearch();
            ErrorSentence = LastResultSentence = null;
            _joining = true;
            int generation = ++_generation;
            Task task = _lan.JoinAsync(Nickname, host);
            Changed?.Invoke();
            task.ContinueWith(result => _mainThread.Post(() =>
            {
                if (_disposed || generation != _generation)
                    return;
                if (!result.IsCompletedSuccessfully)
                {
                    _joining = false;
                    OnFailure(result.Exception?.GetBaseException().Message ?? "Connection cancelled.");
                }
                Changed?.Invoke();
            }), TaskScheduler.Default);
        }

        /// <summary>Resolves a six-digit room number or an IPv4 address through fresh discovery.</summary>
        public void JoinByNumber()
        {
            if (_joining || _leaving || HasRoom || !ValidNickname())
                return;
            string target = RoomAddress?.Trim() ?? "";
            IPAddress address = null;
            int? port = null;
            bool number = target.Length == 6 && target.All(char.IsAsciiDigit);
            string[] parts = target.Split(':');
            if (!number && (parts.Length > 2 || !IPAddress.TryParse(parts[0], out address)
                || address.AddressFamily != System.Net.Sockets.AddressFamily.InterNetwork
                || (parts.Length == 2 && (!int.TryParse(parts[1], out int parsedPort)
                    || parsedPort < LanProtocol.FirstGamePort || parsedPort > LanProtocol.LastGamePort))))
            {
                ErrorSentence = LanTexts.InvalidRoomAddress;
                Changed?.Invoke();
                return;
            }
            if (!number && parts.Length == 2)
                port = int.Parse(parts[1]);
            CancelSearch();
            ErrorSentence = null;
            _search = new CancellationTokenSource();
            int generation = ++_generation;
            var task = LanDiscovery.FindAsync(TimeSpan.FromSeconds(2), _search.Token, address);
            Changed?.Invoke();
            task.ContinueWith(result => _mainThread.Post(() =>
            {
                if (_disposed || generation != _generation)
                    return;
                _search.Dispose();
                _search = null;
                if (result.IsCompletedSuccessfully)
                {
                    var matches = result.Result.Where(host => number ? host.RoomId == target
                        : host.Endpoint.Address.Equals(address) && (!port.HasValue || host.Endpoint.Port == port)).ToArray();
                    if (matches.Length == 1)
                        Join(matches[0]);
                    else
                        ErrorSentence = matches.Length > 1 ? LanTexts.AmbiguousRoom : LanTexts.RoomNotFound;
                }
                else if (!result.IsCanceled)
                    OnFailure(result.Exception?.GetBaseException().Message ?? "Discovery failed.");
                Changed?.Invoke();
            }), TaskScheduler.Default);
        }

        public void ChooseKind(GameKind kind)
        {
            if (CanEditSettings && OnlineMenuPresenter.IsWired(kind))
                _lan.UpdateSettings(OnlineRoomSettings.Create(kind, RoomRules, FirstMover, false));
        }

        public void ChooseMode(OnlineMode mode)
        {
            if (CanEditSettings && OnlineMenuPresenter.IsWired(mode))
                _lan.UpdateSettings(OnlineRoomSettings.Create(GameKind.Traditional, RoomRules, FirstMover, false));
        }

        public void ToggleReady()
        {
            if (CanReady)
                _lan.SetReady(MySeat?.Ready != true);
        }

        public void LeaveRoom()
        {
            if (_leaving)
                return;
            _leaving = true;
            _joining = false;
            _bridge.Leave();
            Changed?.Invoke();
        }

        public void Close()
        {
            SetShown(false);
            _generation++;
            _joining = false;
            _bridge.Leave();
        }

        /// <summary>Applies one control's edit to the room's own settings, without saving local settings.</summary>
        public void Edit(Action<PlayerSettings> edit)
        {
            ArgumentNullException.ThrowIfNull(edit);
            if (!CanEditSettings)
                return;
            edit(RoomSettingsValues);
            _lan.UpdateSettings(OnlineRoomSettings.Create(GameKind.Traditional, RoomRules, FirstMover, false));
            Changed?.Invoke();
        }

        public void Dispose()
        {
            if (_disposed)
                return;
            SetShown(false);
            _disposed = true;
            _lan.RoomChanged -= OnRoom;
            _lan.GameEnded -= OnEnd;
            _lan.GameStarted -= OnStart;
            _lan.Failed -= OnFailure;
            _lan.Disconnected -= OnDisconnect;
            _bridge.RoomRequested -= ShowRoom;
        }

        private IReadOnlyList<SettingsMenuSection> CreateSections()
        {
            var sections = new List<SettingsMenuSection>(SettingsMenuContent.RoomRuleSections(
                GameKind.Traditional, OnlineTexts.SectionRules, OnlineTexts.SectionTimer));
            sections.Add(new SettingsMenuSection(OnlineTexts.SectionRoom, new SettingsMenuItem[]
            {
                new SettingsChoiceItem(OnlineTexts.FirstMover, FirstMovers.Select(first =>
                    OnlineTexts.FirstMoverName(OnlineRoomSettings.FirstMoverName(first))).ToArray(),
                    _ => Array.IndexOf(FirstMovers, FirstMover), (_, index) => FirstMover = FirstMovers[index]),
            }));
            return sections;
        }

        private bool ValidNickname()
        {
            try { Nickname = LanProtocol.ValidateNickname(Nickname ?? ""); return true; }
            catch (ArgumentException) { ErrorSentence = LanTexts.InvalidNickname; Changed?.Invoke(); return false; }
        }

        private void CancelSearch()
        {
            if (_search == null)
                return;
            _generation++;
            _search?.Cancel();
            _search?.Dispose();
            _search = null;
        }

        private void StopRefresh()
        {
            _refresh?.Dispose();
            _refresh = null;
        }

        private void OnRoom(RoomStateDto room)
        {
            _joining = _leaving = false;
            if (room != null)
            {
                if (_lan.IsHost)
                {
                    var rules = RoomRules;
                    foreach (RuleOption option in RuleSettings.OptionsFor(GameKind.Traditional))
                    {
                        string key = option switch
                        {
                            RuleOption.GeneralCanSeeGeneral => "canGeneralSeeGeneral",
                            RuleOption.GeneralCanLeavePalace => "canGeneralLeavePalace",
                            RuleOption.AdvisorCanLeavePalace => "canAdvisorLeavePalace",
                            RuleOption.ElephantEyeBlocks => "canElephantEyeBlocked",
                            RuleOption.HorseLegBlocks => "canHorseLegHobbled",
                            RuleOption.CaptureOwnPiece => "canCaptureOwnPiece",
                            _ => null,
                        };
                        if (key != null && room.Settings.Rules != null && room.Settings.Rules.TryGetValue(key, out bool value))
                            rules.Set(option, value);
                    }
                    rules.TimerMode = room.Settings.Timer.Mode == RoomNames.TimerCountUp ? TimerMode.CountUp : TimerMode.CountDown;
                    rules.TotalTimeMinutes = room.Settings.Timer.TotalMinutes;
                    rules.StepTimeSeconds = room.Settings.Timer.StepSeconds;
                    rules.StepTimerEnabled = room.Settings.Timer.StepTimer;
                    rules.IncrementSeconds = room.Settings.Timer.IncrementSeconds;
                    FirstMover = room.Settings.FirstMover == RoomNames.FirstMoverHost ? OnlineFirstMover.Host
                        : room.Settings.FirstMover == RoomNames.FirstMoverGuest ? OnlineFirstMover.Guest : OnlineFirstMover.Random;
                }
                StopRefresh();
                CancelSearch();
            }
            else if (IsShown)
                SetShown(true);
            Changed?.Invoke();
        }

        private void OnStart(StartGameDto start)
        {
            _players = start.Players;
            LastResultSentence = null;
            Changed?.Invoke();
        }

        private void OnEnd(EndGameDto end)
        {
            string winner = end.Winner is 1 or 2 && _players.Count == 2 ? _players[end.Winner - 1].Name : null;
            string loser = end.Winner is 1 or 2 && _players.Count == 2 ? _players[2 - end.Winner].Name : null;
            LastResultSentence = end.Reason == "Disconnect" ? LanTexts.Disconnected
                : OnlineTexts.GameOver(end.Winner, winner, loser, end.Reason);
            Changed?.Invoke();
        }

        private void OnFailure(string detail)
        {
            ErrorSentence = LanTexts.NetworkFailure;
            AppLogger.Log($"(LAN) {detail}", LogLevel.WARN);
            Changed?.Invoke();
        }

        private void OnDisconnect()
        {
            _joining = false;
            ErrorSentence = LanTexts.Disconnected;
            Changed?.Invoke();
        }

        private void ShowRoom() => _online.ShowPanel(OnlineMenuPage.Lan);
    }
}

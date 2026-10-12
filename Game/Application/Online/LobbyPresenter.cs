/* ----- ----- ----- ----- */
// LobbyPresenter.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/12
// Update Date: 2026/10/12
// Version: v1.1
/* ----- ----- ----- ----- */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

using Chinese_Chess_v3.Game.Application.Settings;
using Chinese_Chess_v3.Game.Application.Texts;
using Chinese_Chess_v3.Game.Configs;
using Chinese_Chess_v3.Game.Core;
using Chinese_Chess_v3.Network.Protocol;

using Engine.Localization;
using Engine.Timing;

namespace Chinese_Chess_v3.Game.Application.Online
{
    /// <summary>One waiting room of the lobby's list, as the view shows it.</summary>
    public sealed class LobbyRoomRow
    {
        /// <summary>Creates a row.</summary>
        /// <param name="roomId">The room number.</param>
        /// <param name="text">The row's text (number, host, seats).</param>
        /// <param name="summary">The room's settings, one sentence per line.</param>
        /// <param name="canJoin">Whether a seat is free.</param>
        /// <param name="join">Joins the room.</param>
        public LobbyRoomRow(string roomId, string text, IReadOnlyList<string> summary, bool canJoin, Action join)
        {
            RoomId = roomId;
            Text = text;
            Summary = summary;
            CanJoin = canJoin;
            Join = join;
        }

        /// <summary>The room number.</summary>
        public string RoomId { get; }

        /// <summary>The row's text.</summary>
        public string Text { get; }

        /// <summary>The room's settings, one sentence per line.</summary>
        public IReadOnlyList<string> Summary { get; }

        /// <summary>The room's settings as one line (the sentences joined as a list).</summary>
        public string SummaryText => Lang.JoinList(Summary);

        /// <summary>Whether a seat is free.</summary>
        public bool CanJoin { get; }

        /// <summary>Joins the room.</summary>
        public Action Join { get; }
    }

    /// <summary>
    /// The custom rooms' lobby (a page of the online menu): the waiting rooms, refreshed every
    /// <see cref="RefreshInterval"/> while the page is on display and by 重新整理 (ONLINE-PLAY
    /// §8.27), joining by the six-digit room number or from the list, and creating a room. A new
    /// room's settings are edited on <see cref="RoomRules"/> (the same option model as the local rule
    /// settings, starting at the defaults: the room's settings are the game's rules, not the
    /// player's local ones), <see cref="FirstMover"/> and <see cref="AllowPause"/>; the view's
    /// editor shows <see cref="RoomSettingsSections"/> (the local rule settings' rows) over
    /// <see cref="RoomSettingsValues"/> and sends its edits to <see cref="SetToggle"/>,
    /// <see cref="SetChoice"/>, <see cref="SetNumber"/>, <see cref="SetInteger"/> and <see cref="SetText"/>.
    /// </summary>
    public sealed class LobbyPresenter
    {
        /// <summary>How often the list refreshes itself while the lobby is on display.</summary>
        public static readonly TimeSpan RefreshInterval = TimeSpan.FromSeconds(5);

        /// <summary>How many digits a room number has.</summary>
        public const int RoomNumberLength = 6;

        // The first mover choices in the editor's order.
        private static readonly OnlineFirstMover[] FirstMoverOrder = { OnlineFirstMover.Random, OnlineFirstMover.Host, OnlineFirstMover.Guest };

        private readonly OnlineSession _session;
        private readonly IMainThreadQueue _mainThread;
        private readonly Dictionary<GameKind, IReadOnlyList<SettingsMenuSection>> _sections = new();
        private Timer _refreshTimer;
        private string _localError;

        /// <summary>Creates the presenter (a single long-lived instance).</summary>
        /// <param name="session">The online session.</param>
        /// <param name="mainThread">Where the refresh timer's ticks run (the UI thread).</param>
        public LobbyPresenter(OnlineSession session, IMainThreadQueue mainThread)
        {
            _session = session ?? throw new ArgumentNullException(nameof(session));
            _mainThread = mainThread ?? throw new ArgumentNullException(nameof(mainThread));
            _session.RoomListChanged += _ => Changed?.Invoke();
            _session.ErrorRaised += _ =>
            {
                _localError = null;
                Changed?.Invoke();
            };
            _session.StateChanged += () =>
            {
                // Back in the lobby (e.g. after leaving a room or reconnecting): fresh list at once.
                if (IsActive && _session.State == OnlineState.LoggedIn)
                    _session.RequestRoomList();
            };
        }

        // ----- Texts -----

        /// <summary>The 重新整理 button's text.</summary>
        public string RefreshText => OnlineTexts.Refresh;

        /// <summary>The 建立房間 button's text.</summary>
        public string CreateText => OnlineTexts.CreateRoom;

        /// <summary>The 加入 button's text.</summary>
        public string JoinText => OnlineTexts.Join;

        /// <summary>The room number field's label.</summary>
        public string RoomNumberLabel => OnlineTexts.RoomNumber;

        /// <summary>The list's text when it is empty, otherwise null.</summary>
        public string EmptyText => Rooms.Count == 0 ? OnlineTexts.NoRooms : null;

        /// <summary>The lobby page's title (自訂房間).</summary>
        public string TitleText => OnlineTexts.ModeName(OnlineMode.CustomRoom);

        /// <summary>The header above the waiting rooms.</summary>
        public string RoomListTitle => OnlineTexts.RoomList;

        /// <summary>The header above a new room's settings.</summary>
        public string NewRoomTitle => OnlineTexts.NewRoomSettings;

        // ----- State -----

        /// <summary>The game kind of the rooms created here (the online menu's choice).</summary>
        public GameKind Kind { get; set; } = GameKind.Traditional;

        /// <summary>Whether the lobby is on display (refreshing itself).</summary>
        public bool IsActive { get; private set; }

        /// <summary>The waiting rooms of the last list.</summary>
        public IReadOnlyList<LobbyRoomRow> Rooms => _session.Rooms.Where(r => r != null).Select(ToRow).ToList();

        /// <summary>The room number field.</summary>
        public string RoomNumber { get; set; } = "";

        /// <summary>
        /// The settings instance a new room's choices are edited in (its own, at the defaults until
        /// edited; never the player's local settings, never saved). Only its
        /// <see cref="RoomRules"/> are used.
        /// </summary>
        public PlayerSettings RoomSettingsValues { get; } = new PlayerSettings();

        /// <summary>A new room's rule and clock choices for <see cref="Kind"/> (defaults until edited).</summary>
        public RuleSettings RoomRules => RoomSettingsValues.Rules.RulesFor(Kind);

        /// <summary>
        /// The new room's settings editor for <see cref="Kind"/>: the local rule settings' rule and
        /// clock rows a room sends (<see cref="SettingsMenuContent.RoomRuleSections"/>), then
        /// 先手 and 允許暫停. Its items read and write <see cref="RoomSettingsValues"/>.
        /// </summary>
        public IReadOnlyList<SettingsMenuSection> RoomSettingsSections
        {
            get
            {
                if (!_sections.TryGetValue(Kind, out var sections))
                {
                    var list = new List<SettingsMenuSection>(
                        SettingsMenuContent.RoomRuleSections(Kind, OnlineTexts.SectionRules, OnlineTexts.SectionTimer));
                    list.Add(new SettingsMenuSection(OnlineTexts.SectionRoom, new SettingsMenuItem[]
                    {
                        new SettingsChoiceItem(OnlineTexts.FirstMover,
                            FirstMoverOrder.Select(f => OnlineTexts.FirstMoverName(OnlineRoomSettings.FirstMoverName(f))).ToArray(),
                            _ => Math.Max(0, Array.IndexOf(FirstMoverOrder, FirstMover)), (_, index) => FirstMover = FirstMoverOrder[index]),
                        new SettingsToggleItem(OnlineTexts.AllowPause, _ => AllowPause, (_, on) => AllowPause = on),
                    }));
                    sections = list;
                    _sections[Kind] = sections;
                }
                return sections;
            }
        }

        /// <summary>A new room's first mover.</summary>
        public OnlineFirstMover FirstMover { get; set; } = OnlineFirstMover.Random;

        /// <summary>Whether a new room allows pausing.</summary>
        public bool AllowPause { get; set; }

        /// <summary>The error to show (a bad room number, or the server's answer to a request), or null.</summary>
        public string ErrorSentence => _localError ?? (_session.State == OnlineState.LoggedIn ? _session.LastError : null);

        /// <summary>The view redraws.</summary>
        public event Action Changed;

        // ----- Commands -----

        /// <summary>The lobby is on display: refreshes now and every <see cref="RefreshInterval"/>.</summary>
        public void Activate()
        {
            if (IsActive)
                return;
            IsActive = true;
            Refresh();
            _refreshTimer = new Timer(_ => _mainThread.Post(Tick), null, RefreshInterval, RefreshInterval);
        }

        /// <summary>The lobby is no longer on display: stops refreshing.</summary>
        public void Deactivate()
        {
            if (!IsActive)
                return;
            IsActive = false;
            _refreshTimer?.Dispose();
            _refreshTimer = null;
        }

        /// <summary>重新整理: asks for the list now.</summary>
        public void Refresh() => _session.RequestRoomList();

        /// <summary>加入 with the room number field: <see cref="RoomNumberLength"/> digits, otherwise an error sentence.</summary>
        public void JoinByNumber()
        {
            string number = RoomNumber?.Trim() ?? "";
            if (number.Length != RoomNumberLength || !number.All(c => c >= '0' && c <= '9'))
            {
                _localError = OnlineTexts.InvalidRoomNumber;
                Changed?.Invoke();
                return;
            }
            Join(number);
        }

        /// <summary>Joins the room <paramref name="roomId"/> (then the room screen, or an error sentence).</summary>
        /// <param name="roomId">The room number.</param>
        public void Join(string roomId)
        {
            ArgumentNullException.ThrowIfNull(roomId);
            _localError = null;
            _session.JoinRoom(roomId);
            Changed?.Invoke();
        }

        /// <summary>The settings a room created now gets.</summary>
        /// <returns>The CreateRoom data.</returns>
        public RoomSettingsDto BuildSettings() => OnlineRoomSettings.Create(Kind, RoomRules, FirstMover, AllowPause);

        /// <summary>A new room's settings, one sentence per line.</summary>
        public IReadOnlyList<string> NewRoomSummary => OnlineRoomSettings.Summary(BuildSettings());

        /// <summary>A new room's settings as one line (the sentences joined as a list).</summary>
        public string NewRoomSummaryText => Lang.JoinList(NewRoomSummary);

        /// <summary>The editor's switch flipped (<see cref="RoomSettingsSections"/>).</summary>
        /// <param name="item">The switch's setting.</param>
        /// <param name="value">On or off.</param>
        public void SetToggle(SettingsToggleItem item, bool value)
        {
            ArgumentNullException.ThrowIfNull(item);
            item.Set(RoomSettingsValues, value);
            Changed?.Invoke();
        }

        /// <summary>The editor's dropdown chose an option (kept within the options).</summary>
        /// <param name="item">The dropdown's setting.</param>
        /// <param name="index">The option.</param>
        public void SetChoice(SettingsChoiceItem item, int index)
        {
            ArgumentNullException.ThrowIfNull(item);
            item.SetIndex(RoomSettingsValues, Math.Clamp(index, 0, item.Options.Count - 1));
            Changed?.Invoke();
        }

        /// <summary>The editor's slider moved (kept within its range).</summary>
        /// <param name="item">The slider's setting.</param>
        /// <param name="value">The value.</param>
        public void SetNumber(SettingsNumberItem item, float value)
        {
            ArgumentNullException.ThrowIfNull(item);
            item.Set(RoomSettingsValues, Math.Clamp(value, item.Min, item.Max));
            Changed?.Invoke();
        }

        /// <summary>The editor's text field edited (cut to its longest length).</summary>
        /// <param name="item">The field's setting.</param>
        /// <param name="value">The text.</param>
        public void SetText(SettingsTextItem item, string value)
        {
            ArgumentNullException.ThrowIfNull(item);
            value ??= string.Empty;
            item.Set(RoomSettingsValues, value.Length > item.MaxLength ? value.Substring(0, item.MaxLength) : value);
            Changed?.Invoke();
        }

        /// <summary>The editor's number field ended an edit (kept within its range).</summary>
        /// <param name="item">The field's setting.</param>
        /// <param name="value">The value.</param>
        public void SetInteger(SettingsIntegerItem item, int value)
        {
            ArgumentNullException.ThrowIfNull(item);
            item.Set(RoomSettingsValues, Math.Clamp(value, item.Min, item.Max));
            Changed?.Invoke();
        }

        /// <summary>建立房間 with the current settings (then the room screen, or an error sentence).</summary>
        public void Create()
        {
            _localError = null;
            _session.CreateRoom(BuildSettings());
            Changed?.Invoke();
        }

        // ----- Internals -----

        private void Tick()
        {
            if (IsActive && _session.State == OnlineState.LoggedIn)
                _session.RequestRoomList();
        }

        private LobbyRoomRow ToRow(RoomStateDto room)
        {
            int total = room.Seats?.Count ?? 0;
            int taken = room.Seats?.Count(s => s != null) ?? 0;
            string host = room.Seats?.FirstOrDefault(s => s != null && s.Id == room.HostId)?.Name;
            string text = OnlineTexts.LobbyRow(OnlineTexts.RoomTitle(room.RoomId), host, OnlineTexts.SeatCount(taken, total));
            string roomId = room.RoomId ?? "";
            return new LobbyRoomRow(roomId, text, OnlineRoomSettings.Summary(room.Settings), taken < total, () => Join(roomId));
        }
    }
}

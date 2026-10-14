/* ----- ----- ----- ----- */
// OnlineMenuPresenter.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/12
// Update Date: 2026/10/13
// Version: v1.5
/* ----- ----- ----- ----- */

using System;
using System.Collections.Generic;

using Chinese_Chess_v3.Game.Application.MainMenu;
using Chinese_Chess_v3.Game.Application.Menus;
using Chinese_Chess_v3.Game.Application.Texts;
using Chinese_Chess_v3.Game.Core;

using Engine.Diagnostics;

namespace Chinese_Chess_v3.Game.Application.Online
{
    /// <summary>
    /// The Multiplayer panel's decisions: login, then kinds, modes and lobby. Its pages
    /// (<see cref="OnlineMenuPage"/>; the custom rooms' lobby is the last page,
    /// driven by <see cref="LobbyPresenter"/>), the logged-in account, 登出, and 重新連線 after the
    /// connection was lost here. Every game kind and mode is listed with the normal look; only
    /// 傳統 and 自訂房間 are wired, choosing another does nothing yet (author §8.26: no greying, no
    /// 尚未開放 label).
    /// </summary>
    public sealed class OnlineMenuPresenter
    {
        private static readonly GameKind[] KindOrder =
        {
            GameKind.Traditional, GameKind.Flip, GameKind.DarkHalf, GameKind.OpenHalf, GameKind.ThreeKingdoms,
        };

        private static readonly OnlineMode[] ModeOrder =
        {
            OnlineMode.Ranked, OnlineMode.Evaluation, OnlineMode.Arena, OnlineMode.CustomRoom,
        };

        private readonly OnlineSession _session;
        private readonly LobbyPresenter _lobby;
        private readonly LanMenuPresenter _lan;
        private bool _shown;

        /// <summary>Creates the presenter (a single long-lived instance, like the panel it drives).</summary>
        /// <param name="session">The online session.</param>
        /// <param name="lobby">The lobby page's presenter.</param>
        /// <param name="online">Subscribes the online bridge before login can restore a running game.</param>
        public OnlineMenuPresenter(OnlineSession session, LobbyPresenter lobby, LanMenuPresenter lan = null, OnlineGameBridge online = null)
        {
            _session = session ?? throw new ArgumentNullException(nameof(session));
            _lobby = lobby ?? throw new ArgumentNullException(nameof(lobby));
            _lan = lan;
            _session.StateChanged += () => Changed?.Invoke();
            _session.ErrorRaised += _ => Changed?.Invoke();
        }

        // ----- Texts and lists -----

        /// <summary>The game kind page's title.</summary>
        public string ChooseKindTitle => OnlineTexts.ChooseGameKind;

        /// <summary>The mode page's title.</summary>
        public string ChooseModeTitle => OnlineTexts.ChooseMode;

        /// <summary>The 登出 button's text.</summary>
        public string LogoutText => OnlineTexts.Logout;

        /// <summary>The 重新連線 button's text.</summary>
        public string ReconnectText => OnlineTexts.Reconnect;

        /// <summary>The back button's text.</summary>
        public string BackText => OnlineTexts.Back;

        /// <summary>The logged-in account's line.</summary>
        public string AccountText => IsLan ? LanTexts.Title : OnlineTexts.LoggedInAs(_session.Username);

        public string ConnectionTitle => LanTexts.ChooseConnection;
        public string ServerText => LanTexts.Server;
        public string LanText => LanTexts.Title;
        public bool IsLan { get; private set; }
        public bool IsLoggedIn => _session.State is OnlineState.LoggedIn or OnlineState.InRoom or OnlineState.InGame;
        public bool IsFirstPage => Page is OnlineMenuPage.Connection or OnlineMenuPage.Login or OnlineMenuPage.GameKinds;
        public LanMenuPresenter Lan => _lan;

        /// <summary>The game kinds, every one with its normal look (the new-game menu's names).</summary>
        /// <returns>The entries, in button order.</returns>
        public IReadOnlyList<MenuEntry<GameKind>> GameKinds()
        {
            var list = new List<MenuEntry<GameKind>>();
            foreach (GameKind kind in KindOrder)
                list.Add(new MenuEntry<GameKind>(kind, SettingsTexts.GameKindName(kind), () => ChooseKind(kind)));
            return list;
        }

        /// <summary>The game kinds grouped by board (大盤, 小盤, 其他), every button with its normal look.</summary>
        /// <returns>The groups, in display order.</returns>
        public IReadOnlyList<BoardGroup> GroupedKinds() => BoardGroups.Create(ChooseKind);

        /// <summary>The modes, every one with its normal look.</summary>
        /// <returns>The entries, in button order.</returns>
        public IReadOnlyList<MenuEntry<OnlineMode>> Modes()
        {
            var list = new List<MenuEntry<OnlineMode>>();
            foreach (OnlineMode mode in ModeOrder)
                list.Add(new MenuEntry<OnlineMode>(mode, OnlineTexts.ModeName(mode), () => ChooseMode(mode)));
            return list;
        }

        // ----- State -----

        /// <summary>The page shown.</summary>
        public OnlineMenuPage Page { get; private set; } = OnlineMenuPage.Connection;

        /// <summary>The chosen game kind (the mode page's and the lobby's).</summary>
        public GameKind SelectedKind { get; private set; } = GameKind.Traditional;

        /// <summary>Whether the connection was lost here and 重新連線 is offered.</summary>
        public bool CanReconnect => !IsLan && _session.CanReconnect;

        /// <summary>Whether a reconnect runs.</summary>
        public bool IsBusy => IsLan ? _lan?.IsBusy == true : _session.IsBusy;

        /// <summary>The last error sentence (e.g. the connection was lost, the room was lost), or null.</summary>
        public string ErrorSentence => IsLan ? _lan?.ErrorSentence : _session.LastError;

        /// <summary>The view shows <see cref="Page"/>.</summary>
        public event Action<OnlineMenuPage> PageChanged;

        /// <summary>The view redraws (account, error, reconnect offered).</summary>
        public event Action Changed;

        // ----- Commands -----

        /// <summary>Whether choosing <paramref name="kind"/> does something yet.</summary>
        /// <param name="kind">A game kind.</param>
        /// <returns>True for 傳統 only so far.</returns>
        public static bool IsWired(GameKind kind) => kind == GameKind.Traditional;

        /// <summary>Whether choosing <paramref name="mode"/> does something yet.</summary>
        /// <param name="mode">A mode.</param>
        /// <returns>True for 自訂房間 only so far.</returns>
        public static bool IsWired(OnlineMode mode) => mode == OnlineMode.CustomRoom;

        /// <summary>A game kind: its modes (a kind not wired yet does nothing).</summary>
        /// <param name="kind">The chosen kind.</param>
        public void ChooseKind(GameKind kind)
        {
            if (!IsWired(kind))
            {
                if (DebugOptions.ConsoleTrace)
                    Console.WriteLine($"OnlineMenu: {kind} is not wired yet");
                return;
            }
            SelectedKind = kind;
            if (IsLan)
            {
                ShowPage(OnlineMenuPage.Lan);
                return;
            }
            ShowPage(OnlineMenuPage.Modes);
        }

        /// <summary>Uses the existing online login and account flow.</summary>
        public void ChooseServer()
        {
            _lan?.Close();
            IsLan = false;
            _session.EnterOnline();
        }

        /// <summary>Uses local discovery and nicknames without attempting an account login.</summary>
        public void ChooseLan()
        {
            if (_lan == null)
                return;
            if (_session.State == OnlineState.LoggingIn)
                _session.CancelLogin();
            IsLan = true;
            ShowPage(OnlineMenuPage.Lan);
        }

        public void CreateLanRoom()
        {
            ChooseLan();
            _lan?.Create();
        }

        /// <summary>A mode: 自訂房間 opens the lobby (a mode not wired yet does nothing).</summary>
        /// <param name="mode">The chosen mode.</param>
        public void ChooseMode(OnlineMode mode)
        {
            if (!IsWired(mode))
            {
                if (DebugOptions.ConsoleTrace)
                    Console.WriteLine($"OnlineMenu: {mode} is not wired yet");
                return;
            }
            _lobby.Kind = SelectedKind;
            ShowPage(OnlineMenuPage.Lobby);
        }

        /// <summary>Back: lobby to modes to kinds, then closes the panel (still logged in).</summary>
        public void Back()
        {
            switch (Page)
            {
                case OnlineMenuPage.Lan:
                    if (_lan.HasRoom)
                        _lan.LeaveRoom();
                    else
                    {
                        IsLan = false;
                        ShowPage(IsLoggedIn ? OnlineMenuPage.GameKinds : OnlineMenuPage.Login);
                    }
                    break;
                case OnlineMenuPage.GameKinds:
                    if (IsLan)
                        _lan.Close();
                    IsLan = false;
                    ShowPage(OnlineMenuPage.Connection);
                    break;
                case OnlineMenuPage.Lobby:
                    ShowPage(OnlineMenuPage.Modes);
                    break;
                case OnlineMenuPage.Modes:
                    ShowPage(OnlineMenuPage.GameKinds);
                    break;
                default:
                    if (Page == OnlineMenuPage.Login)
                    {
                        _session.CancelLogin();
                        ShowPage(OnlineMenuPage.Connection);
                        break;
                    }
                    _session.LeaveToMainMenu();
                    break;
            }
        }

        /// <summary>The panel is shown (or hidden: false): the lobby refreshes itself only while its page is on display.</summary>
        /// <param name="shown">Whether the panel is on display.</param>
        public void SetShown(bool shown)
        {
            _shown = shown;
            _lan?.SetShown(shown && Page == OnlineMenuPage.Lan);
            if (shown && Page == OnlineMenuPage.Lobby)
                _lobby.Activate();
            else
                _lobby.Deactivate();
        }

        /// <summary>重新連線 (offered after the connection was lost here).</summary>
        public void Reconnect() => _session.Reconnect();

        /// <summary>Logout returns to the login page and forgets the remembered login.</summary>
        public void Logout()
        {
            _lobby.Deactivate();
            _session.Logout();
        }

        /// <summary>Shows a page requested by the session when the main menu opens its panel.</summary>
        /// <param name="page">The requested page.</param>
        public void OpenPage(OnlineMenuPage page)
        {
            IsLan = page == OnlineMenuPage.Lan;
            ShowPage(page == OnlineMenuPage.Connection ? (IsLoggedIn ? OnlineMenuPage.GameKinds : OnlineMenuPage.Login) : page);
        }

        private void ShowPage(OnlineMenuPage page)
        {
            Page = page;
            _lan?.SetShown(_shown && page == OnlineMenuPage.Lan);
            if (_shown && page == OnlineMenuPage.Lobby)
                _lobby.Activate();
            else
                _lobby.Deactivate();
            PageChanged?.Invoke(page);
        }
    }
}

/* ----- ----- ----- ----- */
// MainMenuPresenter.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/05
// Update Date: 2026/10/12
// Version: v1.3
/* ----- ----- ----- ----- */

using System;
using System.Collections.Generic;

using Chinese_Chess_v3.Game.Application.Online;
using Chinese_Chess_v3.Game.Application.Services;
using Chinese_Chess_v3.Game.Application.Session;
using Chinese_Chess_v3.Game.Application.Settings;
using Chinese_Chess_v3.Game.Application.Texts;
using Chinese_Chess_v3.Game.Core;

using Engine.Diagnostics;
using Engine.Logging;

namespace Chinese_Chess_v3.Game.Application.MainMenu
{
    /// <summary>
    /// The main menu's decisions, apart from the views that draw it: which submenu is open
    /// (clicking an open submenu's button again closes it), entering 多人連線 (the online
    /// session's pages inside the right panel), asking before 離開遊戲 and exiting, which settings screen each settings entry
    /// opens, and choosing a game kind from a purpose-specific submenu.
    /// <para>
    /// The views bind to it: the menu's buttons call <see cref="Select"/>; the main menu shows a
    /// submenu on <see cref="SubmenuOpened"/> and hides it on <see cref="SubmenuClosed"/>, and
    /// calls <see cref="CloseSubmenu"/> when it is left. The single-player submenu's buttons call
    /// <see cref="StartSinglePlayer"/>.
    /// </para>
    /// </summary>
    public sealed class MainMenuPresenter
    {
        /// <summary>The options that open a submenu inside the main menu, in the order the menu creates them.</summary>
        private static readonly MainMenuOption[] SubmenuOptionList =
        {
            MainMenuOption.SinglePlayer,
            MainMenuOption.VsComputer,
            MainMenuOption.Multiplayer,
            MainMenuOption.LoadGame,
            MainMenuOption.EndgameChallenge,
            MainMenuOption.OpeningPractice,
            MainMenuOption.RuleSettings,
            MainMenuOption.Help,
            MainMenuOption.Settings,
        };

        private readonly GameSession _session;
        private readonly IDialogService _dialogs;
        private readonly IAppLifetime _lifetime;
        private readonly OnlineSession _online;
        private readonly OnlineMenuPresenter _onlinePanel;

        /// <summary>Creates the presenter (a single long-lived instance, like the main menu it drives).</summary>
        /// <param name="session">The game flow (starts a new game).</param>
        /// <param name="dialogs">The confirm dialogs.</param>
        /// <param name="lifetime">Exits the application.</param>
        /// <param name="online">The online session.</param>
        /// <param name="onlinePanel">The Multiplayer panel pages.</param>
        public MainMenuPresenter(GameSession session, IDialogService dialogs, IAppLifetime lifetime, OnlineSession online, OnlineMenuPresenter onlinePanel)
        {
            _session = session ?? throw new ArgumentNullException(nameof(session));
            _dialogs = dialogs ?? throw new ArgumentNullException(nameof(dialogs));
            _lifetime = lifetime ?? throw new ArgumentNullException(nameof(lifetime));
            _online = online ?? throw new ArgumentNullException(nameof(online));
            _onlinePanel = onlinePanel ?? throw new ArgumentNullException(nameof(onlinePanel));
            _online.PanelRequested += Shown;
            _online.PanelClosed += CloseSubmenu;
        }

        // ----- Events and state for the views -----

        /// <summary>The open submenu, or null when none is.</summary>
        public MainMenuOption? CurrentSubmenu { get; private set; }

        /// <summary>The view shows the submenu of the option (and lets it refresh its content).</summary>
        public event Action<MainMenuOption> SubmenuOpened;

        /// <summary>The view hides the submenu of the option.</summary>
        public event Action<MainMenuOption> SubmenuClosed;

        /// <summary>The options that open a submenu inside the main menu (the view creates one element for each).</summary>
        public static IReadOnlyList<MainMenuOption> SubmenuOptions => SubmenuOptionList;

        /// <summary>Whether <paramref name="option"/> opens a submenu inside the main menu.</summary>
        /// <param name="option">A main menu option.</param>
        /// <returns>True for the submenu options (<see cref="SubmenuOptions"/>).</returns>
        public static bool IsSubmenu(MainMenuOption option) => Array.IndexOf(SubmenuOptionList, option) >= 0;

        /// <summary>The settings screen <paramref name="option"/> opens, if it is a settings entry.</summary>
        /// <param name="option">A main menu option.</param>
        /// <returns>單機規則設定 → rules, 遊戲設定 → general settings; null for any other option.</returns>
        public static SettingsScreen? SettingsScreenOf(MainMenuOption option) => option switch
        {
            MainMenuOption.RuleSettings => SettingsScreen.Rules,
            MainMenuOption.Settings => SettingsScreen.Game,
            _ => null,
        };

        // ----- Commands -----

        /// <summary>
        /// A main menu button: opens its submenu (closing the open one; the open one's own
        /// button closes it), or asks before exiting.
        /// </summary>
        /// <param name="option">The clicked option.</param>
        public void Select(MainMenuOption option)
        {
            if (DebugOptions.ConsoleTrace)
                Console.WriteLine($"MainMenu: selected: {option}");

            if (IsSubmenu(option))
            {
                ToggleSubmenu(option);
                return;
            }

            switch (option)
            {
                case MainMenuOption.Default:
                    break;

                case MainMenuOption.Exit:
                    ConfirmExit();
                    break;

                default:
                    if (DebugOptions.ConsoleTrace)
                        Console.WriteLine($"MainMenu: selected: 'Not defined'");
                    break;
            }
        }

        /// <summary>
        /// Closes the open submenu (if any) and forgets it: the main menu is being left, so back
        /// on it the first click on that submenu's button opens it again instead of counting as
        /// "clicked again" and collapsing the (already closed) submenu.
        /// </summary>
        public void CloseSubmenu()
        {
            if (CurrentSubmenu is MainMenuOption current)
            {
                SubmenuClosed?.Invoke(current);
                CurrentSubmenu = null;
            }
        }

        /// <summary>
        /// A single-player submenu button: starts that game kind's game on the game screen, or, for a
        /// kind without a game yet (<see cref="GameSession.CanStartNew"/>; 三國半盤 whose 自訂分隊 leaves a
        /// team empty, <see cref="GameSession.IsTeamSplitInvalid"/>), says so and stays on the menu.
        /// </summary>
        /// <param name="kind">The chosen game kind.</param>
        public void StartSinglePlayer(GameKind kind)
        {
            if (DebugOptions.ConsoleTrace)
                Console.WriteLine($"NewGameMenu: selected: {kind}");

            if (!GameSession.CanStartNew(kind))
            {
                AppLogger.Log($"(NewGame) {kind} is not implemented yet; staying on the new-game menu", LogLevel.WARN);
                _dialogs.ShowConfirm(MenuTexts.NewGameModeUnavailable(NewGameOptions.LabelOf(kind)), ConfirmDialogType.Ok, _ => { });
                return;
            }

            if (kind == GameKind.ThreeKingdoms && _session.IsTeamSplitInvalid)
            {
                AppLogger.Log("(NewGame) 三國 team split leaves a team without pieces; staying on the new-game menu", LogLevel.WARN);
                _dialogs.ShowConfirm(MenuTexts.TeamSplitInvalid, ConfirmDialogType.Ok, _ => { });
                return;
            }

            // Game screen, restart (views reset, log cleared), then the chosen mode's game.
            _session.StartNew(kind);
        }

        /// <summary>A computer-game kind button; this flow is not wired yet.</summary>
        /// <param name="kind">The chosen game kind.</param>
        public void StartVsComputer(GameKind kind) { }

        /// <summary>Consumes a session request after entering the main menu, or while it is already shown.</summary>
        public void Shown()
        {
            if (_online.TakePanelPage() is not OnlineMenuPage page)
                return;
            _onlinePanel.OpenPage(page);
            if (CurrentSubmenu == MainMenuOption.Multiplayer)
                return;
            CloseSubmenu();
            CurrentSubmenu = MainMenuOption.Multiplayer;
            SubmenuOpened?.Invoke(MainMenuOption.Multiplayer);
        }

        // ----- Decisions -----

        /// <summary>Closes the open submenu, then opens <paramref name="option"/>'s unless it was the open one.</summary>
        private void ToggleSubmenu(MainMenuOption option)
        {
            MainMenuOption? previous = CurrentSubmenu;
            if (previous.HasValue)
                SubmenuClosed?.Invoke(previous.Value);

            if (previous == option)  // Same menu clicked again, collapse
                CurrentSubmenu = null;
            else  // Show new submenu
            {
                CurrentSubmenu = option;
                SubmenuOpened?.Invoke(option);
                if (option == MainMenuOption.Multiplayer)
                    _online.EnterMultiplayer();
            }
        }

        /// <summary>離開遊戲: asks first; yes exits the application, no stays (the dialog has already closed).</summary>
        private void ConfirmExit()
        {
            _dialogs.ShowConfirm(
                MenuTexts.ConfirmExit,
                ConfirmDialogType.YesNo,
                result =>
                {
                    if (result == ConfirmDialogResult.Yes)
                        _lifetime.Exit();
                });
        }
    }
}

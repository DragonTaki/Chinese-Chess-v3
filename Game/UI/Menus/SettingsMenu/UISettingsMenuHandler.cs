/* ----- ----- ----- ----- */
// UISettingsMenuHandler.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/01
// Update Date: 2026/10/04
// Version: v2.2
/* ----- ----- ----- ----- */

using System;
using System.Collections.Generic;
using System.Linq;

using Chinese_Chess_v3.Game.Application.Services;
using Chinese_Chess_v3.Game.Configs;
using Chinese_Chess_v3.Game.Core;
using Chinese_Chess_v3.Game.UI.Constants;

using Engine.Logging;
using Engine.Timing;
using Engine.UI.Core.Handlers;
using Engine.UI.Core.Interfaces;
using Engine.UI.Input;

using Microsoft.Extensions.DependencyInjection;

namespace Chinese_Chess_v3.Game.UI.Menus.SettingsMenu
{
    /// <summary>
    /// A settings screen's logic (遊戲設定 / 單機規則設定 alike). An edit changes the <b>live</b>
    /// <see cref="PlayerSettings"/> (the instance registered in DI) at once: the hint settings
    /// are read from it every frame by the board, the player name and the DEBUG tab's switches are
    /// pushed to where they are used, and each game kind's rule / clock settings are copied onto
    /// the rules new games of that kind start with (<see cref="GameManager.DefaultRuleSets"/>,
    /// <see cref="PlayerSettings.ApplyTo"/>), so a game started after the edit already plays by
    /// them. Every edit is also written to <c>settings.ini</c> at once (there is no save or
    /// discard button): switches, dropdowns and number fields when the value is set, a slider
    /// when the mouse is released (not on every drag step), a text field when its edit ends.
    /// The 恢復初始 button resets the shown tab's settings to the defaults
    /// (<see cref="PlayerSettings.Defaults"/>) after a confirmation, then saves and applies
    /// like any edit. Settings that are not implemented yet keep their value for the session
    /// only (<see cref="UnimplementedSettings"/>) and are neither saved nor reset. The
    /// settings are only opened from the main menu, and a game in progress is never affected:
    /// every game plays by its own copy of the rules, taken when it starts
    /// (<see cref="GameManager.Rules"/>).
    /// <see cref="IScreen"/>: the main menu calls <see cref="OnEnter"/>/<see cref="OnExit"/>
    /// when it opens/closes the screen.
    /// </summary>
    public class UISettingsMenuHandler : UIMenuHandler<UISettingsMenu, UISettingsMenuHandler, UISettingsMenuRenderer>, IScreen
    {
        /// <summary>The live settings (the DI instance, or a private default when none is registered).</summary>
        private PlayerSettings _live = PlayerSettings.Defaults;

        /// <summary>Whether an edit is applied but not written yet (a slider drag or text edit in progress).</summary>
        private bool _unsaved;

        /// <summary>Whether the last write failed and the failure was already reported (so a run of failing edits shows the message once).</summary>
        private bool _saveFailureShown;

        /// <summary>The shown tab's index.</summary>
        private int _currentTab;

        public UISettingsMenuHandler() { }

        /// <summary>The shown screen's tabs (<see cref="SettingsMenuContent.PagesFor"/>).</summary>
        private IReadOnlyList<SettingsMenuPage> _pages = Array.Empty<SettingsMenuPage>();

        /// <summary>The app's confirm dialogs (the <see cref="IDialogService"/> registered in DI).</summary>
        private IDialogService Dialogs => _factory.ServiceProvider.GetRequiredService<IDialogService>();

        /// <summary>Screen opened: set up the tabs and show the initial one.</summary>
        public void OnEnter()
        {
            _live = _factory.ServiceProvider.GetService<PlayerSettings>() ?? _live;
            _unsaved = false;

            _pages = SettingsMenuContent.PagesFor(Element.Screen);
            int initial = Math.Clamp(Element.InitialTab, 0, Math.Max(0, _pages.Count - 1));
            _currentTab = initial;
            Element.SetTabs(_pages.Select(p => p.Title), initial);
            if (_pages.Count > 0)
                Element.ShowPage(_pages[initial], _live);
        }

        /// <summary>A tab selected: show its rows.</summary>
        public void SelectTab(int index)
        {
            if (index >= 0 && index < _pages.Count)
            {
                _currentTab = index;
                Element.ShowPage(_pages[index], _live);
            }
        }

        /// <summary>Screen closed: nothing is discarded; an edit still pending (applied, not yet written) is written.</summary>
        public void OnExit()
        {
            if (_unsaved)
                SaveNow();
        }

        /// <summary>A switch flipped: set the setting and apply it live.</summary>
        public void SetToggle(SettingsToggleItem item, bool value)
        {
            item.Set(_live, value);
            ApplyChange();
            SaveNow();
        }

        /// <summary>A dropdown's option chosen: set the setting to choice <paramref name="index"/> (kept within the options) and apply it live.</summary>
        public void SetChoice(SettingsChoiceItem item, int index)
        {
            item.SetIndex(_live, Math.Clamp(index, 0, item.Options.Count - 1));
            ApplyChange();
            SaveNow();
        }

        /// <summary>A slider moved (also while dragging): set the setting (kept within its range) and apply it live; written when the drag ends (<see cref="CommitEdit"/>).</summary>
        public void SetNumber(SettingsNumberItem item, float value)
        {
            item.Set(_live, Math.Clamp(value, item.Min, item.Max));
            ApplyChange();
            _unsaved = true;
        }

        /// <summary>A number field's edit ended with a legal value: set the setting (kept within its range) and apply it live.</summary>
        public void SetInteger(SettingsIntegerItem item, int value)
        {
            item.Set(_live, Math.Clamp(value, item.Min, item.Max));
            ApplyChange();
            SaveNow();
        }

        /// <summary>A text field edited: set the setting (cut to its longest length) and apply it live; written when its edit ends (<see cref="CommitEdit"/>).</summary>
        public void SetText(SettingsTextItem item, string value)
        {
            value ??= string.Empty;
            item.Set(_live, value.Length > item.MaxLength ? value.Substring(0, item.MaxLength) : value);
            ApplyChange();
            _unsaved = true;
        }

        /// <summary>A slider drag or text edit ended: write the settings (once for the whole burst of changes).</summary>
        public void CommitEdit() => SaveNow();

        /// <summary>After an edit: push the live settings to the game and refresh the rows (e.g. a timer mode change enables / disables the time-limit switches).</summary>
        private void ApplyChange()
        {
            ApplyToGame();
            Element.RefreshValues(_live);
        }

        /// <summary>
        /// Writes <c>settings.ini</c>. On failure logs it and shows the save-failed message, but
        /// only for the first failure of a run (until a write succeeds again).
        /// </summary>
        private void SaveNow()
        {
            _unsaved = false;
            if (PlayerSettingsFile.Save(_live, SystemSettings.PlayerSettingsFilePath))
            {
                _saveFailureShown = false;
                return;
            }

            AppLogger.Log($"(Settings) Could not save {SystemSettings.PlayerSettingsFilePath}", LogLevel.ERROR);
            if (_saveFailureShown)
                return;
            _saveFailureShown = true;
            Dialogs.ShowConfirm(GameMenuTexts.SettingsSaveFailed, ConfirmDialogType.Ok, _ => { });
        }

        /// <summary>恢復初始 clicked: after a confirmation, reset the shown tab's settings to the defaults, refresh the controls, apply and save.</summary>
        public void ResetRequested()
        {
            if (_currentTab < 0 || _currentTab >= _pages.Count)
                return;

            var page = _pages[_currentTab];
            Dialogs.ShowConfirm(
                GameMenuTexts.ResetTabToDefaults,
                ConfirmDialogType.YesNo,
                result =>
                {
                    if (result != ConfirmDialogResult.Yes)
                        return;
                    page.ResetToDefaults(_live, PlayerSettings.Defaults);
                    ApplyChange();
                    SaveNow();
                });
        }

        /// <summary>
        /// Copies each kind's live rule / clock settings onto the rules new games of that kind
        /// start with, the DEBUG tab's switches onto the engine's debug switchboard (the launchers
        /// only push them once at startup), the player name onto the log greeting / save names, the wheel step onto
        /// the shared scroll handler and the frame rate onto the engine's frame timer (both also
        /// only set at startup otherwise).
        /// </summary>
        private void ApplyToGame()
        {
            var gameManager = _factory.ServiceProvider.GetService<GameManager>();
            if (gameManager != null)
            {
                _live.ApplyTo(gameManager.DefaultRuleSets);
                _live.ApplyPlayerNamesTo(gameManager);
            }

            _live.ApplyDebugOptions();

            Settings.CurrentUser = _live.PlayerName;
            AppLogger.CurrentUser = Settings.CurrentUser;

            if (_factory.ServiceProvider.GetService<IScrollInputHandler>() is ScrollInputHandler scroll)
                scroll.WheelStep = _live.WheelScrollStep;

            TimerSettings.GameAnimationFPS = _live.Fps;
        }
    }
}

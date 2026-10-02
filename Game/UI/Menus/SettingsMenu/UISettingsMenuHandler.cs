/* ----- ----- ----- ----- */
// UISettingsMenuHandler.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/01
// Update Date: 2026/10/02
// Version: v2.1
/* ----- ----- ----- ----- */

using System;
using System.Collections.Generic;
using System.Linq;

using Chinese_Chess_v3.Game.Configs;
using Chinese_Chess_v3.Game.Core;
using Chinese_Chess_v3.Game.UI.Constants;
using Chinese_Chess_v3.Game.UI.Dialogs;
using Chinese_Chess_v3.Game.UI.Menus.MainMenu;

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
    /// are read from it every frame by the board, the player name and the debug log switch are
    /// pushed to where they are used, and each game kind's rule / clock settings are copied onto
    /// the rules new games of that kind start with (<see cref="GameManager.DefaultRuleSets"/>,
    /// <see cref="PlayerSettings.ApplyTo"/>), so a game started after the edit already plays by
    /// them. The settings as they were when the screen opened (or last saved) are kept:
    /// <b>save</b> writes <c>settings.ini</c> and makes the current values the new baseline;
    /// leaving without saving asks to discard, and discarding puts the baseline back (live
    /// instance and rules). Settings that are not implemented yet keep their value for the
    /// session only (<see cref="UnimplementedSettings"/>) and are not part of this. The
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

        /// <summary>The settings as opened / last saved.</summary>
        private PlayerSettings _baseline = PlayerSettings.Defaults;

        public UISettingsMenuHandler() { }

        /// <summary>Whether the live settings differ from the last saved / opened ones.</summary>
        public bool HasUnsavedChanges => !PlayerSettingsFile.AreEqual(_live, _baseline);

        /// <summary>The shown screen's tabs (<see cref="SettingsMenuContent.PagesFor"/>).</summary>
        private IReadOnlyList<SettingsMenuPage> _pages = Array.Empty<SettingsMenuPage>();

        /// <summary>Screen opened: take the baseline, set up the tabs and show the initial one.</summary>
        public void OnEnter()
        {
            _live = _factory.ServiceProvider.GetService<PlayerSettings>() ?? _live;
            _baseline = PlayerSettingsFile.Clone(_live);

            _pages = SettingsMenuContent.PagesFor(Element.Screen);
            int initial = Math.Clamp(Element.InitialTab, 0, Math.Max(0, _pages.Count - 1));
            Element.SetTabs(_pages.Select(p => p.Title), initial);
            if (_pages.Count > 0)
                Element.ShowPage(_pages[initial], _live);
        }

        /// <summary>A tab selected: show its rows.</summary>
        public void SelectTab(int index)
        {
            if (index >= 0 && index < _pages.Count)
                Element.ShowPage(_pages[index], _live);
        }

        /// <summary>
        /// Screen closed. Normally nothing is unsaved here (leaving asks first, see
        /// <see cref="UIMainMenuHandler.SwitchSubmenu"/>); if something still is (the whole
        /// main menu was left), it is discarded so the live settings match the file.
        /// </summary>
        public void OnExit() => DiscardChanges();

        /// <summary>A switch flipped: set the setting and apply it live.</summary>
        public void SetToggle(SettingsToggleItem item, bool value)
        {
            item.Set(_live, value);
            ApplyChange();
        }

        /// <summary>A choice's button clicked: select the next choice and apply it live.</summary>
        public void CycleChoice(SettingsChoiceItem item)
        {
            item.Cycle(_live);
            ApplyChange();
        }

        /// <summary>A dropdown's option chosen: set the setting to choice <paramref name="index"/> (kept within the options) and apply it live.</summary>
        public void SetChoice(SettingsChoiceItem item, int index)
        {
            item.SetIndex(_live, Math.Clamp(index, 0, item.Options.Count - 1));
            ApplyChange();
        }

        /// <summary>A slider moved (also while dragging): set the setting (kept within its range) and apply it live.</summary>
        public void SetNumber(SettingsNumberItem item, float value)
        {
            item.Set(_live, Math.Clamp(value, item.Min, item.Max));
            ApplyChange();
        }

        /// <summary>A text field edited: set the setting (cut to its longest length) and apply it live.</summary>
        public void SetText(SettingsTextItem item, string value)
        {
            value ??= string.Empty;
            item.Set(_live, value.Length > item.MaxLength ? value.Substring(0, item.MaxLength) : value);
            ApplyChange();
        }

        /// <summary>After an edit: push the live settings to the game and refresh the rows (e.g. a timer mode change enables / disables the time-limit switches).</summary>
        private void ApplyChange()
        {
            ApplyToGame();
            Element.RefreshValues(_live);
        }

        /// <summary>儲存並返回 clicked: write <c>settings.ini</c>, then close; stays open (with a message) when the write fails.</summary>
        public void SaveAndClose()
        {
            if (!PlayerSettingsFile.Save(_live, SystemSettings.PlayerSettingsFilePath))
            {
                AppLogger.Log($"(Settings) Could not save {SystemSettings.PlayerSettingsFilePath}", LogLevel.ERROR);
                DialogManager.ShowConfirm(GameMenuTexts.SettingsSaveFailed, ConfirmDialogType.Ok, _ => { });
                return;
            }

            _baseline = PlayerSettingsFile.Clone(_live);
            AppLogger.Log("(Settings) Settings saved", LogLevel.INFO);
            Close();
        }

        /// <summary>返回 clicked: close; with unsaved changes ask first.</summary>
        public void BackRequested() => ConfirmDiscard(Close);

        /// <summary>
        /// Runs <paramref name="proceed"/> now when nothing is unsaved; otherwise asks whether
        /// to discard the changes, and discards them and runs it on yes.
        /// </summary>
        public void ConfirmDiscard(System.Action proceed)
        {
            if (!HasUnsavedChanges)
            {
                proceed();
                return;
            }

            DialogManager.ShowConfirm(
                GameMenuTexts.DiscardUnsavedSettings,
                ConfirmDialogType.YesNo,
                result =>
                {
                    if (result != ConfirmDialogResult.Yes)
                        return;
                    DiscardChanges();
                    proceed();
                });
        }

        /// <summary>Puts the baseline back: the live settings, the rules new games start with and the button texts.</summary>
        public void DiscardChanges()
        {
            if (!HasUnsavedChanges)
                return;

            PlayerSettingsFile.Copy(_baseline, _live);
            ApplyToGame();
            Element.RefreshValues(_live);
        }

        /// <summary>
        /// Copies each kind's live rule / clock settings onto the rules new games of that kind
        /// start with, the debug log switch onto the logger (the launchers only push it once at
        /// startup), the player name onto the log greeting / save names, the wheel step onto
        /// the shared scroll handler and the frame rate onto the engine's frame timer (both also
        /// only set at startup otherwise).
        /// </summary>
        private void ApplyToGame()
        {
            var gameManager = _factory.ServiceProvider.GetService<GameManager>();
            if (gameManager != null)
                _live.ApplyTo(gameManager.DefaultRuleSets);

            Settings.EnableDebugMode = _live.ShowDebugLog;
            AppLogger.EnableDebug = Settings.EnableDebugMode;

            Settings.CurrentUser = _live.PlayerName;
            AppLogger.CurrentUser = Settings.CurrentUser;

            if (_factory.ServiceProvider.GetService<IScrollInputHandler>() is ScrollInputHandler scroll)
                scroll.WheelStep = _live.WheelScrollStep;

            TimerSettings.GameAnimationFPS = _live.Fps;
        }

        /// <summary>Closes the screen by selecting its own main menu entry again.</summary>
        private void Close()
        {
            if (Element.Parent is UIMainMenu mainMenu && mainMenu.Handler.CurrentSubmenu is UIMainMenuType current)
                mainMenu.Handler.SwitchSubmenu(current);
        }
    }
}

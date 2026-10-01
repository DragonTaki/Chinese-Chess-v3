/* ----- ----- ----- ----- */
// UISettingsMenuHandler.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/01
// Update Date: 2026/10/01
// Version: v1.0
/* ----- ----- ----- ----- */

using System.Linq;

using Chinese_Chess_v3.Game.Configs;
using Chinese_Chess_v3.Game.Core;
using Chinese_Chess_v3.Game.UI.Constants;
using Chinese_Chess_v3.Game.UI.Dialogs;
using Chinese_Chess_v3.Game.UI.Menus.MainMenu;

using Engine.Logging;
using Engine.UI.Core.Handlers;
using Engine.UI.Core.Interfaces;

using Microsoft.Extensions.DependencyInjection;

namespace Chinese_Chess_v3.Game.UI.Menus.SettingsMenu
{
    /// <summary>
    /// The settings submenu's logic. A click edits the <b>live</b> <see cref="PlayerSettings"/>
    /// (the instance registered in DI) at once: the hint settings are read from it every frame
    /// by the board, and the rule / clock settings are copied onto the rules new games start
    /// with (<see cref="GameManager.DefaultRules"/>, <see cref="PlayerSettings.ApplyTo"/>), so a
    /// game started after the click already plays by them. The settings as they were when the
    /// submenu opened (or last saved) are kept: <b>save</b> writes <c>settings.ini</c> and makes the
    /// current values the new baseline; leaving without saving asks to discard, and discarding
    /// puts the baseline back (live instance and rules). A game already in progress when
    /// opening the settings from the main menu is not expected: the game screen is left first.
    /// <see cref="IScreen"/>: the main menu calls <see cref="OnEnter"/>/<see cref="OnExit"/>
    /// when it opens/closes the submenu.
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

        /// <summary>Submenu opened: take the baseline and list the settings.</summary>
        public void OnEnter()
        {
            _live = _factory.ServiceProvider.GetService<PlayerSettings>() ?? _live;
            _baseline = PlayerSettingsFile.Clone(_live);

            var sections = SettingsMenuContent.Sections.Where(s => Element.Scope == SettingsMenuScope.All || s.IsRules);
            Element.ShowSections(sections, _live);
        }

        /// <summary>
        /// Submenu closed. Normally nothing is unsaved here (leaving asks first, see
        /// <see cref="UIMainMenuHandler.SwitchSubmenu"/>); if something still is (the whole
        /// main menu was left), it is discarded so the live settings match the file.
        /// </summary>
        public void OnExit() => DiscardChanges();

        /// <summary>A setting's button clicked: flip / cycle it and apply it live.</summary>
        public void ChangeItem(SettingsMenuItem item)
        {
            item.Change(_live);
            ApplyToGame();
            Element.RefreshTexts(_live);
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
            Element.RefreshTexts(_live);
        }

        /// <summary>
        /// Copies the live rule / clock settings onto the rules new games start with, and the
        /// debug log switch onto the logger (the launchers only push it once at startup, so
        /// the 顯示 DEBUG 紀錄 toggle did nothing until the next launch).
        /// </summary>
        private void ApplyToGame()
        {
            var gameManager = _factory.ServiceProvider.GetService<GameManager>();
            if (gameManager != null)
                _live.ApplyTo(gameManager.DefaultRules);

            Settings.EnableDebugMode = _live.ShowDebugLog;
            AppLogger.EnableDebug = Settings.EnableDebugMode;
        }

        /// <summary>Closes the submenu by selecting its own main menu entry again.</summary>
        private void Close()
        {
            if (Element.Parent is UIMainMenu mainMenu && mainMenu.Handler.CurrentSubmenu is UIMainMenuType current)
                mainMenu.Handler.SwitchSubmenu(current);
        }
    }
}

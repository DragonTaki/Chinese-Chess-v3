/* ----- ----- ----- ----- */
// SettingsScreenModel.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/05
// Update Date: 2026/10/05
// Version: v1.0
/* ----- ----- ----- ----- */

using System;
using System.Collections.Generic;

using Chinese_Chess_v3.Game.Application.Services;
using Chinese_Chess_v3.Game.Application.Texts;
using Chinese_Chess_v3.Game.Configs;
using Chinese_Chess_v3.Game.Core;

using Engine.Logging;

namespace Chinese_Chess_v3.Game.Application.Settings
{
    /// <summary>
    /// A settings screen's logic (遊戲設定 / 單機規則設定 alike), apart from the view that draws it.
    /// An edit changes the <b>live</b> <see cref="PlayerSettings"/> (the instance registered in
    /// DI) at once and is applied at once (<see cref="ISettingsApplier"/>): the hint settings are
    /// read from it every frame by the board, the player names and the DEBUG tab's switches are
    /// pushed to where they are used, and each game kind's rule / clock settings are copied onto
    /// the rules new games of that kind start with (<see cref="GameManager.DefaultRuleSets"/>), so
    /// a game started after the edit already plays by them. Every edit is also written to
    /// <c>settings.ini</c> at once (there is no save or discard button): switches, dropdowns and
    /// number fields when the value is set, a slider when the mouse is released (not on every
    /// drag step), a text field when its edit ends (<see cref="CommitEdit"/>). A failed write is
    /// logged and reported once per run of failures. The 恢復初始 button resets the shown tab's
    /// settings to the defaults (<see cref="PlayerSettings.Defaults"/>) after a confirmation, then
    /// applies and saves like any edit. Settings that are not implemented yet keep their value for
    /// the session only (<see cref="UnimplementedSettings"/>) and are neither saved nor reset. The
    /// settings are only opened from the main menu, and a game in progress is never affected:
    /// every game plays by its own copy of the rules, taken when it starts
    /// (<see cref="GameManager.Rules"/>).
    /// <para>
    /// The view binds to it: its controls call the edit commands, it refreshes its rows on
    /// <see cref="ValuesChanged"/>, and it shows <see cref="CurrentPage"/> after
    /// <see cref="Enter"/> / <see cref="SelectTab"/>.
    /// </para>
    /// </summary>
    public sealed class SettingsScreenModel
    {
        private readonly IDialogService _dialogs;
        private readonly ISettingsApplier _applier;
        private readonly string _settingsFilePath;

        /// <summary>Whether an edit is applied but not written yet (a slider drag or text edit in progress).</summary>
        private bool _unsaved;

        /// <summary>Whether the last write failed and the failure was already reported (so a run of failing edits shows the message once).</summary>
        private bool _saveFailureShown;

        /// <param name="screen">Which settings screen this is.</param>
        /// <param name="live">The live settings (the DI instance).</param>
        /// <param name="dialogs">The confirm dialogs (save failed, 恢復初始).</param>
        /// <param name="applier">Pushes the settings to where they are used after each edit.</param>
        /// <param name="settingsFilePath">Where the settings are written (null: <see cref="SystemSettings.PlayerSettingsFilePath"/>).</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="screen"/> is not a <see cref="SettingsScreen"/>.</exception>
        public SettingsScreenModel(SettingsScreen screen, PlayerSettings live, IDialogService dialogs, ISettingsApplier applier,
            string settingsFilePath = null)
        {
            Live = live ?? throw new ArgumentNullException(nameof(live));
            _dialogs = dialogs ?? throw new ArgumentNullException(nameof(dialogs));
            _applier = applier ?? throw new ArgumentNullException(nameof(applier));
            _settingsFilePath = settingsFilePath ?? SystemSettings.PlayerSettingsFilePath;
            Screen = screen;
            Pages = SettingsMenuContent.PagesFor(screen);
        }

        /// <summary>Which settings screen this is.</summary>
        public SettingsScreen Screen { get; }

        /// <summary>The live settings the controls show and edit.</summary>
        public PlayerSettings Live { get; }

        /// <summary>The screen's tabs (<see cref="SettingsMenuContent.PagesFor"/>).</summary>
        public IReadOnlyList<SettingsMenuPage> Pages { get; }

        /// <summary>The shown tab's index.</summary>
        public int CurrentTab { get; private set; }

        /// <summary>The shown tab (null when the screen has no tabs).</summary>
        public SettingsMenuPage CurrentPage => CurrentTab >= 0 && CurrentTab < Pages.Count ? Pages[CurrentTab] : null;

        /// <summary>Whether an edit is applied but not written yet (a slider drag or text edit in progress).</summary>
        public bool HasUnsavedEdit => _unsaved;

        /// <summary>An edit or a reset was applied: the view refreshes its controls from <see cref="Live"/> (e.g. a timer mode change enables / disables the time-limit switches).</summary>
        public event Action ValuesChanged;

        /// <summary>Screen opened: shows tab <paramref name="initialTab"/> (kept within the tabs).</summary>
        public void Enter(int initialTab)
        {
            _unsaved = false;
            CurrentTab = Math.Clamp(initialTab, 0, Math.Max(0, Pages.Count - 1));
        }

        /// <summary>A tab selected.</summary>
        /// <returns>Whether <paramref name="index"/> is a tab (and is now shown).</returns>
        public bool SelectTab(int index)
        {
            if (index < 0 || index >= Pages.Count)
                return false;
            CurrentTab = index;
            return true;
        }

        /// <summary>Screen closed: nothing is discarded; an edit still pending (applied, not yet written) is written.</summary>
        public void Exit()
        {
            if (_unsaved)
                SaveNow();
        }

        /// <summary>A switch flipped: set the setting, apply it and write it.</summary>
        public void SetToggle(SettingsToggleItem item, bool value)
        {
            ArgumentNullException.ThrowIfNull(item);
            item.Set(Live, value);
            ApplyChange();
            SaveNow();
        }

        /// <summary>A dropdown's option chosen: set the setting to choice <paramref name="index"/> (kept within the options), apply it and write it.</summary>
        public void SetChoice(SettingsChoiceItem item, int index)
        {
            ArgumentNullException.ThrowIfNull(item);
            item.SetIndex(Live, Math.Clamp(index, 0, item.Options.Count - 1));
            ApplyChange();
            SaveNow();
        }

        /// <summary>A slider moved (also while dragging): set the setting (kept within its range) and apply it; written when the drag ends (<see cref="CommitEdit"/>).</summary>
        public void SetNumber(SettingsNumberItem item, float value)
        {
            ArgumentNullException.ThrowIfNull(item);
            item.Set(Live, Math.Clamp(value, item.Min, item.Max));
            ApplyChange();
            _unsaved = true;
        }

        /// <summary>A number field's edit ended with a legal value: set the setting (kept within its range), apply it and write it.</summary>
        public void SetInteger(SettingsIntegerItem item, int value)
        {
            ArgumentNullException.ThrowIfNull(item);
            item.Set(Live, Math.Clamp(value, item.Min, item.Max));
            ApplyChange();
            SaveNow();
        }

        /// <summary>A text field edited: set the setting (cut to its longest length) and apply it; written when its edit ends (<see cref="CommitEdit"/>).</summary>
        public void SetText(SettingsTextItem item, string value)
        {
            ArgumentNullException.ThrowIfNull(item);
            value ??= string.Empty;
            item.Set(Live, value.Length > item.MaxLength ? value.Substring(0, item.MaxLength) : value);
            ApplyChange();
            _unsaved = true;
        }

        /// <summary>A slider drag or text edit ended: write the settings (once for the whole burst of changes).</summary>
        public void CommitEdit() => SaveNow();

        /// <summary>恢復初始 clicked: after a confirmation, reset the shown tab's settings to the defaults, apply and write them.</summary>
        public void RequestReset()
        {
            var page = CurrentPage;
            if (page == null)
                return;

            _dialogs.ShowConfirm(
                SettingsTexts.ResetTabToDefaults,
                ConfirmDialogType.YesNo,
                result =>
                {
                    if (result != ConfirmDialogResult.Yes)
                        return;
                    page.ResetToDefaults(Live, PlayerSettings.Defaults);
                    ApplyChange();
                    SaveNow();
                });
        }

        /// <summary>After an edit: push the live settings to where they are used, then let the view refresh.</summary>
        private void ApplyChange()
        {
            _applier.Apply(Live);
            ValuesChanged?.Invoke();
        }

        /// <summary>
        /// Writes <c>settings.ini</c>. On failure logs it and shows the save-failed message, but
        /// only for the first failure of a run (until a write succeeds again).
        /// </summary>
        private void SaveNow()
        {
            _unsaved = false;
            if (PlayerSettingsFile.Save(Live, _settingsFilePath))
            {
                _saveFailureShown = false;
                return;
            }

            AppLogger.Log($"(Settings) Could not save {_settingsFilePath}", LogLevel.ERROR);
            if (_saveFailureShown)
                return;
            _saveFailureShown = true;
            _dialogs.ShowConfirm(SettingsTexts.SettingsSaveFailed, ConfirmDialogType.Ok, _ => { });
        }
    }
}

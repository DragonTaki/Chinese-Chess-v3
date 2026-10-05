/* ----- ----- ----- ----- */
// SavedGameCatalog.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/05
// Update Date: 2026/10/05
// Version: v1.1
/* ----- ----- ----- ----- */

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

using Chinese_Chess_v3.Game.Application.GameLog;
using Chinese_Chess_v3.Game.Application.Services;
using Chinese_Chess_v3.Game.Application.Texts;
using Chinese_Chess_v3.Game.Configs;
using Chinese_Chess_v3.Game.Core;
using Chinese_Chess_v3.Game.Core.Saves;

using Engine.Logging;

namespace Chinese_Chess_v3.Game.Application.Catalogs
{
    /// <summary>
    /// The player's saved games (棋譜存檔) in <see cref="Folder"/>, for both save lists: the game
    /// screen's (載入) as a category list (<see cref="List"/>, categories = the mode folders) and
    /// the main menu's (讀取存檔) grouped by mode, newest first (<see cref="Group"/>). Also reads
    /// a save's name and time from its file name for the buttons (<see cref="TryGetNameAndTime"/>),
    /// and deletes a save after asking (<see cref="ConfirmDelete"/>).
    /// A single long-lived instance, so the switched-off categories are kept while the game runs.
    /// </summary>
    public sealed class SavedGameCatalog
    {
        private readonly IDialogService _dialogs;
        private readonly GameLogComposer _gameLog;

        /// <summary>Creates the catalog; nothing is loaded until <c>List.Reload</c> / <see cref="LoadAll"/>.</summary>
        /// <param name="dialogs">Asks before a save is deleted.</param>
        /// <param name="gameLog">The game log, which shows a failed delete (like a failed save).</param>
        public SavedGameCatalog(IDialogService dialogs, GameLogComposer gameLog)
        {
            _dialogs = dialogs ?? throw new ArgumentNullException(nameof(dialogs));
            _gameLog = gameLog ?? throw new ArgumentNullException(nameof(gameLog));
            List = new CategoryListModel<SavedGame>(LoadAll);
        }

        /// <summary>The saves folder (<see cref="SystemSettings.SavesFolder"/>), created by <see cref="LoadAll"/> when missing.</summary>
        public static string Folder => SystemSettings.SavesFolder;

        /// <summary>The saves as a category list (categories = the mode folders, no sections).</summary>
        public CategoryListModel<SavedGame> List { get; }

        /// <summary>Every save (<see cref="GameSaveFiles.LoadAll"/>; bad files skipped, one line each in <paramref name="warnings"/>).</summary>
        public static List<SavedGame> LoadAll(List<string> warnings) => GameSaveFiles.LoadAll(warnings);

        /// <summary>
        /// <paramref name="saves"/> grouped by <see cref="SavedGame.Mode"/> (the <c>[Event]</c>
        /// tag) in <see cref="GameMode"/> order (對局, 殘局, 開局), headed by
        /// <see cref="SystemSettings.SaveModeName"/>; empty groups left out. Within a group
        /// newest first by <see cref="SaveTime"/>, then file name (ordinal, descending: the
        /// later <c>_2</c> of two saves in one second first). Null saves are left out.
        /// </summary>
        public static IReadOnlyList<(string Header, IReadOnlyList<SavedGame> Saves)> Group(IEnumerable<SavedGame> saves)
        {
            ArgumentNullException.ThrowIfNull(saves);
            var list = saves.Where(s => s != null).ToList();
            var groups = new List<(string Header, IReadOnlyList<SavedGame> Saves)>();
            foreach (var mode in Enum.GetValues<GameMode>())
            {
                var inMode = list.Where(s => s.Mode == mode)
                    .OrderByDescending(SaveTime)
                    .ThenByDescending(s => s.FileName, StringComparer.Ordinal)
                    .ToList();
                if (inMode.Count > 0)
                    groups.Add((SystemSettings.SaveModeName(mode), inMode));
            }
            return groups;
        }

        /// <summary>
        /// When <paramref name="saved"/> was saved: the time stamp of its file name
        /// (<see cref="SystemSettings.TryParseSaveFileName"/>), else its <c>[Date]</c> tag
        /// (<c>yyyy.MM.dd</c>), else <see cref="DateTime.MinValue"/> (listed last).
        /// </summary>
        public static DateTime SaveTime(SavedGame saved)
        {
            if (saved == null)
                return DateTime.MinValue;
            if (SystemSettings.TryParseSaveFileName(saved.FileName, out _, out var time))
                return time;
            if (DateTime.TryParseExact(saved.Date, "yyyy.MM.dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
                return date;
            return DateTime.MinValue;
        }

        /// <summary>
        /// The name part and time stamp of <paramref name="saved"/>'s title (the file name
        /// without <c>.pgn</c>, <see cref="SystemSettings.TryParseSaveFileName"/>), for its
        /// button; false for a file named otherwise (the button then shows the title).
        /// </summary>
        public static bool TryGetNameAndTime(SavedGame saved, out string name, out DateTime time) =>
            SystemSettings.TryParseSaveFileName(saved?.Title, out name, out time);

        /// <summary>
        /// Deletes <paramref name="saved"/>'s file (<see cref="GameSaveFiles.Delete"/>). A write
        /// error (IO, access) is returned, not thrown.
        /// </summary>
        /// <param name="saved">A save from the list (loaded from a file).</param>
        /// <param name="error">Why the file could not be deleted; null on success.</param>
        /// <returns>True when the file is gone.</returns>
        /// <exception cref="ArgumentException"><paramref name="saved"/> was not loaded from a file.</exception>
        public static bool TryDelete(SavedGame saved, out string error)
        {
            try
            {
                GameSaveFiles.Delete(saved);
                error = null;
                return true;
            }
            catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException)
            {
                error = ex.Message;
                return false;
            }
        }

        /// <summary>
        /// A save chosen for deleting: asks first (<see cref="GameTexts.DeleteSaveConfirm"/>, yes /
        /// no); yes deletes its file (<see cref="TryDelete"/>; a failure is reported like a failed
        /// save: the debug log, and <see cref="GameTexts.DeleteSaveFailed"/> in the game log),
        /// then calls <paramref name="onDone"/> so the list reloads. No does nothing.
        /// </summary>
        /// <param name="saved">The save to delete.</param>
        /// <param name="onDone">Called after a delete was tried (also when it failed: the list then shows what is really on disk).</param>
        /// <param name="reportFailureInDialog">Also show a failure in an Ok dialog (the main menu, which has no game log).</param>
        public void ConfirmDelete(SavedGame saved, Action onDone, bool reportFailureInDialog = false)
        {
            ArgumentNullException.ThrowIfNull(saved);
            _dialogs.ShowConfirm(
                GameTexts.DeleteSaveConfirm,
                ConfirmDialogType.YesNo,
                result =>
                {
                    if (result != ConfirmDialogResult.Yes)
                        return;

                    if (TryDelete(saved, out string error))
                    {
                        AppLogger.Log($"(Delete) Deleted {saved.FilePath}", LogLevel.DEBUG);
                    }
                    else
                    {
                        AppLogger.Log($"(Delete) Cannot delete {saved.FilePath}: {error}", LogLevel.ERROR);
                        _gameLog.Write(GameTexts.DeleteSaveFailed(error));
                        if (reportFailureInDialog)
                            _dialogs.ShowConfirm(GameTexts.DeleteSaveFailedDialog(error), ConfirmDialogType.Ok, _ => { });
                    }
                    onDone?.Invoke();
                });
        }
    }
}

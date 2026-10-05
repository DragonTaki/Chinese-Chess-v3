/* ----- ----- ----- ----- */
// SavedGameCatalog.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/05
// Update Date: 2026/10/05
// Version: v1.0
/* ----- ----- ----- ----- */

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

using Chinese_Chess_v3.Game.Configs;
using Chinese_Chess_v3.Game.Core;
using Chinese_Chess_v3.Game.Core.Saves;

namespace Chinese_Chess_v3.Game.Application.Catalogs
{
    /// <summary>
    /// The player's saved games (棋譜存檔) in <see cref="Folder"/>, for both save lists: the game
    /// screen's (載入) as a category list (<see cref="List"/>, categories = the mode folders) and
    /// the main menu's (讀取存檔) grouped by mode, newest first (<see cref="Group"/>). Also reads
    /// a save's name and time from its file name for the buttons (<see cref="TryGetNameAndTime"/>).
    /// A single long-lived instance, so the switched-off categories are kept while the game runs.
    /// </summary>
    public sealed class SavedGameCatalog
    {
        /// <summary>Creates the catalog; nothing is loaded until <c>List.Reload</c> / <see cref="LoadAll"/>.</summary>
        public SavedGameCatalog()
        {
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
    }
}

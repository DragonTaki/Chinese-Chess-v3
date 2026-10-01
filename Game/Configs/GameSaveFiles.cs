/* ----- ----- ----- ----- */
// GameSaveFiles.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/01
// Update Date: 2026/10/01
// Version: v1.0
/* ----- ----- ----- ----- */

using System;
using System.Collections.Generic;
using System.IO;

using Chinese_Chess_v3.Game.Core;
using Chinese_Chess_v3.Game.Core.Saves;

namespace Chinese_Chess_v3.Game.Configs
{
    /// <summary>
    /// Saving and listing games in the game's saves folder, by the location and naming rules
    /// of <see cref="SystemSettings"/> (docs/SETTINGS.md). The PGN format itself is Core's
    /// (<see cref="SavedGamePgn"/>); loading a listed game into play is
    /// <see cref="GameManager.LoadSavedGame"/>.
    /// </summary>
    public static class GameSaveFiles
    {
        /// <summary>
        /// Saves <paramref name="game"/> to <see cref="SystemSettings.SaveFilePath"/> for
        /// <paramref name="time"/> (null: now); when that file already exists (two saves in one
        /// second) <c>_2</c>, <c>_3</c>, ... is added before <c>.pgn</c>. Player names default
        /// to <see cref="Settings.CurrentUser"/> for both sides (local play: one person plays
        /// both colors). Clears <see cref="GameManager.HasUnsavedChanges"/>.
        /// </summary>
        /// <returns>The full path of the written file.</returns>
        /// <exception cref="InvalidOperationException">The game cannot be saved (<see cref="GameManager.CanSave"/>).</exception>
        /// <exception cref="IOException">The file cannot be written (also <see cref="UnauthorizedAccessException"/>).</exception>
        public static string Save(GameManager game, string redName = null, string blackName = null, DateTime? time = null)
        {
            ArgumentNullException.ThrowIfNull(game);
            var now = time ?? DateTime.Now;
            string path = UniquePath(SystemSettings.SaveFilePath(game, now));
            return game.SaveGame(path, redName ?? Settings.CurrentUser, blackName ?? Settings.CurrentUser, now);
        }

        /// <summary>
        /// Every saved game under <see cref="SystemSettings.SavesFolder"/> (created when
        /// missing), sorted by category = mode folder (<see cref="SystemSettings.SaveModeName"/>),
        /// then by file name (oldest first within a mode and name). Bad files are skipped with a
        /// warning.
        /// </summary>
        public static List<SavedGame> LoadAll(List<string> warnings = null) =>
            SavedGameLoader.LoadFolder(SystemSettings.SavesFolder, createIfMissing: true, warnings);

        /// <summary><paramref name="path"/>, or the first free <c>name_N.pgn</c> next to it when it exists.</summary>
        private static string UniquePath(string path)
        {
            if (!File.Exists(path))
                return path;

            string folder = Path.GetDirectoryName(path) ?? string.Empty;
            string name = Path.GetFileNameWithoutExtension(path);
            string extension = Path.GetExtension(path);
            for (int i = 2; ; i++)
            {
                string candidate = Path.Combine(folder, $"{name}_{i}{extension}");
                if (!File.Exists(candidate))
                    return candidate;
            }
        }
    }
}

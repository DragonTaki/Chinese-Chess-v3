/* ----- ----- ----- ----- */
// GameSaveFiles.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/01
// Update Date: 2026/10/05
// Version: v1.0
/* ----- ----- ----- ----- */

using System;
using System.Collections.Generic;
using System.IO;

using Chinese_Chess_v3.Game.Core;
using Chinese_Chess_v3.Game.Core.Pieces;
using Chinese_Chess_v3.Game.Core.Players;
using Chinese_Chess_v3.Game.Core.Saves;

namespace Chinese_Chess_v3.Game.Configs
{
    /// <summary>
    /// Saving, listing and deleting games in the game's saves folder, by the location and naming rules
    /// of <see cref="SystemSettings"/>. The PGN format itself is Core's
    /// (<see cref="SavedGamePgn"/>); loading a listed game into play is
    /// <see cref="GameManager.LoadSavedGame"/>.
    /// </summary>
    public static class GameSaveFiles
    {
        /// <summary>
        /// Saves <paramref name="game"/> to <see cref="SystemSettings.SaveFilePath"/> for
        /// <paramref name="time"/> (null: now); when that file already exists (two saves in one
        /// second) <c>_2</c>, <c>_3</c>, ... is added before <c>.pgn</c>. Player names default
        /// to the local players' names (<see cref="GameManager.NameOf"/>, mapped to the colour
        /// each plays), or <paramref name="unnamedPlayerName"/> for an unnamed one. Clears
        /// <see cref="GameManager.HasUnsavedChanges"/>.
        /// </summary>
        /// <param name="game">The game to save.</param>
        /// <param name="unnamedPlayerName">The name of a local player without one (the player's name, <see cref="PlayerSettings.PlayerName"/>).</param>
        /// <param name="redName">Red's name (null: from the game).</param>
        /// <param name="blackName">Black's name (null: from the game).</param>
        /// <param name="time">The save time in the file name and tags (null: now).</param>
        /// <returns>The full path of the written file.</returns>
        /// <exception cref="InvalidOperationException">The game cannot be saved (<see cref="GameManager.CanSave"/>).</exception>
        /// <exception cref="IOException">The file cannot be written (also <see cref="UnauthorizedAccessException"/>).</exception>
        public static string Save(GameManager game, string unnamedPlayerName, string redName = null, string blackName = null, DateTime? time = null)
        {
            ArgumentNullException.ThrowIfNull(game);
            var now = time ?? DateTime.Now;
            string path = UniquePath(SystemSettings.SaveFilePath(game, now));
            var redSide = game.ColorOf(PlayerSide.Player1) == PieceColor.Red ? PlayerSide.Player1 : PlayerSide.Player2;
            var blackSide = redSide == PlayerSide.Player1 ? PlayerSide.Player2 : PlayerSide.Player1;
            redName ??= game.NameOf(redSide) ?? unnamedPlayerName;
            blackName ??= game.NameOf(blackSide) ?? unnamedPlayerName;
            return game.SaveGame(path, redName, blackName, now);
        }

        /// <summary>
        /// Every saved game under <see cref="SystemSettings.SavesFolder"/> (created when
        /// missing), sorted by category = mode folder (<see cref="SystemSettings.SaveModeName"/>),
        /// then by file name (oldest first within a mode and name). Bad files are skipped with a
        /// warning.
        /// </summary>
        public static List<SavedGame> LoadAll(List<string> warnings = null) =>
            SavedGameLoader.LoadFolder(SystemSettings.SavesFolder, createIfMissing: true, warnings);

        /// <summary>
        /// Deletes <paramref name="saved"/>'s file (<see cref="Core.Pgn.PgnGameFile.FilePath"/>).
        /// A file that is already gone is not an error (<see cref="File.Delete"/>).
        /// </summary>
        /// <exception cref="ArgumentException"><paramref name="saved"/> was not loaded from a file (no <c>FilePath</c>).</exception>
        /// <exception cref="IOException">The file cannot be deleted, e.g. it is open elsewhere or its folder is gone (also <see cref="UnauthorizedAccessException"/>).</exception>
        public static void Delete(SavedGame saved)
        {
            ArgumentNullException.ThrowIfNull(saved);
            if (string.IsNullOrEmpty(saved.FilePath))
                throw new ArgumentException("The saved game was not loaded from a file", nameof(saved));
            File.Delete(saved.FilePath);
        }

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

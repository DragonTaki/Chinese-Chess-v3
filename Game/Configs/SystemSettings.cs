/* ----- ----- ----- ----- */
// SystemSettings.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/01
// Update Date: 2026/10/01
// Version: v1.1
/* ----- ----- ----- ----- */

using System;
using System.Globalization;
using System.IO;
using System.Text;

using Chinese_Chess_v3.Game.Core;
using Chinese_Chess_v3.Game.Core.Boards;

using Engine.Configs;

namespace Chinese_Chess_v3.Game.Configs
{
    /// <summary>
    /// Game-level system defaults: fixed values the player never changes (paths, the
    /// window title, the random table). The single place for them - callers reference
    /// these instead of repeating literals. Engine-level ones are in
    /// <see cref="EnginePaths"/>; the player's own settings are <c>PlayerSettings</c>
    /// (docs/SETTINGS.md).
    /// </summary>
    public static class SystemSettings
    {
        /// <summary>Window title (both launchers).</summary>
        public const string WindowTitle = "Chinese Chess v3 - created by @DragonTaki";

        #region Per-user data

        /// <summary>Folder name of the game under the per-user application data folder.</summary>
        public const string AppDataFolderName = "Chinese-Chess-v3";

        /// <summary>
        /// The game's per-user data folder: <c>Chinese-Chess-v3</c> under
        /// <see cref="Environment.SpecialFolder.ApplicationData"/> (Windows:
        /// <c>%APPDATA%\Chinese-Chess-v3</c>; macOS/Linux: <c>~/.config/Chinese-Chess-v3</c>).
        /// </summary>
        public static string UserDataFolder =>
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), AppDataFolderName);

        /// <summary>File name of the player settings file (<c>PlayerSettingsFile</c>).</summary>
        public const string PlayerSettingsFileName = "settings.ini";

        /// <summary>The player settings file: <c>settings.ini</c> in <see cref="UserDataFolder"/>.</summary>
        public static string PlayerSettingsFilePath => Path.Combine(UserDataFolder, PlayerSettingsFileName);

        #endregion

        #region Endgames

        /// <summary>Folder name of the endgame puzzles, both built-in (under <c>Assets/</c>) and the player's own (under <see cref="UserDataFolder"/>).</summary>
        public const string EndgamesFolderName = "Endgames";

        /// <summary>The built-in puzzles: <c>Assets/Endgames</c> next to the game (copied there by the build).</summary>
        public static string BuiltInEndgameFolder => Path.Combine(EnginePaths.AssetsFolder, EndgamesFolderName);

        /// <summary>
        /// The player's own puzzles when the settings do not override it
        /// (<c>PlayerSettings.EndgameUserFolder</c> empty): <c>Endgames</c> in
        /// <see cref="UserDataFolder"/>.
        /// </summary>
        public static string DefaultUserEndgameFolder => Path.Combine(UserDataFolder, EndgamesFolderName);

        #endregion

        #region Openings

        /// <summary>Folder name of the openings (開局練習), both built-in (under <c>Assets/</c>) and the player's own (under <see cref="UserDataFolder"/>).</summary>
        public const string OpeningsFolderName = "Openings";

        /// <summary>The built-in openings: <c>Assets/Openings</c> next to the game (copied there by the build).</summary>
        public static string BuiltInOpeningFolder => Path.Combine(EnginePaths.AssetsFolder, OpeningsFolderName);

        /// <summary>
        /// The player's own openings when the settings do not override it
        /// (<c>PlayerSettings.OpeningUserFolder</c> empty): <c>Openings</c> in
        /// <see cref="UserDataFolder"/>.
        /// </summary>
        public static string DefaultUserOpeningFolder => Path.Combine(UserDataFolder, OpeningsFolderName);

        #endregion

        #region Saved games

        /// <summary>Folder name of the saved games (棋譜存檔) under <see cref="UserDataFolder"/>.</summary>
        public const string SavesFolderName = "Saves";

        /// <summary>
        /// The saves root: <c>Saves</c> in <see cref="UserDataFolder"/>. Each mode has its own
        /// folder directly under it (<see cref="SaveModeName"/>), which is also the category
        /// the save list shows.
        /// </summary>
        public static string SavesFolder => Path.Combine(UserDataFolder, SavesFolderName);

        /// <summary>
        /// The name of a game mode in the saves: the mode folder under <see cref="SavesFolder"/>
        /// and the first part of the file name (<see cref="SaveFileName"/>).
        /// </summary>
        public static string SaveModeName(GameMode mode) => mode switch
        {
            GameMode.Endgame => "殘局",
            GameMode.Opening => "開局",
            _ => "對局",
        };

        /// <summary>
        /// The name of a board type in a normal game's save file name. 揭棋 (dark chess on the
        /// Full board) has no mode of its own yet; <see cref="JieqiSaveName"/> is its name for
        /// when it does.
        /// </summary>
        public static string BoardTypeSaveName(BoardType boardType) => boardType switch
        {
            BoardType.HalfCenter => "半盤",
            BoardType.HalfCross => "三國",
            _ => "大盤",
        };

        /// <summary>The save file name part of a 揭棋 game (no 揭棋 mode exists yet).</summary>
        public const string JieqiSaveName = "揭棋";

        /// <summary>Format of the time stamp in a save file name (local time).</summary>
        public const string SaveTimestampFormat = "yyyyMMdd-HHmmss";

        /// <summary>Replaces an empty name in a save file name.</summary>
        public const string UnnamedSaveName = "未命名";

        /// <summary>
        /// The save file name: <c>&lt;mode&gt;_&lt;name&gt;_&lt;yyyyMMdd-HHmmss&gt;.pgn</c>, e.g.
        /// <c>對局_大盤_20261001-153000.pgn</c> or <c>殘局_七星聚會_20261001-153000.pgn</c>.
        /// <paramref name="name"/> is sanitized (<see cref="SanitizeFileNamePart"/>).
        /// </summary>
        public static string SaveFileName(GameMode mode, string name, DateTime time) =>
            $"{SaveModeName(mode)}_{SanitizeFileNamePart(name)}_{time.ToString(SaveTimestampFormat, CultureInfo.InvariantCulture)}.pgn";

        /// <summary>
        /// The name part of <paramref name="game"/>'s save file: the board type
        /// (<see cref="BoardTypeSaveName"/>) for a normal game, the puzzle / opening title for
        /// an endgame / opening (the board type when the title is unknown).
        /// </summary>
        public static string SaveNameOf(GameManager game) =>
            game.Mode != GameMode.Normal && !string.IsNullOrWhiteSpace(game.OriginTitle)
                ? game.OriginTitle
                : BoardTypeSaveName(game.Board.Type);

        /// <summary>
        /// Where <paramref name="game"/> is saved at <paramref name="time"/>:
        /// <c>&lt;SavesFolder&gt;/&lt;mode&gt;/&lt;SaveFileName&gt;</c>. Does not check whether the
        /// file exists (see <c>GameSaveFiles.Save</c>).
        /// </summary>
        public static string SaveFilePath(GameManager game, DateTime time) =>
            Path.Combine(SavesFolder, SaveModeName(game.Mode), SaveFileName(game.Mode, SaveNameOf(game), time));

        /// <summary>
        /// <paramref name="name"/> made safe as part of a file name on every platform: the
        /// characters Windows forbids (<c>\ / : * ? " &lt; &gt; |</c>) and control characters
        /// become <c>_</c>, surrounding spaces and trailing dots are removed, and an empty
        /// result becomes <see cref="UnnamedSaveName"/>.
        /// </summary>
        public static string SanitizeFileNamePart(string name)
        {
            var sb = new StringBuilder();
            foreach (char c in name ?? string.Empty)
                sb.Append(char.IsControl(c) || "\\/:*?\"<>|".IndexOf(c) >= 0 ? '_' : c);
            string result = sb.ToString().Trim().TrimEnd('.').Trim();
            return result.Length == 0 ? UnnamedSaveName : result;
        }

        #endregion

        #region Random table

        /// <summary>Size of the shared <c>RandomTable</c> registered by the launchers.</summary>
        public const int RandomTableSize = 10000;

        /// <summary>Fixed seed of the shared <c>RandomTable</c> (reproducible sequences).</summary>
        public const int RandomTableSeed = 12345;

        #endregion
    }
}

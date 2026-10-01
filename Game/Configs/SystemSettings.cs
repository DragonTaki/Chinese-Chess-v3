/* ----- ----- ----- ----- */
// SystemSettings.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/01
// Update Date: 2026/10/01
// Version: v1.0
/* ----- ----- ----- ----- */

using System;
using System.IO;

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

        #region Random table

        /// <summary>Size of the shared <c>RandomTable</c> registered by the launchers.</summary>
        public const int RandomTableSize = 10000;

        /// <summary>Fixed seed of the shared <c>RandomTable</c> (reproducible sequences).</summary>
        public const int RandomTableSeed = 12345;

        #endregion
    }
}

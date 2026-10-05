/* ----- ----- ----- ----- */
// FolderSettings.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/05
// Update Date: 2026/10/05
// Version: v1.0
/* ----- ----- ----- ----- */

using System;
using System.Collections.Generic;

using Engine.Configs;

namespace Chinese_Chess_v3.Game.Configs
{
    /// <summary>
    /// The settings area of the player's own folders (<c>[endgame]</c>, <c>[opening]</c>). Read
    /// live by the lists' catalogs when they load, so <see cref="Apply"/> has nothing to push.
    /// </summary>
    public sealed class FolderSettings : ISettingsArea
    {
        private static readonly FolderSettings Default = new();

        /// <summary>
        /// Folder of the player's own endgame puzzles; empty for the default
        /// (<see cref="SystemSettings.DefaultUserEndgameFolder"/>). A relative path is taken
        /// relative to <see cref="SystemSettings.UserDataFolder"/>. Default: "" (empty)
        /// </summary>
        public string EndgameUserFolder { get; set; } = string.Empty;

        /// <summary>The player's endgame folder actually used: <see cref="EndgameUserFolder"/> resolved, or the default when empty.</summary>
        public string ResolvedEndgameUserFolder =>
            string.IsNullOrWhiteSpace(EndgameUserFolder)
                ? SystemSettings.DefaultUserEndgameFolder
                : ResolveUserFolder(EndgameUserFolder);

        /// <summary>
        /// Folder of the player's own openings (開局練習); empty for the default
        /// (<see cref="SystemSettings.DefaultUserOpeningFolder"/>). A relative path is taken
        /// relative to <see cref="SystemSettings.UserDataFolder"/>. Default: "" (empty)
        /// </summary>
        public string OpeningUserFolder { get; set; } = string.Empty;

        /// <summary>The player's opening folder actually used: <see cref="OpeningUserFolder"/> resolved, or the default when empty.</summary>
        public string ResolvedOpeningUserFolder =>
            string.IsNullOrWhiteSpace(OpeningUserFolder)
                ? SystemSettings.DefaultUserOpeningFolder
                : ResolveUserFolder(OpeningUserFolder);

        /// <summary>A folder setting: environment variables expanded, relative to <see cref="SystemSettings.UserDataFolder"/>.</summary>
        private static string ResolveUserFolder(string folder) =>
            System.IO.Path.GetFullPath(Environment.ExpandEnvironmentVariables(folder.Trim()), SystemSettings.UserDataFolder);

        private IReadOnlyList<SettingsKey> _keys;

        /// <inheritdoc/>
        public IReadOnlyList<SettingsKey> Keys => _keys ??= new[]
        {
            SettingsKey.Path("endgame", "user_folder", () => EndgameUserFolder, v => EndgameUserFolder = v, Default.EndgameUserFolder,
                () => SystemSettings.UserDataFolder,
                "自己的殘局題目資料夾；留空 = 預設位置（這個設定檔旁邊的 Endgames 資料夾）。",
                "相對路徑以這個設定檔所在的資料夾為準；可以用 %環境變數%。"),
            SettingsKey.Path("opening", "user_folder", () => OpeningUserFolder, v => OpeningUserFolder = v, Default.OpeningUserFolder,
                () => SystemSettings.UserDataFolder,
                "自己的開局練習資料夾；留空 = 預設位置（這個設定檔旁邊的 Openings 資料夾）。",
                "相對路徑以這個設定檔所在的資料夾為準；可以用 %環境變數%。"),
        };

        /// <summary>Nothing to push: the catalogs read the folders when they load.</summary>
        public void Apply() { }
    }
}

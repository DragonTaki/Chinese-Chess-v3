/* ----- ----- ----- ----- */
// AiSettings.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/12
// Update Date: 2026/10/12
// Version: v1.0
/* ----- ----- ----- ----- */

using System;
using System.Collections.Generic;

using Chinese_Chess_v3.Game.Ai;

using Engine.Configs;

namespace Chinese_Chess_v3.Game.Configs
{
    /// <summary>Which side the human plays against the computer (人機對戰).</summary>
    public enum AiPlayerSide
    {
        /// <summary>先手: the human moves first.</summary>
        First,

        /// <summary>後手: the computer moves first.</summary>
        Second,

        /// <summary>隨機: picked at random for each game.</summary>
        Random,
    }

    /// <summary>
    /// The settings area of the computer opponent (<c>[ai]</c>): the human's side, the computer's
    /// strength and the folder for the player's own engines and data. Read live when a game
    /// against the computer starts and before each search, so <see cref="Apply"/> has nothing to push.
    /// </summary>
    public sealed class AiSettings : ISettingsArea
    {
        private static readonly AiSettings Default = new();

        /// <summary>The side the human plays. Default: <see cref="AiPlayerSide.First"/></summary>
        public AiPlayerSide PlayerSide { get; set; } = AiPlayerSide.First;

        /// <summary>The computer's strength. Default: <see cref="EngineLevel.SearchDepth4"/></summary>
        public EngineLevel Level { get; set; } = EngineLevel.SearchDepth4;

        /// <summary>
        /// Folder of the player's own AI engines and data; empty for the default
        /// (<see cref="SystemSettings.DefaultUserAiFolder"/>). A relative path is taken
        /// relative to <see cref="SystemSettings.UserDataFolder"/>. Default: "" (empty)
        /// </summary>
        public string UserFolder { get; set; } = string.Empty;

        /// <summary>The player's AI folder actually used: <see cref="UserFolder"/> resolved, or the default when empty.</summary>
        public string ResolvedUserFolder =>
            string.IsNullOrWhiteSpace(UserFolder)
                ? SystemSettings.DefaultUserAiFolder
                : System.IO.Path.GetFullPath(Environment.ExpandEnvironmentVariables(UserFolder.Trim()), SystemSettings.UserDataFolder);

        private IReadOnlyList<SettingsKey> _keys;

        /// <inheritdoc/>
        public IReadOnlyList<SettingsKey> Keys => _keys ??= new[]
        {
            SettingsKey.Enum("ai", "player_side", () => PlayerSide, v => PlayerSide = v, Default.PlayerSide,
                "人機對戰：玩家執哪一方（First = 先手、Second = 後手、Random = 每局隨機）。"),
            SettingsKey.Enum("ai", "level", () => Level, v => Level = v, Default.Level,
                "人機對戰：電腦的強度（Random、Greedy、SearchDepth2、SearchDepth4、Search500Ms、Search2000Ms；由弱到強）。"),
            SettingsKey.Path("ai", "user_folder", () => UserFolder, v => UserFolder = v, Default.UserFolder,
                () => SystemSettings.UserDataFolder,
                "自己的 AI 引擎與資料夾；留空 = 預設位置（這個設定檔旁邊的 AI 資料夾）。",
                "相對路徑以這個設定檔所在的資料夾為準；可以用 %環境變數%。"),
        };

        /// <summary>Nothing to push: the computer opponent reads the values when it needs them.</summary>
        public void Apply() { }
    }
}

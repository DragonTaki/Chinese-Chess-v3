/* ----- ----- ----- ----- */
// PlayerNameSettings.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/05
// Update Date: 2026/10/05
// Version: v1.0
/* ----- ----- ----- ----- */

using System;
using System.Collections.Generic;

using Chinese_Chess_v3.Game.Core;

using Engine.Configs;
using Engine.Logging;

namespace Chinese_Chess_v3.Game.Configs
{
    /// <summary>
    /// The settings area of the players' names (<c>[player]</c>): the name in the log's greeting
    /// and the local players' names. <see cref="Apply"/> sets the greeting name and raises
    /// <see cref="Applied"/>, on which the composition root gives a running game the names
    /// (<see cref="ApplyPlayerNamesTo"/>); a new game takes them when it is created.
    /// </summary>
    public sealed class PlayerNameSettings : ISettingsArea
    {
        private static readonly PlayerNameSettings Default = new();

        /// <summary>Longest name (characters).</summary>
        public const int PlayerNameMaxLength = 32;

        /// <summary>Name shown in the log's greeting (at most <see cref="PlayerNameMaxLength"/> characters). Default: "Player"</summary>
        public string PlayerName { get; set; } = "Player";

        /// <summary>Local Player1's name (先手, 玩家一; at most <see cref="PlayerNameMaxLength"/> characters). Default: "玩家一"</summary>
        public string Player1Name { get; set; } = "玩家一";

        /// <summary>Local Player2's name (後手, 玩家二). Default: "玩家二"</summary>
        public string Player2Name { get; set; } = "玩家二";

        /// <summary>Local Player3's name (三國's third player, 玩家三). Default: "玩家三"</summary>
        public string Player3Name { get; set; } = "玩家三";

        /// <summary>Raised at the end of <see cref="Apply"/> (the names changed or were read).</summary>
        public event Action Applied;

        private IReadOnlyList<SettingsKey> _keys;

        /// <inheritdoc/>
        public IReadOnlyList<SettingsKey> Keys => _keys ??= new[]
        {
            SettingsKey.Text("player", "name", () => PlayerName, v => PlayerName = v, Default.PlayerName, PlayerNameMaxLength,
                $"玩家名稱，用在紀錄區的問候語（可留空，最多 {PlayerNameMaxLength} 字）。"),
            SettingsKey.Text("player", "player1_name", () => Player1Name, v => Player1Name = v, Default.Player1Name, PlayerNameMaxLength,
                $"單機對局玩家一（先手）的名稱，顯示在資訊看板、結果視窗與存檔（可留空＝「玩家一」，最多 {PlayerNameMaxLength} 字）。"),
            SettingsKey.Text("player", "player2_name", () => Player2Name, v => Player2Name = v, Default.Player2Name, PlayerNameMaxLength,
                $"單機對局玩家二（後手）的名稱（可留空＝「玩家二」，最多 {PlayerNameMaxLength} 字）。"),
            SettingsKey.Text("player", "player3_name", () => Player3Name, v => Player3Name = v, Default.Player3Name, PlayerNameMaxLength,
                $"單機對局玩家三（三國）的名稱（可留空＝「玩家三」，最多 {PlayerNameMaxLength} 字）。"),
        };

        /// <summary>Sets the log's greeting name (<see cref="AppLogger.CurrentUser"/>), then raises <see cref="Applied"/>.</summary>
        public void Apply()
        {
            AppLogger.CurrentUser = PlayerName;
            Applied?.Invoke();
        }

        /// <summary>Gives <paramref name="game"/> the local players' names (<see cref="GameManager.SetPlayerNames"/>).</summary>
        /// <exception cref="ArgumentNullException"><paramref name="game"/> is null.</exception>
        public void ApplyPlayerNamesTo(GameManager game)
        {
            ArgumentNullException.ThrowIfNull(game);
            game.SetPlayerNames(Player1Name, Player2Name, Player3Name);
        }
    }
}

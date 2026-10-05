/* ----- ----- ----- ----- */
// HintSettings.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/05
// Update Date: 2026/10/05
// Version: v1.0
/* ----- ----- ----- ----- */

using System.Collections.Generic;

using Engine.Configs;

namespace Chinese_Chess_v3.Game.Configs
{
    /// <summary>
    /// The settings area of the board hints (<c>[hints]</c>). Read live by the board's view model,
    /// so <see cref="Apply"/> has nothing to push.
    /// </summary>
    public sealed class HintSettings : ISettingsArea
    {
        private static readonly HintSettings Default = new();

        /// <summary>Rings on the selected piece's legal destinations. Default: true</summary>
        public bool ShowLegalMoveHints { get; set; } = true;

        /// <summary>Rings around hanging pieces (無根子可被吃). Default: true</summary>
        public bool ShowHangingPieceHints { get; set; } = true;

        private IReadOnlyList<SettingsKey> _keys;

        /// <inheritdoc/>
        public IReadOnlyList<SettingsKey> Keys => _keys ??= new[]
        {
            SettingsKey.Bool("hints", "legal_moves", () => ShowLegalMoveHints, v => ShowLegalMoveHints = v, Default.ShowLegalMoveHints,
                "選子時是否用圓圈標出可以走的位置。"),
            SettingsKey.Bool("hints", "hanging_pieces", () => ShowHangingPieceHints, v => ShowHangingPieceHints = v, Default.ShowHangingPieceHints,
                "是否用圓圈標出無根、可被吃的棋子。"),
        };

        /// <summary>Nothing to push: the board reads the values each time.</summary>
        public void Apply() { }
    }
}

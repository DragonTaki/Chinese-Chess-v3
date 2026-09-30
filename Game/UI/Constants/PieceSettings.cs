/* ----- ----- ----- ----- */
// PieceSettings.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/06
// Update Date: 2026/09/30
// Version: v2.1
/* ----- ----- ----- ----- */

using System.Drawing;

using Engine.Platform;
using Engine.Styles;

namespace Chinese_Chess_v3.Game.UI.Constants
{
    /// <summary>
    /// How pieces are drawn: font and colors. Presentation only - nothing in
    /// <c>Game/Core</c> reads it (it used to live in <c>Game/Core/Pieces</c>, which made the
    /// rules layer depend on <c>Engine.Platform</c>/<c>Engine.Styles</c>). The sizes (radius,
    /// margins, outline widths, font size) are layout numbers and live in
    /// <see cref="UILayoutConstants.Board.Piece"/>.
    /// </summary>
    public static class PieceSettings
    {
        // Font
        public static readonly IFont Font = StyleHelper.GetFont("NotoSerif", UILayoutConstants.Board.Piece.FontSize, FontStyleFlags.Bold);

        // Red piece color
        public static readonly IBrush RedTextBrush = StyleHelper.GetBrush("#E83015");  // #E83015
        public static readonly IBrush RedBackgroundBrush = StyleHelper.GetBrush("#FCFAF2");  // #FCFAF2
        public static readonly Color RedOutlineColor = StyleHelper.GetColor("#E83015");  // #E83015
        // Black piece color
        public static readonly IBrush BlackTextBrush = StyleHelper.GetBrush("#FCFAF2");  // #FCFAF2
        public static readonly IBrush BlackBackgroundBrush = StyleHelper.GetBrush("#1C1C1C");  // #1C1C1C
        public static readonly Color BlackOutlineColor = StyleHelper.GetColor("#FCFAF2");  // #FCFAF2
        // Glow color
        public static readonly Color GlowColor = StyleHelper.GetColor("#FFB11B", 0.75f);  // #FFB11B
    }
}

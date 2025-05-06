/* ----- ----- ----- ----- */
// PieceSettings.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/06
// Update Date: 2025/05/06
// Version: v1.0
/* ----- ----- ----- ----- */

using System.Drawing;

using Chinese_Chess_v3.Utils;

namespace Chinese_Chess_v3.Configs
{
    public static class PieceSettings
    {
        // Size
        public const int Radius = 35;
        public const int OuterMargin = 6;

        // Font
        public static readonly Font Font = FontManager.GetFont("NotoSerif", 30, FontStyle.Bold);

        // Red piece color
        public static readonly Brush RedTextBrush = Brushes.Red;
        public static readonly Brush RedBackgroundBrush = Brushes.White;
        public static readonly Color RedOutlineColor = Color.Red;
        // Black piece color
        public static readonly Brush BlackTextBrush = Brushes.White;
        public static readonly Brush BlackBackgroundBrush = Brushes.Black;
        public static readonly Color BlackOutlineColor = Color.White;
    }
}
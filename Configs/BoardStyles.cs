/* ----- ----- ----- ----- */
// BoardStyles.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/06
// Update Date: 2025/05/06
// Version: v1.0
/* ----- ----- ----- ----- */

using System.Drawing;
using System.Drawing.Drawing2D;

public static class BoardStyles
{
    public static Brush CreateBoardBackgroundBrush(Rectangle bounds)
    {
        LinearGradientBrush brush = new LinearGradientBrush(
            bounds,
            Color.FromArgb(7, 81, 138),   // #07518A
            Color.FromArgb(8, 136, 217),  // #0888D9
            LinearGradientMode.Horizontal
        );

        ColorBlend blend = new ColorBlend();
        blend.Positions = new[] { 0.0f, 0.5f, 1.0f };
        blend.Colors = new[]
        {
            Color.FromArgb(7, 81, 138),
            Color.FromArgb(8, 136, 217),
            Color.FromArgb(7, 81, 138)
        };
        brush.InterpolationColors = blend;

        return brush;
    }
}
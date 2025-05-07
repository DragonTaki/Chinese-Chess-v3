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

using Chinese_Chess_v3.Utils;

public static class BoardStyles
{
    public static Brush CreateBoardBackgroundBrush(Rectangle bounds)
    {
        LinearGradientBrush brush = new LinearGradientBrush(
            bounds,
            StyleHelper.GetColor("#07518A"),  // #07518A
            StyleHelper.GetColor("#0888D9"),  // #0888D9
            LinearGradientMode.Horizontal
        );

        ColorBlend blend = new ColorBlend();
        blend.Positions = new[] { 0.0f, 0.5f, 1.0f };
        blend.Colors = new[]
        {
            StyleHelper.GetColor("(7, 81, 138)"),
            StyleHelper.GetColor("(8, 136, 217)"),
            StyleHelper.GetColor("(7, 81, 138)")
        };
        brush.InterpolationColors = blend;

        return brush;
    }
}
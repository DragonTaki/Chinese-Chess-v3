/* ----- ----- ----- ----- */
// BoardStyles.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/06
// Update Date: 2026/09/24
// Version: v2.0
/* ----- ----- ----- ----- */

using System.Drawing;

using Engine.Platform;
using Engine.Styles;

namespace Chinese_Chess_v3.Game.Core.Boards
{
    public static class UIBoardStyles
    {
        public static IBrush CreateBoardBackgroundBrush(RectangleF bounds)
        {
            return GraphicsBackend.Factory.CreateLinearGradientBrush(bounds, GradientDirection.Horizontal, new[]
            {
                (0.0f, StyleHelper.GetColor("(7, 81, 138)", 0.9f)),
                (0.5f, StyleHelper.GetColor("(8, 136, 217)", 0.9f)),
                (1.0f, StyleHelper.GetColor("(7, 81, 138)", 0.9f)),
            });
        }
    }
}

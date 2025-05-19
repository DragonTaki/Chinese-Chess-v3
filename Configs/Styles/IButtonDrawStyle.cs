/* ----- ----- ----- ----- */
// IButtonDrawStyle.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/19
// Update Date: 2025/05/19
// Version: v1.0
/* ----- ----- ----- ----- */

using System.Drawing;

using SharedLib.Geometry;
using SharedLib.MathUtils;

namespace Chinese_Chess_v3.Configs.Style
{
    public interface IButtonDrawStyle
    {
        void Draw(Graphics g, string text, LayoutF bounds);
        void Draw(Graphics g, string text, Vector2F position, Vector2F size);
    }
}

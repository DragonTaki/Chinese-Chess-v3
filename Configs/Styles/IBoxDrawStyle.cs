/* ----- ----- ----- ----- */
// IBoxDrawStyle.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/22
// Update Date: 2025/05/22
// Version: v1.0
/* ----- ----- ----- ----- */

using System.Drawing;

using SharedLib.Geometry;
using SharedLib.MathUtils;

namespace Chinese_Chess_v3.Configs.Style
{
    public interface IBoxDrawStyle
    {
        void Draw(Graphics g, LayoutF bounds);
        void Draw(Graphics g, Vector2F position, Vector2F size);
    }
}

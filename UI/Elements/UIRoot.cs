/* ----- ----- ----- ----- */
// UIRoot.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/17
// Update Date: 2025/05/17
// Version: v1.0
/* ----- ----- ----- ----- */

using System.Drawing;

using Chinese_Chess_v3.UI.Core;

namespace Chinese_Chess_v3.UI.Elements
{
    /// <summary>
    /// The most basic blank container, used as a root node.
    /// Does not do any self-drawing, is only responsible for holding child elements.
    /// </summary>
    public class UIRoot : UIElement
    {
        public UIRoot()
        {
            /* no-op */
        }
        public override bool HitTest(PointF point) => true;
    }
}
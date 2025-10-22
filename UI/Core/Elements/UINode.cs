/* ----- ----- ----- ----- */
// UINode.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/20
// Update Date: 2025/05/20
// Version: v1.0
/* ----- ----- ----- ----- */

using System.Drawing;

using Chinese_Chess_v3.Constants.UI;

namespace Chinese_Chess_v3.UI.Core.Elements
{
    /// <summary>
    /// A generic node element that does not render itself and always passes hit tests.
    /// Used as a structural container for other UI elements.
    /// </summary>
    public class UINode : UIElement
    {
        public override bool IsInteractable => false;
        public UINode(int zIndex = 0, bool isPersistent = false, UIElementType type = UIElementType.Generic)
            : base(zIndex, isPersistent, type)
        {
            /* no-op */
        }

        /// <summary>
        /// Always returns true to participate in hit testing.
        /// </summary>
        public override bool HitTest(PointF point) => true;
    }
}

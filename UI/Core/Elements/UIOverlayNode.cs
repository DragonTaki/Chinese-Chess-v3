/* ----- ----- ----- ----- */
// UIOverlayNode.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/20
// Update Date: 2025/05/20
// Version: v1.0
/* ----- ----- ----- ----- */

using Chinese_Chess_v3.Constants.UI;

namespace Chinese_Chess_v3.UI.Core.Elements
{
    /// <summary>
    /// A persistent overlay layer, typically used for modals or dialogs.
    /// Always appears above regular UI elements.
    /// </summary>
    public sealed class UIOverlayNode : UINode
    {
        public UIOverlayNode(int zIndex = int.MaxValue, bool isPersistent = true, UIElementType type = UIElementType.Overlay)
            : base(zIndex, isPersistent, type)
        {
            /* no-op */
        }
    }
}

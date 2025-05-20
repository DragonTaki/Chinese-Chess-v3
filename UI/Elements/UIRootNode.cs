/* ----- ----- ----- ----- */
// UIRootNode.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/17
// Update Date: 2025/05/17
// Version: v1.0
/* ----- ----- ----- ----- */

using Chinese_Chess_v3.UI.Core;

namespace Chinese_Chess_v3.UI.Elements
{
    /// <summary>
    /// The root container of the UI hierarchy.
    /// Typically the topmost element holding all other UI elements.
    /// </summary>
    public class UIRootNode : UINode
    {
        public UIRootNode(int zIndex = 0, bool isPersistent = true, UIElementType type = UIElementType.Root)
            : base(zIndex, isPersistent, type)
        {
            /* no-op */
        }
    }
}

/* ----- ----- ----- ----- */
// UIRenderContext.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/10/27
// Update Date: 2025/10/27
// Version: v1.0
/* ----- ----- ----- ----- */

using System.Collections.Generic;

namespace Engine.UI.Core.Bases
{
    public class UIRenderContext
    {
        public bool DisableRender { get; set; }
        public bool IsDisposed { get; set; }
        public bool IsVisible { get; set; }

        public IEnumerable<UIRenderContext> Children { get; set; } = [];
        public UIElementBase SourceElement { get; set; }  // 可追溯來源
    }
}

/* ----- ----- ----- ----- */
// UISidebarRenderer.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/06
// Update Date: 2026/10/14
// Version: v2.1
/* ----- ----- ----- ----- */

using Engine.UI.Core.Renderers;
using Engine.Platform;

namespace Chinese_Chess_v3.Game.UI.Sidebars
{
    public class UISidebarRenderer : UIContainerRenderer<UISidebar, UISidebarHandler, UISidebarRenderer>
    {
        public UISidebarRenderer() : base() { }

        public override void OnRender(IGraphics g, UISidebar element)
        {
            element.ArrangePlayerBlocks();
            base.OnRender(g, element);
        }
    }
}

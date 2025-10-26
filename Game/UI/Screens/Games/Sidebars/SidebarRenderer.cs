/* ----- ----- ----- ----- */
// SidebarRenderer.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/06
// Update Date: 2025/10/22
// Version: v2.0
/* ----- ----- ----- ----- */

using System.Drawing;

using Engine.UI.Core.Elements;
using Engine.UI.Core.Renderers;

namespace Chinese_Chess_v3.Game.UI.Screens.Games.Sidebars
{
    public class SidebarRenderer : UIContainerRenderer<SidebarHandler>
    {
        private readonly CompositeRenderer _composite = new CompositeRenderer();

        public SidebarRenderer(Sidebar container) : base(container)
        {
            //
        }

        public override void Render(Graphics g, UIElement element)
        {
            //_composite.Render(g, element);
        }
    }
}

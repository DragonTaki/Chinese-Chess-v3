/* ----- ----- ----- ----- */
// UISidebarHandler.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/10/22
// Update Date: 2025/10/22
// Version: v1.0
/* ----- ----- ----- ----- */

using Engine.UI.Core.Handlers;

namespace Chinese_Chess_v3.Game.UI.Sidebars
{
    /// <summary>
    /// Handler of the Sidebar. It adds no logic of its own yet: the clocks and the turn
    /// live in <c>GameManager</c>, which the info board reads when it draws.
    /// </summary>
    public class UISidebarHandler : UIContainerHandler<UISidebar, UISidebarHandler, UISidebarRenderer>
    {
        public UISidebarHandler() { }
    }
}

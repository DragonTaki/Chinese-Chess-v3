/* ----- ----- ----- ----- */
// UIOnlineGroup.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/12
// Update Date: 2026/10/12
// Version: v1.0
/* ----- ----- ----- ----- */

using Engine.UI.Core.Elements;

namespace Chinese_Chess_v3.Game.UI.Menus.Online
{
    /// <summary>
    /// A plain container for a group of blocks inside an online screen's scroll column (a page of
    /// the online menu, the lobby's room list, a room's lines, a row of buttons). It only arranges
    /// its children (see <c>UILayoutSheet.OnlineMenu.Group</c> / <c>ButtonRow</c>) and draws nothing
    /// itself: its buttons are drawn by the screen's menu renderer, like every menu button.
    /// </summary>
    public class UIOnlineGroup : UIContainer<UIOnlineGroup, UIOnlineGroupHandler, UIOnlineGroupRenderer>
    {
        public UIOnlineGroup() { }
    }
}

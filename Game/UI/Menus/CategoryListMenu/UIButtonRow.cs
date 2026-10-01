/* ----- ----- ----- ----- */
// UIButtonRow.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/01
// Update Date: 2026/10/01
// Version: v1.0
/* ----- ----- ----- ----- */

using Engine.UI.Core.Elements;

namespace Chinese_Chess_v3.Game.UI.Menus.CategoryListMenu
{
    /// <summary>
    /// A plain container for one block of a category list menu's buttons
    /// (<c>UICategoryListMenu</c>: the category filter row, the item grid) inside the menu's
    /// scroll container. It only arranges its buttons (a wrapping flex row, see
    /// <c>UILayoutSheet.CategoryListMenu.ButtonRows</c>)
    /// and draws nothing itself: the buttons are drawn by the menu renderer, like every
    /// menu button.
    /// </summary>
    public class UIButtonRow : UIContainer<UIButtonRow, UIButtonRowHandler, UIButtonRowRenderer>
    {
        public UIButtonRow() { }
    }
}

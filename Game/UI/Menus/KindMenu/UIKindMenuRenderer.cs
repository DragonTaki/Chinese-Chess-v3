/* ----- ----- ----- ----- */
// UIKindMenuRenderer.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/16
// Update Date: 2026/10/13
// Version: v1.2
/* ----- ----- ----- ----- */

using Engine.Platform;
using Engine.Styles;
using Engine.UI.Core.Renderers;

namespace Chinese_Chess_v3.Game.UI.Menus.KindMenu
{
    public class UIKindMenuRenderer : UIMenuRenderer<UIKindMenu, UIKindMenuHandler, UIKindMenuRenderer>
    {
        public UIKindMenuRenderer() : base() { }

        /// <summary>
        /// The panel as every menu draws it, then the pre-game page's fixed bottom row: its buttons are not in the
        /// scroll container, so the menu's scroll-clipped button pass does not draw them.
        /// </summary>
        public override void OnRender(IGraphics g, UIKindMenu element)
        {
            base.OnRender(g, element);
            foreach (var button in element.FixedButtons)
            {
                if (button.DisableRender) continue;
                IButtonDrawStyle style = button.Style ?? DefaultStyles.DefaultButtonStyle;
                style.Draw(g, button.Text, button.GetCurrentAbsolutePosition(), button.Size);
            }
        }
    }
}

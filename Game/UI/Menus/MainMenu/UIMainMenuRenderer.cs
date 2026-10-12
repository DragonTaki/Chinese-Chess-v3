/* ----- ----- ----- ----- */
// UIMainMenuRenderer.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/08
// Update Date: 2026/10/12
// Version: v1.2
/* ----- ----- ----- ----- */

using System.Drawing;

using Chinese_Chess_v3.Game.Application.MainMenu;

using Engine.Diagnostics;
using Engine.Platform;
using Engine.Styles;
using Engine.UI.Core.Renderers;

namespace Chinese_Chess_v3.Game.UI.Menus.MainMenu
{
    public class UIMainMenuRenderer : UIMenuRenderer<UIMainMenu, UIMainMenuHandler, UIMainMenuRenderer>
    {
        /// <summary>How far the pointer sticks out of the submenu's left edge, towards the main menu button.</summary>
        private const float PointerDepth = 28f;

        /// <summary>Half of the pointer's height, along the submenu's left edge.</summary>
        private const float PointerHalfHeight = 24f;

        /// <summary>The colour of the main menu panel's own frame (the submenu's frame takes its button's colour).</summary>
        private static readonly Color PanelFrameColor = Color.FromArgb(255, 170, 170, 170);

        public UIMainMenuRenderer() : base() { }

        public override void OnRender(IGraphics g, UIMainMenu element)
        {
            base.OnRender(g, element);
            DrawSubmenuFrame(g, element);
        }

        /// <summary>The main menu panel's frame, in the style picked in the developer options (replaces the dashed outline).</summary>
        protected override void DrawOutline(IGraphics g, RectangleF bounds)
        {
            FrameStyles.Of(DebugOptions.MenuFrameStyle).Draw(g, bounds, PanelFrameColor, null);
        }

        /// <summary>
        /// The open submenu's frame (author 2026-10-12), in the same style, in the colour of the main
        /// menu button that opened it, with a "&lt;" shaped tail centred on that button: one outline, so
        /// the tail and the frame read as a speech bubble. Placed from the button's and the submenu's
        /// absolute bounds every frame, so it follows the menu when it is scrolled or moved. Drawn
        /// here, before the children (the submenu's own outline is off), so the submenu's content is over it.
        /// </summary>
        private void DrawSubmenuFrame(IGraphics g, UIMainMenu element)
        {
            var handler = element.Handler;
            if (handler.CurrentSubmenu is not MainMenuOption option)
                return;

            var button = element.ButtonOf(option);
            if (button == null || !handler.Submenus.TryGetValue(option, out var submenu) || !submenu.IsVisible)
                return;

            RectangleF bounds = submenu.GetCurrentAbsoluteBounds();
            var buttonBounds = button.GetCurrentAbsoluteBounds();
            float centreY = buttonBounds.Y + buttonBounds.Height / 2f;

            // No tail when the button is scrolled out of the menu's viewport
            var clip = element.GetAbsClipRect();
            FrameTail? tail = centreY < clip.Top || centreY > clip.Bottom
                ? null
                : new FrameTail(centreY, PointerDepth, PointerHalfHeight);

            FrameStyles.Of(DebugOptions.MenuFrameStyle).Draw(g, bounds, element.ColorOf(option), tail);
        }
    }
}

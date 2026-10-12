/* ----- ----- ----- ----- */
// UIMainMenuRenderer.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/08
// Update Date: 2026/10/12
// Version: v1.1
/* ----- ----- ----- ----- */

using Chinese_Chess_v3.Game.Application.MainMenu;

using Engine.Platform;
using Engine.UI.Core.Renderers;

namespace Chinese_Chess_v3.Game.UI.Menus.MainMenu
{
    public class UIMainMenuRenderer : UIMenuRenderer<UIMainMenu, UIMainMenuHandler, UIMainMenuRenderer>
    {
        /// <summary>How far the pointer sticks out of the submenu's left edge, towards the main menu button.</summary>
        private const float PointerDepth = 28f;

        /// <summary>Half of the pointer's height, along the submenu's left edge.</summary>
        private const float PointerHalfHeight = 24f;

        public UIMainMenuRenderer() : base() { }

        public override void OnRender(IGraphics g, UIMainMenu element)
        {
            base.OnRender(g, element);
            DrawPointer(g, element);
        }

        /// <summary>
        /// A "&lt;" shaped bump on the open submenu's left edge, in the colour of the main menu button
        /// that opened it, centred on that button (author §8.40). Placed from the button's and the
        /// submenu's absolute bounds every frame, so it follows the menu when it is scrolled or moved.
        /// </summary>
        private void DrawPointer(IGraphics g, UIMainMenu element)
        {
            var handler = element.Handler;
            if (handler.CurrentSubmenu is not MainMenuOption option)
                return;

            var button = element.ButtonOf(option);
            if (button == null || !handler.Submenus.TryGetValue(option, out var submenu) || !submenu.IsVisible)
                return;

            var buttonBounds = button.GetCurrentAbsoluteBounds();
            float centreY = buttonBounds.Y + buttonBounds.Height / 2f;

            // Not when the button is scrolled out of the menu's viewport
            var clip = element.GetAbsClipRect();
            if (centreY < clip.Top || centreY > clip.Bottom)
                return;

            float baseX = submenu.GetCurrentAbsoluteBounds().X;

            using var brush = GraphicsBackend.Factory.CreateSolidBrush(element.ColorOf(option));
            using var path = GraphicsBackend.Factory.CreatePath();
            path.StartFigure();
            path.AddLine(baseX, centreY - PointerHalfHeight, baseX - PointerDepth, centreY);
            path.AddLine(baseX - PointerDepth, centreY, baseX, centreY + PointerHalfHeight);
            path.AddLine(baseX, centreY + PointerHalfHeight, baseX, centreY - PointerHalfHeight);
            path.CloseFigure();
            g.FillPath(brush, path);
        }
    }
}

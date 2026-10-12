/* ----- ----- ----- ----- */
// UIMainMenuRenderer.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/08
// Update Date: 2026/10/12
// Version: v1.4
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
        /// <summary>How far the pointer sticks out of the submenu's left edge, towards the main menu button (the frame's own margin is added, as the frame is drawn that far inside the edge).</summary>
        private const float PointerDepth = 28f + FrameStyle.EdgeMargin;

        /// <summary>Half of the pointer's height, along the submenu's left edge.</summary>
        private const float PointerHalfHeight = 24f;

        /// <summary>The colour of the main menu panel's own frame (the submenu's frame takes its button's colour).</summary>
        private static readonly Color PanelFrameColor = Color.FromArgb(255, 170, 170, 170);

        public UIMainMenuRenderer() : base() { }

        /// <summary>How far the opening in the main menu's frame reaches past the tail's edges.</summary>
        private const float GapPadding = 4f;

        public override void OnRender(IGraphics g, UIMainMenu element)
        {
            base.OnRender(g, element);
            DrawSubmenuFrame(g, element);
        }

        /// <summary>
        /// The main menu panel's frame, in the style picked in the developer options (replaces the dashed
        /// outline). While a submenu's tail is shown it has an opening on its right edge where the tail
        /// enters, so the two outlines never cross.
        /// </summary>
        protected override void DrawOutline(IGraphics g, RectangleF bounds)
        {
            var style = FrameStyles.Of(DebugOptions.MenuFrameStyle);
            FrameGap? gap = null;
            if (TryGetSubmenuFrame(Element, out _, out var submenuBounds, out var tail) && tail is FrameTail t)
                gap = FrameGap.ForTail(style.ClampedTail(submenuBounds, t), GapPadding);
            style.Draw(g, bounds, PanelFrameColor, null, gap);
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
            if (!TryGetSubmenuFrame(element, out var option, out var bounds, out var tail))
                return;

            FrameStyles.Of(DebugOptions.MenuFrameStyle).Draw(g, bounds, element.ColorOf(option), tail);
        }

        /// <summary>
        /// The open submenu's frame: the option that opened it, its absolute bounds and the tail (null
        /// when the button is scrolled out of the menu's viewport). False when no submenu is shown.
        /// </summary>
        private static bool TryGetSubmenuFrame(UIMainMenu element, out MainMenuOption option, out RectangleF bounds, out FrameTail? tail)
        {
            option = default;
            bounds = default;
            tail = null;

            var handler = element.Handler;
            if (handler.CurrentSubmenu is not MainMenuOption current)
                return false;

            var button = element.ButtonOf(current);
            if (button == null || !handler.Submenus.TryGetValue(current, out var submenu) || !submenu.IsVisible)
                return false;

            option = current;
            bounds = submenu.GetCurrentAbsoluteBounds();
            var buttonBounds = button.GetCurrentAbsoluteBounds();
            float centreY = buttonBounds.Y + buttonBounds.Height / 2f;

            // No tail when the button is scrolled out of the menu's viewport
            var clip = element.GetAbsClipRect();
            tail = centreY < clip.Top || centreY > clip.Bottom
                ? null
                : new FrameTail(centreY, PointerDepth, PointerHalfHeight);
            return true;
        }
    }
}

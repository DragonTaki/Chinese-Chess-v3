/* ----- ----- ----- ----- */
// UIMainMenuRenderer.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/08
// Update Date: 2026/10/12
// Version: v1.6
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
        /// <summary>
        /// How far the submenu's frame is drawn to the left of the submenu's left edge, towards the main
        /// menu's frame (author 2026-10-12: the two frames were too far apart; the distance between their
        /// lines was twice the frame's inset, about 24, now about a third less). Only the frame moves,
        /// never the submenu's content.
        /// </summary>
        private const float FrameOverlap = 8f;

        /// <summary>How far the pointer sticks out of the frame's left line, towards the main menu button (28 plus the frame's margin, less the overlap so the point stays where it was).</summary>
        private const float PointerDepth = 28f + FrameStyle.EdgeMargin - FrameOverlap;

        /// <summary>Half of the pointer's height, along the submenu's left edge.</summary>
        private const float PointerHalfHeight = 24f;

        /// <summary>The colour of the main menu panel's own frame (the submenu's frame takes its button's colour).</summary>
        private static readonly Color PanelFrameColor = Color.FromArgb(255, 170, 170, 170);

        public UIMainMenuRenderer() : base() { }

        /// <summary>How far the submenu bubble's area, cut out of the main menu's frame, is grown so no anti-aliased seam is left.</summary>
        private const float CutoutGrow = 1.5f;

        public override void OnRender(IGraphics g, UIMainMenu element)
        {
            base.OnRender(g, element);
            DrawSubmenuFrame(g, element);
        }

        /// <summary>
        /// The main menu panel's frame, in the style picked in the developer options (replaces the dashed
        /// outline). While a submenu's tail is shown, the area the submenu's bubble covers (its tail and
        /// body, the exact shape of the submenu's frame) is left out of it, fill, glow and lines alike, so
        /// the bubble's edge is the visible boundary and the two outlines never cross.
        /// </summary>
        protected override void DrawOutline(IGraphics g, RectangleF bounds)
        {
            var style = FrameStyles.Of(DebugOptions.MenuFrameStyle);
            if (TryGetSubmenuFrame(Element, out _, out var submenuBounds, out var tail) && tail is FrameTail t)
            {
                using var bubble = style.CreateShapePath(submenuBounds, t, CutoutGrow);
                style.Draw(g, bounds, PanelFrameColor, null, bubble);
                return;
            }
            style.Draw(g, bounds, PanelFrameColor, null);
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
            var panel = submenu.GetCurrentAbsoluteBounds();
            bounds = new RectangleF(panel.X - FrameOverlap, panel.Y, panel.Width + FrameOverlap, panel.Height);
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

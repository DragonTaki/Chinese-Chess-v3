/* ----- ----- ----- ----- */
// MenuLayout.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/09/30
// Update Date: 2026/09/30
// Version: v1.1
/* ----- ----- ----- ----- */

using Engine.Geometry;
using Engine.UI.Constants.Core;
using Engine.UI.Core.Bases;
using Engine.UI.Core.Elements;
using Engine.UI.Core.Handlers;
using Engine.UI.Core.Renderers;
using Engine.UI.Models;

namespace Chinese_Chess_v3.Game.UI.Menus
{
    /// <summary>
    /// The layout rules every menu (main menu, submenus, game menu) shares, expressed with
    /// the Engine layout system instead of hand-computed positions (see docs/LAYOUT.md):
    /// <list type="bullet">
    /// <item>Screen menu (main menu, game menu): the menu element covers its whole parent
    /// (the root), so the screen's other content (submenus, board, sidebar) can be placed
    /// against the real screen edges; the menu's own panel is its left, fixed-width
    /// column (<c>PanelWidth</c>), which the outline and buttons use.</item>
    /// <item>Submenu panel: fixed width, height stretched to the parent's height.</item>
    /// <item>Scroll container: inset by the menu's margins inside the panel.</item>
    /// <item>Buttons: a flex column inside the scroll container, starting at its top,
    /// separated by the menu's button spacing; fixed height, full width, never shrunk
    /// (overflowing buttons are scrolled to, not squeezed).</item>
    /// </list>
    /// All numbers come from <c>UILayoutConstants</c>.
    /// </summary>
    internal static class MenuLayout
    {
        /// <summary>
        /// Makes a screen menu cover its whole parent, with its panel the left
        /// <paramref name="panelWidth"/>-wide column.
        /// </summary>
        public static void ApplyScreen<TElement, THandler, TRenderer>(UIMenu<TElement, THandler, TRenderer> menu, float panelWidth)
            where TElement : UIMenu<TElement, THandler, TRenderer>
            where THandler : UIMenuHandler<TElement, THandler, TRenderer>
            where TRenderer : UIMenuRenderer<TElement, THandler, TRenderer>
        {
            menu.PanelWidth = panelWidth;

            var rules = menu.LayoutRules;
            rules.PositionMode = PositionMode.Absolute;
            rules.Left = 0f;
            rules.Top = 0f;
            rules.Right = 0f;
            rules.Bottom = 0f;
        }

        /// <summary>
        /// Places a menu panel at <paramref name="layout"/>'s position in its parent, with
        /// <paramref name="layout"/>'s width and the parent's full remaining height.
        /// </summary>
        public static void ApplyPanel(UIElementBase menu, LayoutF layout)
        {
            var rules = menu.LayoutRules;
            rules.PositionMode = PositionMode.Absolute;
            rules.Left = layout.Position.X;
            rules.Top = layout.Position.Y;
            rules.Width = LayoutSize.Fixed(layout.Size.X);
            rules.Height = LayoutSize.Stretch;
        }

        /// <summary>
        /// Insets the menu's scroll container by the margins and makes it a top-aligned
        /// flex column with <paramref name="buttonSpacing"/> between buttons.
        /// </summary>
        /// <param name="panelWidth">
        /// The panel width of a screen menu (see <see cref="ApplyScreen"/>): the container is
        /// then the panel's width minus both side margins, from the left margin, instead of
        /// being inset from the menu element's right edge (which is the screen's).
        /// <c>null</c>: the panel is the whole menu element.
        /// </param>
        public static void ApplyScrollContainer(UIScrollContainer scroll, float marginX, float marginY, float buttonSpacing, float? panelWidth = null)
        {
            var rules = scroll.LayoutRules;
            rules.PositionMode = PositionMode.Absolute;
            rules.Left = marginX;
            rules.Top = marginY;
            rules.Bottom = marginY;
            if (panelWidth is float width)
            {
                rules.Right = null;
                rules.Width = LayoutSize.Fixed(width - marginX * 2f);
            }
            else
                rules.Right = marginX;

            rules.Container = LayoutContainer.Flex;
            rules.FlexDirection = FlexDirection.Column;
            rules.JustifyContent = JustifyContent.Start;
            rules.AlignItems = FlexAlign.Stretch;
            rules.RowGap = buttonSpacing;
        }

        /// <summary>
        /// Makes a menu button a flex item: full width, fixed <paramref name="height"/>, not shrinkable.
        /// </summary>
        public static void ApplyButton(UIButton button, float height)
        {
            var rules = button.LayoutRules;
            rules.PositionMode = PositionMode.Flow;
            rules.Width = LayoutSize.Stretch;
            rules.Height = LayoutSize.Fixed(height);
            rules.FlexShrink = 0f;
        }
    }
}

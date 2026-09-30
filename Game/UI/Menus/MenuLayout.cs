/* ----- ----- ----- ----- */
// MenuLayout.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/09/30
// Update Date: 2026/09/30
// Version: v1.0
/* ----- ----- ----- ----- */

using Engine.Geometry;
using Engine.UI.Constants.Core;
using Engine.UI.Core.Bases;
using Engine.UI.Core.Elements;
using Engine.UI.Models;

namespace Chinese_Chess_v3.Game.UI.Menus
{
    /// <summary>
    /// The layout rules every menu (main menu, submenus, game menu) shares, expressed with
    /// the Engine layout system instead of hand-computed positions (see docs/LAYOUT.md):
    /// <list type="bullet">
    /// <item>Menu panel: fixed width, height stretched to the parent's height.</item>
    /// <item>Scroll container: inset by the menu's margins on all four sides.</item>
    /// <item>Buttons: a flex column inside the scroll container, starting at its top,
    /// separated by the menu's button spacing; fixed height, full width, never shrunk
    /// (overflowing buttons are scrolled to, not squeezed).</item>
    /// </list>
    /// All numbers come from <c>UILayoutConstants</c>.
    /// </summary>
    internal static class MenuLayout
    {
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
        public static void ApplyScrollContainer(UIScrollContainer scroll, float marginX, float marginY, float buttonSpacing)
        {
            var rules = scroll.LayoutRules;
            rules.PositionMode = PositionMode.Absolute;
            rules.Left = marginX;
            rules.Right = marginX;
            rules.Top = marginY;
            rules.Bottom = marginY;

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

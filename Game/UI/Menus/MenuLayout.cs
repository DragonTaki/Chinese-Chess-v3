/* ----- ----- ----- ----- */
// MenuLayout.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/09/30
// Update Date: 2026/09/30
// Version: v1.2
/* ----- ----- ----- ----- */

using Chinese_Chess_v3.Game.UI.Constants;

using Engine.Geometry;
using Engine.UI.Core.Bases;
using Engine.UI.Core.Elements;
using Engine.UI.Core.Handlers;
using Engine.UI.Core.Renderers;

namespace Chinese_Chess_v3.Game.UI.Menus
{
    /// <summary>
    /// Imperative helpers that apply the shared menu rules to an element. The rules
    /// themselves are data in <see cref="UILayoutSheet.Menus"/> (see docs/LAYOUT.md section 9);
    /// the menus apply their <see cref="UILayoutSheet"/> entries directly, so these helpers
    /// only remain for code that builds a menu from parameters rather than a sheet entry.
    /// Each one applies the matching <see cref="UILayoutSheet.Menus"/> style.
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
            menu.LayoutRules.Apply(UILayoutSheet.Menus.Screen);
        }

        /// <summary>
        /// Places a menu panel at <paramref name="layout"/>'s position in its parent, with
        /// <paramref name="layout"/>'s width and the parent's full remaining height.
        /// </summary>
        public static void ApplyPanel(UIElementBase menu, LayoutF layout)
        {
            menu.LayoutRules.Apply(UILayoutSheet.Menus.Panel(layout));
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
            scroll.LayoutRules.Apply(UILayoutSheet.Menus.ScrollContainer(marginX, marginY, buttonSpacing, panelWidth));

            // A style cannot unset an inset; this helper always could (re-applying it with a
            // panel width to a container that had a right inset).
            if (panelWidth.HasValue)
                scroll.LayoutRules.Right = null;
        }

        /// <summary>
        /// Makes a menu button a flex item: full width, fixed <paramref name="height"/>, not shrinkable.
        /// </summary>
        public static void ApplyButton(UIButton button, float height)
        {
            button.LayoutRules.Apply(UILayoutSheet.Menus.Button(height));
        }
    }
}

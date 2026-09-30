/* ----- ----- ----- ----- */
// UILayoutSheet.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/09/30
// Update Date: 2026/09/30
// Version: v1.0
/* ----- ----- ----- ----- */

using Engine.Geometry;
using Engine.UI.Constants.Core;
using Engine.UI.Models;

namespace Chinese_Chess_v3.Game.UI.Constants
{
    /// <summary>
    /// The layout "style sheet": every layout rule of the game's screens, as declarative
    /// data (see docs/LAYOUT.md section 9). One static class per screen or element group,
    /// one <see cref="UILayoutStyle"/> entry per element - like one CSS class each - built
    /// from the numbers in <see cref="UILayoutConstants"/>. Elements only name the entry
    /// they use (<c>LayoutRules.Apply(UILayoutSheet.GameScreen.Sidebar)</c>); no rule value
    /// lives in element code.
    /// <para>
    /// A <see cref="UILayoutStyle"/> maps 1:1 onto <see cref="UILayout"/>; the few settings
    /// that belong to an element type rather than to <see cref="UILayout"/> (a menu's
    /// <c>PanelWidth</c>) sit next to the styles as plain values.
    /// </para>
    /// <para>
    /// Positions are relative to the element's parent, like everything in
    /// <see cref="UILayoutConstants"/>. The root is still the letterboxed design size.
    /// </para>
    /// </summary>
    public static class UILayoutSheet
    {
        /// <summary>
        /// Rules every menu (main menu, submenus, game menu) shares - the base "classes" the
        /// per-screen entries are built from:
        /// <list type="bullet">
        /// <item>Screen menu (main menu, game menu): covers its whole parent (the root), so
        /// the screen's other content (submenus, board, sidebar) can be placed against the
        /// real screen edges; its own panel is its left, fixed-width column
        /// (<c>PanelWidth</c>), which the outline and buttons use.</item>
        /// <item>Submenu panel: fixed width, height stretched to the parent's height.</item>
        /// <item>Scroll container: inset by the menu's margins inside the panel, a
        /// top-aligned flex column with the button spacing between buttons.</item>
        /// <item>Buttons: fixed height, full width, never shrunk (overflowing buttons are
        /// scrolled to, not squeezed).</item>
        /// </list>
        /// </summary>
        public static class Menus
        {
            /// <summary>A screen menu: covers its whole parent.</summary>
            public static readonly UILayoutStyle Screen = new()
            {
                PositionMode = PositionMode.Absolute,
                Left = 0f,
                Top = 0f,
                Right = 0f,
                Bottom = 0f,
            };

            /// <summary>
            /// A menu panel at <paramref name="layout"/>'s position in its parent, with
            /// <paramref name="layout"/>'s width and the parent's full remaining height.
            /// </summary>
            public static UILayoutStyle Panel(LayoutF layout) => new()
            {
                PositionMode = PositionMode.Absolute,
                Left = layout.Position.X,
                Top = layout.Position.Y,
                Width = LayoutSize.Fixed(layout.Size.X),
                Height = LayoutSize.Stretch,
            };

            /// <summary>
            /// A menu's scroll container: inset by the margins, a top-aligned flex column
            /// with <paramref name="buttonSpacing"/> between buttons.
            /// </summary>
            /// <param name="panelWidth">
            /// The panel width of a screen menu: the container is then the panel's width
            /// minus both side margins, from the left margin, instead of being inset from the
            /// menu element's right edge (which is the screen's). <c>null</c>: the panel is
            /// the whole menu element.
            /// </param>
            public static UILayoutStyle ScrollContainer(float marginX, float marginY, float buttonSpacing, float? panelWidth = null)
            {
                var style = new UILayoutStyle
                {
                    PositionMode = PositionMode.Absolute,
                    Left = marginX,
                    Top = marginY,
                    Bottom = marginY,
                    Container = LayoutContainer.Flex,
                    FlexDirection = FlexDirection.Column,
                    JustifyContent = JustifyContent.Start,
                    AlignItems = FlexAlign.Stretch,
                    RowGap = buttonSpacing,
                };
                return panelWidth is float width
                    ? style with { Width = LayoutSize.Fixed(width - marginX * 2f) }
                    : style with { Right = marginX };
            }

            /// <summary>A menu button: flex item, full width, fixed <paramref name="height"/>, not shrinkable.</summary>
            public static UILayoutStyle Button(float height) => new()
            {
                PositionMode = PositionMode.Flow,
                Width = LayoutSize.Stretch,
                Height = LayoutSize.Fixed(height),
                FlexShrink = 0f,
            };
        }

        /// <summary>The main menu screen (<c>UIMainMenu</c>).</summary>
        public static class MainMenu
        {
            /// <summary><c>UIMainMenu.PanelWidth</c>: the menu's left column.</summary>
            public static readonly float PanelWidth = UILayoutConstants.MainMenu.Size.X;

            /// <summary><c>UIMainMenu</c>: covers the whole root.</summary>
            public static readonly UILayoutStyle Screen = Menus.Screen;

            /// <summary><c>UIMainMenu.ScrollContainer</c>.</summary>
            public static readonly UILayoutStyle ScrollContainer = Menus.ScrollContainer(
                UILayoutConstants.MainMenu.Margin, UILayoutConstants.MainMenu.Margin,
                UILayoutConstants.MainMenu.Button.Spacing, PanelWidth);

            /// <summary>Each main menu button.</summary>
            public static readonly UILayoutStyle Button = Menus.Button(UILayoutConstants.MainMenu.Button.Size.Y);
        }

        /// <summary>The main menu's submenus (<c>UINewGameMenu</c>, <c>UILoadGameMenu</c>).</summary>
        public static class Submenu
        {
            /// <summary>
            /// The submenu: right of the main menu's panel (a child of the screen-sized main
            /// menu), fixed width, full height.
            /// </summary>
            public static readonly UILayoutStyle Panel = Menus.Panel(UILayoutConstants.Submenu.Layout);

            /// <summary>The submenu's scroll container.</summary>
            public static readonly UILayoutStyle ScrollContainer = Menus.ScrollContainer(
                UILayoutConstants.Submenu.MarginX, UILayoutConstants.Submenu.MarginY,
                UILayoutConstants.Submenu.Button.Spacing);

            /// <summary>Each submenu button.</summary>
            public static readonly UILayoutStyle Button = Menus.Button(UILayoutConstants.Submenu.Button.Size.Y);
        }

        /// <summary>
        /// The game screen: the game menu (<c>UIGameMenu</c>) covers the root, its left
        /// column is the menu, and the board and the sidebar (its children) take the
        /// middle and the right column.
        /// </summary>
        public static class GameScreen
        {
            /// <summary><c>UIGameMenu.PanelWidth</c>: the left (menu) column.</summary>
            public static readonly float MenuPanelWidth = UILayoutConstants.GameMenu.Size.X;

            /// <summary><c>UIGameMenu</c>: covers the whole root.</summary>
            public static readonly UILayoutStyle Menu = Menus.Screen;

            /// <summary><c>UIGameMenu.ScrollContainer</c>: the left column's buttons.</summary>
            public static readonly UILayoutStyle MenuScrollContainer = Menus.ScrollContainer(
                UILayoutConstants.GameMenu.Margin, UILayoutConstants.GameMenu.Margin,
                UILayoutConstants.GameMenu.Button.Spacing, MenuPanelWidth);

            /// <summary>Each game menu button.</summary>
            public static readonly UILayoutStyle MenuButton = Menus.Button(UILayoutConstants.GameMenu.Button.Size.Y);

            /// <summary>
            /// <c>UIBoard</c>: the middle area between the menu column and the sidebar, full
            /// height, kept at its authored aspect ratio and centered in that area.
            /// </summary>
            public static readonly UILayoutStyle Board = new()
            {
                PositionMode = PositionMode.Absolute,
                Left = UILayoutConstants.GameMenu.Size.X,
                Right = UILayoutConstants.Sidebar.Size.X,
                Top = UILayoutConstants.Board.Position.Y,
                Bottom = 0f,
                Width = LayoutSize.Stretch,
                Height = LayoutSize.Stretch,
                AspectRatio = UILayoutConstants.Board.Size.X / UILayoutConstants.Board.Size.Y,
                AspectFit = AspectFit.Contain,
                AlignX = Alignment.Center,
                AlignY = Alignment.Center,
            };

            /// <summary>
            /// <c>UISidebar</c>: fixed-width column pinned to the right edge, full height; a
            /// flex column with the info board at the top and the logger box at the bottom,
            /// both inset by the margin.
            /// </summary>
            public static readonly UILayoutStyle Sidebar = new()
            {
                PositionMode = PositionMode.Absolute,
                Right = 0f,
                Top = UILayoutConstants.Sidebar.Position.Y,
                Width = LayoutSize.Fixed(UILayoutConstants.Sidebar.Size.X),
                Height = LayoutSize.Stretch,
                Padding = new PaddingF(UILayoutConstants.Sidebar.Margin),
                Container = LayoutContainer.Flex,
                FlexDirection = FlexDirection.Column,
                JustifyContent = JustifyContent.SpaceBetween,
                AlignItems = FlexAlign.Stretch,
            };

            /// <summary><c>UIInfoBoard</c>: flex item of the sidebar column, full width, fixed height.</summary>
            public static readonly UILayoutStyle InfoBoard = new()
            {
                PositionMode = PositionMode.Flow,
                Width = LayoutSize.Stretch,
                Height = LayoutSize.Fixed(UILayoutConstants.Sidebar.Infoboard.Size.Y),
                FlexShrink = 0f,
            };

            /// <summary>
            /// <c>UILoggerBox</c>: flex item of the sidebar column, full width, today's fixed
            /// height (placed at the bottom by the sidebar's SpaceBetween). Growing into the
            /// remaining height is for the full-window switch - it would change today's 200px box.
            /// </summary>
            public static readonly UILayoutStyle LoggerBox = new()
            {
                PositionMode = PositionMode.Flow,
                Width = LayoutSize.Stretch,
                Height = LayoutSize.Fixed(UILayoutConstants.Sidebar.LoggerBox.Size.Y),
                FlexShrink = 0f,
            };

            /// <summary><c>UILoggerBox.ScrollContainer</c>: follows the box, inset by the logger margin.</summary>
            public static readonly UILayoutStyle LoggerBoxScrollContainer = new()
            {
                PositionMode = PositionMode.Absolute,
                Left = UILayoutConstants.Sidebar.LoggerBox.Margin,
                Top = UILayoutConstants.Sidebar.LoggerBox.Margin,
                Right = UILayoutConstants.Sidebar.LoggerBox.Margin,
                Bottom = UILayoutConstants.Sidebar.LoggerBox.Margin,
            };
        }

        /// <summary>Dialogs shown in the overlay layer.</summary>
        public static class Overlay
        {
            /// <summary>
            /// <c>UIConfirmDialog</c>: sized from its content (<c>MeasureIntrinsicSize</c>) and
            /// centered in the overlay layer, which spans the whole viewport.
            /// </summary>
            public static readonly UILayoutStyle ConfirmDialog = new()
            {
                PositionMode = PositionMode.Absolute,
                Width = LayoutSize.Auto,
                Height = LayoutSize.Auto,
                AlignX = Alignment.Center,
                AlignY = Alignment.Center,
            };
        }
    }
}

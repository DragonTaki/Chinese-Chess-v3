/* ----- ----- ----- ----- */
// UILayoutSheet.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/09/30
// Update Date: 2026/10/02
// Version: v1.4
/* ----- ----- ----- ----- */

using System;

using Chinese_Chess_v3.Game.Core.Boards;

using Engine.Geometry;
using Engine.UI.Constants.Core;
using Engine.UI.Models;

namespace Chinese_Chess_v3.Game.UI.Constants
{
    /// <summary>
    /// The layout "style sheet": every layout rule of the game's screens, as declarative
    /// data. One static class per screen or element group,
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
    /// <see cref="UILayoutConstants"/>. The root is the whole window in design units (no
    /// letterbox): at least <c>UILayoutConstants.DesignSize</c>, larger on one axis when the
    /// window's aspect ratio differs, so the full-height/full-width rules stretch.
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
            /// <param name="marginX">Left inset (and right inset, when there is no <paramref name="panelWidth"/>).</param>
            /// <param name="marginY">Top and bottom inset.</param>
            /// <param name="buttonSpacing">Row gap between the buttons.</param>
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
        /// The category list submenus (<c>UICategoryListMenu</c>: 殘局闖關, 開局練習): a submenu
        /// panel whose scroll container stacks the category filter row and the item grid.
        /// Both are flex rows that wrap (<c>UILayoutConstants.CategoryListMenu.Columns</c> equal-width buttons per
        /// line, column and row gaps) and take their height from their buttons, so the scroll
        /// container's automatic content size covers every row.
        /// </summary>
        public static class CategoryListMenu
        {
            /// <summary>The submenu: same place and size as the other submenus.</summary>
            public static readonly UILayoutStyle Panel = Submenu.Panel;

            /// <summary>
            /// The submenu's scroll container: inset like the other submenus, a flex column
            /// with the section gap between the category row and the item grid.
            /// </summary>
            public static readonly UILayoutStyle ScrollContainer = Menus.ScrollContainer(
                UILayoutConstants.Submenu.MarginX, UILayoutConstants.Submenu.MarginY,
                UILayoutConstants.CategoryListMenu.SectionGap);

            /// <summary>
            /// A row of buttons inside the scroll column (the category row, the item grid):
            /// full width, height from its content (never shrunk - overflow is scrolled to),
            /// a flex row that wraps, lines packed at the top.
            /// </summary>
            public static readonly UILayoutStyle ButtonRows = new()
            {
                PositionMode = PositionMode.Flow,
                Width = LayoutSize.Stretch,
                Height = LayoutSize.Auto,
                FlexShrink = 0f,
                Container = LayoutContainer.Flex,
                FlexDirection = FlexDirection.Row,
                FlexWrap = FlexWrap.Wrap,
                JustifyContent = JustifyContent.Start,
                AlignItems = FlexAlign.Start,
                AlignContent = AlignContent.Start,
                ColumnGap = UILayoutConstants.CategoryListMenu.ColumnGap,
                RowGap = UILayoutConstants.CategoryListMenu.RowGap,
            };

            /// <summary>The category filter row (<c>UICategoryListMenu.CategoryRow</c>).</summary>
            public static readonly UILayoutStyle CategoryRow = ButtonRows;

            /// <summary>The item grid (<c>UICategoryListMenu.ItemGrid</c>).</summary>
            public static readonly UILayoutStyle ItemGrid = ButtonRows;

            /// <summary>A section's heading label (<c>CategoryListModel.Sections</c>): full width, height from its text.</summary>
            public static readonly UILayoutStyle SectionHeading = new()
            {
                PositionMode = PositionMode.Flow,
                Width = LayoutSize.Stretch,
                Height = LayoutSize.Auto,
                FlexShrink = 0f,
            };

            /// <summary>A button of a row: fixed column width and height, not shrinkable.</summary>
            private static UILayoutStyle RowButton(float height) => new()
            {
                PositionMode = PositionMode.Flow,
                Width = LayoutSize.Fixed(UILayoutConstants.CategoryListMenu.ButtonWidth),
                Height = LayoutSize.Fixed(height),
                FlexShrink = 0f,
            };

            /// <summary>Each category toggle button.</summary>
            public static readonly UILayoutStyle CategoryButton = RowButton(UILayoutConstants.CategoryListMenu.CategoryButtonHeight);

            /// <summary>Each item button (puzzle, opening).</summary>
            public static readonly UILayoutStyle ItemButton = RowButton(UILayoutConstants.CategoryListMenu.ItemButtonHeight);

            /// <summary>
            /// The "nothing found" message (a child of the submenu, over the scroll area): as wide
            /// as the scroll container, height from its text, vertically centered in the panel.
            /// </summary>
            public static readonly UILayoutStyle EmptyMessage = new()
            {
                PositionMode = PositionMode.Absolute,
                Left = UILayoutConstants.Submenu.MarginX,
                Right = UILayoutConstants.Submenu.MarginX,
                Width = LayoutSize.Stretch,
                Height = LayoutSize.Auto,
                AlignY = Alignment.Center,
            };
        }

        /// <summary>
        /// The settings screens (<c>UISettingsMenu</c>): a submenu panel with the tab bar at the
        /// top and a scroll container below it - a flex column of the save / back row, section
        /// headers and setting rows, clipping what is scrolled out (the rows draw themselves).
        /// Each row is a flex row: the name is drawn at its left, the control is pushed to its
        /// right edge (<c>JustifyContent.End</c>) and vertically centered.
        /// </summary>
        public static class SettingsMenu
        {
            /// <summary>The submenu: same place and size as the other submenus.</summary>
            public static readonly UILayoutStyle Panel = Submenu.Panel;

            /// <summary>The tab bar: inset like the scroll container, at the panel's top.</summary>
            public static readonly UILayoutStyle TabBar = new()
            {
                PositionMode = PositionMode.Absolute,
                Left = UILayoutConstants.Submenu.MarginX,
                Right = UILayoutConstants.Submenu.MarginX,
                Top = UILayoutConstants.Submenu.MarginY,
                Width = LayoutSize.Stretch,
                Height = LayoutSize.Fixed(UILayoutConstants.SettingsMenu.TabBarHeight),
            };

            /// <summary>The submenu's scroll container: inset like the other submenus, below the tab bar, a flex column that clips its rows.</summary>
            public static readonly UILayoutStyle ScrollContainer = Menus.ScrollContainer(
                UILayoutConstants.Submenu.MarginX, UILayoutConstants.Submenu.MarginY,
                UILayoutConstants.SettingsMenu.RowGap) with
            {
                Top = UILayoutConstants.Submenu.MarginY + UILayoutConstants.SettingsMenu.TabBarHeight + UILayoutConstants.SettingsMenu.TabBarGap,
                Overflow = OverflowMode.Hidden,
            };

            /// <summary>The row holding the 恢復初始 button (a flex row, height from its buttons).</summary>
            public static readonly UILayoutStyle FooterRow = CategoryListMenu.ButtonRows with
            {
                ColumnGap = UILayoutConstants.SettingsMenu.FooterColumnGap,
            };

            /// <summary>The 恢復初始 button: one column (half the scroll container wide) of the footer row.</summary>
            public static readonly UILayoutStyle FooterButton = new()
            {
                PositionMode = PositionMode.Flow,
                Width = LayoutSize.Fixed(UILayoutConstants.SettingsMenu.FooterButtonWidth),
                Height = LayoutSize.Fixed(UILayoutConstants.SettingsMenu.FooterButtonHeight),
                FlexShrink = 0f,
            };

            /// <summary>A section header: full width, shorter than a row.</summary>
            public static readonly UILayoutStyle Header = Menus.Button(UILayoutConstants.SettingsMenu.HeaderHeight);

            /// <summary>A setting's row (<c>UILabeledRow</c>): full width, fixed height, a flex row with its control at the right, vertically centered.</summary>
            public static readonly UILayoutStyle Item = Menus.Button(UILayoutConstants.SettingsMenu.ItemHeight) with
            {
                Container = LayoutContainer.Flex,
                FlexDirection = FlexDirection.Row,
                JustifyContent = JustifyContent.End,
                AlignItems = FlexAlign.Center,
                Padding = new PaddingF(UILayoutConstants.SettingsMenu.ItemPaddingX, 0f),
            };

            /// <summary>A control of a row: fixed size, never shrunk.</summary>
            private static UILayoutStyle Control(float width, float height) => new()
            {
                PositionMode = PositionMode.Flow,
                Width = LayoutSize.Fixed(width),
                Height = LayoutSize.Fixed(height),
                FlexShrink = 0f,
            };

            /// <summary>A row's switch.</summary>
            public static readonly UILayoutStyle Toggle = Control(UILayoutConstants.SettingsMenu.ToggleWidth, UILayoutConstants.SettingsMenu.ToggleHeight);

            /// <summary>A choice's dropdown, <paramref name="width"/> wide (fitted to its longest choice by <c>UISettingsMenu</c>).</summary>
            public static UILayoutStyle Dropdown(float width) => Control(width, UILayoutConstants.SettingsMenu.ValueHeight);

            /// <summary>A number's slider.</summary>
            public static readonly UILayoutStyle Slider = Control(UILayoutConstants.SettingsMenu.SliderWidth, UILayoutConstants.SettingsMenu.ValueHeight);

            /// <summary>A row's number field (the box and its unit).</summary>
            public static readonly UILayoutStyle NumberField = Control(UILayoutConstants.SettingsMenu.NumberFieldWidth, UILayoutConstants.SettingsMenu.ValueHeight);

            /// <summary>A row's text field.</summary>
            public static readonly UILayoutStyle TextField = Control(UILayoutConstants.SettingsMenu.TextFieldWidth, UILayoutConstants.SettingsMenu.ValueHeight);
        }

        /// <summary>
        /// The main menu's saved-game list (<c>UILoadSavedGameMenu</c>, 讀取存檔): a submenu
        /// panel whose scroll container is a flex column of category headers and save buttons
        /// (full width, never shrunk; the 沒有存檔 row is a save-sized row).
        /// </summary>
        public static class LoadSavedGameMenu
        {
            /// <summary>The submenu: same place and size as the other submenus.</summary>
            public static readonly UILayoutStyle Panel = Submenu.Panel;

            /// <summary>The submenu's scroll container: inset like the other submenus, a flex column.</summary>
            public static readonly UILayoutStyle ScrollContainer = Menus.ScrollContainer(
                UILayoutConstants.Submenu.MarginX, UILayoutConstants.Submenu.MarginY,
                UILayoutConstants.LoadSavedGameMenu.RowGap);

            /// <summary>A category header, and the delete-mode toggle: full width, shorter than a save.</summary>
            public static readonly UILayoutStyle Header = Menus.Button(UILayoutConstants.LoadSavedGameMenu.HeaderHeight);

            /// <summary>A save's button, and the 沒有存檔 row.</summary>
            public static readonly UILayoutStyle Item = Menus.Button(UILayoutConstants.LoadSavedGameMenu.ItemHeight);
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
            /// <c>UIBoard</c> while a HalfCenter game (8×4) is shown: the same area as
            /// <see cref="Board"/>, at the HalfCenter board's own aspect ratio
            /// (<c>UILayoutConstants.Board.HalfCenter.Size</c>), so it fills the area's width and
            /// is centered vertically.
            /// </summary>
            public static readonly UILayoutStyle HalfCenterBoard = new()
            {
                PositionMode = PositionMode.Absolute,
                Left = UILayoutConstants.GameMenu.Size.X,
                Right = UILayoutConstants.Sidebar.Size.X,
                Top = UILayoutConstants.Board.Position.Y,
                Bottom = 0f,
                Width = LayoutSize.Stretch,
                Height = LayoutSize.Stretch,
                AspectRatio = UILayoutConstants.Board.HalfCenter.Size.X / UILayoutConstants.Board.HalfCenter.Size.Y,
                AspectFit = AspectFit.Contain,
                AlignX = Alignment.Center,
                AlignY = Alignment.Center,
            };

            /// <summary>
            /// <c>UIBoard</c> while a 三國 game (HalfCross, 9×5) is shown: as <see cref="HalfCenterBoard"/>,
            /// at the HalfCross board's own aspect ratio (<c>UILayoutConstants.Board.HalfCross.Size</c>).
            /// </summary>
            public static readonly UILayoutStyle HalfCrossBoard = new()
            {
                PositionMode = PositionMode.Absolute,
                Left = UILayoutConstants.GameMenu.Size.X,
                Right = UILayoutConstants.Sidebar.Size.X,
                Top = UILayoutConstants.Board.Position.Y,
                Bottom = 0f,
                Width = LayoutSize.Stretch,
                Height = LayoutSize.Stretch,
                AspectRatio = UILayoutConstants.Board.HalfCross.Size.X / UILayoutConstants.Board.HalfCross.Size.Y,
                AspectFit = AspectFit.Contain,
                AlignX = Alignment.Center,
                AlignY = Alignment.Center,
            };

            /// <summary>The <c>UIBoard</c> style for a game on <paramref name="type"/>.</summary>
            /// <exception cref="NotSupportedException">Not a playable board type.</exception>
            public static UILayoutStyle BoardFor(BoardType type) => type switch
            {
                BoardType.Full => Board,
                BoardType.HalfCenter => HalfCenterBoard,
                BoardType.HalfCross => HalfCrossBoard,
                _ => throw new NotSupportedException($"No board layout for {type} yet"),
            };

            /// <summary>
            /// <c>UISavedGameMenu</c> (載入佈局: the saved-game list, a category list submenu):
            /// applied over <see cref="CategoryListMenu.Panel"/>, it takes the board's area
            /// instead of the main menu's submenu place (which is wider than the board and would
            /// cover part of the sidebar): between the menu column and the sidebar, full height.
            /// The board is hidden while the list is shown. At the design width the area is
            /// narrower than a main-menu submenu, so the rows wrap after 3 buttons instead of 4.
            /// </summary>
            public static readonly UILayoutStyle SavedGameList = new()
            {
                PositionMode = PositionMode.Absolute,
                Left = UILayoutConstants.GameMenu.Size.X,
                Right = UILayoutConstants.Sidebar.Size.X,
                Top = 0f,
                Bottom = 0f,
                Width = LayoutSize.Stretch,
                Height = LayoutSize.Stretch,
            };

            /// <summary>
            /// <c>UISidebar</c>: fixed-width column pinned to the right edge, full height; a
            /// flex column inset by the margin, with the fixed info board at the top, the game
            /// controls under it and the logger box growing into the rest, one margin apart.
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
                JustifyContent = JustifyContent.Start,
                AlignItems = FlexAlign.Stretch,
                RowGap = UILayoutConstants.Sidebar.Margin,
            };

            /// <summary><c>UIInfoBoard</c>: flex item of the sidebar column, full width, fixed height.</summary>
            public static readonly UILayoutStyle InfoBoard = new()
            {
                PositionMode = PositionMode.Flow,
                Width = LayoutSize.Stretch,
                Height = LayoutSize.Fixed(UILayoutConstants.Sidebar.InfoBoard.Size.Y),
                FlexShrink = 0f,
            };

            /// <summary>
            /// <c>UIGameControls</c>: flex item of the sidebar column between the info board and the
            /// logger box, full width, as tall as its rows; a wrapping flex row of the control buttons.
            /// </summary>
            public static readonly UILayoutStyle GameControls = new()
            {
                PositionMode = PositionMode.Flow,
                Width = LayoutSize.Stretch,
                Height = LayoutSize.Auto,
                FlexShrink = 0f,
                Container = LayoutContainer.Flex,
                FlexDirection = FlexDirection.Row,
                FlexWrap = FlexWrap.Wrap,
                JustifyContent = JustifyContent.Start,
                AlignItems = FlexAlign.Start,
                AlignContent = AlignContent.Start,
                ColumnGap = UILayoutConstants.Sidebar.GameControls.Gap,
                RowGap = UILayoutConstants.Sidebar.GameControls.Gap,
            };

            /// <summary>A game control button: fixed column width and height, not shrinkable.</summary>
            public static readonly UILayoutStyle GameControlButton = new()
            {
                PositionMode = PositionMode.Flow,
                Width = LayoutSize.Fixed(UILayoutConstants.Sidebar.GameControls.ButtonWidth),
                Height = LayoutSize.Fixed(UILayoutConstants.Sidebar.GameControls.ButtonHeight),
                FlexShrink = 0f,
            };

            /// <summary>
            /// <c>UILoggerBox</c>: flex item of the sidebar column, full width, growing into
            /// the sidebar height the info board leaves (below it, one margin apart). Its
            /// authored height is the flex basis and, since it doesn't shrink, the minimum.
            /// </summary>
            public static readonly UILayoutStyle LoggerBox = new()
            {
                PositionMode = PositionMode.Flow,
                Width = LayoutSize.Stretch,
                Height = LayoutSize.Fixed(UILayoutConstants.Sidebar.LoggerBox.Size.Y),
                FlexGrow = 1f,
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

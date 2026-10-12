/* ----- ----- ----- ----- */
// UISettingsMenu.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/01
// Update Date: 2026/10/12
// Version: v2.4
/* ----- ----- ----- ----- */

using System;
using System.Collections.Generic;

using Chinese_Chess_v3.Game.Application.Settings;
using Chinese_Chess_v3.Game.Application.Texts;
using Chinese_Chess_v3.Game.Configs;
using Chinese_Chess_v3.Game.UI.Constants;
using Chinese_Chess_v3.Game.UI.Menus.CategoryListMenu;

using Engine.Styles;
using Engine.UI.Core.Elements;
using Engine.UI.Core.Handlers;
using Engine.UI.Core.Interfaces;
using Engine.UI.Core.Renderers;
using Engine.UI.Models;

namespace Chinese_Chess_v3.Game.UI.Menus.SettingsMenu
{
    /// <summary>
    /// A settings screen (遊戲設定 or 單機規則設定, <see cref="Screen"/>):
    /// a tab bar at the top, and below it a scrolling list - the 恢復初始 row, then the
    /// selected tab's sections: a header each and one row per setting, its name at the left and
    /// its control at the right (a switch, a dropdown, a slider or a text field,
    /// by <see cref="SettingsItemKind"/>). The logic - what a control edits, saving, resetting -
    /// is the <see cref="SettingsScreenModel"/>, which the handler (<see cref="UISettingsMenuHandler"/>)
    /// binds to; what is listed is <see cref="SettingsMenuContent"/>.
    /// <para>
    /// A tab's rows are rebuilt when it is shown (<see cref="ShowPage"/>); an edit only
    /// refreshes the controls (<see cref="RefreshValues"/>).
    /// </para>
    /// </summary>
    public class UISettingsMenu : UIMenu<UISettingsMenu, UISettingsMenuHandler, UISettingsMenuRenderer>
    {
        /// <summary>Which screen this is (set by the main menu before the first show).</summary>
        public SettingsScreen Screen { get; set; } = SettingsScreen.Game;

        /// <summary>The tab selected each time the screen opens (set by the main menu; 0 = the first).</summary>
        public int InitialTab { get; set; }

        /// <summary>The tab bar (a child of the panel, above the scroll container).</summary>
        internal UITabBar TabBar { get; private set; }

        /// <summary>The row holding the 恢復初始 button (first block of the scroll content).</summary>
        internal UIButtonRow FooterRow { get; private set; }

        /// <summary>Everything of the shown tab in the scroll container (headers and rows), disposed by <see cref="ClearPage"/>.</summary>
        private readonly List<UIElement> _pageElements = new();

        /// <summary>The shown tab's buttons (the section headers): also in <see cref="UIMenu{TElement, THandler, TRenderer}.Buttons"/>, which draws them.</summary>
        private readonly List<UIButton> _pageButtons = new();

        /// <summary>Per setting row: refreshes its controls from a settings instance.</summary>
        private readonly List<Action<PlayerSettings>> _refreshers = new();

        public UISettingsMenu() { }

        protected override void OnBeforeInit(IUiFactory factory)
        {
            // Only used by UIMenu's legacy stacking; the rows are laid out by the flex gaps.
            ButtonSpacing = UILayoutConstants.SettingsMenu.RowGap;
        }

        protected override void BuildUIObjects()
        {
            TabBar = _factory.CreateElement<UITabBar, UITabBarHandler, UITabBarRenderer>();
            TabBar.Style = UILayoutStyles.SettingsMenu.TabBarStyle;
            TabBar.LayoutRules.Apply(UILayoutSheet.SettingsMenu.TabBar);
            TabBar.Handler.SelectionChanged = index => Handler.SelectTab(index);
            AddChild(TabBar);

            FooterRow = _factory.CreateElement<UIButtonRow, UIButtonRowHandler, UIButtonRowRenderer>();
            FooterRow.LayoutRules.Apply(UILayoutSheet.SettingsMenu.FooterRow);
            ScrollContainer.AddChild(FooterRow);
        }

        protected override void OnInit(IUiFactory factory)
        {
            // Declared sizes (also the pre-layout fallback), then the layout rules that
            // actually place the panel and its scroll container (see UILayoutSheet.SettingsMenu).
            Layout = UILayoutConstants.Submenu.Layout;
            ScrollContainer.Layout = UILayoutConstants.Submenu.ScrollContainer.Layout;

            LayoutRules.Apply(UILayoutSheet.SettingsMenu.Panel);
            ScrollContainer.LayoutRules.Apply(UILayoutSheet.SettingsMenu.ScrollContainer);
        }

        /// <summary>The 恢復初始 button; the tab's rows come with <see cref="ShowPage"/>.</summary>
        protected override void BuildButtons()
        {
            var reset = CreateButton(UILayoutSheet.SettingsMenu.FooterButton, UILayoutStyles.SettingsMenu.ButtonStyle, () => Handler.ResetRequested());
            reset.Text = SettingsTexts.ResetTab;
            Buttons.Add(reset);
            FooterRow.AddChild(reset);
        }

        /// <summary>Sets the tab bar's tabs to <paramref name="titles"/> with tab <paramref name="selected"/> selected (no selection callback).</summary>
        public void SetTabs(IEnumerable<string> titles, int selected)
        {
            TabBar.SetTabs(titles);
            if (TabBar.Tabs.Count > 0)
                TabBar.SelectedIndex = Math.Clamp(selected, 0, TabBar.Tabs.Count - 1);
        }

        /// <summary>
        /// Replaces the shown tab's headers and rows with those of <paramref name="page"/>
        /// (whose controls show <paramref name="settings"/>' values), back at the top of the list.
        /// </summary>
        public void ShowPage(SettingsMenuPage page, PlayerSettings settings)
        {
            ClearPage();

            foreach (var section in page.Sections)
            {
                if (section.Header != null)
                {
                    var header = CreateButton(UILayoutSheet.SettingsMenu.Header, UILayoutStyles.SettingsMenu.HeaderStyle, null);
                    header.Text = section.Header;
                    AddPageButton(header);
                    AddPageElement(header);
                }

                foreach (var item in section.Items)
                    AddPageElement(CreateRow(item));
            }

            RefreshValues(settings);
            Handler.UpdateScrollContentHeight();
        }

        /// <summary>Updates every row (name, switch state and availability, values, text) from <paramref name="settings"/>.</summary>
        public void RefreshValues(PlayerSettings settings)
        {
            foreach (var refresh in _refreshers)
                refresh(settings);
        }

        #region Rows

        /// <summary>A setting's row (<see cref="SettingsRowBuilder"/>): its edits go to the handler, its refreshers to this tab's.</summary>
        private UILabeledRow CreateRow(SettingsMenuItem item) => SettingsRowBuilder.Create(_factory, item, Handler, _refreshers);

        private UIButton CreateButton(UILayoutStyle rules, IButtonDrawStyle style, Action onClick)
        {
            var button = _factory.CreateElement<UIButton, UIButtonHandler, UIButtonRenderer>();
            button.Handler.Action = onClick;
            button.Style = style;
            button.LayoutRules.Apply(rules);
            return button;
        }

        /// <summary>A button of the shown tab: drawn by the menu (<see cref="UIMenu{TElement, THandler, TRenderer}.Buttons"/>), removed by <see cref="ClearPage"/>.</summary>
        private void AddPageButton(UIButton button)
        {
            Buttons.Add(button);
            _pageButtons.Add(button);
        }

        /// <summary>A header or row of the shown tab, added to the scroll content.</summary>
        private void AddPageElement(UIElement element)
        {
            ScrollContainer.AddChild(element);
            _pageElements.Add(element);
        }

        /// <summary>Disposes the shown tab's headers and rows (which also detaches them and their controls), keeping the footer row.</summary>
        private void ClearPage()
        {
            foreach (var button in _pageButtons)
                Buttons.Remove(button);
            foreach (var element in _pageElements)
                element.Dispose();
            _pageButtons.Clear();
            _pageElements.Clear();
            _refreshers.Clear();
        }

        #endregion
    }
}

/* ----- ----- ----- ----- */
// UISettingsMenu.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/01
// Update Date: 2026/10/05
// Version: v2.2
/* ----- ----- ----- ----- */

using System;
using System.Collections.Generic;
using System.Linq;

using Chinese_Chess_v3.Game.Application.Settings;
using Chinese_Chess_v3.Game.Configs;
using Chinese_Chess_v3.Game.UI.Constants;
using Chinese_Chess_v3.Game.UI.Menus.CategoryListMenu;

using Engine.Platform;
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
            reset.Text = GameMenuTexts.SettingsResetTab;
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
                    AddPageElement(CreateRow(item, settings));
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

        /// <summary>A setting's row: the name, and the control of the item's kind.</summary>
        /// <exception cref="ArgumentOutOfRangeException">The item is of an unknown kind.</exception>
        private UILabeledRow CreateRow(SettingsMenuItem item, PlayerSettings settings)
        {
            var row = _factory.CreateElement<UILabeledRow, UILabeledRowHandler, UILabeledRowRenderer>();
            row.Font = UILayoutStyles.SettingsMenu.ItemFont;
            row.TextBrush = UILayoutStyles.SettingsMenu.ItemTextBrush;
            row.LayoutRules.Apply(UILayoutSheet.SettingsMenu.Item);
            _refreshers.Add(s => row.Text = item.RowText(s));

            switch (item)
            {
                case SettingsToggleItem toggle:
                    var toggleSwitch = _factory.CreateElement<UIToggleSwitch, UIToggleSwitchHandler, UIToggleSwitchRenderer>();
                    toggleSwitch.Style = UILayoutStyles.SettingsMenu.ToggleStyle;
                    toggleSwitch.LayoutRules.Apply(UILayoutSheet.SettingsMenu.Toggle);
                    toggleSwitch.Handler.ValueChanged = value => Handler.SetToggle(toggle, value);
                    row.AddChild(toggleSwitch);
                    _refreshers.Add(s =>
                    {
                        toggleSwitch.IsOn = toggle.Get(s);
                        toggleSwitch.IsEnabled = toggle.IsAvailable(s);
                    });
                    break;

                case SettingsChoiceItem choice:
                    var dropdown = _factory.CreateElement<UIDropdown, UIDropdownHandler, UIDropdownRenderer>();
                    dropdown.Style = UILayoutStyles.SettingsMenu.DropdownStyle;
                    dropdown.Font = UILayoutStyles.SettingsMenu.ValueFont;
                    dropdown.SetOptions(choice.Options);
                    dropdown.LayoutRules.Apply(UILayoutSheet.SettingsMenu.Dropdown(DropdownWidth(choice)));
                    dropdown.Handler.SelectionChanged = index => Handler.SetChoice(choice, index);
                    row.AddChild(dropdown);
                    _refreshers.Add(s => dropdown.SelectedIndex = choice.GetIndex(s));
                    break;

                case SettingsNumberItem number:
                    var slider = _factory.CreateElement<UISlider, UISliderHandler, UISliderRenderer>();
                    slider.Style = UILayoutStyles.SettingsMenu.SliderStyle;
                    slider.Font = UILayoutStyles.SettingsMenu.ValueFont;
                    slider.SetRange(number.Min, number.Max, number.Step);
                    slider.ValueFormatter = number.FormatValue;
                    slider.LayoutRules.Apply(UILayoutSheet.SettingsMenu.Slider);
                    slider.Handler.ValueChanged = value => Handler.SetNumber(number, value);
                    slider.Handler.ValueCommitted = _ => Handler.CommitEdit();
                    row.AddChild(slider);
                    _refreshers.Add(s => slider.Value = number.Get(s));
                    break;

                case SettingsIntegerItem integer:
                    var numberField = _factory.CreateElement<UINumberField, UINumberFieldHandler, UINumberFieldRenderer>();
                    numberField.Style = UILayoutStyles.SettingsMenu.TextFieldStyle;
                    numberField.Font = UILayoutStyles.SettingsMenu.ValueFont;
                    numberField.UnitBrush = UILayoutStyles.SettingsMenu.ItemTextBrush;
                    numberField.Unit = integer.Unit;
                    numberField.Min = integer.Min;
                    numberField.Max = integer.Max;
                    numberField.LayoutRules.Apply(UILayoutSheet.SettingsMenu.NumberField);
                    numberField.Handler.ValueCommitted = value => Handler.SetInteger(integer, value);
                    row.AddChild(numberField);
                    // Not while typing: the field itself is the source then.
                    _refreshers.Add(s =>
                    {
                        if (!numberField.IsFocused)
                            numberField.Value = integer.Get(s);
                    });
                    break;

                case SettingsTextItem text:
                    var field = _factory.CreateElement<UITextField, UITextFieldHandler, UITextFieldRenderer>();
                    field.Style = UILayoutStyles.SettingsMenu.TextFieldStyle;
                    field.Font = UILayoutStyles.SettingsMenu.ValueFont;
                    field.MaxLength = text.MaxLength;
                    field.Placeholder = text.Placeholder;
                    field.LayoutRules.Apply(UILayoutSheet.SettingsMenu.TextField);
                    field.Handler.TextChanged = value => Handler.SetText(text, value);
                    field.Handler.TextCommitted = _ => Handler.CommitEdit();
                    row.AddChild(field);
                    // Not while typing: the field itself is the source then.
                    _refreshers.Add(s =>
                    {
                        if (!field.IsFocused)
                            field.Text = text.Get(s);
                    });
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(item), item.Kind, "Unknown settings item kind");
            }

            return row;
        }

        /// <summary>
        /// Width of a choice's dropdown: its longest choice's text with the text padding on both
        /// sides, plus the ▼ arrow and its inset; at least a number's width and at most
        /// <see cref="UILayoutConstants.SettingsMenu.ValueMaxWidth"/>.
        /// </summary>
        private static float DropdownWidth(SettingsChoiceItem choice)
        {
            using var g = GraphicsBackend.Factory.CreateMeasurementContext();
            float longest = choice.Options.Max(option => g.MeasureString(option, UILayoutStyles.SettingsMenu.ValueFont).Width);
            float width = longest + UILayoutConstants.SettingsMenu.ValueTextPaddingX * 2f
                + UILayoutConstants.SettingsMenu.DropdownArrowWidth + UILayoutConstants.SettingsMenu.DropdownArrowInset;
            return Math.Clamp(width, UILayoutConstants.SettingsMenu.ValueWidth, UILayoutConstants.SettingsMenu.ValueMaxWidth);
        }

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

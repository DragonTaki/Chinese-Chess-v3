/* ----- ----- ----- ----- */
// UISettingsMenu.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/01
// Update Date: 2026/10/01
// Version: v1.0
/* ----- ----- ----- ----- */

using System;
using System.Collections.Generic;

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
    /// The settings submenu (遊戲設定 / 規則設定, docs/SETTINGS.md): a save / back row, then
    /// per section a header and one button per setting showing <c>名稱：開</c> (or the current
    /// choice) that flips or cycles the setting on click. The logic - what a click edits, saving,
    /// discarding - is the handler's (<see cref="UISettingsMenuHandler"/>); what is listed is
    /// <see cref="SettingsMenuContent"/>.
    /// <para>
    /// The setting buttons are rebuilt each time the submenu is shown
    /// (<see cref="ShowSections"/>); a click only rewrites the button texts.
    /// </para>
    /// </summary>
    public class UISettingsMenu : UIMenu<UISettingsMenu, UISettingsMenuHandler, UISettingsMenuRenderer>
    {
        /// <summary>Which settings are listed (set by the main menu before the first show).</summary>
        public SettingsMenuScope Scope { get; set; } = SettingsMenuScope.All;

        /// <summary>The row holding the save and back buttons (first block of the scroll content).</summary>
        internal UIButtonRow FooterRow { get; private set; }

        private readonly List<(UIButton Button, SettingsMenuItem Item)> _itemButtons = new();
        private readonly List<UIButton> _headerButtons = new();

        public UISettingsMenu() { }

        protected override void OnBeforeInit(IUiFactory factory)
        {
            // Only used by UIMenu's legacy stacking; the rows are laid out by the flex gaps.
            ButtonSpacing = UILayoutConstants.SettingsMenu.RowGap;
        }

        protected override void BuildUIObjects()
        {
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

        /// <summary>The save and back buttons; the setting buttons come with <see cref="ShowSections"/>.</summary>
        protected override void BuildButtons()
        {
            var save = CreateButton(UILayoutSheet.SettingsMenu.FooterButton, UILayoutStyles.SettingsMenu.ButtonStyle, () => Handler.SaveAndClose());
            save.Text = GameMenuTexts.SettingsSaveAndBack;
            FooterRow.AddChild(save);

            var back = CreateButton(UILayoutSheet.SettingsMenu.FooterButton, UILayoutStyles.SettingsMenu.ButtonStyle, () => Handler.BackRequested());
            back.Text = GameMenuTexts.SettingsBack;
            FooterRow.AddChild(back);
        }

        /// <summary>
        /// Replaces the section headers and setting buttons with those of
        /// <paramref name="sections"/> (whose items show <paramref name="settings"/>' values).
        /// </summary>
        public void ShowSections(IEnumerable<SettingsMenuSection> sections, PlayerSettings settings)
        {
            ClearSectionButtons();

            foreach (var section in sections)
            {
                var header = CreateButton(UILayoutSheet.SettingsMenu.Header, UILayoutStyles.SettingsMenu.HeaderStyle, null);
                header.Text = section.Header;
                ScrollContainer.AddChild(header);
                _headerButtons.Add(header);

                foreach (var item in section.Items)
                {
                    var target = item;
                    var button = CreateButton(UILayoutSheet.SettingsMenu.Item, UILayoutStyles.SettingsMenu.ButtonStyle, () => Handler.ChangeItem(target));
                    button.Text = item.Text(settings);
                    ScrollContainer.AddChild(button);
                    _itemButtons.Add((button, item));
                }
            }

            Handler.UpdateScrollContentHeight();
        }

        /// <summary>Rewrites every setting button's text from <paramref name="settings"/>.</summary>
        public void RefreshTexts(PlayerSettings settings)
        {
            foreach (var (button, item) in _itemButtons)
                button.Text = item.Text(settings);
        }

        private UIButton CreateButton(UILayoutStyle rules, IButtonDrawStyle style, Action onClick)
        {
            var button = _factory.CreateElement<UIButton, UIButtonHandler, UIButtonRenderer>();
            button.Handler.Action = onClick;
            button.Style = style;
            button.LayoutRules.Apply(rules);
            Buttons.Add(button);
            return button;
        }

        /// <summary>Disposes every header and setting button (which also detaches it), keeping the save / back row.</summary>
        private void ClearSectionButtons()
        {
            foreach (var header in _headerButtons)
            {
                Buttons.Remove(header);
                header.Dispose();
            }
            foreach (var (button, _) in _itemButtons)
            {
                Buttons.Remove(button);
                button.Dispose();
            }
            _headerButtons.Clear();
            _itemButtons.Clear();
        }
    }
}

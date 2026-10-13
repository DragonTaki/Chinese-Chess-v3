/* ----- ----- ----- ----- */
// UIKindMenu.AiSetup.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/12
// Update Date: 2026/10/13
// Version: v1.2
/* ----- ----- ----- ----- */

using System;
using System.Collections.Generic;
using System.Linq;

using Chinese_Chess_v3.Game.Application.Settings;
using Chinese_Chess_v3.Game.Application.Texts;
using Chinese_Chess_v3.Game.Configs;
using Chinese_Chess_v3.Game.Core;
using Chinese_Chess_v3.Game.UI.Constants;
using Chinese_Chess_v3.Game.UI.Menus.CategoryListMenu;
using Chinese_Chess_v3.Game.UI.Menus.SettingsMenu;

using Engine.Platform;
using Engine.UI.Constants.Core;
using Engine.UI.Core.Elements;
using Engine.UI.Core.Handlers;
using Engine.UI.Core.Renderers;

namespace Chinese_Chess_v3.Game.UI.Menus.KindMenu
{
    public partial class UIKindMenu
    {
        private readonly List<Action<PlayerSettings>> _refreshers = new();

        internal void RefreshSettings()
        {
            foreach (var refresh in _refreshers) refresh(Handler.Live);
        }

        /// <summary>Shows one local mode's existing rule and clock rows.</summary>
        public void ShowSinglePlayerSetup(GameKind kind)
        {
            ShowRuleSettings(kind);
            AddStartRow();
            PerformLayout();
        }

        private void ShowRuleSettings(GameKind kind)
        {
            ClearPage();
            Handler.PrepareSettings(kind);
            ScrollContainer.LayoutRules.Apply(UILayoutSheet.OnlineMenu.PanelScrollContainer);
            AddSetupHeading(SettingsTexts.GameKindName(kind));
            foreach (var section in SettingsMenuContent.RulePage(kind).Sections)
            {
                if (section.Header != null) AddSetupHeading(section.Header);
                foreach (var item in section.Items)
                    ScrollContainer.AddChild(SettingsRowBuilder.Create(_factory, item, Handler, _refreshers));
            }
            RefreshSettings();
        }

        private void AddSetupHeading(string text)
        {
            var heading = _factory.CreateElement<UILabel, UILabelHandler, UILabelRenderer>();
            heading.Text = text;
            heading.Font = UILayoutStyles.CategoryListMenu.SectionHeading.Font;
            heading.ForeColor = UILayoutStyles.CategoryListMenu.SectionHeading.Color;
            heading.TextAlign = ContentAlign.MiddleLeft;
            heading.LayoutRules.Apply(UILayoutSheet.OnlineMenu.Header);
            ScrollContainer.AddChild(heading);
        }

        /// <summary>Returns this same right-hand panel to the game-kind choices.</summary>
        public void ShowKinds()
        {
            ClearPage();
            ScrollContainer.LayoutRules.Apply(UILayoutSheet.Submenu.ScrollContainer);
            BuildButtons();
            PerformLayout();
        }

        private void ClearPage()
        {
            foreach (var child in ScrollContainer.Children.ToArray()) child.Dispose();
            Buttons.Clear();
            _refreshers.Clear();
            ScrollContainer.ScrollY = 0f;
            ScrollContainer.ScrollVelocity = 0f;
        }

        /// <summary>Shows the functional pre-game form inside this submenu, before the game starts.</summary>
        public void ShowAiSetup()
        {
            ShowRuleSettings(GameKind.Traditional);
            var presenter = Handler.AiSetup;
            AddSetupHeading(SettingsMenuContent.AiSection.Header);
            foreach (var item in SettingsMenuContent.AiSection.Items)
                ScrollContainer.AddChild(SettingsRowBuilder.Create(_factory, item, Handler, _refreshers));
            RefreshSettings();

            var assign = _factory.CreateElement<UICheckbox, UICheckboxHandler, UICheckboxRenderer>();
            assign.Text = presenter.AssignOpeningLabel;
            assign.Font = UILayoutStyles.OnlineMenu.RowFont;
            assign.Style = UILayoutStyles.OnlineMenu.CheckboxStyle;
            assign.LayoutRules.Apply(UILayoutSheet.OnlineMenu.Checkbox);
            ScrollContainer.AddChild(assign);

            var opening = AddSetupChoice(presenter.OpeningLabel, presenter.OpeningStyles);
            opening.Handler.SelectionChanged = index => presenter.OpeningStyle = presenter.OpeningStyles[index];

            void RefreshOpening()
            {
                assign.IsEnabled = presenter.OpeningStyles.Count > 0;
                assign.IsChecked = presenter.AssignOpening;
                opening.SetOptions(presenter.OpeningStyles);
                opening.SelectedIndex = presenter.OpeningStyles.ToList().IndexOf(presenter.OpeningStyle);
                opening.IsEnabled = presenter.AssignOpening;
                opening.Parent.IsVisible = presenter.AssignOpening;
                opening.Parent.LayoutRules.Display = presenter.AssignOpening
                    ? DisplayMode.Normal : DisplayMode.None;
                PerformLayout();
            }
            _refreshers.Add(settings =>
            {
                presenter.Level = settings.Ai.Level;
                presenter.SetPlayerSide(settings.Ai.PlayerSide);
                RefreshOpening();
            });
            assign.Changed += value => { presenter.SetAssignOpening(value); RefreshOpening(); };

            AddStartRow();
            RefreshOpening();
        }

        private UIDropdown AddSetupChoice(string label, IEnumerable<string> choices)
        {
            var row = _factory.CreateElement<UILabeledRow, UILabeledRowHandler, UILabeledRowRenderer>();
            row.Text = label;
            row.Font = UILayoutStyles.SettingsMenu.ItemFont;
            row.TextBrush = UILayoutStyles.SettingsMenu.ItemTextBrush;
            row.LayoutRules.Apply(UILayoutSheet.SettingsMenu.Item);
            ScrollContainer.AddChild(row);
            var choice = _factory.CreateElement<UIDropdown, UIDropdownHandler, UIDropdownRenderer>();
            choice.Font = UILayoutStyles.SettingsMenu.ValueFont;
            choice.Style = UILayoutStyles.SettingsMenu.DropdownStyle;
            choice.SetOptions(choices);
            choice.LayoutRules.Apply(UILayoutSheet.SettingsMenu.Dropdown(300f));
            row.AddChild(choice);
            return choice;
        }

        private void AddStartRow()
        {
            var row = _factory.CreateElement<UIButtonRow, UIButtonRowHandler, UIButtonRowRenderer>();
            row.LayoutRules.Apply(UILayoutSheet.Submenu.GroupRow);
            ScrollContainer.AddChild(row);
            AddSetupButton(row, AiSetupTexts.Back, Handler.BackToKinds);
            AddSetupButton(row, AiSetupTexts.Start, Handler.StartGame);
        }

        private void AddSetupButton(UIButtonRow row, string text, Action action)
        {
            var button = _factory.CreateElement<UIButton, UIButtonHandler, UIButtonRenderer>();
            button.Text = text;
            button.Style = UILayoutStyles.SettingsMenu.ButtonStyle;
            button.Handler.Action = action;
            button.LayoutRules.Apply(UILayoutSheet.Submenu.GroupButton);
            row.AddChild(button);
            Buttons.Add(button);
        }
    }
}

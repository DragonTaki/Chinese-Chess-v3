/* ----- ----- ----- ----- */
// UIKindMenu.AiSetup.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/12
// Update Date: 2026/10/13
// Version: v1.3
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

        /// <summary>
        /// Shows the pre-game form: strength, who moves first, the clock, then 指定電腦開局, a checkbox with its
        /// always-present dropdown directly to the right (greyed until checked), and 返回 / 開始遊戲.
        /// </summary>
        public void ShowAiSetup()
        {
            ClearPage();
            Handler.PrepareSettings(GameKind.Traditional);
            ScrollContainer.LayoutRules.Apply(UILayoutSheet.OnlineMenu.PanelScrollContainer);
            var presenter = Handler.AiSetup;
            presenter.Load(Handler.Live.Ai);
            AddSetupHeading(presenter.Title);
            // Rule switches first (when the mode has any), then strength, side, clock, opening.
            foreach (var section in SettingsMenuContent.AiRuleSections.Where(x => x != SettingsMenuContent.AiTimerSection))
            {
                AddSetupHeading(section.Header);
                foreach (var item in section.Items)
                    ScrollContainer.AddChild(SettingsRowBuilder.Create(_factory, item, Handler, _refreshers));
            }
            AddSetupHeading(SettingsMenuContent.AiSection.Header);
            foreach (var item in SettingsMenuContent.AiSection.Items)
                ScrollContainer.AddChild(SettingsRowBuilder.Create(_factory, item, Handler, _refreshers));
            AddSetupHeading(SettingsMenuContent.AiTimerSection.Header);
            foreach (var item in SettingsMenuContent.AiTimerSection.Items)
                ScrollContainer.AddChild(SettingsRowBuilder.Create(_factory, item, Handler, _refreshers));

            var openingRow = _factory.CreateElement<UIButtonRow, UIButtonRowHandler, UIButtonRowRenderer>();
            openingRow.LayoutRules.Apply(UILayoutSheet.Submenu.OpeningRow);
            ScrollContainer.AddChild(openingRow);
            var assign = _factory.CreateElement<UICheckbox, UICheckboxHandler, UICheckboxRenderer>();
            assign.Text = presenter.AssignOpeningLabel;
            assign.Font = UILayoutStyles.OnlineMenu.RowFont;
            assign.Style = UILayoutStyles.OnlineMenu.CheckboxStyle;
            assign.LayoutRules.Apply(UILayoutSheet.Submenu.OpeningCheckbox);
            openingRow.AddChild(assign);
            var opening = _factory.CreateElement<UIDropdown, UIDropdownHandler, UIDropdownRenderer>();
            opening.Font = UILayoutStyles.SettingsMenu.ValueFont;
            opening.Style = UILayoutStyles.SettingsMenu.DropdownStyle;
            opening.LayoutRules.Apply(UILayoutSheet.Submenu.OpeningDropdown);
            openingRow.AddChild(opening);
            opening.Handler.SelectionChanged = index =>
            {
                if (index < 0 || index >= presenter.OpeningStyles.Count) return;
                presenter.SelectOpening(presenter.OpeningStyles[index]);
                Handler.PersistOpening();
            };

            // The dropdown always exists and is always shown: only its enabled state follows the checkbox.
            void RefreshOpening()
            {
                assign.IsEnabled = presenter.OpeningStyles.Count > 0;
                assign.IsChecked = presenter.AssignOpening;
                opening.SetOptions(presenter.OpeningStyles);
                opening.SelectedIndex = presenter.OpeningStyles.ToList().IndexOf(presenter.OpeningStyle);
                opening.IsEnabled = presenter.AssignOpening;
            }
            _refreshers.Add(settings =>
            {
                presenter.SyncChoices(settings.Ai);
                RefreshOpening();
            });
            assign.Changed += value => { presenter.SetAssignOpening(value); RefreshOpening(); Handler.PersistOpening(); };

            RefreshSettings();
            AddStartRow();
            PerformLayout();
        }

        private void AddStartRow()
        {
            var row = _factory.CreateElement<UIButtonRow, UIButtonRowHandler, UIButtonRowRenderer>();
            row.LayoutRules.Apply(UILayoutSheet.Submenu.SetupButtonRow);
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

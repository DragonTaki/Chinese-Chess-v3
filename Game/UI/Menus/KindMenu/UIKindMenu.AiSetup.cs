/* ----- ----- ----- ----- */
// UIKindMenu.AiSetup.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/12
// Update Date: 2026/10/13
// Version: v1.4
/* ----- ----- ----- ----- */

using System;
using System.Collections.Generic;
using System.Linq;

using Chinese_Chess_v3.Game.Application.MainMenu;
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

        /// <summary>The tab bar of the pre-game pages (a child of the panel, above the scrolling area); null while the mode list shows.</summary>
        private UITabBar _setupTabs;

        /// <summary>The fixed bottom row (a child of the panel, not of the scroll container); null while the mode list shows.</summary>
        private UIButtonRow _setupRow;

        private IReadOnlyList<SettingsMenuPage> _setupPages = Array.Empty<SettingsMenuPage>();
        private string _setupTitle = string.Empty;

        /// <summary>The bottom row's buttons: drawn by <see cref="UIKindMenuRenderer"/> without the scroll area's clip.</summary>
        internal List<UIButton> FixedButtons { get; } = new();

        /// <summary>Shows one local mode's pages: 規則 and 計時.</summary>
        public void ShowSinglePlayerSetup(GameKind kind)
        {
            ClearPage();
            ClearSetupChrome();
            Handler.PrepareSettings(kind);
            BeginSetup(SettingsTexts.GameKindName(kind));
            ShowSetupPage(0);
        }

        /// <summary>Creates the tab bar, the scrolling area's layout and the fixed bottom row for the prepared pages.</summary>
        private void BeginSetup(string title)
        {
            _setupTitle = title;
            _setupPages = Handler.SetupPages;
            ScrollContainer.LayoutRules.Apply(UILayoutSheet.Submenu.SetupScrollContainer);

            _setupTabs = _factory.CreateElement<UITabBar, UITabBarHandler, UITabBarRenderer>();
            _setupTabs.Style = UILayoutStyles.SettingsMenu.TabBarStyle;
            _setupTabs.LayoutRules.Apply(UILayoutSheet.SettingsMenu.TabBar);
            _setupTabs.SetTabs(_setupPages.Select(page => page.Title));
            _setupTabs.SelectedIndex = 0;
            _setupTabs.Handler.SelectionChanged = index => Handler.SelectSetupPage(index);
            AddChild(_setupTabs);

            _setupRow = _factory.CreateElement<UIButtonRow, UIButtonRowHandler, UIButtonRowRenderer>();
            _setupRow.LayoutRules.Apply(UILayoutSheet.Submenu.SetupButtonRow);
            AddChild(_setupRow);
            foreach (var (action, text) in PreGameSetup.BottomRow)
                AddSetupButton(_setupRow, text, action switch
                {
                    PreGameAction.Back => Handler.BackToKinds,
                    PreGameAction.Reset => Handler.ResetSettings,
                    _ => Handler.StartGame,
                });
        }

        /// <summary>Replaces the scrolling area's content with page <paramref name="index"/> and scrolls it to the top.</summary>
        internal void ShowSetupPage(int index)
        {
            if (index < 0 || index >= _setupPages.Count) return;
            ClearPage();
            if (_setupTabs != null) _setupTabs.SelectedIndex = index;
            // The computer form's choices (strength, side, clock) follow the rows of every page, not only its own.
            if (Purpose == KindMenuPurpose.VsComputer) _refreshers.Add(settings => Handler.AiSetup.SyncChoices(settings.Ai));
            AddSetupHeading(_setupTitle);
            foreach (var section in _setupPages[index].Sections)
            {
                if (section.Header != null) AddSetupHeading(section.Header);
                foreach (var item in section.Items)
                    ScrollContainer.AddChild(SettingsRowBuilder.Create(_factory, item, Handler, _refreshers));
            }
            if (Purpose == KindMenuPurpose.VsComputer && index == PreGameSetup.ComputerPageIndex) AddOpeningRow();
            RefreshSettings();
            PerformLayout();
        }

        private void ClearSetupChrome()
        {
            _setupTabs?.Dispose();
            _setupTabs = null;
            _setupRow?.Dispose();
            _setupRow = null;
            FixedButtons.Clear();
            _setupPages = Array.Empty<SettingsMenuPage>();
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
            ClearSetupChrome();
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
        /// Shows the computer game's pre-game pages: 規則, 計時 and 電腦 (strength, who moves first, then 指定電腦開局, a
        /// checkbox with its always-present dropdown directly to the right, greyed until checked).
        /// </summary>
        public void ShowAiSetup()
        {
            ClearPage();
            ClearSetupChrome();
            Handler.PrepareComputerSettings();
            Handler.AiSetup.Load(Handler.Live.Ai);
            BeginSetup(Handler.AiSetup.Title);
            ShowSetupPage(0);
        }

        /// <summary>The computer page's opening row: the checkbox and, directly to its right, the dropdown.</summary>
        private void AddOpeningRow()
        {
            var presenter = Handler.AiSetup;
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
        }

        private void AddSetupButton(UIButtonRow row, string text, Action action)
        {
            var button = _factory.CreateElement<UIButton, UIButtonHandler, UIButtonRenderer>();
            button.Text = text;
            button.Style = UILayoutStyles.SettingsMenu.ButtonStyle;
            button.Handler.Action = action;
            button.LayoutRules.Apply(UILayoutSheet.Submenu.SetupButton);
            row.AddChild(button);
            FixedButtons.Add(button);
        }
    }
}

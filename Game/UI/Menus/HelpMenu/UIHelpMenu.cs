/* ----- ----- ----- ----- */
// UIHelpMenu.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/14
// Update Date: 2026/10/14
// Version: v1.1
/* ----- ----- ----- ----- */

using System.Linq;

using Chinese_Chess_v3.Game.Application.MainMenu;
using Chinese_Chess_v3.Game.UI.Constants;
using Chinese_Chess_v3.Game.UI.Menus.MainMenu;

using Engine.UI.Core.Elements;
using Engine.UI.Core.Handlers;
using Engine.UI.Core.Interfaces;
using Engine.UI.Core.Renderers;

namespace Chinese_Chess_v3.Game.UI.Menus.HelpMenu
{
    public class UIHelpMenu : UIMenu<UIHelpMenu, UIHelpMenuHandler, UIHelpMenuRenderer>
    {
        public UITabBar TabBar { get; private set; }

        protected override void OnBeforeInit(IUiFactory factory)
        {
            ButtonSpacing = UILayoutConstants.Submenu.Button.Spacing;
        }

        protected override void OnInit(IUiFactory factory)
        {
            Layout = UILayoutConstants.Submenu.Layout;
            ScrollContainer.Layout = UILayoutConstants.Submenu.ScrollContainer.Layout;
            LayoutRules.Apply(UILayoutSheet.Submenu.Panel);
            ScrollContainer.LayoutRules.Apply(UILayoutSheet.SettingsMenu.ScrollContainer with
            {
                RowGap = UILayoutConstants.Submenu.Button.Spacing,
            });
            TabBar = factory.CreateElement<UITabBar, UITabBarHandler, UITabBarRenderer>();
            TabBar.Style = UILayoutStyles.SettingsMenu.TabBarStyle;
            TabBar.LayoutRules.Apply(UILayoutSheet.SettingsMenu.TabBar);
            TabBar.Handler.SelectionChanged = Handler.Presenter.SelectTab;
            AddChild(TabBar);
        }

        protected override void OnBeforeDispose()
        {
            Handler.Presenter.Changed -= RefreshPage;
            base.OnBeforeDispose();
        }

        protected override void BuildButtons() => RefreshPage();

        internal void RefreshPage()
        {
            var model = Handler.Presenter.Model;
            TabBar.SetTabs(model.Tabs);
            TabBar.SelectedIndex = (int)model.Tab;
            ScrollContainer.ScrollY = 0f;
            ScrollContainer.ScrollVelocity = 0f;
            SetupButtons(model.Entries.Select(entry => (entry.Label, entry.Action)));
            var mainEntries = MainMenuOptions.Create(_ => { });
            int index = mainEntries.ToList().FindIndex(entry => entry.Id == MainMenuOption.Help);
            var accent = MenuAccent.Gradient(mainEntries.Count)[index];
            foreach (var button in Buttons)
            {
                button.Size = UILayoutConstants.Submenu.Button.Size;
                button.LayoutRules.Apply(UILayoutSheet.Submenu.Button);
                button.Style = UILayoutStyles.MainMenu.Button.StyleFor(accent);
            }
            Handler.UpdateScrollContentHeight();
        }
    }
}

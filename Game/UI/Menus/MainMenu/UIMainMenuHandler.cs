/* ----- ----- ----- ----- */
// UIMainMenuHandler.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/17
// Update Date: 2026/10/05
// Version: v2.3
/* ----- ----- ----- ----- */

using System;
using System.Collections.Generic;

using Chinese_Chess_v3.Game.Application.MainMenu;
using Chinese_Chess_v3.Game.Application.Settings;
using Chinese_Chess_v3.Game.UI.Menus.EndgameMenu;
using Chinese_Chess_v3.Game.UI.Menus.LoadGameMenu;
using Chinese_Chess_v3.Game.UI.Menus.LoadSavedGameMenu;
using Chinese_Chess_v3.Game.UI.Menus.NewGameMenu;
using Chinese_Chess_v3.Game.UI.Menus.OpeningMenu;
using Chinese_Chess_v3.Game.UI.Menus.SettingsMenu;

using Engine.UI.Core.Elements;
using Engine.UI.Core.Handlers;
using Engine.UI.Core.Interfaces;

using Microsoft.Extensions.DependencyInjection;

namespace Chinese_Chess_v3.Game.UI.Menus.MainMenu
{
    /// <summary>
    /// Binds the UIMainMenu to the <see cref="MainMenuPresenter"/>, which makes the decisions
    /// (which submenu is open, 多人連線, 離開遊戲): each button calls
    /// <see cref="MainMenuPresenter.Select"/>, and the presenter's events show / hide the
    /// submenu elements this handler creates.
    /// </summary>
    public class UIMainMenuHandler : UIMenuHandler<UIMainMenu, UIMainMenuHandler, UIMainMenuRenderer>, IScreen
    {
        private readonly Dictionary<MainMenuOption, UIElement> _submenus = new();

        public UIMainMenuHandler() { }

        /// <summary>
        /// Creates the submenu elements (hidden) and subscribes to the presenter's events once:
        /// the screen (and this handler) is a single long-lived instance, like the
        /// <see cref="MainMenuPresenter"/> it listens to.
        /// </summary>
        protected override void OnInit(IUiFactory factory)
        {
            // Initialize _submenus
            foreach (var option in MainMenuPresenter.SubmenuOptions)
                _submenus[option] = CreateSubMenu(() => CreateSubmenuElement(factory, option));

            var presenter = Presenter;
            presenter.SubmenuOpened += ShowSubmenu;
            presenter.SubmenuClosed += HideSubmenu;
        }

        /// <summary>The main menu's decisions (the <see cref="MainMenuPresenter"/> registered in DI).</summary>
        private MainMenuPresenter Presenter => _factory.ServiceProvider.GetRequiredService<MainMenuPresenter>();

        /// <summary>The element of <paramref name="option"/>'s submenu.</summary>
        private static UIElement CreateSubmenuElement(IUiFactory factory, MainMenuOption option)
        {
            if (MainMenuPresenter.SettingsScreenOf(option) is SettingsScreen screen)
                return CreateSettingsMenu(factory, screen);

            return option switch
            {
                MainMenuOption.NewGame => factory.CreateDIElement<UINewGameMenu, UINewGameMenuHandler, UINewGameMenuRenderer>(),
                // 讀取存檔: the player's saved games (the files the game screen's 載入 list shows).
                MainMenuOption.LoadGame => factory.CreateDIElement<UILoadSavedGameMenu, UILoadSavedGameMenuHandler, UILoadSavedGameMenuRenderer>(),
                MainMenuOption.EndgameChallenge => factory.CreateDIElement<UIEndgameMenu, UIEndgameMenuHandler, UIEndgameMenuRenderer>(),
                MainMenuOption.OpeningPractice => factory.CreateDIElement<UIOpeningMenu, UIOpeningMenuHandler, UIOpeningMenuRenderer>(),
                MainMenuOption.Help => factory.CreateDIElement<UILoadGameMenu, UILoadGameMenuHandler, UILoadGameMenuRenderer>(),
                _ => throw new ArgumentOutOfRangeException(nameof(option), option, "Not a submenu option"),
            };
        }

        /// <summary>
        /// A settings screen (<paramref name="screen"/>: 遊戲設定 or 單機規則設定), opening on its
        /// first tab (畫面 / 傳統大盤); set before it is first shown, which is when its rows are built.
        /// </summary>
        private static UIElement CreateSettingsMenu(IUiFactory factory, SettingsScreen screen)
        {
            var menu = factory.CreateDIElement<UISettingsMenu, UISettingsMenuHandler, UISettingsMenuRenderer>();
            menu.Screen = screen;
            menu.InitialTab = 0;
            return menu;
        }

        /// <summary>
        /// Create a submenu and set it to invisible by default.
        /// </summary>
        private static UIElement CreateSubMenu(Func<UIElement> factory)
        {
            var submenu = factory();
            submenu.IsVisible = false;
            return submenu;
        }

        /// <summary>
        /// A main menu button: passed to the presenter (<see cref="MainMenuPresenter.Select"/>),
        /// which opens / closes a submenu, connects, or asks before exiting.
        /// </summary>
        public void SwitchSubmenu(MainMenuOption selectedMenu) => Presenter.Select(selectedMenu);

        /// <summary>Shows <paramref name="option"/>'s submenu (<see cref="MainMenuPresenter.SubmenuOpened"/>).</summary>
        private void ShowSubmenu(MainMenuOption option)
        {
            var submenu = _submenus[option];
            submenu.IsVisible = true;
            Element.AddChild(submenu);
            AsScreen(submenu)?.OnEnter();
        }

        /// <summary>Hides <paramref name="option"/>'s submenu and removes it from the view (<see cref="MainMenuPresenter.SubmenuClosed"/>).</summary>
        private void HideSubmenu(MainMenuOption option)
        {
            var submenu = _submenus[option];
            submenu.IsVisible = false;
            Element.RemoveChild(submenu);
            AsScreen(submenu)?.OnExit();
        }

        /// <summary>
        /// A submenu's <see cref="IScreen"/> implementation (the element itself or its
        /// handler, as NavigationManager looks it up for screens): submenus that refresh
        /// their content when opened (e.g. the endgame menu reloading its files) get
        /// OnEnter/OnExit as they are shown/closed.
        /// </summary>
        private static IScreen AsScreen(UIElement submenu) =>
            submenu as IScreen ?? submenu.HandlerBase as IScreen;

        public Dictionary<MainMenuOption, UIElement> Submenus => _submenus;
        public MainMenuOption? CurrentSubmenu => Presenter.CurrentSubmenu;


        public void OnEnter() { }

        /// <summary>Leaving the main menu: closes the open submenu and forgets it (<see cref="MainMenuPresenter.CloseSubmenu"/>).</summary>
        public void OnExit() => Presenter.CloseSubmenu();
    }
}

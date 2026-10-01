/* ----- ----- ----- ----- */
// UIMainMenuHandler.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/17
// Update Date: 2026/10/02
// Version: v2.2
/* ----- ----- ----- ----- */

using System;
using System.Collections.Generic;

using Chinese_Chess_v3.Game.UI.Dialogs;
using Chinese_Chess_v3.Game.UI.Menus.EndgameMenu;
using Chinese_Chess_v3.Game.UI.Menus.LoadGameMenu;
using Chinese_Chess_v3.Game.UI.Menus.LoadSavedGameMenu;
using Chinese_Chess_v3.Game.UI.Menus.NewGameMenu;
using Chinese_Chess_v3.Game.UI.Menus.OpeningMenu;
using Chinese_Chess_v3.Game.UI.Menus.SettingsMenu;

using Engine.Network;
using Engine.Platform;
using Engine.UI.Core.Elements;
using Engine.UI.Core.Handlers;
using Engine.UI.Core.Interfaces;

using Microsoft.Extensions.DependencyInjection;

namespace Chinese_Chess_v3.Game.UI.Menus.MainMenu
{
    /// <summary>
    /// Handles logic and interactions for the UIMainMenu.
    /// </summary>
    public class UIMainMenuHandler : UIMenuHandler<UIMainMenu, UIMainMenuHandler, UIMainMenuRenderer>, IScreen
    {
        private UIMainMenuType? _currentSubmenu = null;
        private readonly Dictionary<UIMainMenuType, UIElement> _submenus = new();

        public UIMainMenuHandler() { }
        protected override void OnInit(IUiFactory factory)
        {
            // Initialize _submenus
            _submenus[UIMainMenuType.NewGame] = CreateSubMenu(() => factory.CreateDIElement<UINewGameMenu, UINewGameMenuHandler, UINewGameMenuRenderer>());
            // 讀取存檔: the player's saved games (the files the game screen's 載入 list shows).
            _submenus[UIMainMenuType.LoadGame] = CreateSubMenu(() => factory.CreateDIElement<UILoadSavedGameMenu, UILoadSavedGameMenuHandler, UILoadSavedGameMenuRenderer>());
            _submenus[UIMainMenuType.EndgameChallenge] = CreateSubMenu(() => factory.CreateDIElement<UIEndgameMenu, UIEndgameMenuHandler, UIEndgameMenuRenderer>());
            _submenus[UIMainMenuType.OpeningPractice] = CreateSubMenu(() => factory.CreateDIElement<UIOpeningMenu, UIOpeningMenuHandler, UIOpeningMenuRenderer>());
            _submenus[UIMainMenuType.RuleSettings] = CreateSubMenu(() => CreateSettingsMenu(factory, SettingsMenuScope.Rules));
            _submenus[UIMainMenuType.Help] = CreateSubMenu(() => factory.CreateDIElement<UILoadGameMenu, UILoadGameMenuHandler, UILoadGameMenuRenderer>());
            _submenus[UIMainMenuType.Settings] = CreateSubMenu(() => CreateSettingsMenu(factory, SettingsMenuScope.All));
        }

        /// <summary>A settings submenu listing the settings of <paramref name="scope"/> (set before it is first shown, which is when its buttons are built).</summary>
        private static UIElement CreateSettingsMenu(IUiFactory factory, SettingsMenuScope scope)
        {
            var menu = factory.CreateDIElement<UISettingsMenu, UISettingsMenuHandler, UISettingsMenuRenderer>();
            menu.Scope = scope;
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
        /// Switch to the selected submenu. Clicking the same submenu closes it.
        /// </summary>
        public void SwitchSubmenu(UIMainMenuType selectedMenu)
        {
            Console.WriteLine($"MainMenu: selected: {selectedMenu}");

            // Leaving an open settings submenu (for another entry, or collapsing it by its own
            // entry) with unsaved changes asks first; on yes the changes are discarded and the
            // switch is redone.
            if (IsSubmenuEntry(selectedMenu) && _currentSubmenu.HasValue
                && _submenus[_currentSubmenu.Value] is UISettingsMenu settingsMenu
                && settingsMenu.Handler.HasUnsavedChanges)
            {
                settingsMenu.Handler.ConfirmDiscard(() => SwitchSubmenu(selectedMenu));
                return;
            }

            switch (selectedMenu)
            {
                case UIMainMenuType.Default:
                    break;

                case UIMainMenuType.NewGame:
                case UIMainMenuType.LoadGame:
                case UIMainMenuType.EndgameChallenge:
                case UIMainMenuType.OpeningPractice:
                case UIMainMenuType.RuleSettings:
                case UIMainMenuType.Help:
                case UIMainMenuType.Settings:
                    CancelCurrentSubmenu();

                    if (_currentSubmenu == selectedMenu)  // Same menu clicked again, collapse
                        _currentSubmenu = null;
                    else  // Show new submenu
                    {
                        _currentSubmenu = selectedMenu;
                        var submenu = _submenus[_currentSubmenu.Value];
                        submenu.IsVisible = true;
                        Element.AddChild(submenu);
                        AsScreen(submenu)?.OnEnter();
                    }
                    break;

                case UIMainMenuType.Multiplayer:
                    var networkManager = _factory.ServiceProvider.GetRequiredService<NetworkManager>();

                    if (!networkManager.IsConnected)
                        _ = networkManager.ConnectAsync();
                    else
                        networkManager.Reconnect();

                    break;

                case UIMainMenuType.Exit:
                    ClickExitAction();
                    break;

                default:
                    Console.WriteLine($"MainMenu: selected: 'Not defined'");
                    break;
            }
        }

        /// <summary>Whether <paramref name="menu"/> opens or collapses a submenu (the entries handled together in <see cref="SwitchSubmenu"/>).</summary>
        private static bool IsSubmenuEntry(UIMainMenuType menu) => menu switch
        {
            UIMainMenuType.NewGame or UIMainMenuType.LoadGame or UIMainMenuType.EndgameChallenge
                or UIMainMenuType.OpeningPractice or UIMainMenuType.RuleSettings or UIMainMenuType.Help
                or UIMainMenuType.Settings => true,
            _ => false,
        };

        /// <summary>
        /// Cancel and remove current submenu from the view.
        /// </summary>
        public void CancelCurrentSubmenu()
        {
            if (_currentSubmenu.HasValue)
            {
                var submenu = _submenus[_currentSubmenu.Value];
                submenu.IsVisible = false;
                Element.RemoveChild(submenu);
                AsScreen(submenu)?.OnExit();
            }
        }

        /// <summary>
        /// A submenu's <see cref="IScreen"/> implementation (the element itself or its
        /// handler, as NavigationManager looks it up for screens): submenus that refresh
        /// their content when opened (e.g. the endgame menu reloading its files) get
        /// OnEnter/OnExit as they are shown/closed.
        /// </summary>
        private static IScreen AsScreen(UIElement submenu) =>
            submenu as IScreen ?? submenu.HandlerBase as IScreen;

        private void ClickExitAction()
        {
            DialogManager.ShowConfirm(
                "確認要離開遊戲嗎？",
                ConfirmDialogType.YesNo,
                result =>
                {
                    if (result == ConfirmDialogResult.Yes)
                    {
                        ExitApplication();
                    }
                    else if (result == ConfirmDialogResult.No)
                    {
                        // Stay in the game: the dialog has already closed.
                    }
                }
            );
        }

        /// <summary>
        /// Exit the application.
        /// </summary>
        public static void ExitApplication()
        {
            AppControl.ExitCallback?.Invoke();
        }

        public Dictionary<UIMainMenuType, UIElement> Submenus => _submenus;
        public UIMainMenuType? CurrentSubmenu => _currentSubmenu;


        public void OnEnter() { }
        public void OnExit()
        {
            CancelCurrentSubmenu();

            // Forget it too: otherwise, back on the main menu, the first click on the same
            // submenu's button counted as "clicked again" and collapsed the (already
            // closed) submenu instead of opening it.
            _currentSubmenu = null;
        }
    }
}

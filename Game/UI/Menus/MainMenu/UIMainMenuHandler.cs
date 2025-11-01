/* ----- ----- ----- ----- */
// UIMainMenuHandler.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/17
// Update Date: 2025/10/24
// Version: v2.0
/* ----- ----- ----- ----- */

using System;
using System.Collections.Generic;
using System.Windows.Forms;

using Chinese_Chess_v3.Game.UI.Menus.LoadGameMenu;
using Chinese_Chess_v3.Game.UI.Menus.NewGameMenu;

using Engine.Network;
using Engine.UI.Core.Elements;
using Engine.UI.Core.Handlers;
using Engine.UI.Core.Infrastructure;
using Engine.UI.Core.Interfaces;
using Engine.UI.Dialogs;

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
            _submenus[UIMainMenuType.LoadGame] = CreateSubMenu(() => factory.CreateDIElement<UILoadGameMenu, UILoadGameMenuHandler, UILoadGameMenuRenderer>());
            _submenus[UIMainMenuType.EndgameChallenge] = CreateSubMenu(() => factory.CreateDIElement<UILoadGameMenu, UILoadGameMenuHandler, UILoadGameMenuRenderer>());
            _submenus[UIMainMenuType.RuleSettings] = CreateSubMenu(() => factory.CreateDIElement<UILoadGameMenu, UILoadGameMenuHandler, UILoadGameMenuRenderer>());
            _submenus[UIMainMenuType.Help] = CreateSubMenu(() => factory.CreateDIElement<UILoadGameMenu, UILoadGameMenuHandler, UILoadGameMenuRenderer>());
            _submenus[UIMainMenuType.Settings] = CreateSubMenu(() => factory.CreateDIElement<UILoadGameMenu, UILoadGameMenuHandler, UILoadGameMenuRenderer>());
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
            Console.WriteLine($"MaunMenu: selected: {selectedMenu}");
            switch (selectedMenu)
            {
                case UIMainMenuType.Default:
                    break;

                case UIMainMenuType.NewGame:
                case UIMainMenuType.LoadGame:
                case UIMainMenuType.EndgameChallenge:
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
                    }
                    break;

                case UIMainMenuType.Multiplayer:
                    var networkManager = _factory.ServiceProvider.GetRequiredService<NetworkManager>();

                    if (!networkManager.IsConnected)
                        networkManager.Connect();
                    else
                        networkManager.Reconnect();

                    break;

                case UIMainMenuType.Exit:
                    ClickExitAction();
                    break;

                default:
                    Console.WriteLine($"MaunMenu: selected: 'Not defined'");
                    break;
            }
        }

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
            }
        }

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
                        //
                    }
                }
            );
        }

        /// <summary>
        /// Exit the application.
        /// </summary>
        public static void ExitApplication()
        {
            Application.Exit();
        }

        public Dictionary<UIMainMenuType, UIElement> Submenus => _submenus;
        public UIMainMenuType? CurrentSubmenu => _currentSubmenu;


        public void OnEnter() { }
        public void OnExit()
        {
            CancelCurrentSubmenu();
        }
    }
}

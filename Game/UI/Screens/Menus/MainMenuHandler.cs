/* ----- ----- ----- ----- */
// MainMenuHandler.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/17
// Update Date: 2025/10/24
// Version: v2.0
/* ----- ----- ----- ----- */

using System;
using System.Collections.Generic;
using System.Windows.Forms;

using Chinese_Chess_v3.Game.UI.Screens.Menus.Options;
using Chinese_Chess_v3.Game.UI.Screens.Menus.Submenus;

using Engine.UI.Core.Elements;
using Engine.UI.Core.Handlers;
using Engine.UI.Core.Infrastructure;
using Engine.UI.Core.Interfaces;
using Engine.UI.Dialogs;

namespace Chinese_Chess_v3.Game.UI.Screens.Menus
{
    /// <summary>
    /// Handles logic and interactions for the MainMenu.
    /// </summary>
    public class MainMenuHandler : UIMenuHandler<MainMenuHandler>
    {
        private MainMenuType? _currentSubmenu = null;
        private readonly Dictionary<MainMenuType, UIElement> _submenus = new();

        public MainMenuHandler() { }
        protected override void OnMenuInit((IUiFactory, UIContainer<MainMenuHandler>) arg)
        {
            base.OnMenuInit(arg);

            // Initialize _submenus
            _submenus[MainMenuType.NewGame] = CreateSubMenu(() => _factory.CreateScreen<NewGameMenu, NewGameMenuHandler>());
            _submenus[MainMenuType.LoadGame] = CreateSubMenu(() => _factory.CreateScreen<LoadGameMenu, LoadGameMenuHandler>());
            _submenus[MainMenuType.EndgameChallenge] = CreateSubMenu(() => _factory.CreateScreen<LoadGameMenu, LoadGameMenuHandler>());
            _submenus[MainMenuType.RuleSettings] = CreateSubMenu(() => _factory.CreateScreen<LoadGameMenu, LoadGameMenuHandler>());
            _submenus[MainMenuType.Help] = CreateSubMenu(() => _factory.CreateScreen<LoadGameMenu, LoadGameMenuHandler>());
            _submenus[MainMenuType.Settings] = CreateSubMenu(() => _factory.CreateScreen<LoadGameMenu, LoadGameMenuHandler>());
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
        public void SwitchSubmenu(MainMenuType selectedMenu)
        {
            Console.WriteLine($"MaunMenu: selected: {selectedMenu}");
            if (selectedMenu != MainMenuType.Exit)
            {
                CancelCurrentSubmenu();

                if (_currentSubmenu == selectedMenu)
                {
                    // Same menu clicked again, collapse
                    _currentSubmenu = null;
                }
                else
                {
                    // Show new submenu
                    _currentSubmenu = selectedMenu;
                    var submenu = _submenus[_currentSubmenu.Value];
                    submenu.IsVisible = true;
                    _menu.AddChild(submenu);
                }
            }
            else
            {
                ClickExitAction();
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
                _menu.RemoveChild(submenu);
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

        public Dictionary<MainMenuType, UIElement> Submenus => _submenus;
        public MainMenuType? CurrentSubmenu => _currentSubmenu;

        public override void OnExit()
        {
            CancelCurrentSubmenu();
        }
    }
}

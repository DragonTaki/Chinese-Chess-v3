/* ----- ----- ----- ----- */
// MainMenuHandler.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/17
// Update Date: 2025/05/17
// Version: v1.0
/* ----- ----- ----- ----- */

using System;
using System.Collections.Generic;
using System.Windows.Forms;

using Chinese_Chess_v3.UI.Core.Base;
using Chinese_Chess_v3.UI.Core.Elements;
using Chinese_Chess_v3.UI.Core.Infrastructure;
using Chinese_Chess_v3.UI.Core.Interfaces;
using Chinese_Chess_v3.UI.Dialogs;
using Chinese_Chess_v3.UI.Screens.Menus.Options;
using Chinese_Chess_v3.UI.Screens.Menus.Submenus;

namespace Chinese_Chess_v3.UI.Screens.Menus
{
    /// <summary>
    /// Handles logic and interactions for the MainMenu.
    /// </summary>
    public class MainMenuHandler : InitializableOnceBase<(IUiFactory factory, MainMenu menu)>
    {
        private MainMenu _menu;
        private MainMenuType? _currentSubmenu = null;
        private IUiFactory _factory;
        private NavigationManager _nav;
        private DialogManager _dia;
        private readonly Dictionary<MainMenuType, UIElement> _submenus = new();

        public MainMenuHandler() { }
        protected override void OnInit((IUiFactory factory, MainMenu menu) arg)
        {
            _menu = arg.menu;
            _factory = arg.factory;
            _nav = _factory.Resolve<NavigationManager>();
            _dia = _factory.Resolve<DialogManager>();

            RegisterFactory();
            // Initialize _submenus
            _submenus[MainMenuType.NewGame] = CreateSubMenu(() => _factory.Create<NewGameMenu>());
            _submenus[MainMenuType.LoadGame] = CreateSubMenu(() => _factory.Create<LoadGameMenu>());
            _submenus[MainMenuType.EndgameChallenge] = CreateSubMenu(() => _factory.Create<LoadGameMenu>());
            _submenus[MainMenuType.RuleSettings] = CreateSubMenu(() => _factory.Create<LoadGameMenu>());
            _submenus[MainMenuType.Help] = CreateSubMenu(() => _factory.Create<LoadGameMenu>());
            _submenus[MainMenuType.Settings] = CreateSubMenu(() => _factory.Create<LoadGameMenu>());
        }

        private void RegisterFactory()
        {
            // Initialize submenu creation factory
            _factory.RegisterFactory<NewGameMenu>(ctx =>
            {
                var menu = new NewGameMenu();
                var renderer = new NewGameMenuRenderer(menu);
                var handler = new NewGameMenuHandler(_factory, menu);
                menu.Init((_factory, handler, renderer));
                return menu;
            });

            _factory.RegisterFactory<LoadGameMenu>(ctx =>
            {
                var menu = new LoadGameMenu();
                var renderer = new LoadGameMenuRenderer(menu);
                var handler = new LoadGameMenuHandler(_factory, menu);
                menu.Init((_factory, handler, renderer));
                return menu;
            });
        }

        /// <summary>
        /// Create a submenu and set it to invisible by default.
        /// </summary>
        private static UIElement CreateSubMenu<T>() where T : UIElement, new()
        {
            var submenu = new T();
            submenu.IsVisible = false;
            return submenu;
        }
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

        public void OnEnter()
        {
            //
        }
        public void OnExit()
        {
            CancelCurrentSubmenu();
            
        }
    }
}

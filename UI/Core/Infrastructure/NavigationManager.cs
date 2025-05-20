/* ----- ----- ----- ----- */
// NavigationManager.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/18
// Update Date: 2025/05/18
// Version: v1.0
/* ----- ----- ----- ----- */

using System;
using System.Linq;

using Chinese_Chess_v3.UI.Core.Elements;
using Chinese_Chess_v3.UI.Core.Interfaces;
using Chinese_Chess_v3.UI.Screens.Game;
using Chinese_Chess_v3.UI.Screens.Menu;

namespace Chinese_Chess_v3.UI.Core.Infrastructure
{
    /// <summary>
    /// NavigationManager 負責管理遊戲中各個主要UI畫面的切換。
    /// 它操作一個 Root UIElement 容器，
    /// 可以 Clear 舊畫面、Add 新畫面。
    /// </summary>
    public class NavigationManager
    {
        private readonly IUiFactory _factory;
        private UIElement _rootElement;

        private MainMenu _mainMenu;
        private GameMenu _gameMenu;
        public NavigationManager(IUiFactory factory)
        {
            _factory = factory;
        }

        public void Init(UIElement root)
        {
            _rootElement = root ?? throw new ArgumentNullException(nameof(root));
        }

        /// <summary>
        /// 切換到主選單畫面
        /// </summary>
        public void ShowMainMenu()
        {
            ClearNonPersistentChildren(_rootElement);
            _mainMenu ??= _factory.CreateMainMenu();
            _mainMenu.IsVisible = true;
            _rootElement.AddChild(_mainMenu);
        }

        /// <summary>
        /// 切換到遊戲畫面
        /// </summary>
        public void ShowGameScreen()
        {
            ClearNonPersistentChildren(_rootElement);
            _gameMenu ??= _factory.CreateGameMenu();
            _gameMenu.IsVisible = true;
            _rootElement.AddChild(_gameMenu);
        }

        public static void ClearNonPersistentChildren(UIElement parent)
        {
            var toRemove = parent.Children.Where(c => !c.IsPersistent).ToList();

            foreach (var child in toRemove)
            {
                parent.RemoveChild(child);
            }
        }
    }
}
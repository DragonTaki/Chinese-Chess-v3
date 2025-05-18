/* ----- ----- ----- ----- */
// NavigationManager.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/18
// Update Date: 2025/05/18
// Version: v1.0
/* ----- ----- ----- ----- */

using System;
using Chinese_Chess_v3.UI.Core;
using Chinese_Chess_v3.UI.Screens.Game;
using Chinese_Chess_v3.UI.Screens.Menu;

namespace Chinese_Chess_v3.UI.Screens
{
    /// <summary>
    /// NavigationManager 負責管理遊戲中各個主要UI畫面的切換。
    /// 它操作一個 Root UIElement 容器，
    /// 可以 Clear 舊畫面、Add 新畫面。
    /// </summary>
    public class NavigationManager
    {
        private readonly IUiFactory _factory;
        private UIElement rootElement;

        private MainMenu _mainMenu;
        private GameMenu _gameMenu;
        public NavigationManager(IUiFactory factory)
        {
            _factory = factory;
        }

        public void Init(UIElement root)
        {
            rootElement = root ?? throw new ArgumentNullException(nameof(root));
        }

        /// <summary>
        /// 切換到主選單畫面
        /// </summary>
        public void ShowMainMenu()
        {
            rootElement.RemoveAllChild();
            _mainMenu ??= _factory.CreateMainMenu();
            rootElement.AddChild(_mainMenu);
        }

        /// <summary>
        /// 切換到遊戲畫面
        /// </summary>
        public void ShowGameScreen()
        {
            rootElement.RemoveAllChild();
            _gameMenu ??= _factory.CreateGameMenu();
            rootElement.AddChild(_gameMenu);
        }

        /// <summary>
        /// 可擴充其他畫面切換方法...
        /// </summary>
    }
}
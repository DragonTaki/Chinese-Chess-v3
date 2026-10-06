/* ----- ----- ----- ----- */
// UIGameMenuHandler.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/17
// Update Date: 2026/10/05
// Version: v2.3
/* ----- ----- ----- ----- */

using System;

using Chinese_Chess_v3.Game.Application.GameScreen;
using Chinese_Chess_v3.Game.Core;
using Chinese_Chess_v3.Game.UI.Menus.SavedGameMenu;

using Engine.Diagnostics;
using Engine.UI.Core.Interfaces;
using Engine.UI.Core.Handlers;

using Microsoft.Extensions.DependencyInjection;

namespace Chinese_Chess_v3.Game.UI.Menus.GameMenu
{
    /// <summary>
    /// Binds the UIGameMenu (the game screen's left menu) to the <see cref="GameScreenPresenter"/>,
    /// which makes the decisions: each button calls a presenter command (navigation and files:
    /// 儲存遊戲, 載入佈局, 回到主畫面; the game controls are in the sidebar, <c>UIGameControls</c>), and the
    /// presenter's events drive the views (reset on a restart, the saved-game list shown /
    /// hidden in the board's place, the game-over dialog brought up on the UI thread).
    /// </summary>
    public class UIGameMenuHandler : UIMenuHandler<UIGameMenu, UIGameMenuHandler, UIGameMenuRenderer>
    {
        /// <summary>The saved-game list; created the first time 載入 opens it.</summary>
        private UISavedGameMenu _savedGameMenu;

        public UIGameMenuHandler() { }

        /// <summary>
        /// Subscribes to the presenter's events once: the screen (and this handler) is a single
        /// long-lived instance, like the <see cref="GameScreenPresenter"/> it listens to.
        /// </summary>
        protected override void OnInit(IUiFactory factory)
        {
            var presenter = Presenter;
            presenter.GameReset += OnGameReset;
            presenter.GameOverDialogRequested += OnGameOverDialogRequested;
            presenter.SavedGameListOpenRequested += OpenSavedGameList;
            presenter.SavedGameListCloseRequested += HideSavedGameList;
        }

        /// <summary>The game screen's decisions (the <see cref="GameScreenPresenter"/> registered in DI).</summary>
        private GameScreenPresenter Presenter => _factory.ServiceProvider.GetRequiredService<GameScreenPresenter>();

        /// <summary>
        /// The game was restarted (<see cref="GameScreenPresenter.GameReset"/>: 重新開始, or the
        /// start of any game): resets the game screen's views, so the log, board and sidebar start clean.
        /// </summary>
        private void OnGameReset() => Element.ResetGameUI();

        /// <summary>
        /// The game just ended and is to be announced: the event may come from the clock update
        /// or a board click, so the dialog is shown on the UI thread, once the game state has
        /// settled, and only while the game screen is shown.
        /// </summary>
        private void OnGameOverDialogRequested(GameOverInfo info)
        {
            Element.Post(() =>
            {
                if (Element.Parent == null || !Element.IsVisible)
                    return;
                Presenter.ShowGameOverDialog(info);
            });
        }

        /// <summary>Whether the saved-game list is shown (in the board's place).</summary>
        public bool IsSavedGameListOpen => _savedGameMenu != null && Element.Children.Contains(_savedGameMenu);

        public void UIGameMenuAction(GameMenuOption selectedAction)
        {
            if (DebugOptions.ConsoleTrace)
                Console.WriteLine($"UIGameMenu: selected: {selectedAction}");

            var presenter = Presenter;
            switch (selectedAction)
            {
                // Navigation and files.
                case GameMenuOption.SaveGame:
                    presenter.Navigation.SaveGame();
                    break;
                case GameMenuOption.LoadLayout:
                    presenter.Navigation.LoadGame();
                    break;
                case GameMenuOption.ReturnToMain:
                    presenter.Navigation.ReturnToMain();
                    break;

                case GameMenuOption.Default:
                default:
                    // Like any button but 載入, closes the saved-game list.
                    presenter.CloseSavedGameList();
                    break;
            }
        }

        /// <summary>
        /// Shows the saved-game list in the board's place (the board is hidden meanwhile, so it
        /// takes no clicks under the list) and reloads the files.
        /// </summary>
        private void OpenSavedGameList()
        {
            if (_savedGameMenu == null)
            {
                _savedGameMenu = _factory.CreateDIElement<UISavedGameMenu, UISavedGameMenuHandler, UISavedGameMenuRenderer>();
                _savedGameMenu.Handler.ItemStarted = CloseSavedGameList;
            }

            if (Element.ChessBoard != null)
                Element.ChessBoard.IsVisible = false;
            _savedGameMenu.IsVisible = true;
            if (!Element.Children.Contains(_savedGameMenu))
                Element.AddChild(_savedGameMenu);
            _savedGameMenu.Handler.OnEnter();
        }

        /// <summary>Closes the saved-game list through the presenter (so its state follows).</summary>
        public void CloseSavedGameList() => Presenter.CloseSavedGameList();

        /// <summary>Hides the saved-game list (if shown) and shows the board again.</summary>
        private void HideSavedGameList()
        {
            if (_savedGameMenu != null && Element.Children.Contains(_savedGameMenu))
            {
                _savedGameMenu.IsVisible = false;
                Element.RemoveChild(_savedGameMenu);
                _savedGameMenu.Handler.OnExit();
            }
            if (Element.ChessBoard != null)
                Element.ChessBoard.IsVisible = true;
        }
    }
}

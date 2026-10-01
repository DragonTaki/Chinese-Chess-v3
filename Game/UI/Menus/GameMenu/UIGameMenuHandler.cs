/* ----- ----- ----- ----- */
// UIGameMenuHandler.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/17
// Update Date: 2026/10/01
// Version: v2.0
/* ----- ----- ----- ----- */

using System;
using System.IO;

using Chinese_Chess_v3.Game.Configs;
using Chinese_Chess_v3.Game.Core;
using Chinese_Chess_v3.Game.Core.Boards;
using Chinese_Chess_v3.Game.UI.Constants;
using Chinese_Chess_v3.Game.UI.Dialogs;
using Chinese_Chess_v3.Game.UI.Menus.MainMenu;
using Chinese_Chess_v3.Game.UI.Menus.SavedGameMenu;

using Engine.Logging;
using Engine.UI.Core.Handlers;

using Microsoft.Extensions.DependencyInjection;

namespace Chinese_Chess_v3.Game.UI.Menus.GameMenu
{
    /// <summary>
    /// Handles logic and interactions for the UIGameMenu: the game screen's left menu
    /// (docs/PLAN.md in-game menu). 撤銷上步 = round undo, 儲存遊戲 = save, 載入佈局 = the
    /// saved-game list, 放棄對局 = the side to move resigns, 回到主畫面 = back to the main
    /// menu (asks first while the game is in progress). 重新開始 has no action yet.
    /// <para>
    /// Local hot-seat play: one person plays both sides, so 放棄 always resigns for the side
    /// to move (<see cref="GameManager.CurrentTurn"/>).
    /// </para>
    /// </summary>
    public class UIGameMenuHandler : UIMenuHandler<UIGameMenu, UIGameMenuHandler, UIGameMenuRenderer>
    {
        /// <summary>The saved-game list; created the first time 載入 opens it.</summary>
        private UISavedGameMenu _savedGameMenu;

        public UIGameMenuHandler() { }

        private GameManager Game => _factory.ServiceProvider.GetRequiredService<GameManager>();

        /// <summary>Whether the saved-game list is shown (in the board's place).</summary>
        public bool IsSavedGameListOpen => _savedGameMenu != null && Element.Children.Contains(_savedGameMenu);

        public void UIGameMenuAction(UIGameMenuType selectedAction)
        {
            Console.WriteLine($"UIGameMenu: selected: {selectedAction}");

            // Any other button closes the saved-game list first (載入 toggles it).
            if (selectedAction != UIGameMenuType.LoadLayout)
                CloseSavedGameList();

            switch (selectedAction)
            {
                case UIGameMenuType.Default:
                    break;
                case UIGameMenuType.Restart:
                    break;
                case UIGameMenuType.Undo:
                    UndoRound();
                    break;
                case UIGameMenuType.SaveGame:
                    SaveGame();
                    break;
                case UIGameMenuType.LoadLayout:
                    LoadGame();
                    break;
                case UIGameMenuType.Surrender:
                    ResignSideToMove();
                    break;
                case UIGameMenuType.ReturnToMain:
                    ReturnToMain();
                    break;
                default:
                    break;
            }
        }

        /// <summary>撤銷: takes back one round (both sides' last move); a log line when there is none.</summary>
        private void UndoRound()
        {
            var game = Game;
            if (!game.CanUndo)
            {
                Log(GameMenuTexts.UndoUnavailable);
                return;
            }
            // The Core logs each move taken back.
            game.Undo();
        }

        /// <summary>儲存: saves to the saves folder (the Core logs the file name); a log line with the reason when it cannot.</summary>
        private void SaveGame()
        {
            var game = Game;
            if (!game.CanSave)
            {
                Log(game.Board.Type != BoardType.Full ? GameMenuTexts.SaveUnavailableBoardType : GameMenuTexts.SaveUnavailableNoStartPosition);
                return;
            }

            try
            {
                string path = GameSaveFiles.Save(game);
                AppLogger.Log($"(Save) Saved to {path}", LogLevel.DEBUG);
            }
            catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException)
            {
                AppLogger.Log($"(Save) Cannot save: {ex.Message}", LogLevel.ERROR);
                Log(GameMenuTexts.SaveFailed(ex.Message));
            }
        }

        /// <summary>
        /// 載入: shows the saved-game list (asking first when the current game has unsaved
        /// changes); a second click closes it again.
        /// </summary>
        private void LoadGame()
        {
            if (IsSavedGameListOpen)
            {
                CloseSavedGameList();
                return;
            }

            if (!Game.HasUnsavedChanges)
            {
                OpenSavedGameList();
                return;
            }

            DialogManager.ShowConfirm(
                GameMenuTexts.DiscardUnsavedGame,
                ConfirmDialogType.YesNo,
                result =>
                {
                    if (result == ConfirmDialogResult.Yes)
                        OpenSavedGameList();
                });
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

        /// <summary>Hides the saved-game list (if shown) and shows the board again.</summary>
        public void CloseSavedGameList()
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

        /// <summary>放棄: the side to move resigns now (clocks stop); a log line either way.</summary>
        private void ResignSideToMove()
        {
            var game = Game;
            if (game.IsGameOver)
            {
                Log(GameMenuTexts.ResignGameOver);
                return;
            }

            var side = game.CurrentTurn;
            Log(GameMenuTexts.Resigned(side));
            // Not an unsaved change: the game is over, nothing is left to save (the Core logs the result).
            game.Resign(side);
        }

        /// <summary>
        /// 回到主畫面: while the game is in progress, asks whether to give it up (yes: the side
        /// to move resigns, then back to the main menu; no: stay); an ended game goes back
        /// directly, saved or not.
        /// </summary>
        private void ReturnToMain()
        {
            if (Game.IsGameOver)
            {
                ShowMainMenu();
                return;
            }

            DialogManager.ShowConfirm(
                GameMenuTexts.ResignAndReturnToMain,
                ConfirmDialogType.YesNo,
                result =>
                {
                    if (result != ConfirmDialogResult.Yes)
                        return;
                    ResignSideToMove();
                    ShowMainMenu();
                });
        }

        private void ShowMainMenu()
        {
            CloseSavedGameList();
            _navigationManager.Show<UIMainMenu, UIMainMenuHandler, UIMainMenuRenderer>();
        }

        /// <summary>A line in the game log (the sidebar's log box) and the debug log.</summary>
        private void Log(string message)
        {
            AppLogger.Log(message, LogLevel.DEBUG);
            Game.Logger?.AddMessage(message);
        }
    }
}

/* ----- ----- ----- ----- */
// UIGameMenuHandler.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/17
// Update Date: 2026/10/02
// Version: v2.1
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
using Engine.UI.Core.Interfaces;
using Engine.UI.Core.Handlers;

using Microsoft.Extensions.DependencyInjection;

namespace Chinese_Chess_v3.Game.UI.Menus.GameMenu
{
    /// <summary>
    /// Handles logic and interactions for the UIGameMenu: the game screen's left menu
    /// (docs/PLAN.md in-game menu). 撤銷上步 = round undo, 儲存遊戲 = save, 載入佈局 = the
    /// saved-game list, 放棄對局 = the side to move resigns, 回到主畫面 = back to the main
    /// menu (asks first while the game is in progress). 重新開始 = the current game restarted
    /// in its mode (<see cref="GameManager.Restart"/>; a loaded saved game comes back exactly
    /// as it was when loaded) (asks first while a move has been made and the game is not over).
    /// <para>
    /// Local hot-seat play: one person plays both sides, so 放棄 always resigns for the side
    /// to move (<see cref="GameManager.CurrentTurn"/>).
    /// </para>
    /// </summary>
    public class UIGameMenuHandler : UIMenuHandler<UIGameMenu, UIGameMenuHandler, UIGameMenuRenderer>
    {
        /// <summary>The saved-game list; created the first time 載入 opens it.</summary>
        private UISavedGameMenu _savedGameMenu;

        /// <summary>
        /// Set while 回到主畫面 resigns the game itself: that ending is not announced (the
        /// screen is being left).
        /// </summary>
        private bool _suppressGameOverDialog = false;

        public UIGameMenuHandler() { }

        /// <summary>
        /// Subscribes to the game's end once: the screen (and this handler) is a single
        /// long-lived instance, like the <see cref="GameManager"/> it listens to.
        /// </summary>
        protected override void OnInit(IUiFactory factory)
        {
            Game.GameOver += OnGameOver;
        }

        /// <summary>
        /// The game just ended (checkmate, stalemate, time-up, resignation, no pieces left): shows
        /// who won and why, with 重新開始 / 回到主畫面 / 關閉. Not announced for a saved game's old
        /// ending coming back while it loads, nor while the game screen is not shown. 關閉 (or a
        /// click outside the dialog) keeps the final board on screen.
        /// </summary>
        private void OnGameOver(GameOverInfo info)
        {
            var game = Game;
            if (_suppressGameOverDialog || game.IsReplaying)
                return;

            // The GameOver event may come from the clock update or a board click: show the
            // dialog on the UI thread, once the game state has settled.
            Element.Post(() =>
            {
                if (Element.Parent == null || !Element.IsVisible || !game.IsGameOver)
                    return;

                DialogManager.ShowConfirm(
                    GameMenuTexts.GameOverMessage(info.Winner, game.ColorOf(info.Winner), info.Reason, game.Board.Type),
                    ConfirmDialogType.GameOver,
                    result =>
                    {
                        switch (result)
                        {
                            case ConfirmDialogResult.Restart:
                                RestartNow();
                                break;
                            case ConfirmDialogResult.ReturnToMain:
                                ShowMainMenu();
                                break;
                        }
                    });
            });
        }

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
                    Restart();
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

        /// <summary>
        /// 重新開始: restarts the current game in its current mode - from its start position, or
        /// a loaded saved game exactly as it was when loaded (<see cref="GameManager.Restart"/>, through the game UI's reset so the log, board and
        /// sidebar start clean, like a new game). While a move has been made and the game is
        /// not over it asks first; an ended game or an untouched start restarts directly.
        /// </summary>
        public void Restart()
        {
            var game = Game;
            if (game.IsGameOver || game.Moves.Count <= game.UndoFloor)
            {
                RestartNow();
                return;
            }

            DialogManager.ShowConfirm(
                GameMenuTexts.DiscardAndRestart,
                ConfirmDialogType.YesNo,
                result =>
                {
                    if (result == ConfirmDialogResult.Yes)
                        RestartNow();
                });
        }

        private void RestartNow()
        {
            CloseSavedGameList();
            Element.ResetGameUI();
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
            Log(GameMenuTexts.Resigned(side, game.ColorOf(side)));
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
                    _suppressGameOverDialog = true;
                    try
                    {
                        ResignSideToMove();
                    }
                    finally
                    {
                        _suppressGameOverDialog = false;
                    }
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

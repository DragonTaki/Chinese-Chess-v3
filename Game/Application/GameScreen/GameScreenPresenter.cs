/* ----- ----- ----- ----- */
// GameScreenPresenter.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/04
// Update Date: 2026/10/05
// Version: v1.1
/* ----- ----- ----- ----- */

using System;
using System.IO;

using Chinese_Chess_v3.Game.Application.GameLog;
using Chinese_Chess_v3.Game.Application.Services;
using Chinese_Chess_v3.Game.Application.Session;
using Chinese_Chess_v3.Game.Application.Texts;
using Chinese_Chess_v3.Game.Configs;
using Chinese_Chess_v3.Game.Core;
using Chinese_Chess_v3.Game.Core.Boards;

using Engine.Logging;

namespace Chinese_Chess_v3.Game.Application.GameScreen
{
    /// <summary>
    /// The game screen's decisions, apart from the views that draw it: when the game-over dialog
    /// is shown, when 重新開始 / 載入 / 回到主畫面 ask first, round undo, saving, resigning for the
    /// side to move, and whether the saved-game list is open. The commands come in two groups,
    /// <see cref="Controls"/> (game controls) and <see cref="Navigation"/> (navigation and
    /// files), so the screen can later place them apart.
    /// <para>
    /// Local hot-seat play: one person plays both sides, so 放棄 always resigns for the side to
    /// move (<see cref="GameManager.CurrentTurn"/>).
    /// </para>
    /// <para>
    /// The views bind to it: the menu's buttons call the commands; the screen resets its views
    /// on <see cref="GameReset"/>, shows / hides the saved-game list on
    /// <see cref="SavedGameListOpenRequested"/> / <see cref="SavedGameListCloseRequested"/>, and
    /// on <see cref="GameOverDialogRequested"/> calls <see cref="ShowGameOverDialog"/> from the UI
    /// thread while it is displayed. Game-log lines go to the game log
    /// (<see cref="GameLogComposer"/>), which the sidebar shows.
    /// </para>
    /// </summary>
    public sealed class GameScreenPresenter : IGameControlCommands, IGameNavigationCommands
    {
        private readonly GameSession _session;
        private readonly IDialogService _dialogs;
        private readonly INavigator _navigator;
        private readonly PlayerSettings _settings;
        private readonly GameLogComposer _gameLog;

        /// <summary>
        /// Set while 回到主畫面 resigns the game itself: that ending is not announced (the
        /// screen is being left).
        /// </summary>
        private bool _suppressGameOverDialog = false;

        /// <summary>
        /// Creates the presenter and subscribes to the game's end and to the session's reset
        /// once (it is a single long-lived instance, like the <see cref="GameManager"/> and the
        /// <see cref="GameSession"/>). Creating it creates the game (<see cref="GameSession.Game"/>).
        /// </summary>
        /// <param name="session">The game flow (restart; the game being played).</param>
        /// <param name="dialogs">The confirm dialogs.</param>
        /// <param name="navigator">Goes back to the main menu.</param>
        /// <param name="settings">The live player settings (the player's name for a save's unnamed players).</param>
        /// <param name="gameLog">The game log the presenter's own lines go to.</param>
        public GameScreenPresenter(GameSession session, IDialogService dialogs, INavigator navigator, PlayerSettings settings, GameLogComposer gameLog)
        {
            _session = session ?? throw new ArgumentNullException(nameof(session));
            _dialogs = dialogs ?? throw new ArgumentNullException(nameof(dialogs));
            _navigator = navigator ?? throw new ArgumentNullException(nameof(navigator));
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _gameLog = gameLog ?? throw new ArgumentNullException(nameof(gameLog));

            Game.GameOver += OnGameOver;
            _session.GameReset += OnGameReset;
        }

        private GameManager Game => _session.Game;

        // ----- Command groups -----

        /// <summary>Game controls: 撤銷, 放棄, 重新開始 (later 暫停 / 求和).</summary>
        public IGameControlCommands Controls => this;

        /// <summary>Navigation and files: 儲存, 載入, 回到主畫面.</summary>
        public IGameNavigationCommands Navigation => this;

        // ----- Events and state for the views -----

        /// <summary>
        /// The game was restarted (<see cref="GameSession.GameReset"/>: 重新開始, or the start of
        /// any game): the game screen resets its views, so the log, board and sidebar start clean.
        /// </summary>
        public event Action GameReset;

        /// <summary>
        /// The game just ended and the ending is to be announced (not for a saved game's old
        /// ending coming back while it loads, nor for 回到主畫面's own resignation). Raised from
        /// wherever the game ended (the clock update or a board click): the view calls
        /// <see cref="ShowGameOverDialog"/> on the UI thread, if the game screen is displayed.
        /// </summary>
        public event Action<GameOverInfo> GameOverDialogRequested;

        /// <summary>The view shows the saved-game list in the board's place and reloads its files.</summary>
        public event Action SavedGameListOpenRequested;

        /// <summary>The view hides the saved-game list (if shown) and shows the board again.</summary>
        public event Action SavedGameListCloseRequested;

        /// <summary>Whether the saved-game list was opened (載入) and not closed since.</summary>
        public bool IsSavedGameListOpen { get; private set; }

        private void OnGameReset() => GameReset?.Invoke();

        private void OnGameOver(GameOverInfo info)
        {
            if (_suppressGameOverDialog || Game.IsReplaying)
                return;
            GameOverDialogRequested?.Invoke(info);
        }

        /// <summary>
        /// Shows the game-over dialog: who won and why, with 重新開始 / 回到主畫面 / 關閉 (關閉,
        /// or a click outside the dialog, keeps the final board on screen). Nothing when the game
        /// is no longer over (e.g. restarted before the dialog came up).
        /// </summary>
        /// <param name="info">The ending, from <see cref="GameOverDialogRequested"/>.</param>
        public void ShowGameOverDialog(GameOverInfo info)
        {
            ArgumentNullException.ThrowIfNull(info);
            var game = Game;
            if (!game.IsGameOver)
                return;

            _dialogs.ShowConfirm(
                GameTexts.GameOverMessage(info.Winner, game.NameOf(info.Winner), game.ColorOf(info.Winner), info.Reason, game.Board.Type),
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
        }

        // ----- Game controls -----

        /// <inheritdoc/>
        /// <remarks>
        /// Closes the saved-game list first. Restarts from the start position, or a loaded saved
        /// game exactly as it was when loaded (<see cref="GameSession.Restart"/>, whose
        /// <see cref="GameSession.GameReset"/> resets the views like a new game). An ended game or
        /// an untouched start restarts directly.
        /// </remarks>
        public void Restart()
        {
            CloseSavedGameList();
            var game = Game;
            if (game.IsGameOver || game.Moves.Count <= game.UndoFloor)
            {
                RestartNow();
                return;
            }

            _dialogs.ShowConfirm(
                GameTexts.DiscardAndRestart,
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
            _session.Restart();
        }

        /// <inheritdoc/>
        /// <remarks>Closes the saved-game list first.</remarks>
        public void UndoRound()
        {
            CloseSavedGameList();
            var game = Game;
            if (!game.CanUndo)
            {
                Log(GameTexts.UndoUnavailable);
                return;
            }
            // The Core logs each move taken back.
            game.Undo();
        }

        /// <inheritdoc/>
        /// <remarks>Closes the saved-game list first.</remarks>
        public void ResignSideToMove()
        {
            CloseSavedGameList();
            var game = Game;
            if (game.IsGameOver)
            {
                Log(GameTexts.ResignGameOver);
                return;
            }

            var side = game.CurrentTurn;
            Log(GameTexts.Resigned(side, game.ColorOf(side)));
            // Not an unsaved change: the game is over, nothing is left to save (the Core logs the result).
            game.Resign(side);
        }

        // ----- Navigation and files -----

        /// <inheritdoc/>
        /// <remarks>Closes the saved-game list first. A write error (IO, access) is logged, not thrown.</remarks>
        public void SaveGame()
        {
            CloseSavedGameList();
            var game = Game;
            if (!game.CanSave)
            {
                Log(game.Board.Type != BoardType.Full ? GameTexts.SaveUnavailableBoardType : GameTexts.SaveUnavailableNoStartPosition);
                return;
            }

            try
            {
                string path = GameSaveFiles.Save(game, _settings.PlayerName);
                AppLogger.Log($"(Save) Saved to {path}", LogLevel.DEBUG);
            }
            catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException)
            {
                AppLogger.Log($"(Save) Cannot save: {ex.Message}", LogLevel.ERROR);
                Log(GameTexts.SaveFailed(ex.Message));
            }
        }

        /// <inheritdoc/>
        public void LoadGame()
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

            _dialogs.ShowConfirm(
                GameTexts.DiscardUnsavedGame,
                ConfirmDialogType.YesNo,
                result =>
                {
                    if (result == ConfirmDialogResult.Yes)
                        OpenSavedGameList();
                });
        }

        /// <inheritdoc/>
        /// <remarks>
        /// Closes the saved-game list first. Asking: yes = the side to move resigns (not
        /// announced), then back to the main menu; no = stay. Going back directly when nobody has
        /// moved yet is an author decision (2026-10-02).
        /// </remarks>
        public void ReturnToMain()
        {
            CloseSavedGameList();
            if (Game.IsGameOver || !Game.HasPlayedMoves)
            {
                ShowMainMenu();
                return;
            }

            _dialogs.ShowConfirm(
                GameTexts.ResignAndReturnToMain,
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

        // ----- Saved-game list -----

        private void OpenSavedGameList()
        {
            IsSavedGameListOpen = true;
            SavedGameListOpenRequested?.Invoke();
        }

        /// <summary>
        /// Closes the saved-game list (the view hides it, if shown, and shows the board again).
        /// Also called by the view when a saved game is started from the list.
        /// </summary>
        public void CloseSavedGameList()
        {
            IsSavedGameListOpen = false;
            SavedGameListCloseRequested?.Invoke();
        }

        private void ShowMainMenu()
        {
            CloseSavedGameList();
            _navigator.Show(ScreenId.MainMenu);
        }

        /// <summary>A line in the game log (the sidebar's log box) and the debug log.</summary>
        private void Log(string message)
        {
            AppLogger.Log(message, LogLevel.DEBUG);
            _gameLog.Write(message);
        }
    }
}

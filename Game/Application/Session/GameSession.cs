/* ----- ----- ----- ----- */
// GameSession.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/04
// Update Date: 2026/10/04
// Version: v1.0
/* ----- ----- ----- ----- */

using System;

using Chinese_Chess_v3.Game.Application.Services;
using Chinese_Chess_v3.Game.Core;

using Engine.Logging;

namespace Chinese_Chess_v3.Game.Application.Session
{
    /// <summary>
    /// The flow around the <see cref="GameManager"/>: starting a game on the game screen (a new
    /// game of a <see cref="GameKind"/>, or any set-up such as an endgame, an opening or a saved
    /// game), restarting the current game, and the per-frame clock tick. Every start and restart
    /// goes through <see cref="Restart"/>, which raises <see cref="GameReset"/> for the UI to
    /// reset its views (board, sidebar, game log).
    /// </summary>
    public sealed class GameSession
    {
        private readonly Func<GameManager> _game;
        private readonly INavigator _navigator;

        /// <summary>
        /// Creates the session.
        /// </summary>
        /// <param name="game">
        /// Gets the app's game. A factory rather than the instance, so creating the session
        /// does not create the <see cref="GameManager"/> (it is created on first use and reads
        /// the player's settings at that point).
        /// </param>
        /// <param name="navigator">Switches to the game screen and tells whether it is shown.</param>
        public GameSession(Func<GameManager> game, INavigator navigator)
        {
            _game = game ?? throw new ArgumentNullException(nameof(game));
            _navigator = navigator ?? throw new ArgumentNullException(nameof(navigator));
        }

        /// <summary>The game being played.</summary>
        public GameManager Game => _game();

        /// <summary>
        /// Raised by <see cref="Restart"/> (and so by every start, before the chosen game is set
        /// up), after <see cref="GameManager.Restart"/>: the UI resets its views (the game log is
        /// cleared, so the restart's own log lines go with it; the set-up's lines that follow stay).
        /// </summary>
        public event Action GameReset;

        /// <summary>Whether <see cref="StartNew"/> can start <paramref name="kind"/> (揭棋 and 三國 cannot yet).</summary>
        public static bool CanStartNew(GameKind kind) => kind switch
        {
            GameKind.Traditional or GameKind.DarkHalf or GameKind.OpenHalf => true,
            // 揭棋: Board.IsJieqi has no start position yet; 三國: its own rule system is
            // still being specified.
            GameKind.Flip or GameKind.ThreeKingdoms => false,
            _ => throw new ArgumentOutOfRangeException(nameof(kind), kind, "Unknown game kind"),
        };

        /// <summary>
        /// Starts a new game of <paramref name="kind"/> on the game screen (<see cref="Start"/>):
        /// 傳統大盤 the standard position, 暗棋／明棋半盤 a shuffled HalfCenter game
        /// (<see cref="GameManager.StartHalfCenter"/>).
        /// </summary>
        /// <exception cref="NotSupportedException"><paramref name="kind"/> cannot be played yet (<see cref="CanStartNew"/>).</exception>
        public void StartNew(GameKind kind)
        {
            Action<GameManager> setUp = kind switch
            {
                GameKind.Traditional => game => game.ResetBoardToDefault(),
                GameKind.DarkHalf => game => game.StartHalfCenter(hiddenChess: true),
                GameKind.OpenHalf => game => game.StartHalfCenter(hiddenChess: false),
                GameKind.Flip or GameKind.ThreeKingdoms => throw new NotSupportedException($"A {kind} game cannot be started yet"),
                _ => throw new ArgumentOutOfRangeException(nameof(kind), kind, "Unknown game kind"),
            };
            Start(setUp);
        }

        /// <summary>
        /// Starts a game on the game screen: shows the game screen, restarts the game being
        /// played (<see cref="Restart"/>: the views reset, the log cleared), then sets the
        /// chosen game up with <paramref name="setUp"/> (its log lines stay).
        /// </summary>
        /// <param name="setUp">Sets the chosen game up on the restarted game.</param>
        public void Start(Action<GameManager> setUp)
        {
            ArgumentNullException.ThrowIfNull(setUp);
            _navigator.Show(ScreenId.Game);
            Restart();
            setUp(Game);
        }

        /// <summary>
        /// <see cref="Start"/> for a set-up read from a file (an endgame, an opening, a saved
        /// game): a <see cref="FormatException"/> from <paramref name="setUp"/> is logged
        /// (<c>(<paramref name="logLabel"/>) cannot start <paramref name="name"/>: ...</c>) and
        /// the restarted previous game stays on the board. Only the set-up is guarded: an
        /// exception from the restart itself propagates, as from <see cref="Start"/>.
        /// </summary>
        /// <param name="setUp">Sets the chosen game up on the restarted game.</param>
        /// <param name="logLabel">Log prefix without parentheses, e.g. <c>Endgame</c>.</param>
        /// <param name="name">The item's name in the log, e.g. its file name.</param>
        /// <returns>False when the set-up threw a <see cref="FormatException"/>.</returns>
        public bool TryStart(Action<GameManager> setUp, string logLabel, string name)
        {
            ArgumentNullException.ThrowIfNull(setUp);
            _navigator.Show(ScreenId.Game);
            Restart();

            try
            {
                setUp(Game);
                return true;
            }
            catch (FormatException ex)
            {
                // Items from the loaders were already validated; the restarted previous game stays on the board.
                AppLogger.Log($"({logLabel}) cannot start {name}: {ex.Message}", LogLevel.ERROR);
                return false;
            }
        }

        /// <summary>
        /// Restarts the game being played (<see cref="GameManager.Restart"/>: the standard start,
        /// a new shuffled HalfCenter game of the same variant, the same endgame / opening, or a
        /// loaded saved game exactly as it was when loaded), then raises <see cref="GameReset"/>.
        /// </summary>
        /// <exception cref="NotSupportedException">The board type cannot be played yet (HalfCross); <see cref="GameReset"/> is not raised.</exception>
        public void Restart()
        {
            Game.Restart();
            GameReset?.Invoke();
        }

        /// <summary>
        /// Advances the clocks (<see cref="GameManager.UpdateTimers"/>); called once per frame by
        /// the frame loop. Only while the game screen is shown: the clocks used to be advanced by
        /// the game screen's per-frame update, which does not run while another screen is shown.
        /// The clocks measure wall time, so a skipped tick only delays a time-up, it loses no time.
        /// </summary>
        public void Tick()
        {
            if (!_navigator.IsShown(ScreenId.Game))
                return;
            Game.UpdateTimers();
        }
    }
}

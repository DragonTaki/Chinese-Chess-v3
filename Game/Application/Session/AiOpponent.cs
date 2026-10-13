/* ----- ----- ----- ----- */
// AiOpponent.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/12
// Update Date: 2026/10/13
// Version: v1.5
/* ----- ----- ----- ----- */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Chinese_Chess_v3.Game.Ai;
using Chinese_Chess_v3.Game.Application.Services;
using Chinese_Chess_v3.Game.Application.Texts;
using Chinese_Chess_v3.Game.Configs;
using Chinese_Chess_v3.Game.Core;
using Chinese_Chess_v3.Game.Core.Pgn;
using Chinese_Chess_v3.Game.Core.Players;

using Engine.Logging;
using Engine.Timing;

namespace Chinese_Chess_v3.Game.Application.Session
{
    /// <summary>
    /// The computer player of a game against the computer (人機對戰): while it is the turn of
    /// <see cref="GameManager.ComputerSide"/> it searches the position on a background task with
    /// an <see cref="IChessEngine"/> and plays the result through <see cref="GameManager.TryMove"/>
    /// on the main thread. The frame loop calls <see cref="Tick"/> (through <see cref="GameSession.Tick"/>,
    /// only while the game screen is shown), which starts a search whenever the computer is to move
    /// and none is running. A search is dropped (its result is never played) when the game changes
    /// under it: a move taken back, a new game or restart, a pause, the end of the game, a change of
    /// the selected game options, or leaving the game screen before the result arrives.
    /// </summary>
    public sealed class AiOpponent : IDisposable
    {
        /// <summary>The shortest time a reply takes, so it is not instant (shorter when the clock is nearly out).</summary>
        public static readonly TimeSpan DefaultMinThinkTime = TimeSpan.FromMilliseconds(300);

        private sealed class Search
        {
            public Search(int generation, int ply) { Generation = generation; Ply = ply; }
            public int Generation { get; }
            public int Ply { get; }
            public CancellationTokenSource Cancellation { get; } = new();
        }

        private readonly Func<GameManager> _game;
        private readonly IMainThreadQueue _mainThread;
        private readonly INavigator _navigator;
        private readonly AiSettings _settings;
        private readonly Func<EngineLevel, IChessEngine> _createEngine;
        private readonly TimeSpan _minThinkTime;
        private readonly Random _random;

        private bool _attached;
        private IChessEngine _engine;
        private EngineLevel _engineLevel;
        private EngineLevel _level;
        private AiOpeningBook _openingBook;
        private string _openingStyle;
        private Search _search;
        private int _generation;
        // The ply (move count) at which the last search failed; no retry until something changes.
        private int _failedPly = -1;

        /// <summary>Creates the opponent (a single long-lived instance).</summary>
        /// <param name="game">Gets the app's game (a factory, so creating this does not create the game).</param>
        /// <param name="mainThread">Brings a search's result back to the main thread.</param>
        /// <param name="navigator">Tells whether the game screen is shown (a result is only played there).</param>
        /// <param name="settings">Defaults copied when a computer game starts.</param>
        /// <param name="locator">Where the player's own engines are looked for (logged only); null for none.</param>
        /// <param name="createEngine">Creates the engine of a level; null for the built-in engine.</param>
        /// <param name="minThinkTime">The shortest time a reply takes; null for <see cref="DefaultMinThinkTime"/>.</param>
        /// <param name="random">Picks the side for the 隨機 choice; null for the shared generator.</param>
        public AiOpponent(Func<GameManager> game, IMainThreadQueue mainThread, INavigator navigator, AiSettings settings,
            AiDataLocator locator = null, Func<EngineLevel, IChessEngine> createEngine = null, TimeSpan? minThinkTime = null, Random random = null)
        {
            _game = game ?? throw new ArgumentNullException(nameof(game));
            _mainThread = mainThread ?? throw new ArgumentNullException(nameof(mainThread));
            _navigator = navigator ?? throw new ArgumentNullException(nameof(navigator));
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _createEngine = createEngine ?? (level => BuiltInEngine.Create(level, _random));
            _minThinkTime = minThinkTime ?? DefaultMinThinkTime;
            _random = random ?? Random.Shared;
            LogAiData(locator);
        }

        /// <summary>Whether a search is running.</summary>
        public bool IsThinking => _search != null;

        /// <summary>
        /// Starts a new game against the computer on <paramref name="game"/> (already restarted by the
        /// session): the human plays the side the settings choose (隨機: picked now), the computer the other.
        /// A search for a first move starts at the next <see cref="Tick"/>.
        /// </summary>
        /// <param name="game">The game to set up.</param>
        public void Begin(GameManager game)
            => Begin(game, new AiGameOptions(_settings.Level, _settings.PlayerSide));

        /// <summary>Starts with a rules snapshot and options belonging only to this game.</summary>
        public void Begin(GameManager game, AiGameOptions options, AiOpeningBook openingBook = null)
        {
            ArgumentNullException.ThrowIfNull(game);
            ArgumentNullException.ThrowIfNull(options);
            if (!Enum.IsDefined(options.Level)) throw new ArgumentOutOfRangeException(nameof(options));
            Attach();
            Cancel();
            _level = options.Level;
            _openingBook = openingBook;
            _openingStyle = options.OpeningStyle;
            var computer = options.PlayerSide switch
            {
                AiPlayerSide.First => PlayerSide.Player2,
                AiPlayerSide.Second => PlayerSide.Player1,
                AiPlayerSide.Random => _random.Next(2) == 0 ? PlayerSide.Player1 : PlayerSide.Player2,
                _ => throw new ArgumentOutOfRangeException(nameof(options.PlayerSide), options.PlayerSide, "Unknown AI side choice"),
            };
            var rules = game.DefaultRulesFor(GameKind.Traditional).Clone();
            if (!options.UseConfiguredTimer)
                rules.TimerMode = TimerMode.CountUp;
            if (options.TimerMode.HasValue) rules.TimerMode = options.TimerMode.Value;
            game.StartVsComputer(computer, GameTexts.ComputerPlayerName, rules);
        }

        /// <summary>
        /// Once per frame: starts a search when the computer is to move in a running game and none is
        /// running (nor failed at this position). Does nothing in any other game.
        /// </summary>
        public void Tick()
        {
            Attach();
            if (_engine != null && _engineLevel != _level)
            {
                Cancel();
                _engine.Dispose();
                _engine = null;
            }
            if (_search != null)
                return;

            var game = _game();
            if (game.ComputerSide is not PlayerSide side || game.CurrentTurn != side
                || game.IsGameOver || game.IsPaused || game.IsReplaying || game.IsRemote)
                return;
            int ply = game.Moves.Count;
            if (_failedPly == ply)
                return;
            StartSearch(game, side, ply);
        }

        /// <summary>Drops the running search: its result will not be played.</summary>
        public void Cancel()
        {
            _generation++;
            _failedPly = -1;
            if (_search == null)
                return;
            _search.Cancellation.Cancel();
            _search = null;
        }

        /// <summary>Stops any search and releases the engine.</summary>
        public void Dispose()
        {
            Cancel();
            _engine?.Dispose();
            _engine = null;
        }

        /// <summary>Hooks the game's events once: whatever changes the position under a search drops it.</summary>
        private void Attach()
        {
            if (_attached)
                return;
            _attached = true;
            var game = _game();
            game.BoardReset += Cancel;
            game.MoveUndone += _ => Cancel();
            game.PausedChanged += _ => Cancel();
            game.GameOver += _ => Cancel();
        }

        private void StartSearch(GameManager game, PlayerSide side, int ply)
        {
            if (game.InitialFen == null)
            {
                AppLogger.Log("(AI) The game has no start position (FEN); the computer cannot play", LogLevel.ERROR);
                _failedPly = ply;
                return;
            }
            var level = _level;
            IccsMove bookMove = default;
            bool fromBook = _openingBook?.TryGetMove(game, side, _openingStyle, out bookMove) == true;
            if (!fromBook)
            {
                _engine ??= CreateEngine(level);
                _engineLevel = level;
            }
            var engine = _engine;

            var position = new EnginePosition(game.InitialFen, game.Rules,
                game.Moves.Select(m => new IccsMove(m.FromX, m.FromY, m.ToX, m.ToY)).ToList());
            var (limits, minThink) = LimitsFor(game, side, level);
            var search = new Search(_generation, ply);
            _search = search;
            var token = search.Cancellation.Token;

            _ = Task.Run(async () =>
            {
                try
                {
                    var clock = Stopwatch.StartNew();
                    var result = fromBook
                        ? new EngineResult(bookMove, 0, null, 0.5, new[] { bookMove }, 0, 0)
                        : await engine.AnalyzeAsync(position, limits, token).ConfigureAwait(false);
                    var wait = minThink - clock.Elapsed;
                    if (wait > TimeSpan.Zero)
                        await Task.Delay(wait, token).ConfigureAwait(false);
                    _mainThread.Post(() => Apply(search, result));
                }
                catch (OperationCanceledException)
                {
                }
                catch (Exception ex)
                {
                    AppLogger.Log($"(AI) {engine?.Name ?? "Opening book"} failed: {ex.Message}", LogLevel.ERROR);
                    _mainThread.Post(() => Fail(search));
                }
            });
        }

        /// <summary>Plays <paramref name="result"/> on the main thread, unless the search is stale.</summary>
        private void Apply(Search search, EngineResult result)
        {
            if (search.Generation != _generation || _search != search)
                return;
            var game = _game();
            if (!_navigator.IsShown(ScreenId.Game))
            {
                // Left the game: the result is dropped, a search starts again when it is shown.
                _search = null;
                return;
            }
            if (game.ComputerSide is not PlayerSide side || game.CurrentTurn != side || game.Moves.Count != search.Ply
                || game.IsGameOver || game.IsPaused || game.IsReplaying)
            {
                _search = null;
                return;
            }
            if (result.BestMove is not IccsMove move)
            {
                AppLogger.Log("(AI) The engine returned no move", LogLevel.WARN);
                Fail(search);
                return;
            }
            _search = null;
            if (!game.TryMove(move.FromX, move.FromY, move.ToX, move.ToY))
            {
                AppLogger.Log($"(AI) The engine's move {move} was not legal; the computer does not move", LogLevel.ERROR);
                _failedPly = search.Ply;
            }
        }

        private void Fail(Search search)
        {
            if (search.Generation != _generation || _search != search)
                return;
            _search = null;
            _failedPly = search.Ply;
        }

        /// <summary>The engine of <paramref name="level"/>; the built-in one when creating it fails.</summary>
        private IChessEngine CreateEngine(EngineLevel level)
        {
            try
            {
                return _createEngine(level);
            }
            catch (Exception ex)
            {
                AppLogger.Log($"(AI) Cannot create the engine ({ex.Message}); using the built-in {level} engine", LogLevel.WARN);
                return BuiltInEngine.Create(level, _random);
            }
        }

        /// <summary>The search bounds of <paramref name="level"/> and the shortest reply time, kept inside the mover's clock.</summary>
        private (SearchLimits Limits, TimeSpan MinThink) LimitsFor(GameManager game, PlayerSide side, EngineLevel level)
        {
            var profile = EngineLevelProfile.For(level);
            int depth = profile.DepthCap;
            var time = profile.TimeBudget;
            var minThink = _minThinkTime;
            long? own = RemainingMs(game, side), opponent = RemainingMs(game, ComputerOpponent(side));
            if (own is long left)
            {
                // A quarter of what is left at most, so the computer cannot flag itself.
                var cap = TimeSpan.FromMilliseconds(Math.Max(50, left / 4));
                if (time > cap)
                    time = cap;
                if (minThink > cap)
                    minThink = cap;
            }
            return (new SearchLimits(time, depth, OwnRemainingMs: own, OpponentRemainingMs: opponent,
                IncrementMs: (long)game.Rules.IncrementPerMove.TotalMilliseconds), minThink);
        }

        private static PlayerSide ComputerOpponent(PlayerSide side) => side == PlayerSide.Player1 ? PlayerSide.Player2 : PlayerSide.Player1;

        /// <summary>Milliseconds left on <paramref name="side"/>'s clock before it would lose on time; null without a limit.</summary>
        private static long? RemainingMs(GameManager game, PlayerSide side)
        {
            var timer = game.PlayerOf(side).Timer;
            if (!timer.HasTimeLimit)
                return null;
            var left = timer.TotalTimeLimit - timer.CurrentTotalTime;
            if (timer.EnableStepTimer)
            {
                var step = timer.StepTimeLimit - timer.CurrentStepTime;
                if (step < left)
                    left = step;
            }
            return Math.Max(0, (long)left.TotalMilliseconds);
        }

        /// <summary>Looks in the player's AI folder (creating it) and logs what is found; only the built-in engine is used for now.</summary>
        private static void LogAiData(AiDataLocator locator)
        {
            if (locator == null)
                return;
            try
            {
                IReadOnlyList<AiDataEntry> entries = locator.Enumerate();
                var engines = entries.Where(e => e.Kind == AiDataKind.Engine).Select(e => e.Name).ToList();
                AppLogger.Log($"(AI) AI folder: {engines.Count} engine(s) [{string.Join(", ", engines)}], {entries.Count - engines.Count} other file(s); the built-in engine is used", LogLevel.INFO);
            }
            catch (Exception ex)
            {
                AppLogger.Log($"(AI) Cannot read the AI folder: {ex.Message}; the built-in engine is used", LogLevel.WARN);
            }
        }
    }
}

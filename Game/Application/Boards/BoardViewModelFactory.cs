/* ----- ----- ----- ----- */
// BoardViewModelFactory.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/05
// Update Date: 2026/10/05
// Version: v1.0
/* ----- ----- ----- ----- */

using System;

using Chinese_Chess_v3.Game.Configs;
using Chinese_Chess_v3.Game.Core;

namespace Chinese_Chess_v3.Game.Application.Boards
{
    /// <summary>
    /// Creates each board view's <see cref="BoardViewModel"/> (a board element creates its own,
    /// with its own thread callback), so the view never needs the <see cref="GameManager"/> itself.
    /// </summary>
    public sealed class BoardViewModelFactory
    {
        private readonly Func<GameManager> _game;
        private readonly PlayerSettings _settings;

        /// <param name="game">The game the boards show (taken through a factory, so it is still created on first use).</param>
        /// <param name="settings">The live player settings (which board hints are shown).</param>
        public BoardViewModelFactory(Func<GameManager> game, PlayerSettings settings)
        {
            _game = game ?? throw new ArgumentNullException(nameof(game));
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
        }

        /// <summary>A new view model of the game (see <see cref="BoardViewModel(GameManager, PlayerSettings, Action{Action})"/>); the caller disposes it.</summary>
        /// <param name="post">Runs an action on the view's thread.</param>
        public BoardViewModel Create(Action<Action> post) => new BoardViewModel(_game(), _settings, post);
    }
}

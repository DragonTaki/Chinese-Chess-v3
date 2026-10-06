/* ----- ----- ----- ----- */
// InfoBoardViewModel.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/05
// Update Date: 2026/10/05
// Version: v1.0
/* ----- ----- ----- ----- */

using System;
using System.Collections.Generic;

using Chinese_Chess_v3.Game.Application.Texts;
using Chinese_Chess_v3.Game.Core;
using Chinese_Chess_v3.Game.Core.Families.ThreeKingdoms;
using Chinese_Chess_v3.Game.Core.Pieces;
using Chinese_Chess_v3.Game.Core.Players;

namespace Chinese_Chess_v3.Game.Application.InfoBoards
{
    /// <summary>
    /// What the info board (計時看板) shows, apart from the view that draws it: which player each
    /// half shows, the players' names (with 將軍 while in check), the colours they play, whose
    /// turn it is, and the clock texts. Everything is read live from the game, so it follows
    /// every new game; it holds no state of its own apart from the name overrides and the
    /// <see cref="Clock"/> template.
    /// </summary>
    public sealed class InfoBoardViewModel
    {
        private readonly GameManager _game;

        /// <summary>Creates the view model for <paramref name="game"/>.</summary>
        /// <param name="game">The game shown (clocks, turn, check, colours, names).</param>
        public InfoBoardViewModel(GameManager game)
        {
            _game = game ?? throw new ArgumentNullException(nameof(game));
        }

        /// <summary>Player2's name on the board; null (the default) for the name of the colour it plays (see <see cref="GetPlayerName"/>).</summary>
        public string Player2Name { get; set; } = null;

        /// <summary>Player1's name on the board; null (the default) for the name of the colour it plays (see <see cref="GetPlayerName"/>).</summary>
        public string Player1Name { get; set; } = null;

        /// <summary>Writes the clock texts (<see cref="TotalTimeText"/>, <see cref="StepTimeText"/>).</summary>
        public ClockFormatter Clock { get; } = new ClockFormatter();

        /// <summary>
        /// The side shown in the left half: 己方 (<see cref="GameManager.LocalSide"/> — Player1, the
        /// first mover, in a new local game, an endgame and on a half board; the side the player
        /// plays in a loaded save, an opening or a custom position). Each half is coloured
        /// by the colour its player actually plays (<see cref="ColorOf"/>).
        /// </summary>
        public PlayerSide LeftSide => _game.LocalSide;

        /// <summary>The side shown in the right half: the other one of <see cref="LeftSide"/>.</summary>
        public PlayerSide RightSide => LeftSide == PlayerSide.Player1 ? PlayerSide.Player2 : PlayerSide.Player1;

        /// <summary>Whether a 三國 game is shown: three columns (<see cref="Sides"/>) with teams and points.</summary>
        public bool IsThreeKingdoms => _game.Board.ThreeKingdoms != null;

        /// <summary>The players shown, left to right: <see cref="LeftSide"/> and <see cref="RightSide"/>; 三國 Player1..Player3 in turn order.</summary>
        public IReadOnlyList<PlayerSide> Sides => IsThreeKingdoms
            ? new[] { PlayerSide.Player1, PlayerSide.Player2, PlayerSide.Player3 }
            : new[] { LeftSide, RightSide };

        /// <summary>三國: the team (1..3) <paramref name="side"/> claimed; 0 before it has one.</summary>
        public int TeamOf(PlayerSide side) => _game.TeamOf(side);

        /// <summary>三國: whether <paramref name="side"/> no longer plays (resigned or out).</summary>
        public bool IsInactive(PlayerSide side) => !_game.IsStillPlaying(side);

        /// <summary>三國: the line under <paramref name="side"/>'s name (<see cref="GameTexts.ThreeKingdomsStatus"/>).</summary>
        public string ThreeKingdomsStatus(PlayerSide side)
        {
            var rules = _game.Rules;
            int team = _game.TeamOf(side);
            int? threshold = rules.HalfCrossWinCondition == HalfCrossWinCondition.Points && team != 0
                ? rules.HalfCrossTeams.PieceCount(team) : null;
            bool resigned = _game.HasResigned(side);
            return GameTexts.ThreeKingdomsStatus(team, rules.HalfCrossTeams, _game.ScoreOf(side), threshold,
                resigned, !resigned && !_game.IsStillPlaying(side));
        }

        /// <summary>The side to move (its half is highlighted).</summary>
        public PlayerSide CurrentTurn => _game.CurrentTurn;

        /// <summary>
        /// The colour <paramref name="side"/> plays (<see cref="GameManager.ColorOf"/>): fixed on the
        /// Full board; on a half board decided by the first flip / first move, none before.
        /// </summary>
        public PieceColor ColorOf(PlayerSide side) => _game.ColorOf(side);

        /// <summary>
        /// The name shown for <paramref name="side"/>: <see cref="Player1Name"/> /
        /// <see cref="Player2Name"/> when set, else the player's name from the settings
        /// (<see cref="GameManager.NameOf"/>), otherwise the colour it plays
        /// (<see cref="GameManager.ColorOf"/>) — 紅方玩家 / 黑方玩家, or 先手玩家 / 後手玩家 while a
        /// dark-chess game has not decided the colours yet. The half is coloured by its colour either way.
        /// </summary>
        /// <param name="side">Player1 or Player2.</param>
        /// <returns>The name to draw.</returns>
        public string GetPlayerName(PlayerSide side)
        {
            string name = side switch
            {
                PlayerSide.Player1 => Player1Name,
                PlayerSide.Player2 => Player2Name,
                _ => null,
            };
            if (name != null)
                return name;
            name = _game.NameOf(side);
            if (name != null)
                return name;
            // 三國's players own teams, not colours.
            if (IsThreeKingdoms)
                return GameTexts.DefaultPlayerName(side);

            return GameTexts.InfoBoardColorName(side, _game.ColorOf(side));
        }

        /// <summary>
        /// The player's name (<see cref="GetPlayerName"/>), followed by 將軍
        /// (<see cref="GameTexts.InCheckSuffix"/>) while that side is to move and in check
        /// (<c>GameManager.IsInCheck</c>; false once the game is over and on boards without check).
        /// </summary>
        public string NameWithCheck(PlayerSide side)
        {
            string name = GetPlayerName(side);
            return _game.IsInCheck && _game.CurrentTurn == side ? name + GameTexts.InCheckSuffix : name;
        }

        /// <summary><paramref name="side"/>'s total time (<see cref="ClockFormatter.GetTotalTimeString"/>).</summary>
        public string TotalTimeText(PlayerSide side) => Clock.GetTotalTimeString(TimerOf(side));

        /// <summary><paramref name="side"/>'s step time (<see cref="ClockFormatter.GetStepTimeString"/>).</summary>
        public string StepTimeText(PlayerSide side) => Clock.GetStepTimeString(TimerOf(side));

        // The clock of the player (Player1 for a side that is not one).
        private PlayerTimer TimerOf(PlayerSide side) => (_game.PlayerOf(side) ?? _game.Player1).Timer;
    }
}

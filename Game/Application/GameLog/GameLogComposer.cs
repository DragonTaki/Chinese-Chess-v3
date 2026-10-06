/* ----- ----- ----- ----- */
// GameLogComposer.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/05
// Update Date: 2026/10/06
// Version: v1.2
/* ----- ----- ----- ----- */

using System;

using Chinese_Chess_v3.Game.Application.Texts;
using Chinese_Chess_v3.Game.Core;
using Chinese_Chess_v3.Game.Core.Pieces;

namespace Chinese_Chess_v3.Game.Application.GameLog
{
    /// <summary>
    /// The game log's sentences: turns each <see cref="GameManager.Logged"/> entry into its line
    /// (<see cref="GameTexts"/>) and hands it to the log set with <see cref="SetLog"/> (the
    /// sidebar's log box), so the rules layer only reports data and the display only shows the
    /// finished text. The logic layer's own lines (a failed save, an undo that is not possible...)
    /// go through <see cref="Write"/>. A single long-lived instance, like the <see cref="GameManager"/>.
    /// </summary>
    public sealed class GameLogComposer
    {
        private IGameLog _log;

        /// <summary>Subscribes to <paramref name="game"/>'s log entries; nothing is written until <see cref="SetLog"/>.</summary>
        public GameLogComposer(GameManager game)
        {
            ArgumentNullException.ThrowIfNull(game);
            game.Logged += OnLogged;
        }

        /// <summary>Where the lines go from now on (the game screen's log box, set each time it is built).</summary>
        public void SetLog(IGameLog log)
        {
            _log = log ?? throw new ArgumentNullException(nameof(log));
        }

        /// <summary>Writes <paramref name="line"/> to the log; dropped while no log is set.</summary>
        public void Write(string line) => _log?.AddMessage(line);

        private void OnLogged(GameLogEvent entry)
        {
            string line = Compose(entry);
            if (line != null)
                Write(line);
        }

        /// <summary>The line of <paramref name="entry"/>; null when it has none.</summary>
        public static string Compose(GameLogEvent entry) => entry switch
        {
            GameLogEvent.HalfCenterStarted e => GameTexts.HalfCenterStarted(e.IsHiddenChess),
            GameLogEvent.JieqiStarted => GameTexts.JieqiStarted,
            GameLogEvent.ThreeKingdomsStarted e => GameTexts.ThreeKingdomsStarted(e.WinCondition),
            GameLogEvent.TeamClaimed e => GameTexts.TeamClaimed(e.Side, e.Team),
            GameLogEvent.TurnSkipped e => GameTexts.TurnSkipped(e.Side),
            GameLogEvent.PlayerForfeited e => GameTexts.PlayerForfeited(e.Side, e.TimeUp),
            GameLogEvent.EndgameStarted e => GameTexts.EndgameStarted(e.Title, e.Goal),
            GameLogEvent.OpeningStarted e => GameTexts.OpeningStarted(e.Title, e.Ecco),
            GameLogEvent.SavedGameStarted e => GameTexts.SavedGameStarted(e.Title, e.IsRestart),
            GameLogEvent.GameSaved e => GameTexts.GameSaved(e.FileName),
            GameLogEvent.BoardClicked e => GameTexts.BoardClicked(e.Turn, e.Held, e.X, e.Y, e.Clicked, e.ClickedFaceDown),
            GameLogEvent.SelectionChanged e => GameTexts.SelectionChanged(e.Change, e.Type, e.X, e.Y),
            GameLogEvent.PieceTaken e => GameTexts.PieceTaken(e.Type, e.X, e.Y),
            GameLogEvent.PieceMoved e => GameTexts.PieceMoved(e.Type, e.X, e.Y),
            GameLogEvent.MovePlayed e => MoveLine(e.Move, e.MoverColor, e.Style),
            GameLogEvent.FactionsDecided e => GameTexts.FactionsDecided(e.Player1Color, e.Player2Color),
            GameLogEvent.CheckGiven e => GameTexts.CheckGiven(e.Side),
            GameLogEvent.TacticDetected e => GameTexts.TacticDetected(e.Event),
            GameLogEvent.MoveTakenBack e => GameTexts.MoveTakenBack(
                MoveLine(e.Move, e.MoverColor, e.Style) ?? GameTexts.MoveBackTo(e.PieceType, e.Move.FromX, e.Move.FromY)),
            GameLogEvent.TimeRanOut e => GameTexts.TimeRanOut(e.Side),
            GameLogEvent.GameEnded e => GameTexts.GameEnded(e.Winner, e.Reason),
            _ => throw new ArgumentOutOfRangeException(nameof(entry), entry, "Unknown game-log entry"),
        };

        /// <summary>A move's line in <paramref name="style"/>; null for <see cref="MoveLineStyle.Plain"/>.</summary>
        private static string MoveLine(MoveRecord move, PieceColor moverColor, MoveLineStyle style) => style switch
        {
            MoveLineStyle.Notation => GameTexts.MoveLine(move),
            MoveLineStyle.DarkChess => GameTexts.DarkChessLine(move, moverColor),
            MoveLineStyle.ThreeKingdoms => GameTexts.ThreeKingdomsLine(move),
            MoveLineStyle.Plain => null,
            _ => throw new ArgumentOutOfRangeException(nameof(style), style, "Unknown move line style"),
        };
    }
}

/* ----- ----- ----- ----- */
// PositionLoader.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/12
// Update Date: 2026/10/12
// Version: v1.0
/* ----- ----- ----- ----- */

using Chinese_Chess_v3.Game.Core;
using Chinese_Chess_v3.Game.Core.Boards;
using Chinese_Chess_v3.Game.Core.Pgn;
using Chinese_Chess_v3.Game.Core.Pieces;
using Chinese_Chess_v3.Game.Core.Players;

namespace Chinese_Chess_v3.Game.Ai;

internal static class PositionLoader
{
    internal static (Board Board, PlayerSide Side, Mailbox Position, List<ulong> History) Load(EnginePosition position, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(position);
        ArgumentNullException.ThrowIfNull(position.Rules);
        var parsed = XiangqiFen.Parse(position.Fen);
        if (parsed.Pieces.Count(p => p.Type == PieceType.General && p.Color == PieceColor.Red) != 1 ||
            parsed.Pieces.Count(p => p.Type == PieceType.General && p.Color == PieceColor.Black) != 1)
            throw new ArgumentException("A position must contain one general of each color.", nameof(position));
        var board = new Board(BoardType.Full, position.Rules.Clone());
        board.Initialize(parsed.Pieces);
        var mailbox = new Mailbox(parsed.Pieces, parsed.FirstColor == PieceColor.Red ? 1 : -1);
        var history = new List<ulong> { mailbox.Hash };
        var side = PlayerSide.Player1;
        foreach (var move in position.EarlierMoves ?? Array.Empty<IccsMove>())
        {
            ct.ThrowIfCancellationRequested();
            if (!IsLegal(board, side, move))
                throw new ArgumentException($"Illegal history move: {move}", nameof(position));
            ActionResolver.ApplyMove(board, side, board.GetPiece(move.FromX, move.FromY), move.ToX, move.ToY);
            mailbox.Make(move);
            history.Add(mailbox.Hash);
            side = ActionResolver.OpponentOf(side);
        }
        return (board, side, mailbox, history);
    }

    internal static bool IsLegal(Board board, PlayerSide side, IccsMove move) =>
        ActionResolver.IsLegalMove(board, side, move.FromX, move.FromY, move.ToX, move.ToY);

    internal static List<IccsMove> RootMoves(Board board, PlayerSide side)
    {
        var moves = new List<IccsMove>();
        foreach (var piece in board.GetAllPieces().Where(p => p.Side == side))
            foreach (var (x, y) in piece.GetLegalMoves(board))
            {
                var move = new IccsMove(piece.X, piece.Y, x, y);
                if (IsLegal(board, side, move)) moves.Add(move);
            }
        return moves;
    }
}

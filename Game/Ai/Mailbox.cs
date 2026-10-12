/* ----- ----- ----- ----- */
// Mailbox.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/12
// Update Date: 2026/10/12
// Version: v1.0
/* ----- ----- ----- ----- */

using Chinese_Chess_v3.Game.Core.Pgn;
using Chinese_Chess_v3.Game.Core.Pieces;

namespace Chinese_Chess_v3.Game.Ai;

internal sealed class Mailbox
{
    internal readonly int[] Squares = new int[90];
    internal int Side;
    internal ulong Hash { get; private set; }
    private static readonly ulong[,] Keys = CreateKeys();
    private static readonly int[,] PieceSquareTables = CreatePieceSquareTables();
    private static readonly int[] Values = { 0, 20000, 120, 120, 900, 400, 450, 100 };
    private static readonly (int X, int Y)[] Orthogonal = { (1, 0), (-1, 0), (0, 1), (0, -1) };
    private static readonly (int X, int Y)[] Diagonal = { (1, 1), (1, -1), (-1, 1), (-1, -1) };
    private static readonly (int X, int Y)[] Horse = { (2, 1), (2, -1), (-2, 1), (-2, -1), (1, 2), (-1, 2), (1, -2), (-1, -2) };

    internal Mailbox(IEnumerable<PieceInfo> pieces, int side)
    {
        Side = side;
        foreach (var p in pieces) Squares[p.Y * 9 + p.X] = (p.Color == PieceColor.Red ? 1 : -1) * (int)p.Type;
        RefreshHash();
    }

    private static ulong[,] CreateKeys()
    {
        var keys = new ulong[90, 15];
        ulong state = 0x9e3779b97f4a7c15;
        for (int i = 0; i < 90; i++)
            for (int j = 0; j < 15; j++)
            {
                state ^= state << 13; state ^= state >> 7; state ^= state << 17;
                keys[i, j] = state;
            }
        return keys;
    }

    private void RefreshHash()
    {
        Hash = Side == 1 ? 0 : ulong.MaxValue;
        for (int i = 0; i < 90; i++) if (Squares[i] != 0) Hash ^= Keys[i, Squares[i] + 7];
    }

    internal int Make(IccsMove move)
    {
        int from = move.FromY * 9 + move.FromX, to = move.ToY * 9 + move.ToX;
        int captured = Squares[to], piece = Squares[from];
        Hash ^= Keys[from, piece + 7] ^ Keys[to, piece + 7] ^ ulong.MaxValue;
        if (captured != 0) Hash ^= Keys[to, captured + 7];
        Squares[to] = piece; Squares[from] = 0; Side = -Side;
        return captured;
    }

    internal void Undo(IccsMove move, int captured)
    {
        int from = move.FromY * 9 + move.FromX, to = move.ToY * 9 + move.ToX;
        int piece = Squares[to];
        Hash ^= Keys[from, piece + 7] ^ Keys[to, piece + 7] ^ ulong.MaxValue;
        if (captured != 0) Hash ^= Keys[to, captured + 7];
        Squares[from] = piece; Squares[to] = captured; Side = -Side;
    }

    internal int CaptureValue(IccsMove m) => Values[Math.Abs(Squares[m.ToY * 9 + m.ToX])];
    internal int Ordering(IccsMove m) => 16 * CaptureValue(m) - Values[Math.Abs(Squares[m.FromY * 9 + m.FromX])];
    private static bool Inside(int x, int y) => x is >= 0 and < 9 && y is >= 0 and < 10;
    private static bool Palace(int side, int x, int y) => x is >= 3 and <= 5 && (side == 1 ? y >= 7 : y <= 2);

    private IEnumerable<IccsMove> Pseudo(int side)
    {
        for (int from = 0; from < 90; from++)
        {
            int piece = Squares[from];
            if (piece * side <= 0) continue;
            int x = from % 9, y = from / 9, type = Math.Abs(piece);
            if (type is 4 or 6)
            {
                foreach (var (dx, dy) in Orthogonal)
                {
                    bool screen = false;
                    for (int tx = x + dx, ty = y + dy; Inside(tx, ty); tx += dx, ty += dy)
                    {
                        int target = Squares[ty * 9 + tx];
                        if (!screen)
                        {
                            if (target == 0) yield return new(x, y, tx, ty);
                            else if (type == 4)
                            {
                                if (target * side < 0) yield return new(x, y, tx, ty);
                                break;
                            }
                            else screen = true;
                        }
                        else if (target != 0)
                        {
                            if (target * side < 0) yield return new(x, y, tx, ty);
                            break;
                        }
                    }
                }
                continue;
            }
            var offsets = type switch { 1 => Orthogonal, 2 or 3 => Diagonal, 5 => Horse, 7 => Orthogonal,
                _ => throw new InvalidOperationException("Unsupported mailbox piece.") };
            foreach (var (dx0, dy0) in offsets)
            {
                int dx = type == 3 ? dx0 * 2 : dx0, dy = type == 3 ? dy0 * 2 : dy0;
                int tx = x + dx, ty = y + dy;
                if (!Inside(tx, ty) || Squares[ty * 9 + tx] * side > 0) continue;
                if (type is 1 or 2 && !Palace(side, tx, ty)) continue;
                if (type == 3 && ((side == 1 ? ty < 5 : ty > 4) || Squares[(y + dy / 2) * 9 + x + dx / 2] != 0)) continue;
                if (type == 5 && Squares[(y + (Math.Abs(dy) == 2 ? Math.Sign(dy) : 0)) * 9 + x + (Math.Abs(dx) == 2 ? Math.Sign(dx) : 0)] != 0) continue;
                if (type == 7 && (dy != -side && !(dy == 0 && (side == 1 ? y < 5 : y > 4)))) continue;
                yield return new(x, y, tx, ty);
            }
        }
    }

    internal bool InCheck(int side)
    {
        int king = Array.IndexOf(Squares, side);
        if (king < 0) return true;
        int other = Array.IndexOf(Squares, -side);
        if (other >= 0 && king % 9 == other % 9)
        {
            bool facing = true;
            for (int i = Math.Min(king, other) + 9; i < Math.Max(king, other); i += 9)
                if (Squares[i] != 0) { facing = false; break; }
            if (facing) return true;
        }
        foreach (var m in Pseudo(-side)) if (m.ToY * 9 + m.ToX == king) return true;
        return false;
    }

    internal List<IccsMove> LegalMoves(bool capturesOnly = false)
    {
        var moves = new List<IccsMove>();
        int side = Side;
        // Materialize before simulation because the generator reads the mutable board.
        foreach (var move in Pseudo(side).ToArray())
        {
            if (capturesOnly && CaptureValue(move) == 0) continue;
            int captured = Make(move);
            bool legal = !InCheck(side);
            Undo(move, captured);
            if (legal) moves.Add(move);
        }
        return moves;
    }

    private static int[,] CreatePieceSquareTables()
    {
        var tables = new int[8, 90];
        for (int type = 1; type <= 7; type++)
            for (int i = 0; i < 90; i++)
            {
                int advance = 9 - i / 9, center = 4 - Math.Abs(i % 9 - 4);
                tables[type, i] = type switch
                {
                    7 => advance * 8 + (advance >= 5 ? 30 + center * 4 : 0),
                    5 => center * 8 + Math.Min(advance, 6) * 3,
                    4 => center * 3 + Math.Min(advance, 6) * 2,
                    6 => center * 5,
                    _ => 0,
                };
            }
        return tables;
    }

    internal int Evaluate()
    {
        int score = 0;
        for (int i = 0; i < 90; i++)
        {
            int piece = Squares[i];
            if (piece == 0) continue;
            int type = Math.Abs(piece), side = Math.Sign(piece);
            score += side * (Values[type] + PieceSquareTables[type, side == 1 ? i : 89 - i]);
        }
        return score * Side;
    }
}

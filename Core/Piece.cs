/* ----- ----- ----- ----- */
// Piece.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/06
// Update Date: 2025/05/07
// Version: v1.1
/* ----- ----- ----- ----- */

using System.Collections.Generic;
using System.Drawing;

using Chinese_Chess_v3.Models;

namespace Chinese_Chess_v3.Core
{
    public abstract class Piece
    {
        public PieceType Type { get; }
        public PlayerSide Side { get; }
        public int X { get; set; }
        public int Y { get; set; }
        public Point Position => new Point(X, Y);

        protected Piece(PieceType type, int x, int y, PlayerSide side)
        {
            Type = type;
            Side = side;
            X = x;
            Y = y;
        }

        // If specific piece can land here
        public virtual bool IsInLegalZone(int x, int y) => true;  // Default true for all

        public abstract bool IsValidMove(int targetX, int targetY, Board board);

        // Main entrance
        public bool CanMoveTo(int targetX, int targetY, Board board)
        {
            return IsValidMove(targetX, targetY, board);
        }
        public abstract List<(int x, int y)> GetLegalMoves(int x, int y, Board board);

        protected bool IsDestinationLegal(int targetX, int targetY, Board board)
        {
            Piece targetPiece = board.Grid[targetX, targetY];
            return targetPiece == null || targetPiece.Side != this.Side;
        }
    }
}   
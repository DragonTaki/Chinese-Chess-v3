using System.Collections.Generic;
using Chinese_Chess_v3.Constants;

namespace Chinese_Chess_v3.Core
{
    public abstract class Piece
    {
        public PieceType Type { get; }
        public PlayerSide Side { get; }
        public int X { get; set; }
        public int Y { get; set; }

        protected Piece(PieceType type, PlayerSide side, int x, int y)
        {
            Type = type;
            Side = side;
            X = x;
            Y = y;
        }


        // If specific piece can land here
        public virtual bool IsInLegalZone(int targetX, int targetY)
        {
            // Defult true for all
            return true;
        }
        public abstract bool IsValidMove(int targetX, int targetY, Board board);

        // Main entrance
        public bool CanMoveTo(int targetX, int targetY, Board board)
        {
            return IsValidMove(targetX, targetY, board);
        }
        public abstract List<(int x, int y)> GetLegalMoves(int x, int y, Board board);
    }
}   
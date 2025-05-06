using System;
using System.Collections.Generic;

using Chinese_Chess_v3.Constants;

namespace Chinese_Chess_v3.Core
{
    public class General : Piece
    {
        // Constructor
        public General(PlayerSide side, int x, int y)
            : base(PieceType.General, side, x, y)
        {
        }

        // Check if moved to valid area
        private bool IsInLegalZone(int targetX, int targetY)
        {
            if (targetX < BoardSettings.PalaceXRange.MinX || targetX > BoardSettings.PalaceXRange.MaxX)
                return false;

            if (Side == PlayerSide.Red)
                return targetY >= BoardSettings.RedPalaceYRange.MinY && targetY <= BoardSettings.RedPalaceYRange.MaxY;
            else
                return targetY >= BoardSettings.BlackPalaceYRange.MinY && targetY <= BoardSettings.BlackPalaceYRange.MaxY;
        }

        // Check if is a valid move
        public override bool IsValidMove(int targetX, int targetY, Board board)
        {
            int dx = Math.Abs(targetX - X);
            int dy = Math.Abs(targetY - Y);

            // Must move exactly 1 square either horizontally or vertically
            bool isStepMove = (dx == 1 && dy == 0) || (dx == 0 && dy == 1);
            if (!isStepMove)
                return false;

            // Check if in palace
            if (!IsInLegalZone(targetX, targetY))
                return false;
            
            // Check if destination has ally
            Piece targetPiece = board.Grid[targetX, targetY];
            if (targetPiece != null && targetPiece.Side == this.Side)
                return false;

            return true;
        }

        // Get every moves can do
        public override List<(int x, int y)> GetLegalMoves(int x, int y, Board board)
        {
            List<(int x, int y)> legalMoves = new List<(int x, int y)>();

            // Try all possible horizontal and vertical moves
            // Top-right
            if (IsInLegalZone(x + 1, y + 2))
                legalMoves.Add((x + 1, y + 2));

            // Top-left
            if (IsInLegalZone(x - 2, y + 2))
                legalMoves.Add((x - 2, y + 2));

            // Bottom-right
            if (IsInLegalZone(x + 2, y - 2))
                legalMoves.Add((x + 2, y - 2));

            // Bottom-left
            if (IsInLegalZone(x - 2, y - 2))
                legalMoves.Add((x - 2, y - 2));

            return legalMoves;
        }
    }
}
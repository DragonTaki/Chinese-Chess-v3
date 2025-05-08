/* ----- ----- ----- ----- */
// Horse.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/06
// Update Date: 2025/05/06
// Version: v1.0
/* ----- ----- ----- ----- */

using System;
using System.Collections.Generic;

using Chinese_Chess_v3.Configs.Board;

namespace Chinese_Chess_v3.Core.Pieces
{
    public class Horse : Piece
    {
        // Constructor
        public Horse(int x, int y, PlayerSide side)
            : base(PieceType.Horse, x, y, side)
        {
        }

        // Check if moved to valid area
        public override bool IsInLegalZone(int targetX, int targetY)
        {
            // No specific zone limit for chariot, but method reserved for consistency
            return BoardConstants.IsInBounds(targetX, targetY);
        }

        // Check if is a valid move
        public override bool IsValidMove(int targetX, int targetY, Board board)
        {
            if (!IsInLegalZone(targetX, targetY))
                return false;

            int dx = targetX - X;
            int dy = targetY - Y;

            int absDx = Math.Abs(dx);
            int absDy = Math.Abs(dy);

            // Must move in "L" shape (1 step + 2 step) (馬走日)
            if (!((absDx == 1 && absDy == 2) || (absDx == 2 && absDy == 1)))
                return false;

            // Check if "horse's leg" is hobbled (蹩馬腳)
            int blockX = X + (dx / absDx);  // one step in X if 2-step move is in X
            int blockY = Y + (dy / absDy);  // one step in Y if 2-step move is in Y

            if (absDx == 2 && board.Grid[blockX, Y] != null)
                return false;
            if (absDy == 2 && board.Grid[X, blockY] != null)
                return false;

            // Check if destination has ally
            if (!IsDestinationLegal(targetX, targetY, board))
                return false;

            return true;
        }

        // Get every moves can do
        public override List<(int x, int y)> GetLegalMoves(int x, int y, Board board)
        {
            List<(int x, int y)> legalMoves = new List<(int x, int y)>();

            // Define all L-shaped move directions
            (int dx, int dy)[] directions = new (int, int)[]
            {
                (2, 1),    // Right 2, Up 1
                (1, 2),    // Right 1, Up 2
                (-1, 2),   // Left 1, Up 2
                (-2, 1),   // Left 2, Up 1
                (-2, -1),  // Left 2, Down 1
                (-1, -2),  // Left 1, Down 2
                (1, -2),   // Right 1, Down 2
                (2, -1)    // Right 2, Down 1
            };

            foreach (var (dx, dy) in directions)
            {
                int newX = x + dx;
                int newY = y + dy;

                if (!BoardConstants.IsInBounds(newX, newY))
                    continue;

                // Check if horse's leg is hobbled
                int blockX = x + (Math.Abs(dx) == 2 ? Math.Sign(dx) : 0);
                int blockY = y + (Math.Abs(dy) == 2 ? Math.Sign(dy) : 0);

                if (board.Grid[blockX, blockY] != null)
                    continue;

                // Check if destination has ally
                if (!IsDestinationLegal(newX, newY, board))
                    continue;

                legalMoves.Add((newX, newY));
            }

            return legalMoves;
        }
    }
}
/* ----- ----- ----- ----- */
// Soldier.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/06
// Update Date: 2025/05/06
// Version: v1.0
/* ----- ----- ----- ----- */

using System;
using System.Collections.Generic;

using Chinese_Chess_v3.Configs;
using Chinese_Chess_v3.Models;

namespace Chinese_Chess_v3.Core.Pieces
{
    public class Soldier : Piece
    {
        // Constructor
        public Soldier(int x, int y, PlayerSide side)
            : base(PieceType.Soldier, x, y, side)
        {
        }

        // Check if moved to valid area
        public override bool IsInLegalZone(int targetX, int targetY)
        {
            // No specific zone limit for chariot, but method reserved for consistency
            return Constants.Board.IsInBounds(targetX, targetY);
        }

        // Check if move is valid
        public override bool IsValidMove(int targetX, int targetY, Board board)
        {
            if (!IsInLegalZone(targetX, targetY))
                return false;

            int dx = targetX - X;
            int dy = targetY - Y;

            // Soldier can only move 1 step
            if (Math.Abs(dx) + Math.Abs(dy) != 1)
                return false;

            // Forward direction: Red (Y--), Black (Y++)
            int forward = (Side == PlayerSide.Red) ? -1 : 1;

            // Always allow forward step
            if (dy == forward && dx == 0)
                return true;

            // Allow side move only after crossing river
            if (dy == 0 && Math.Abs(dx) == 1 && HasCrossedRiver(Y))
                return true;

            return false;
        }

        // Get every moves can do
        public override List<(int x, int y)> GetLegalMoves(int x, int y, Board board)
        {
            List<(int x, int y)> legalMoves = new List<(int x, int y)>();

            // Define every possible move directions
            int forward = (Side == PlayerSide.Red) ? -1 : 1;
            (int dx, int dy)[] directions = HasCrossedRiver(y)
                ? new (int, int)[]
                {
                    (0, forward),   // Forward
                    (-1, 0),        // Left
                    (1, 0)          // Right
                }
                : new (int, int)[]
                {
                    (0, forward)    // Only Forward
                };

            foreach (var (dx, dy) in directions)
            {
                int newX = x + dx;
                int newY = y + dy;

                if (!Constants.Board.IsInBounds(newX, newY))
                    continue;

                // Check if destination has ally
                if (!IsDestinationLegal(newX, newY, board))
                    continue;

                legalMoves.Add((newX, newY));
            }

            return legalMoves;
        }

        // Check if crossed the river
        private bool HasCrossedRiver(int y)
        {
            return Side == PlayerSide.Red
                ? y <= Constants.Board.RedYSideRiverLine
                : y >= Constants.Board.BlackYSideRiverLine;
        }
    }
}
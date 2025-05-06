/* ----- ----- ----- ----- */
// Elephant.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/06
// Update Date: 2025/05/06
// Version: v1.0
/* ----- ----- ----- ----- */

using System;
using System.Collections.Generic;

using Chinese_Chess_v3.Configs;
using Chinese_Chess_v3.Core;

namespace Chinese_Chess_v3.Core.Pieces
{
    public class Elephant : Piece
    {
        // Constructor
        public Elephant(PlayerSide side, int x, int y)
            : base(PieceType.Elephant, side, x, y)
        {
        }

        // Check if moved to valid area
        public override bool IsInLegalZone(int targetX, int targetY)
        {
            // Elephants cannot across the river (不可過河)
            if (Side == PlayerSide.Red)
                return targetY >= BoardConstants.RedYSideRiverLine && targetY <= 9;
            else
                return targetY <= BoardConstants.BlackYSideRiverLine && targetY >= 0;
        }

        // Check if is a valid move
        public override bool IsValidMove(int targetX, int targetY, Board board)
        {
            int dx = Math.Abs(targetX - X);
            int dy = Math.Abs(targetY - Y);

            // Must move 2 squares diagonally (象走田)
            if (dx != 2 || dy != 2)
                return false;

            // Check if in allowed side
            if (!IsInLegalZone(targetX, targetY))
                return false;

            // Check if "elephant's eye" is blocked (卡象眼)
            int midX = X + dx / 2;
            int midY = Y + dy / 2;
            if (board.Grid[midX, midY] != null)
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

            // Define every move directions
            (int dx, int dy)[] directions = new (int, int)[]
            {
                (2, 2),   // Top-right
                (-2, 2),  // Top-left
                (2, -2),  // Bottom-right
                (-2, -2)  // Bottom-left
            };

            // Try all possible diagonal moves
            foreach (var (dx, dy) in directions)
            {
                int newX = x + dx;
                int newY = y + dy;

                if (!IsInLegalZone(newX, newY))
                    continue;

                int midX = x + dx / 2;
                int midY = y + dy / 2;

                // Check if elephant's eye is blocked
                if (board.Grid[midX, midY] != null)
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
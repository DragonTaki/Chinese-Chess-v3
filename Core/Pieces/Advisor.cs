/* ----- ----- ----- ----- */
// Advisor.cs
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
    public class Advisor : Piece
    {
        // Constructor
        public Advisor(PlayerSide side, int x, int y)
            : base(PieceType.Advisor, side, x, y)
        {
        }

        // Check if moved to valid area
        public override bool IsInLegalZone(int targetX, int targetY)
        {
            // Only can stay in palace (九宮格)
            if (targetX < BoardConstants.PalaceXRange.MinX || targetX > BoardConstants.PalaceXRange.MaxX)
                return false;

            if (Side == PlayerSide.Red)
                return targetY >= BoardConstants.RedPalaceYRange.MinY && targetY <= BoardConstants.RedPalaceYRange.MaxY;
            else
                return targetY >= BoardConstants.BlackPalaceYRange.MinY && targetY <= BoardConstants.BlackPalaceYRange.MaxY;
        }

        // Check if is a valid move
        public override bool IsValidMove(int targetX, int targetY, Board board)
        {
            int dx = Math.Abs(targetX - X);
            int dy = Math.Abs(targetY - Y);

            // Must move 1 squares diagonally
            if (dx != 1 || dy != 1)
                return false;

            // Check if in palace
            if (!IsInLegalZone(targetX, targetY))
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
                (1, 1),   // Top-right
                (-1, 1),  // Top-left
                (1, -1),  // Bottom-right
                (-1, -1)  // Bottom-left
            };

            // Try all possible diagonal moves
            foreach (var (dx, dy) in directions)
            {
                int newX = x + dx;
                int newY = y + dy;

                if (!IsInLegalZone(newX, newY))
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
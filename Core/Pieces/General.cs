/* ----- ----- ----- ----- */
// General.cs
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

namespace Chinese_Chess_v3.Core
{
    public class General : Piece
    {
        private PlayerSide side;

        // Constructor
        public General(int x, int y, PlayerSide side)
            : base(PieceType.General, x, y, side)
        {
        }

        // Check if moved to valid area
        public override bool IsInLegalZone(int targetX, int targetY)
        {
            // Only can stay in palace (九宮格)
            if (targetX < Constants.Board.PalaceXRange.MinX || targetX > Constants.Board.PalaceXRange.MaxX)
                return false;

            if (Side == PlayerSide.Red)
                return targetY >= Constants.Board.RedPalaceYRange.MinY && targetY <= Constants.Board.RedPalaceYRange.MaxY;
            else
                return targetY >= Constants.Board.BlackPalaceYRange.MinY && targetY <= Constants.Board.BlackPalaceYRange.MaxY;
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
                (1, 0),  // Right
                (-1, 0), // Left
                (0, 1),  // Up
                (0, -1)  // Down
            };

            // Try all possible horizontal or vertical moves
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
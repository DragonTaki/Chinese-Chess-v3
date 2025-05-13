/* ----- ----- ----- ----- */
// Chariot.cs
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
    public class Chariot : Piece
    {
        // Constructor
        public Chariot(int x, int y, PlayerSide side)
            : base(PieceType.Chariot, x, y, side)
        {
        }

        // Check if moved to valid area
        public override bool IsInLegalZone(int targetX, int targetY)
        {
            // No specific zone limit for chariot, but method reserved for consistency
            return Constants.Board.IsInBounds(targetX, targetY);
        }

        // Check if is a valid move
        public override bool IsValidMove(int targetX, int targetY, Board board)
        {
            if (!IsInLegalZone(targetX, targetY))
                return false;

            int dx = targetX - X;
            int dy = targetY - Y;

            // Only allow straight line movement
            if (dx != 0 && dy != 0)
                return false;

            // Determine direction and step (get X/Y, +1/-1)
            int stepX = Math.Sign(dx);
            int stepY = Math.Sign(dy);

            int currX = X + stepX;
            int currY = Y + stepY;

            // Check path obstruction (車衝無障礙物)
            while (currX != targetX || currY != targetY)
            {
                if (board.Grid[currX, currY] != null)
                    return false;

                currX += stepX;
                currY += stepY;
            }

            // Check if destination has ally
            if (!IsDestinationLegal(targetX, targetY, board))
                return false;

            return true;
        }

        // Get every moves can do
        public override List<(int x, int y)> GetLegalMoves(int x, int y, Board board)
        {
            List<(int x, int y)> legalMoves = new List<(int x, int y)>();

            // Define four directions can move
            (int dx, int dy)[] directions = new (int, int)[]
            {
                (1, 0),  // Right
                (-1, 0), // Left
                (0, 1),  // Up
                (0, -1)  // Down
            };

            foreach (var (dx, dy) in directions)
            {
                int currX = x + dx;
                int currY = y + dy;

                while (Constants.Board.IsInBounds(currX, currY))
                {
                    Piece obstacle = board.Grid[currX, currY];

                    if (obstacle == null)
                    {
                        legalMoves.Add((currX, currY));
                    }
                    else
                    {
                        if (obstacle.Side != this.Side)
                            legalMoves.Add((currX, currY));
                        break;
                    }

                    currX += dx;
                    currY += dy;
                }
            }

            return legalMoves;
        }
    }
}
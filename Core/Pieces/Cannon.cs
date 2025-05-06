/* ----- ----- ----- ----- */
// Cannon.cs
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
    public class Cannon : Piece
    {
        // Constructor
        public Cannon(PlayerSide side, int x, int y)
            : base(PieceType.Cannon, side, x, y)
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

            // Only allow straight line movement
            if (dx != 0 && dy != 0)
                return false;

            // Determine if jumps or not
            int count = CountPiecesBetween(X, Y, targetX, targetY, board);

            Piece targetPiece = board.Grid[targetX, targetY];
            if (targetPiece == null)
            {
                // No piece on target — must have no pieces in between
                return count == 0;
            }
            else
            {
                // Capturing — must have exactly one piece in between, and target is enemy
                return count == 1 && targetPiece.Side != this.Side;
            }
        }

        // Get every moves can do
        public override List<(int x, int y)> GetLegalMoves(int x, int y, Board board)
        {
            List<(int x, int y)> legalMoves = new List<(int x, int y)>();

            // Define four directions can move
            (int dx, int dy)[] directions = new (int, int)[]
            {
                (1, 0),   // Right
                (-1, 0),  // Left
                (0, 1),   // Up
                (0, -1)   // Down
            };

            foreach (var (dx, dy) in directions)
            {
                bool jumped = false;

                int currX = x + dx;
                int currY = y + dy;

                while (BoardConstants.IsInBounds(currX, currY))
                {
                    Piece target = board.Grid[currX, currY];

                    if (!jumped)
                    {
                        if (target == null)
                        {
                            legalMoves.Add((currX, currY));
                        }
                        else
                        {
                            jumped = true;
                        }
                    }
                    else
                    {
                        if (target != null && target.Side != this.Side)
                        {
                            legalMoves.Add((currX, currY));
                        }
                        break;
                    }

                    currX += dx;
                    currY += dy;
                }
            }

            return legalMoves;
        }

        // Count pieces between (used in move validation)
        private int CountPiecesBetween(int startX, int startY, int endX, int endY, Board board)
        {
            int count = 0;

            int dx = Math.Sign(endX - startX);
            int dy = Math.Sign(endY - startY);

            int x = startX + dx;
            int y = startY + dy;

            while (x != endX || y != endY)
            {
                if (board.Grid[x, y] != null)
                    count++;

                x += dx;
                y += dy;
            }

            return count;
        }
    }
}
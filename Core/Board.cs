using System.Collections.Generic;

namespace Chinese_Chess_v3.Core
{
    public class Board
    {
        public Piece[,] Grid { get; }

        public Board()
        {
            Grid = new Piece[9, 10];
            /*
                    X →           Black Side
                 (0,Y)(1,Y)(2,Y)(3,Y)(4,Y)(5,Y)(6,Y)(7,Y)(8,Y)
             (X, 0)  +---+---+---+---+---+---+---+---+---+ ← Y=0
                     |   |   |   |   |   |   |   |   |   |
             (X, 1)  +---+---+---+---+---+---+---+---+---+ ← Y=1
                     |   |   |   |   |   |   |   |   |   |
             (X, 2)  +---+---+---+---+---+---+---+---+---+ ← Y=2
                     |   |   |   |   |   |   |   |   |   |
             (X, 3)  +---+---+---+---+---+---+---+---+---+ ← Y=3
                     |   |   |   |   |   |   |   |   |   |
             (X, 4)  +---+---+---+---+---+---+---+---+---+ ← Y=4
                     |         T h e   R i v e r         |
             (X, 5)  +---+---+---+---+---+---+---+---+---+ ← Y=5
                     |   |   |   |   |   |   |   |   |   |
             (X, 6)  +---+---+---+---+---+---+---+---+---+ ← Y=6
                     |   |   |   |   |   |   |   |   |   |
             (X, 7)  +---+---+---+---+---+---+---+---+---+ ← Y=7
                     |   |   |   |   |   |   |   |   |   |
             (X, 8)  +---+---+---+---+---+---+---+---+---+ ← Y=8
                     |   |   |   |   |   |   |   |   |   |
             (X, 9)  +---+---+---+---+---+---+---+---+---+ ← Y=9
                 (0,Y)(1,Y)(2,Y)(3,Y)(4,Y)(5,Y)(6,Y)(7,Y)(8,Y)
                    X →             Red Side
            */
            // Left to right (x-axis): 0~8; Top to bottom (y-axis): 0~9
            // Red area (y-axis): 0~4; Black area (y-axis): 5~9
        }

        public Piece GetPiece(int x, int y) => Grid[x, y];

        public void PlacePiece(Piece piece)
        {
            Grid[piece.X, piece.Y] = piece;
        }

        public void MovePiece(int fromX, int fromY, int toX, int toY)
        {
            var piece = Grid[fromX, fromY];
            Grid[toX, toY] = piece;
            Grid[fromX, fromY] = null;
            piece.X = toX;
            piece.Y = toY;
        }
    }
}
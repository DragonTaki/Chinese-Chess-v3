/* ----- ----- ----- ----- */
// Board.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/06
// Update Date: 2025/05/06
// Version: v1.0
/* ----- ----- ----- ----- */

using System;
using System.Collections.Generic;

using Chinese_Chess_v3.Configs.Board;
using Chinese_Chess_v3.Core.Pieces;

namespace Chinese_Chess_v3.Core
{
    public class Board
    {
        public Piece[,] Grid { get; }
        private List<Piece> pieces = new List<Piece>();
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

        public void Initialize()
        {
            pieces.Clear();

            // Load preset in PieceConstants
            foreach (var info in PieceConstants.InitialPieces)
            {
                var piece = CreatePieceFromInfo(info);
                Grid[info.X, info.Y] = piece;
                pieces.Add(piece);
            }
        }
        
        private Piece CreatePieceFromInfo(PieceInfo info)
        {
            PlayerSide side = info.IsRed ? PlayerSide.Red : PlayerSide.Black;
            return info.Type switch
            {
                PieceType.General   => new General(info.X, info.Y, side),
                PieceType.Advisor   => new Advisor(info.X, info.Y, side),
                PieceType.Elephant  => new Elephant(info.X, info.Y, side),
                PieceType.Horse     => new Horse(info.X, info.Y, side),
                PieceType.Chariot   => new Chariot(info.X, info.Y, side),
                PieceType.Cannon    => new Cannon(info.X, info.Y, side),
                PieceType.Soldier   => new Soldier(info.X, info.Y, side),
                _ => throw new Exception("Unknown piece type"),
            };
        }
        public List<Piece> GetAllPieces()
        {
            return pieces;
        }
        public Piece GetPiece(int x, int y)
        {
            if (x >= 0 && x < BoardConstants.Columns && y >= 0 && y < BoardConstants.Rows)
            {
                return Grid[x, y];
            }
            return null;
        }

        public void PlacePiece(Piece piece)
        {
            Grid[piece.X, piece.Y] = piece;
        }

        public void MovePiece(int fromX, int fromY, int toX, int toY)
        {
            var piece = Grid[fromX, fromY];
            Grid[toX, toY] = piece;
            Grid[fromX, fromY] = null;
            if (piece != null)
            {
                piece.X = toX;
                piece.Y = toY;
            }
        }

        public void RemovePiece(int x, int y)
        {
            var piece = Grid[x, y];
            if (piece != null)
            {
                pieces.Remove(piece);
            }
            Grid[x, y] = null;
        }
    }
}
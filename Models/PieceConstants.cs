/* ----- ----- ----- ----- */
// PieceConstants.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/06
// Update Date: 2025/05/06
// Version: v1.0
/* ----- ----- ----- ----- */

using System.Collections.Generic;
using System.Linq;

namespace Chinese_Chess_v3.Models
{
    public static class PieceConstants
    {
        public static string GetPieceText(PieceType type, bool isRed)
        {
            switch (type)
            {
                case PieceType.General:  return isRed ? "帥" : "將";
                case PieceType.Advisor:  return isRed ? "仕" : "士";
                case PieceType.Elephant: return isRed ? "相" : "象";
                case PieceType.Chariot:  return isRed ? "俥" : "車";
                case PieceType.Horse:    return isRed ? "傌" : "馬";
                case PieceType.Cannon:   return isRed ? "炮" : "包";
                case PieceType.Soldier:  return isRed ? "兵" : "卒";
                default:                 return "";
            }
        }

        // Piece initial locations
        private static (PieceType type, int x, int y, bool isRed)[] pieceData = new[]
        {
            (PieceType.General,  4, 0, false),
            (PieceType.Advisor,  3, 0, false),
            (PieceType.Advisor,  5, 0, false),
            (PieceType.Elephant, 2, 0, false),
            (PieceType.Elephant, 6, 0, false),
            (PieceType.Horse,    1, 0, false),
            (PieceType.Horse,    7, 0, false),
            (PieceType.Chariot,  0, 0, false),
            (PieceType.Chariot,  8, 0, false),
            (PieceType.Cannon,   1, 2, false),
            (PieceType.Cannon,   7, 2, false),
            (PieceType.Soldier,  0, 3, false),
            (PieceType.Soldier,  2, 3, false),
            (PieceType.Soldier,  4, 3, false),
            (PieceType.Soldier,  6, 3, false),
            (PieceType.Soldier,  8, 3, false),

            (PieceType.General,  4, 9, true),
            (PieceType.Advisor,  3, 9, true),
            (PieceType.Advisor,  5, 9, true),
            (PieceType.Elephant, 2, 9, true),
            (PieceType.Elephant, 6, 9, true),
            (PieceType.Horse,    1, 9, true),
            (PieceType.Horse,    7, 9, true),
            (PieceType.Chariot,  0, 9, true),
            (PieceType.Chariot,  8, 9, true),
            (PieceType.Cannon,   1, 7, true),
            (PieceType.Cannon,   7, 7, true),
            (PieceType.Soldier,  0, 6, true),
            (PieceType.Soldier,  2, 6, true),
            (PieceType.Soldier,  4, 6, true),
            (PieceType.Soldier,  6, 6, true),
            (PieceType.Soldier,  8, 6, true),
        };
        public static List<PieceInfo> InitialPieces = pieceData
            .Select(p => new PieceInfo(p.type, p.x, p.y, p.isRed))
            .ToList();
    }
}
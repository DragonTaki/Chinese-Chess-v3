/* ----- ----- ----- ----- */
// PieceConstants.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/06
// Update Date: 2025/05/06
// Version: v1.0
/* ----- ----- ----- ----- */

using System.Collections.Generic;

namespace Chinese_Chess_v3.Configs
{
    public enum PieceType
    {
        General,   // 帥；將
        Advisor,   // 仕；士
        Elephant,  // 相；象
        Chariot,   // 俥；車
        Horse,     // 傌；馬
        Cannon,    // 炮；包
        Soldier    // 兵；卒
    }

    public enum PlayerSide
    {
        Red,
        Black
    }
    
    public class PieceInfo
    {
        public PieceType Type;
        public int X;
        public int Y;
        public bool IsRed;
        public bool FaceUp;

        public PieceInfo(PieceType type, int x, int y, bool isRed, bool faceUp = true)
        {
            Type = type;
            X = x;
            Y = y;
            IsRed = isRed;
            FaceUp = faceUp;
        }
    }
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
                default: return "";
            }
        }

        public static List<PieceInfo> InitialPieces = new List<PieceInfo>
        {
            new PieceInfo(PieceType.General, 4, 0, false),
            new PieceInfo(PieceType.Advisor, 3, 0, false),
            new PieceInfo(PieceType.Advisor, 5, 0, false),
            new PieceInfo(PieceType.Elephant, 2, 0, false),
            new PieceInfo(PieceType.Elephant, 6, 0, false),
            new PieceInfo(PieceType.Horse, 1, 0, false),
            new PieceInfo(PieceType.Horse, 7, 0, false),
            new PieceInfo(PieceType.Chariot, 0, 0, false),
            new PieceInfo(PieceType.Chariot, 8, 0, false),
            new PieceInfo(PieceType.Cannon, 1, 2, false),
            new PieceInfo(PieceType.Cannon, 7, 2, false),
            new PieceInfo(PieceType.Soldier, 0, 3, false),
            new PieceInfo(PieceType.Soldier, 2, 3, false),
            new PieceInfo(PieceType.Soldier, 4, 3, false),
            new PieceInfo(PieceType.Soldier, 6, 3, false),
            new PieceInfo(PieceType.Soldier, 8, 3, false),

            new PieceInfo(PieceType.General, 4, 9, true),
            new PieceInfo(PieceType.Advisor, 3, 9, true),
            new PieceInfo(PieceType.Advisor, 5, 9, true),
            new PieceInfo(PieceType.Elephant, 2, 9, true),
            new PieceInfo(PieceType.Elephant, 6, 9, true),
            new PieceInfo(PieceType.Horse, 1, 9, true),
            new PieceInfo(PieceType.Horse, 7, 9, true),
            new PieceInfo(PieceType.Chariot, 0, 9, true),
            new PieceInfo(PieceType.Chariot, 8, 9, true),
            new PieceInfo(PieceType.Cannon, 1, 7, true),
            new PieceInfo(PieceType.Cannon, 7, 7, true),
            new PieceInfo(PieceType.Soldier, 0, 6, true),
            new PieceInfo(PieceType.Soldier, 2, 6, true),
            new PieceInfo(PieceType.Soldier, 4, 6, true),
            new PieceInfo(PieceType.Soldier, 6, 6, true),
            new PieceInfo(PieceType.Soldier, 8, 6, true),
        };
    }
}
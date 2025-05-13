/* ----- ----- ----- ----- */
// PieceInfo.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/14
// Update Date: 2025/05/14
// Version: v1.0
/* ----- ----- ----- ----- */

namespace Chinese_Chess_v3.Models
{
    public class PieceInfo
    {
        public PieceType Type { get; }
        public int X { get; set; }
        public int Y { get; set; }
        public bool IsRed { get; }
        public bool FaceUp { get; set; }

        public PieceInfo(PieceType type, int x, int y, bool isRed, bool faceUp = true)
        {
            Type = type;
            X = x;
            Y = y;
            IsRed = isRed;
            FaceUp = faceUp;
        }
    }
}
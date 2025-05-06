/* ----- ----- ----- ----- */
// GameManager.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/06
// Update Date: 2025/05/06
// Version: v1.0
/* ----- ----- ----- ----- */

using Chinese_Chess_v3.Configs;

namespace Chinese_Chess_v3.Core
{
    public class GameManager
    {
        public Board Board { get; private set; }
        public PlayerSide CurrentTurn { get; private set; }

        public GameManager()
        {
            Board = new Board();
            CurrentTurn = PlayerSide.Red;
            // 初始化棋子待補充
        }

        public bool TryMove(int fromX, int fromY, int toX, int toY)
        {
            var piece = Board.GetPiece(fromX, fromY);
            if (piece == null || piece.Side != CurrentTurn)
                return false;

            if (piece.CanMoveTo(toX, toY, Board))
            {
                Board.MovePiece(fromX, fromY, toX, toY);
                SwitchTurn();
                return true;
            }

            return false;
        }

        private void SwitchTurn()
        {
            CurrentTurn = CurrentTurn == PlayerSide.Red ? PlayerSide.Black : PlayerSide.Red;
        }
    }
}
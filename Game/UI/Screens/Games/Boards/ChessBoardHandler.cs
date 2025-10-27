/* ----- ----- ----- ----- */
// ChessBoardHandler.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/10/21
// Update Date: 2025/10/21
// Version: v1.0
/* ----- ----- ----- ----- */

using Engine.UI.Core.Handlers;

namespace Chinese_Chess_v3.Game.UI.Screens.Games.Boards
{
    /// <summary>
    /// 處理棋盤互動邏輯，例如滑鼠點擊選取棋子
    /// </summary>
    public class ChessBoardHandler : UIContainerHandler<ChessBoard, ChessBoardHandler, ChessBoardRenderer>
    {
        public ChessBoardHandler() { }
        public void HandleClick(int gridX, int gridY)
        {
            if (Element is ChessBoard board)
            {
                board.GameManager.HandleClick(gridX, gridY);
            }
        }

        internal override void OnUpdate()
        {
            var actions = Element._pendingActions.ToArray();
            Element._pendingActions.Clear();
            foreach (var a in actions) a();
        }
    }
}

/* ----- ----- ----- ----- */
// ChessBoardHandler.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/10/21
// Update Date: 2025/10/21
// Version: v1.0
/* ----- ----- ----- ----- */

using Chinese_Chess_v3.Game.Core;

using Engine.UI.Core.Handlers;
using Engine.UI.Core.Infrastructure;
using Engine.UI.Core.Interfaces;

namespace Chinese_Chess_v3.Game.UI.Screens.Games.Boards
{
    /// <summary>
    /// 處理棋盤互動邏輯，例如滑鼠點擊選取棋子
    /// </summary>
    public class ChessBoardHandler : UIContainerHandler<ChessBoardHandler>
    {
        public ChessBoardHandler() {}
        public void HandleClick(int gridX, int gridY)
        {
            GameManager.Instance.HandleClick(gridX, gridY);
        }
    }
}

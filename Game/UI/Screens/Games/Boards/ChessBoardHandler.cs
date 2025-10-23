/* ----- ----- ----- ----- */
// ChessBoardHandler.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/10/21
// Update Date: 2025/10/21
// Version: v1.0
/* ----- ----- ----- ----- */

using Chinese_Chess_v3.Game.Core;

using Engine.UI.Core.Base;
using Engine.UI.Core.Infrastructure;
using Engine.UI.Core.Interfaces;

namespace Chinese_Chess_v3.Game.UI.Screens.Games.Boards
{
    /// <summary>
    /// 處理棋盤互動邏輯，例如滑鼠點擊選取棋子
    /// </summary>
    public class ChessBoardHandler : InitializableOnceBase<(IUiFactory factory, ChessBoard chessBoard)>
    {
        private NavigationManager _navigationManager;
        private ChessBoard _chessBoard;
        public ChessBoardHandler() {}
        protected override void OnInit((IUiFactory factory, ChessBoard chessBoard) arg)
        {
            _chessBoard = arg.chessBoard;
            _navigationManager = arg.factory.Resolve<NavigationManager>();
        }

        public void HandleClick(int gridX, int gridY)
        {
            GameManager.Instance.HandleClick(gridX, gridY);
        }
        public void OnEnter()
        {
            //
        }
        
        public void OnExit()
        {
            //
        }
    }
}

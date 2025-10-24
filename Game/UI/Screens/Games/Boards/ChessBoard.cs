/* ----- ----- ----- ----- */
// ChessBoard.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/10/21
// Update Date: 2025/10/21
// Version: v1.0
/* ----- ----- ----- ----- */

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

using Chinese_Chess_v3.Game.Core;
using Chinese_Chess_v3.Game.Constants.UI;
using Chinese_Chess_v3.Game.UI.Binders;

using Engine.UI.Core.Base;
using Engine.UI.Core.Interfaces;
using Engine.UI.Core.Elements;

namespace Chinese_Chess_v3.Game.UI.Screens.Games.Boards
{
    /// <summary>
    /// 棋盤元件，作為 GameMenu 的子元件
    /// </summary>
    public class ChessBoard : UIContainer<ChessBoardHandler>, IScreen, IDisposable
    {
        // fields
        private bool disposed = false;
        public UIPieceBinder PieceBinder { get; private set; }
        private GameManager _gameManager = GameManager.Instance;
        public Piece SelectedPiece => _gameManager.SelectedPiece;
        
        // IUiContainer 實作

        public ChessBoard() {}
        protected override void OnInit((IUiFactory, ChessBoardHandler) arg)
        {
            base.OnInit();

            PieceBinder = new UIPieceBinder(_gameManager, this /* or boardPanel */);

            LocalPosition = UILayoutConstants.Board.Position;
            Size = UILayoutConstants.Board.Size;
        }

        protected override void OnDraw(Graphics g)
        {
            var uiPieces = PieceBinder.GetUIPieces();
            //_renderer.Draw(g, uiPieces);
        }
        
        public override bool OnMouseDown(MouseEventArgs e)
        {
            var board = GameManager.Instance.Board;

            // 1. 判斷是否在棋盤範圍
            if (!board.IsWithinBoard(e.X, e.Y))
                return false;

            // 2. Pixel -> Grid
            board.PixelToGrid(e.X, e.Y, out int gridX, out int gridY);

            _handler.HandleClick(gridX, gridY);

            return true;
        }

        // 在每個更新週期呼叫
        protected override void OnUpdate()
        {
            var actions = _pendingActions.ToArray();
            _pendingActions.Clear();
            foreach (var a in actions) a();
        }

        public override void DisposeUI()
        {
            _pendingActions.Clear();
            PieceBinder?.Dispose();
            PieceBinder = null;
        }
    }
}

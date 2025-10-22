/* ----- ----- ----- ----- */
// ChessBoard.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/10/21
// Update Date: 2025/10/21
// Version: v1.0
/* ----- ----- ----- ----- */

using System.Drawing;
using System.Windows.Forms;

using Chinese_Chess_v3.Core;
using Chinese_Chess_v3.Constants.UI;
using Chinese_Chess_v3.UI.Core.Base;
using Chinese_Chess_v3.UI.Core.Interfaces;

namespace Chinese_Chess_v3.UI.Screens.Games.Boards
{
    /// <summary>
    /// 棋盤元件，作為 GameMenu 的子元件
    /// </summary>
    public class ChessBoard
        : InitializableOnceElement<(IUiFactory factory, ChessBoardHandler handler, ChessBoardRenderer renderer)>
        , IScreen
    {
        private ChessBoardHandler _handler;
        private ChessBoardRenderer _renderer;

        public Piece SelectedPiece => GameManager.Instance.SelectedPiece;

        public ChessBoard() {}
        protected override void OnInit((IUiFactory factory, ChessBoardHandler handler, ChessBoardRenderer renderer) arg)
        {
            _handler = arg.handler;
            _renderer = arg.renderer;

            LocalPosition = UILayoutConstants.Board.Position;
            Size = UILayoutConstants.Board.Size;
        }

        protected override void OnDraw(Graphics g)
        {
            var pieces = GameManager.Instance.GetCurrentPieces();
            _renderer.Draw(g, pieces, SelectedPiece);
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

        public void OnEnter()
        {
            _handler.OnEnter();
        }

        public void OnExit()
        {
            _handler.OnExit();
        }
    }
}

/* ----- ----- ----- ----- */
// ChessBoard.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/10/21
// Update Date: 2025/10/21
// Version: v1.0
/* ----- ----- ----- ----- */

using System;
using System.Drawing;
using System.Windows.Forms;

using Chinese_Chess_v3.Game.Core;
using Chinese_Chess_v3.Game.Constants.UI;
using Chinese_Chess_v3.Game.UI.Binders;

using Engine.UI.Core.Interfaces;
using Engine.UI.Core.Elements;
using Microsoft.Extensions.DependencyInjection;

namespace Chinese_Chess_v3.Game.UI.Screens.Games.Boards
{
    /// <summary>
    /// 棋盤元件，作為 GameMenu 的子元件
    /// </summary>
    public class ChessBoard : UIContainer<ChessBoardHandler>, IScreen, IDisposable
    {
        // fields
        public UIPieceBinder PieceBinder { get; private set; }
        private GameManager _gameManager;
        public GameManager GameManager => _gameManager;
        public Piece SelectedPiece => _gameManager.SelectedPiece;
        
        // IUiContainer 實作

        public ChessBoard() {}
        protected override void OnInit((IUiFactory, ChessBoardHandler) arg)
        {
            base.OnInit(arg);

            _gameManager = _factory.ServiceProvider.GetRequiredService<GameManager>();
            PieceBinder = new UIPieceBinder(_gameManager, this /* or boardPanel */);

            LocalPosition = UILayoutConstants.Board.Position;
            Size = UILayoutConstants.Board.Size;

            _renderer = new ChessBoardRenderer(this);
            _factory.ServiceProvider.GetRequiredService<GameManager>();
        }

        protected override void OnDraw(Graphics g)
        {
            if (_renderer == null)
                throw new InvalidOperationException("Renderer not initialized for ChessBoard");

            // 將自身（ChessBoard）傳給 Renderer，讓 CompositeRenderer 依序呼叫 BoardRenderer 和 PieceRenderer
            _renderer.Render(g, this);
        }
        
        public override bool OnMouseDown(MouseEventArgs e)
        {
            var board = _gameManager.Board;

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

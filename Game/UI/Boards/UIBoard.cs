/* ----- ----- ----- ----- */
// UIBoard.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/10/21
// Update Date: 2025/10/21
// Version: v1.0
/* ----- ----- ----- ----- */

using System;
using System.Windows.Forms;

using Chinese_Chess_v3.Game.Core;
using Chinese_Chess_v3.Game.Core.Pieces;
using Chinese_Chess_v3.Game.UI.Constants;
using Chinese_Chess_v3.Game.UI.Binders;

using Engine.UI.Core.Interfaces;
using Engine.UI.Core.Elements;

using Microsoft.Extensions.DependencyInjection;

namespace Chinese_Chess_v3.Game.UI.Boards
{
    /// <summary>
    /// 棋盤元件，作為 GameMenu 的子元件
    /// </summary>
    public class UIBoard : UIContainer<UIBoard, UIBoardHandler, UIBoardRenderer>, IResettable
    {
        // fields
        public UIPieceBinder PieceBinder { get; private set; }
        private GameManager _gameManager;
        public GameManager GameManager => _gameManager;
        public Piece SelectedPiece => _gameManager.SelectedPiece;
        
        // IUiContainer 實作

        public UIBoard() { }
        protected override void OnInit(IUiFactory factory)
        {
            _gameManager = _factory.ServiceProvider.GetRequiredService<GameManager>();
            PieceBinder = new UIPieceBinder(_gameManager, this /* or boardPanel */);

            LocalPosition = UILayoutConstants.Board.Position;
            Size = UILayoutConstants.Board.Size;
        }
        
        public override bool OnMouseDown(MouseEventArgs e)
        {
            var board = _gameManager.Board;

            // 1. 判斷是否在棋盤範圍
            if (!board.IsWithinBoard(e.X, e.Y))
                return false;

            // 2. Pixel -> Grid
            board.PixelToGrid(e.X, e.Y, out int gridX, out int gridY);

            Handler.HandleClick(gridX, gridY);

            return true;
        }


        protected override void DisposeUI()
        {
            _pendingActions.Clear();
            PieceBinder?.Dispose();
            PieceBinder = null;
        }

        protected override void OnReset()
        {
            GameManager.ResetBoardToDefault();
        }
    }
}

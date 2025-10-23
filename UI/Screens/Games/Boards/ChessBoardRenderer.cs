/* ----- ----- ----- ----- */
// ChessBoardRenderer.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/10/21
// Update Date: 2025/10/21
// Version: v1.0
/* ----- ----- ----- ----- */

using System.Collections.Generic;
using System.Drawing;

using Chinese_Chess_v3.Core;
using Chinese_Chess_v3.UI.Elements;

namespace Chinese_Chess_v3.UI.Screens.Games.Boards
{
    /// <summary>
    /// 負責繪製棋盤與棋子
    /// </summary>
    public class ChessBoardRenderer
    {
        private readonly BoardRenderer _boardRenderer = new();
        private readonly PieceRenderer _pieceRenderer = new();

        /// <summary>
        /// 繪製棋盤與棋子
        /// </summary>
        /// <param name="g">Graphics物件</param>
        /// <param name="pieces">棋子列表</param>
        /// <param name="selectedPiece">目前選取的棋子（可為null）</param>
        public void Draw(Graphics g, List<UIPiece> uiPieces)
        {
            // 畫棋盤
            _boardRenderer.DrawBoard(g);

            // 畫棋子
            _pieceRenderer.DrawPieces(g, uiPieces);
        }
    }
}

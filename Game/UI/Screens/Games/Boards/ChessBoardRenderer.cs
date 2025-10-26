/* ----- ----- ----- ----- */
// ChessBoardRenderer.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/10/21
// Update Date: 2025/10/25
// Version: v1.1
/* ----- ----- ----- ----- */

using System.Drawing;

using Engine.UI.Core.Elements;
using Engine.UI.Core.Renderers;

namespace Chinese_Chess_v3.Game.UI.Screens.Games.Boards
{
    /// <summary>
    /// 負責繪製棋盤與棋子
    /// </summary>
    public class ChessBoardRenderer : UIContainerRenderer<ChessBoardHandler>
    {
        private readonly CompositeRenderer _composite = new CompositeRenderer();

        public ChessBoardRenderer(ChessBoard container) : base(container)
        {
            // 將原本的 BoardRenderer 與 PieceRenderer 封裝成單獨 UIRenderer
            _composite
                .Add(new BoardRenderer())   // 畫棋盤格子
                .Add(new PieceRenderer());  // 畫棋子
        }

        public override void Render(Graphics g, UIElement element)
        {
            _composite.Render(g, element);
        }
    }
}

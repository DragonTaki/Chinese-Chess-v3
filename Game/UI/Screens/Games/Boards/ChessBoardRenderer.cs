/* ----- ----- ----- ----- */
// ChessBoardRenderer.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/10/21
// Update Date: 2025/10/25
// Version: v1.1
/* ----- ----- ----- ----- */

using System.Drawing;

using Engine.UI.Core.Renderers;

namespace Chinese_Chess_v3.Game.UI.Screens.Games.Boards
{
    /// <summary>
    /// 負責繪製棋盤與棋子
    /// </summary>
    public class ChessBoardRenderer : UIContainerRenderer<ChessBoard, ChessBoardHandler, ChessBoardRenderer>
    {
        protected CompositeRenderer<ChessBoard, ChessBoardHandler, ChessBoardRenderer> _composite = new();

        public ChessBoardRenderer() { }
        
        protected override void AfterInit()
        {
            SetupRendererChildren();
        }

        private void SetupRendererChildren()
        {
            if (_composite.ListCount == 0)
            {
                _composite
                    .Add(new BoardRenderer())
                    .Add(new PieceRenderer());
            }
        }

        public override void OnRender(Graphics g, ChessBoard element)
        {
            _composite.Render(g, element);
        }
    }
}

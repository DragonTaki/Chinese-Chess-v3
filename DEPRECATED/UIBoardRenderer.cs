/* ----- ----- ----- ----- */
// UIBoardRenderer.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/10/21
// Update Date: 2025/10/25
// Version: v1.1
/* ----- ----- ----- ----- */

using System.Drawing;

using Chinese_Chess_v3.Game.UI.Boards.Pieces;

using Engine.UI.Core.Renderers;

namespace Chinese_Chess_v3.Game.UI.Boards
{
    /// <summary>
    /// 負責繪製棋盤與棋子
    /// </summary>
    public class UIBoardRenderer : UIContainerRenderer<UIBoard, UIBoardHandler, UIBoardRenderer>
    {
        protected CompositeRenderer<UIBoard, UIBoardHandler, UIBoardRenderer> _composite = new();

        public UIBoardRenderer() { }
        
        protected override void AfterInit()
        {
            SetupRendererChildren();
        }

        private void SetupRendererChildren()
        {
            if (_composite.ListCount == 0)
            {
                _composite
                    .Add(new UIBoardRenderer())
                    .Add(new UIPieceRenderer());
            }
        }

        public override void OnRender(Graphics g, UIBoard element)
        {
            _composite.Render(g, element);
        }
    }
}

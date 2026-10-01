/* ----- ----- ----- ----- */
// UIBoardHandler.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/10/21
// Update Date: 2025/10/21
// Version: v1.0
/* ----- ----- ----- ----- */

using Engine.UI.Core.Handlers;

namespace Chinese_Chess_v3.Game.UI.Boards
{
    /// <summary>
    /// Handles board interaction logic, e.g. a mouse click selecting a piece
    /// </summary>
    public class UIBoardHandler : UIContainerHandler<UIBoard, UIBoardHandler, UIBoardRenderer>
    {
        public UIBoardHandler() { }
        public void HandleClick(int gridX, int gridY)
        {
            if (Element is UIBoard board)
            {
                board.GameManager.HandleClick(gridX, gridY);
            }
        }

        internal override void OnUpdate()
        {
            var actions = Element.PendingActions.ToArray();
            Element.PendingActions.Clear();
            foreach (var a in actions) a();

            // Without a per-frame tick the clocks only advanced inside EndStep(),
            // i.e. the displayed time only changed when a move was made.
            Element.GameManager?.UpdateTimers();
        }
    }
}

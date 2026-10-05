/* ----- ----- ----- ----- */
// UIBoardHandler.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/10/21
// Update Date: 2026/10/05
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
                board.ViewModel.HandleClick(gridX, gridY);
            }
        }

        internal override void OnUpdate()
        {
            // A new game on another board type takes that board's layout.
            Element.ApplyBoardLayout();

            var actions = Element.PendingActions.ToArray();
            Element.PendingActions.Clear();
            foreach (var a in actions) a();

            // The clocks are advanced by GameSession.Tick from the frame loop, not here.
        }
    }
}

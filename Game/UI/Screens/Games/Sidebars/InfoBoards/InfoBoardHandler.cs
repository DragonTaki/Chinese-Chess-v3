/* ----- ----- ----- ----- */
// InfoBoardHandler.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/10/22
// Update Date: 2025/10/22
// Version: v1.0
/* ----- ----- ----- ----- */

using Chinese_Chess_v3.Game.Core;
using Chinese_Chess_v3.Game.Models;

using Engine.UI.Core.Handlers;

namespace Chinese_Chess_v3.Game.UI.Screens.Games.Sidebars.InfoBoards
{
    /// <summary>
    /// Handles the data and logic of the InfoBoard.
    /// </summary>
    public class InfoBoardHandler : UIContainerHandler<InfoBoard, InfoBoardHandler, InfoBoardRenderer>
    {

        public InfoBoardHandler() { }
        public void SetGameManager(GameManager gameManager)
        {
            Element.GameManager = gameManager;
        }

        public void SwitchTurn()
        {
            Element.CurrentTurn = Element.CurrentTurn == PlayerSide.Black ? PlayerSide.Red : PlayerSide.Black;
        }
    }
}

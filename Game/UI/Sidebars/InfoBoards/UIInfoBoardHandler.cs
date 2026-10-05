/* ----- ----- ----- ----- */
// UIInfoBoardHandler.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/10/22
// Update Date: 2026/10/05
// Version: v1.2
/* ----- ----- ----- ----- */

using Chinese_Chess_v3.Game.Application.InfoBoards;

using Engine.UI.Core.Handlers;

namespace Chinese_Chess_v3.Game.UI.Sidebars.InfoBoards
{
    /// <summary>
    /// Handles the data and logic of the InfoBoard.
    /// </summary>
    public class UIInfoBoardHandler : UIContainerHandler<UIInfoBoard, UIInfoBoardHandler, UIInfoBoardRenderer>
    {
        public UIInfoBoardHandler() { }
        /// <summary>Sets what the board shows (<see cref="UIInfoBoard.ViewModel"/>).</summary>
        public void SetViewModel(InfoBoardViewModel viewModel)
        {
            Element.ViewModel = viewModel;
        }
    }
}

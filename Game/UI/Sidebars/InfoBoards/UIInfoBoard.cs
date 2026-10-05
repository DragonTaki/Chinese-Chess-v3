/* ----- ----- ----- ----- */
// UIInfoBoard.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/07
// Update Date: 2026/10/05
// Version: v2.3
/* ----- ----- ----- ----- */

using Chinese_Chess_v3.Game.Application.InfoBoards;
using Chinese_Chess_v3.Game.UI.Constants;

using Engine.UI.Core.Elements;
using Engine.UI.Core.Interfaces;

namespace Chinese_Chess_v3.Game.UI.Sidebars.InfoBoards
{
    public class UIInfoBoard : UIContainer<UIInfoBoard, UIInfoBoardHandler, UIInfoBoardRenderer>, IResettable
    {
        /// <summary>
        /// What the board shows (names, sides, turn, colours, clock texts); set by
        /// <see cref="UIInfoBoardHandler.SetViewModel"/>.
        /// </summary>
        public InfoBoardViewModel ViewModel { get; internal set; }

        public UIInfoBoard() { }

        protected override void OnInit(IUiFactory factory)
        {
            Layout = UILayoutConstants.Sidebar.InfoBoard.Layout;

            // Flex item of the sidebar column (see UILayoutSheet.GameScreen.InfoBoard).
            LayoutRules.Apply(UILayoutSheet.GameScreen.InfoBoard);
        }
    }
}

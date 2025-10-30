/* ----- ----- ----- ----- */
// UIInfoBoard.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/07
// Update Date: 2025/10/22
// Version: v2.0
/* ----- ----- ----- ----- */

using Chinese_Chess_v3.Game.UI.Constants;
using Chinese_Chess_v3.Game.Core;

using Engine.UI.Core.Elements;
using Engine.UI.Core.Interfaces;

namespace Chinese_Chess_v3.Game.UI.Sidebars.InfoBoards
{
    public class UIInfoBoard : UIContainer<UIInfoBoard, UIInfoBoardHandler, UIInfoBoardRenderer>, IResettable
    {
        public string BlackPlayerName { get; set; } = "黑方玩家";
        public string RedPlayerName { get; set; } = "紅方玩家";
        public GameManager GameManager;

        public UIInfoBoard() { }

        protected override void OnInit(IUiFactory factory)
        {
            Layout = UILayoutConstants.Sidebar.Infoboard.Layout;
        }
    }
}

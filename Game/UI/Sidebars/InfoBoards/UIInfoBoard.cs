/* ----- ----- ----- ----- */
// UIInfoBoard.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/07
// Update Date: 2026/09/30
// Version: v2.1
/* ----- ----- ----- ----- */

using Chinese_Chess_v3.Game.UI.Constants;
using Chinese_Chess_v3.Game.Core;

using Engine.UI.Constants.Core;
using Engine.UI.Core.Elements;
using Engine.UI.Core.Interfaces;
using Engine.UI.Models;

namespace Chinese_Chess_v3.Game.UI.Sidebars.InfoBoards
{
    public class UIInfoBoard : UIContainer<UIInfoBoard, UIInfoBoardHandler, UIInfoBoardRenderer>, IResettable
    {
        public string Player2Name { get; set; } = "黑方玩家";
        public string Player1Name { get; set; } = "紅方玩家";
        public GameManager GameManager;

        public UIInfoBoard() { }

        protected override void OnInit(IUiFactory factory)
        {
            Layout = UILayoutConstants.Sidebar.Infoboard.Layout;

            // Flex item of the sidebar column: full width, fixed height.
            LayoutRules.PositionMode = PositionMode.Flow;
            LayoutRules.Width = LayoutSize.Stretch;
            LayoutRules.Height = LayoutSize.Fixed(UILayoutConstants.Sidebar.Infoboard.Size.Y);
            LayoutRules.FlexShrink = 0f;
        }
    }
}

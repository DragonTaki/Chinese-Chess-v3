/* ----- ----- ----- ----- */
// UIInfoBoard.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/07
// Update Date: 2026/10/14
// Version: v2.4
/* ----- ----- ----- ----- */

using Chinese_Chess_v3.Game.Application.InfoBoards;
using Chinese_Chess_v3.Game.Core.Players;
using Chinese_Chess_v3.Game.UI.Constants;

using Engine.UI.Core.Elements;
using Engine.UI.Core.Interfaces;
using Engine.UI.Core.Handlers;
using Engine.UI.Core.Renderers;

namespace Chinese_Chess_v3.Game.UI.Sidebars.InfoBoards
{
    public class UIInfoBoard : UIContainer<UIInfoBoard, UIInfoBoardHandler, UIInfoBoardRenderer>, IResettable
    {
        /// <summary>
        /// What the board shows (names, sides, turn, colours, clock texts); set by
        /// <see cref="UIInfoBoardHandler.SetViewModel"/>.
        /// </summary>
        public InfoBoardViewModel ViewModel { get; internal set; }

        /// <summary>The player shown in this block.</summary>
        public PlayerSide Side { get; internal set; }
        internal UILabel Avatar { get; private set; }
        internal UILabel NameSlot { get; private set; }
        internal UILabel TotalSlot { get; private set; }
        internal UILabel StepSlot { get; private set; }
        internal UILabel StatusSlot { get; private set; }

        public UIInfoBoard() { }

        protected override void BuildUIObjects()
        {
            Avatar = CreateSlot();
            NameSlot = CreateSlot();
            TotalSlot = CreateSlot();
            StepSlot = CreateSlot();
            StatusSlot = CreateSlot();
        }

        private UILabel CreateSlot()
        {
            var slot = _factory.CreateElement<UILabel, UILabelHandler, UILabelRenderer>();
            slot.IsVisible = false;
            AddChild(slot);
            return slot;
        }

        internal void ArrangeSlots()
        {
            float width = Size.X;
            float inset = UILayoutConstants.Sidebar.InfoBoard.Inset;
            float left = width * UILayoutConstants.Sidebar.InfoBoard.TextLeft;
            float textWidth = width * UILayoutConstants.Sidebar.InfoBoard.TextWidth;
            float nameHeight = UILayoutConstants.Sidebar.InfoBoard.NameHeight;
            float clockHeight = UILayoutConstants.Sidebar.InfoBoard.ClockHeight;
            Avatar.LayoutRules.Apply(UILayoutSheet.GameScreen.PlayerSlot(inset,
                Size.Y * UILayoutConstants.Sidebar.InfoBoard.AvatarTop,
                width * UILayoutConstants.Sidebar.InfoBoard.AvatarWidth,
                Size.Y * UILayoutConstants.Sidebar.InfoBoard.AvatarHeight));
            NameSlot.LayoutRules.Apply(UILayoutSheet.GameScreen.PlayerSlot(left, inset, textWidth, nameHeight));
            TotalSlot.LayoutRules.Apply(UILayoutSheet.GameScreen.PlayerSlot(left, inset + nameHeight, textWidth, clockHeight));
            StepSlot.LayoutRules.Apply(UILayoutSheet.GameScreen.PlayerSlot(left, inset + nameHeight + clockHeight, textWidth, clockHeight));
            StatusSlot.LayoutRules.Apply(UILayoutSheet.GameScreen.PlayerSlot(inset,
                UILayoutConstants.Sidebar.InfoBoard.BlockHeight, width - inset - inset,
                UILayoutConstants.Sidebar.InfoBoard.StatusHeight));
            PerformLayout();
        }

        protected override void OnInit(IUiFactory factory)
        {
            Layout = UILayoutConstants.Sidebar.InfoBoard.Layout;

            // Flex item of the sidebar column (see UILayoutSheet.GameScreen.PlayerBlock).
            LayoutRules.Apply(UILayoutSheet.GameScreen.PlayerBlock);
        }
    }
}

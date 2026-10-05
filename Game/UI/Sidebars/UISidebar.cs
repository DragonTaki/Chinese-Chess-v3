/* ----- ----- ----- ----- */
// UISidebar.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/10/22
// Update Date: 2026/10/05
// Version: v1.3
/* ----- ----- ----- ----- */

using System;

using Chinese_Chess_v3.Game.Application.GameLog;
using Chinese_Chess_v3.Game.Application.InfoBoards;
using Chinese_Chess_v3.Game.Core;
using Chinese_Chess_v3.Game.Core.Players;
using Chinese_Chess_v3.Game.UI.Constants;
using Chinese_Chess_v3.Game.UI.Sidebars.InfoBoards;
using Chinese_Chess_v3.Game.UI.Sidebars.LoggerBoxes;

using Engine.UI.Core.Elements;
using Engine.UI.Core.Interfaces;

using Microsoft.Extensions.DependencyInjection;

namespace Chinese_Chess_v3.Game.UI.Sidebars
{
    /// <summary>
    /// The sidebar (right column of the game screen): a container that holds the info
    /// board (<see cref="UIInfoBoard"/>) and the logger box (<see cref="UILoggerBox"/>),
    /// and stores the current turn and whether to highlight it.
    /// </summary>
    public class UISidebar : UIContainer<UISidebar, UISidebarHandler, UISidebarRenderer>, IResettable
    {
        internal UIInfoBoard InfoBoard { get; private set; }
        internal UILoggerBox LoggerBox { get; private set; }

        /// <summary>
        /// Indicates which side's turn it currently is.
        /// <see cref="PlayerSide.Player1"/> (moves first) or <see cref="PlayerSide.Player2"/>; the
        /// colour each plays is <see cref="GameManager.ColorOf"/>.
        /// </summary>
        public PlayerSide CurrentTurn { get; set; } = PlayerSide.Player1;

        /// <summary>
        /// Indicates whether the sidebar should visually highlight the current turn.
        /// </summary>
        public bool HighlightTurn { get; set; } = true;

        /// <summary>
        /// Creates a new Sidebar; its children (info board, logger box) are created in
        /// <c>BuildUIObjects</c>.
        /// </summary>
        public UISidebar() { }

        protected override void OnInit(IUiFactory factory)
        {
            // Declared size (pre-layout fallback), then the layout rules.
            Layout = UILayoutConstants.Sidebar.Layout;

            // Right column of the game screen: info board on top, logger box at the bottom
            // (see UILayoutSheet.GameScreen.Sidebar).
            LayoutRules.Apply(UILayoutSheet.GameScreen.Sidebar);
        }

        protected override void BuildUIObjects()
        {
            if (InfoBoard == null)
                InfoBoard = _factory.CreateDIElement<UIInfoBoard, UIInfoBoardHandler, UIInfoBoardRenderer>();
            if (!Children.Contains(InfoBoard))
                AddChild(InfoBoard);

            if (LoggerBox == null)
                LoggerBox = _factory.CreateDIElement<UILoggerBox, UILoggerBoxHandler, UILoggerBoxRenderer>();
            if (!Children.Contains(LoggerBox))
                AddChild(LoggerBox);

            _factory.ServiceProvider.GetRequiredService<GameLogComposer>().SetLog(LoggerBox.Handler);
            InfoBoard.Handler.SetViewModel(_factory.ServiceProvider.GetRequiredService<InfoBoardViewModel>());
        }

        public void ResetGameUI() => BuildUIObjects();
    }
}

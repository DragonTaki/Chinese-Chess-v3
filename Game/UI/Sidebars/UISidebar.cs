/* ----- ----- ----- ----- */
// UISidebar.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/10/22
// Update Date: 2026/10/14
// Version: v1.6
/* ----- ----- ----- ----- */

using System;
using System.Collections.Generic;

using Chinese_Chess_v3.Game.Application.GameLog;
using Chinese_Chess_v3.Game.Application.InfoBoards;
using Chinese_Chess_v3.Game.Core;
using Chinese_Chess_v3.Game.Core.Players;
using Chinese_Chess_v3.Game.UI.Constants;
using Chinese_Chess_v3.Game.UI.Sidebars.GameControls;
using Chinese_Chess_v3.Game.UI.Sidebars.InfoBoards;
using Chinese_Chess_v3.Game.UI.Sidebars.LoggerBoxes;

using Engine.UI.Constants.Core;
using Engine.UI.Models;
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
        private readonly List<UIInfoBoard> _playerBlocks = new();

        internal UIInfoBoard InfoBoard { get; private set; }
        internal UIGameControls GameControls { get; private set; }
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

            // Right column of the game screen: opponents, logger box, then the human side
            // (see UILayoutSheet.GameScreen.Sidebar).
            LayoutRules.Apply(UILayoutSheet.GameScreen.Sidebar);
        }

        protected override void BuildUIObjects()
        {
            if (InfoBoard == null)
                InfoBoard = _factory.CreateDIElement<UIInfoBoard, UIInfoBoardHandler, UIInfoBoardRenderer>();
            if (!Children.Contains(InfoBoard))
                AddChild(InfoBoard);

            // Retained legacy controls; the active controls belong to the left menu.
            if (GameControls == null)
                GameControls = _factory.CreateDIElement<UIGameControls, UIGameControlsHandler, UIGameControlsRenderer>();
            GameControls.LayoutRules.Display = DisplayMode.None;

            if (LoggerBox == null)
                LoggerBox = _factory.CreateDIElement<UILoggerBox, UILoggerBoxHandler, UILoggerBoxRenderer>();
            if (!Children.Contains(LoggerBox))
                AddChild(LoggerBox);

            _factory.ServiceProvider.GetRequiredService<GameLogComposer>().SetLog(LoggerBox.Handler);
            var viewModel = _factory.ServiceProvider.GetRequiredService<InfoBoardViewModel>();
            if (_playerBlocks.Count == 0)
            {
                _playerBlocks.Add(InfoBoard);
                for (int i = 1; i < 3; i++)
                {
                    var block = _factory.CreateDIElement<UIInfoBoard, UIInfoBoardHandler, UIInfoBoardRenderer>();
                    AddChild(block);
                    _playerBlocks.Add(block);
                }
            }
            foreach (var block in _playerBlocks)
                block.Handler.SetViewModel(viewModel);
            ArrangePlayerBlocks();
        }

        internal void ArrangePlayerBlocks()
        {
            if (_playerBlocks.Count == 0)
                return;
            var viewModel = InfoBoard.ViewModel;
            var top = viewModel.TopSides;
            for (int i = 0; i < _playerBlocks.Count; i++)
            {
                var block = _playerBlocks[i];
                PlayerSide? side = i < top.Count ? top[i] : i == top.Count ? viewModel.OwnSide : null;
                block.LayoutRules.Display = side.HasValue ? DisplayMode.Normal : DisplayMode.None;
                if (!side.HasValue)
                    continue;
                block.Side = side.Value;
                block.LayoutRules.Order = i < top.Count ? i : _playerBlocks.Count + 2;
                block.LayoutRules.Height = LayoutSize.Fixed(UILayoutConstants.Sidebar.InfoBoard.BlockHeight
                    + (viewModel.IsThreeKingdoms ? UILayoutConstants.Sidebar.InfoBoard.StatusHeight : 0f));
            }

            LoggerBox.LayoutRules.Order = _playerBlocks.Count + 1;
            PerformLayout();
        }

        public void ResetGameUI() => BuildUIObjects();
    }
}

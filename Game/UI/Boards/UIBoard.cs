/* ----- ----- ----- ----- */
// UIBoard.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/10/21
// Update Date: 2026/09/30
// Version: v1.1
/* ----- ----- ----- ----- */

using System;
using Engine.Platform;

using Chinese_Chess_v3.Game.Core;
using Chinese_Chess_v3.Game.Core.Pieces;
using Chinese_Chess_v3.Game.UI.Constants;
using Chinese_Chess_v3.Game.UI.Binders;

using Engine.Mathematics;
using Engine.UI.Constants.Core;
using Engine.UI.Core.Interfaces;
using Engine.UI.Core.Elements;
using Engine.UI.Models;

using Microsoft.Extensions.DependencyInjection;

namespace Chinese_Chess_v3.Game.UI.Boards
{
    /// <summary>
    /// 棋盤元件，作為 GameMenu 的子元件
    /// </summary>
    public class UIBoard : UIContainer<UIBoard, UIBoardHandler, UIBoardRenderer>, IResettable
    {
        // fields
        public UIPieceBinder PieceBinder { get; private set; }
        private GameManager _gameManager;
        public GameManager GameManager => _gameManager;
        public Piece SelectedPiece => _gameManager.SelectedPiece;
        
        // IUiContainer 實作

        public UIBoard() { }
        protected override void OnInit(IUiFactory factory)
        {
            _gameManager = _factory.ServiceProvider.GetRequiredService<GameManager>();
            PieceBinder = new UIPieceBinder(_gameManager, this /* or boardPanel */);

            // Declared size (pre-layout fallback), then the layout rules.
            LocalPosition = UILayoutConstants.Board.Position;
            Size = UILayoutConstants.Board.Size;

            // The middle area of the screen-sized game menu (between the menu column and
            // the sidebar), full height, with the board kept at its authored aspect ratio,
            // centered in that area.
            var rules = LayoutRules;
            rules.PositionMode = PositionMode.Absolute;
            rules.Left = UILayoutConstants.GameMenu.Size.X;
            rules.Right = UILayoutConstants.Sidebar.Size.X;
            rules.Top = UILayoutConstants.Board.Position.Y;
            rules.Bottom = 0f;
            rules.Width = LayoutSize.Stretch;
            rules.Height = LayoutSize.Stretch;
            rules.AspectRatio = UILayoutConstants.Board.Size.X / UILayoutConstants.Board.Size.Y;
            rules.AspectFit = AspectFit.Contain;
            rules.AlignX = Alignment.Center;
            rules.AlignY = Alignment.Center;
        }

        #region Grid Geometry

        // Drawing and hit testing derive the grid from the board's resolved (absolute)
        // rectangle instead of UILayoutConstants.Board.Grid, so pieces and clicks stay
        // aligned with the drawn grid whenever the layout resizes or moves the board.

        /// <summary>
        /// Resolved size relative to the authored <c>UILayoutConstants.Board.Size</c>
        /// (1 at the authored size; the aspect ratio is locked, so one factor fits both axes).
        /// </summary>
        public float GridScale
        {
            get
            {
                var authored = UILayoutConstants.Board.Size;
                if (authored.X <= 0f || authored.Y <= 0f || Size.X <= 0f || Size.Y <= 0f)
                    return 1f;
                return System.Math.Min(Size.X / authored.X, Size.Y / authored.Y);
            }
        }

        /// <summary>Distance between grid lines, in design units.</summary>
        public float GridCellSize => UILayoutConstants.Board.Grid.CellSize * GridScale;

        /// <summary>
        /// Absolute position of grid point (0, 0): the grid area centered inside the board,
        /// as <c>UILayoutConstants.Board.Grid.Position</c> does for the authored layout.
        /// </summary>
        public Vector2F GridOrigin
        {
            get
            {
                var position = GetCurrentAbsolutePosition();
                var area = UILayoutConstants.Board.Grid.GridAreaSize * GridScale;
                return new Vector2F(
                    position.X + (Size.X - area.X) / 2f,
                    position.Y + (Size.Y - area.Y) / 2f);
            }
        }

        /// <summary>Absolute position of grid point (<paramref name="x"/>, <paramref name="y"/>).</summary>
        public Vector2F GridToPixel(float x, float y)
        {
            var origin = GridOrigin;
            float cell = GridCellSize;
            return new Vector2F(origin.X + x * cell, origin.Y + y * cell);
        }

        /// <summary>
        /// Converts an absolute point to the nearest grid point. Same rules as
        /// <c>Board.IsWithinBoard</c>/<c>PixelToGrid</c> (which use the authored constants):
        /// inside when within Columns x Rows cells from the grid origin, rounded to the
        /// nearest intersection and clamped to the board.
        /// </summary>
        /// <returns>False when the point is outside the board.</returns>
        public bool TryPixelToGrid(float pixelX, float pixelY, out int gridX, out int gridY)
        {
            var board = _gameManager.Board;
            var origin = GridOrigin;
            float cell = GridCellSize;
            gridX = gridY = 0;

            if (cell <= 0f
                || pixelX < origin.X || pixelX > origin.X + board.Columns * cell
                || pixelY < origin.Y || pixelY > origin.Y + board.Rows * cell)
                return false;

            gridX = Math.Clamp((int)((pixelX - origin.X) / cell + 0.5f), 0, board.Columns - 1);
            gridY = Math.Clamp((int)((pixelY - origin.Y) / cell + 0.5f), 0, board.Rows - 1);
            return true;
        }

        #endregion

        public override bool OnMouseDown(IMouseEvent e)
        {
            // Pixel -> Grid, from the resolved board rectangle (see Grid Geometry).
            if (!TryPixelToGrid(e.X, e.Y, out int gridX, out int gridY))
                return false;

            Handler.HandleClick(gridX, gridY);

            return true;
        }


        protected override void DisposeUI()
        {
            _pendingActions.Clear();
            PieceBinder?.Dispose();
            PieceBinder = null;
        }

        protected override void OnReset()
        {
            GameManager.ResetBoardToDefault();
        }
    }
}

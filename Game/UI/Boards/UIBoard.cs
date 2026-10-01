/* ----- ----- ----- ----- */
// UIBoard.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/10/21
// Update Date: 2026/10/01
// Version: v1.2
/* ----- ----- ----- ----- */

using System;
using Engine.Platform;

using Chinese_Chess_v3.Game.Configs;
using Chinese_Chess_v3.Game.Core;
using Chinese_Chess_v3.Game.Core.Boards;
using Chinese_Chess_v3.Game.Core.Pieces;
using Chinese_Chess_v3.Game.UI.Constants;
using Chinese_Chess_v3.Game.UI.Binders;

using Engine.Mathematics;
using Engine.UI.Core.Interfaces;
using Engine.UI.Core.Elements;

using Microsoft.Extensions.DependencyInjection;

namespace Chinese_Chess_v3.Game.UI.Boards
{
    /// <summary>
    /// The board component (棋盤), a child component of GameMenu
    /// </summary>
    public class UIBoard : UIContainer<UIBoard, UIBoardHandler, UIBoardRenderer>, IResettable
    {
        // fields
        public UIPieceBinder PieceBinder { get; private set; }
        private GameManager _gameManager;
        public GameManager GameManager => _gameManager;
        public Piece SelectedPiece => _gameManager.SelectedPiece;

        /// <summary>The player settings (board hints); the code defaults when none are registered.</summary>
        public PlayerSettings PlayerSettings { get; private set; } = PlayerSettings.Defaults;

        /// <summary>The board type the layout rules were last applied for (see <see cref="ApplyBoardLayout"/>).</summary>
        private BoardType? _layoutBoardType;

        /// <summary>The type of the board being played (<c>GameManager.Board</c> is replaced when a game changes it).</summary>
        public BoardType BoardType => _gameManager?.Board.Type ?? BoardType.Full;
        
        // IUiContainer implementation

        public UIBoard() { }
        protected override void OnInit(IUiFactory factory)
        {
            _gameManager = _factory.ServiceProvider.GetRequiredService<GameManager>();
            PlayerSettings = _factory.ServiceProvider.GetService<PlayerSettings>() ?? PlayerSettings.Defaults;
            PieceBinder = new UIPieceBinder(_gameManager, this /* or boardPanel */);

            // Declared size (pre-layout fallback), then the layout rules.
            LocalPosition = UILayoutConstants.Board.Position;
            Size = UILayoutConstants.Board.Size;

            // The middle column of the game screen, at the board type's authored aspect ratio
            // (see UILayoutSheet.GameScreen.Board / HalfCenterBoard).
            ApplyBoardLayout();
        }

        /// <summary>
        /// Applies the layout rules of the board type being played
        /// (<see cref="UILayoutSheet.GameScreen.BoardFor"/>) when it differs from the one last
        /// applied: a new game on another board type gets that board's aspect ratio. Called
        /// once per frame by the handler (a cheap comparison when nothing changed).
        /// </summary>
        internal void ApplyBoardLayout()
        {
            var type = BoardType;
            if (_layoutBoardType == type)
                return;
            LayoutRules.Apply(UILayoutSheet.GameScreen.BoardFor(type));
            _layoutBoardType = type;
        }

        #region Grid Geometry

        // Drawing and hit testing derive the grid from the board's resolved (absolute)
        // rectangle instead of UILayoutConstants.Board.Grid, so pieces and clicks stay
        // aligned with the drawn grid whenever the layout resizes or moves the board.
        // Per board type: Full puts pieces on the line crossings of a 9x10 grid, HalfCenter
        // in the cells of an 8x4 grid (UILayoutConstants.Board.HalfCenter).

        /// <summary>The authored size of the board element for <see cref="BoardType"/>.</summary>
        /// <exception cref="NotSupportedException">The board type has no board drawing yet (HalfCross).</exception>
        public Vector2F AuthoredSize => BoardType switch
        {
            BoardType.Full => UILayoutConstants.Board.Size,
            BoardType.HalfCenter => UILayoutConstants.Board.HalfCenter.Size,
            _ => throw new NotSupportedException($"No board drawing for {BoardType} yet"),
        };

        /// <summary>
        /// Resolved size relative to the board type's authored size (<see cref="AuthoredSize"/>;
        /// 1 at the authored size; the aspect ratio is locked, so one factor fits both axes).
        /// </summary>
        public float GridScale
        {
            get
            {
                var authored = AuthoredSize;
                if (authored.X <= 0f || authored.Y <= 0f || Size.X <= 0f || Size.Y <= 0f)
                    return 1f;
                return System.Math.Min(Size.X / authored.X, Size.Y / authored.Y);
            }
        }

        /// <summary>
        /// Scale for the board's details (line width, piece radius and font, marks, frame):
        /// <see cref="GridScale"/> rounded to <c>UILayoutConstants.Board.DetailScaleStep</c>,
        /// so it is exactly 1 at (and within half a step of) the authored size.
        /// </summary>
        public float DetailScale
        {
            get
            {
                float step = UILayoutConstants.Board.DetailScaleStep;
                float scale = GridScale;
                if (step <= 0f)
                    return scale;
                // Steps per unit, so the authored size gives exactly 1 (100 / 100, not 100 * 0.01f).
                float stepsPerUnit = System.MathF.Round(1f / step);
                float rounded = System.MathF.Round(scale * stepsPerUnit) / stepsPerUnit;
                return rounded > 0f ? rounded : 1f / stepsPerUnit;
            }
        }

        /// <summary>Distance between grid lines (Full) or width of a cell (HalfCenter), in design units.</summary>
        public float GridCellSize => BoardType switch
        {
            BoardType.Full => UILayoutConstants.Board.Grid.CellSize,
            BoardType.HalfCenter => UILayoutConstants.Board.HalfCenter.Grid.CellSize,
            _ => throw new NotSupportedException($"No board drawing for {BoardType} yet"),
        } * GridScale;

        /// <summary>
        /// Absolute position of the grid's top-left corner (Full: grid point (0, 0); HalfCenter:
        /// the top-left corner of cell (0, 0)): the grid area centered inside the board, as
        /// <c>UILayoutConstants.Board.Grid.Position</c> does for the authored Full layout.
        /// </summary>
        public Vector2F GridOrigin
        {
            get
            {
                var position = GetCurrentAbsolutePosition();
                var area = BoardType switch
                {
                    BoardType.Full => UILayoutConstants.Board.Grid.GridAreaSize,
                    BoardType.HalfCenter => UILayoutConstants.Board.HalfCenter.Grid.GridAreaSize,
                    _ => throw new NotSupportedException($"No board drawing for {BoardType} yet"),
                } * GridScale;
                return new Vector2F(
                    position.X + (Size.X - area.X) / 2f,
                    position.Y + (Size.Y - area.Y) / 2f);
            }
        }

        /// <summary>
        /// Absolute position where a piece on square (<paramref name="x"/>, <paramref name="y"/>)
        /// is drawn: the grid point on the Full board, the cell's centre on HalfCenter.
        /// </summary>
        public Vector2F GridToPixel(float x, float y)
        {
            var origin = GridOrigin;
            float cell = GridCellSize;
            // HalfCenter: pieces stand in the cells, half a cell in from the cell's corner.
            float inset = BoardType == BoardType.HalfCenter ? 0.5f : 0f;
            return new Vector2F(origin.X + (x + inset) * cell, origin.Y + (y + inset) * cell);
        }

        /// <summary>
        /// Converts an absolute point to a board square. Full: same rules as
        /// <see cref="BoardPixelExtensions"/> (which use the authored constants) - inside
        /// when within Columns x Rows cells from the grid origin, rounded to the nearest
        /// intersection and clamped to the board. HalfCenter: inside when on the grid area
        /// (Columns x Rows cells), the cell under the point.
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

            // Full rounds to the nearest crossing; HalfCenter takes the cell (the far edge,
            // exactly on the border, clamps into the last cell).
            float round = board.Type == BoardType.HalfCenter ? 0f : 0.5f;
            gridX = Math.Clamp((int)((pixelX - origin.X) / cell + round), 0, board.Columns - 1);
            gridY = Math.Clamp((int)((pixelY - origin.Y) / cell + round), 0, board.Rows - 1);
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
            PendingActions.Clear();
            PieceBinder?.Dispose();
            PieceBinder = null;
        }

        protected override void OnReset()
        {
            GameManager.ResetBoardToDefault();
        }
    }
}

/* ----- ----- ----- ----- */
// UIBoard.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/10/21
// Update Date: 2026/10/05
// Version: v1.4
/* ----- ----- ----- ----- */

using System;

using Chinese_Chess_v3.Game.Application.Boards;
using Chinese_Chess_v3.Game.Configs;
using Chinese_Chess_v3.Game.Core;
using Chinese_Chess_v3.Game.Core.Boards;
using Chinese_Chess_v3.Game.Core.Pieces;
using Chinese_Chess_v3.Game.UI.Binders;
using Chinese_Chess_v3.Game.UI.Constants;

using Engine.Mathematics;
using Engine.Platform;
using Engine.UI.Constants.Core;
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
        /// Whether the board is drawn rotated 180 degrees so the player's own side (己方,
        /// <c>GameManager.LocalSide</c>) is at the bottom: the Full board when 己方 plays black
        /// (<see cref="BoardPerspective"/>). Read live from the game, so it follows every new
        /// game (an opening practised as 後手 after a 先手 one, a black-to-move endgame...).
        /// Grid lines, palaces and cannon/soldier marks are 180-degree symmetric; pieces and
        /// rings go through <see cref="GridToPixel"/>, clicks through <see cref="TryPixelToGrid"/>.
        /// </summary>
        public bool IsFlipped => BoardPerspective.IsFlipped(_gameManager);

        /// <summary>
        /// Absolute position where a piece on square (<paramref name="x"/>, <paramref name="y"/>)
        /// (board coordinates) is drawn: the grid point on the Full board, the cell's centre on
        /// HalfCenter; rotated 180 degrees when <see cref="IsFlipped"/>.
        /// </summary>
        public Vector2F GridToPixel(float x, float y)
        {
            var board = _gameManager.Board;
            var (viewX, viewY) = BoardPerspective.Map(x, y, board.Columns, board.Rows, IsFlipped);
            return ViewToPixel(viewX, viewY);
        }

        /// <summary>
        /// Absolute position of the piece centre at view (screen) square
        /// (<paramref name="viewX"/>, <paramref name="viewY"/>), (0, 0) being the top-left one.
        /// </summary>
        private Vector2F ViewToPixel(float viewX, float viewY)
        {
            var origin = GridOrigin;
            float cell = GridCellSize;
            // HalfCenter: pieces stand in the cells, half a cell in from the cell's corner.
            float inset = BoardType == BoardType.HalfCenter ? 0.5f : 0f;
            return new Vector2F(origin.X + (viewX + inset) * cell, origin.Y + (viewY + inset) * cell);
        }

        /// <summary>
        /// The radius pieces are drawn at (<c>UILayoutConstants.Board.Piece.Radius</c> at
        /// <see cref="DetailScale"/>); the clickable area also reaches this far past the
        /// outermost piece centres (<see cref="TryPixelToGrid"/>).
        /// </summary>
        public float PieceRadius => UILayoutConstants.Board.Piece.Radius * DetailScale;

        /// <summary>
        /// The per-edge adjustment of the clickable area for <see cref="BoardType"/>
        /// (<c>UILayoutConstants.Board.ClickArea</c> / <c>Board.HalfCenter.ClickArea</c>), in
        /// design-space units, not scaled with the board.
        /// </summary>
        /// <exception cref="NotSupportedException">The board type has no board drawing yet (HalfCross).</exception>
        public PaddingF ClickAreaEdgeAdjust => BoardType switch
        {
            BoardType.Full => UILayoutConstants.Board.ClickArea.EdgeAdjust,
            BoardType.HalfCenter => UILayoutConstants.Board.HalfCenter.ClickArea.EdgeAdjust,
            _ => throw new NotSupportedException($"No board drawing for {BoardType} yet"),
        };

        /// <summary>
        /// Converts an absolute point to a board square, from the drawing geometry
        /// (<see cref="BoardHitTest"/>): inside when within the drawn extent - the outermost
        /// piece centres (<see cref="GridToPixel"/>: crossings on Full, cell centres on
        /// HalfCenter) out by <see cref="PieceRadius"/>, adjusted per edge by
        /// <see cref="ClickAreaEdgeAdjust"/> - then the square whose piece centre is nearest.
        /// The hit test runs in view (screen) space, so the per-edge adjustment stays with the
        /// screen edges; the view square is then rotated back when <see cref="IsFlipped"/>.
        /// </summary>
        /// <returns>False when the point is outside the clickable area.</returns>
        public bool TryPixelToGrid(float pixelX, float pixelY, out int gridX, out int gridY)
        {
            var board = _gameManager.Board;
            bool hit = BoardHitTest.TryPixelToGrid(ViewToPixel(0, 0), GridCellSize, board.Columns, board.Rows,
                PieceRadius, ClickAreaEdgeAdjust, pixelX, pixelY, out int viewX, out int viewY);
            (gridX, gridY) = BoardPerspective.Map(viewX, viewY, board.Columns, board.Rows, hit && IsFlipped);
            return hit;
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
    }
}

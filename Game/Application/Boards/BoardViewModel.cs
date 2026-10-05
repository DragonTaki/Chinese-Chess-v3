/* ----- ----- ----- ----- */
// BoardViewModel.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/05
// Update Date: 2026/10/05
// Version: v1.0
/* ----- ----- ----- ----- */

using System;
using System.Collections.Generic;

using Chinese_Chess_v3.Game.Configs;
using Chinese_Chess_v3.Game.Core;
using Chinese_Chess_v3.Game.Core.Pieces;

namespace Chinese_Chess_v3.Game.Application.Boards
{
    /// <summary>
    /// The board's state for the view (棋盤), apart from the view that draws it: the selected
    /// piece, its legal destinations (move hints), the hanging pieces (無根子可被吃), which way up
    /// the board is drawn (<see cref="BoardPerspective"/>), and the click command. The hints are
    /// already filtered by the player settings ([hints] in settings.ini), read live.
    /// <para>
    /// Subscribes to the <see cref="GameManager"/> events and hands each one to the view's
    /// thread through the <c>post</c> callback; its state changes there and its own events are
    /// raised there, after the state is updated. The view keeps only its piece elements (one per
    /// <see cref="Piece"/>, created and dropped on <see cref="PieceAdded"/> /
    /// <see cref="PieceRemoved"/> / <see cref="BoardReset"/>) and their visual state.
    /// </para>
    /// Must be disposed to unsubscribe from the game.
    /// </summary>
    public sealed class BoardViewModel : IDisposable
    {
        private readonly GameManager _game;
        private readonly PlayerSettings _settings;
        private readonly Action<Action> _post;

        // Legal destinations of the selected piece, taken from the game once per selection.
        private IReadOnlyList<(int x, int y)> _legalTargets = Array.Empty<(int x, int y)>();
        // The piece _legalTargets belongs to (null when empty).
        private Piece _legalTargetsOwner;

        // The hanging pieces of the last HangingPiecesChanged, in the game's order. A piece
        // added or removed since (undo, capture) or a board reset drops out until the next one.
        private readonly List<Piece> _hanging = new();

        /// <summary>
        /// Creates the view model and subscribes to <paramref name="game"/>.
        /// </summary>
        /// <param name="game">The game shown.</param>
        /// <param name="settings">The live player settings (which board hints are shown).</param>
        /// <param name="post">Runs an action on the view's thread (e.g. the board element's
        /// <c>Post</c>; it may drop the action once the view is gone).</param>
        public BoardViewModel(GameManager game, PlayerSettings settings, Action<Action> post)
        {
            _game = game ?? throw new ArgumentNullException(nameof(game));
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _post = post ?? throw new ArgumentNullException(nameof(post));

            _game.PieceSelected += OnPieceSelected;
            _game.PieceUnselected += OnPieceUnselected;
            _game.PieceMoved += OnPieceMoved;
            _game.PieceCaptured += OnPieceCaptured;
            _game.PieceAdded += OnPieceAdded;
            _game.PieceRemoved += OnPieceRemoved;
            _game.BoardReset += OnBoardReset;
            _game.HangingPiecesChanged += OnHangingPiecesChanged;

            _hanging.AddRange(_game.HangingPieces);
        }

        // ----- State -----

        /// <summary>The game shown.</summary>
        public GameManager Game => _game;

        /// <summary>
        /// Whether the board is drawn rotated 180 degrees so 己方 is at the bottom
        /// (<see cref="BoardPerspective.IsFlipped(GameManager)"/>); read live, so it follows every new game.
        /// </summary>
        public bool IsFlipped => BoardPerspective.IsFlipped(_game);

        /// <summary>The selected piece; null when nothing is selected.</summary>
        public Piece SelectedPiece { get; private set; }

        /// <summary>
        /// Where the move-hint rings go: the selected piece's legal destinations while
        /// <see cref="PlayerSettings.ShowLegalMoveHints"/> is on; otherwise, and when nothing is
        /// selected, empty.
        /// </summary>
        public IReadOnlyList<(int x, int y)> LegalTargets =>
            _settings.ShowLegalMoveHints ? _legalTargets : Array.Empty<(int x, int y)>();

        /// <summary>
        /// Where the hanging-piece rings go: the face-up hanging pieces' squares and colours while
        /// <see cref="PlayerSettings.ShowHangingPieceHints"/> is on; otherwise empty. A face-down
        /// piece's ring would show its colour; the Core never reports one as hanging anyway
        /// (<c>BoardAnalysis</c>), this only makes sure. Built on each read from the pieces'
        /// current squares.
        /// </summary>
        public IReadOnlyList<HangingSquare> HangingSquares
        {
            get
            {
                if (!_settings.ShowHangingPieceHints || _hanging.Count == 0)
                    return Array.Empty<HangingSquare>();
                var squares = new List<HangingSquare>(_hanging.Count);
                foreach (var piece in _hanging)
                {
                    if (piece.CurrentInfo.IsFaceUp)
                        squares.Add(new HangingSquare(piece.X, piece.Y, piece.Color));
                }
                return squares;
            }
        }

        /// <summary>The pieces on the board now, to build the view's piece elements from.</summary>
        public List<Piece> GetCurrentPieces() => _game.GetCurrentPieces();

        // ----- Commands -----

        /// <summary>A click on square (<paramref name="x"/>, <paramref name="y"/>) (board coordinates): select, move or capture.</summary>
        public void HandleClick(int x, int y) => _game.HandleClick(x, y);

        // ----- Events (raised on the view's thread, after the state above is updated) -----

        /// <summary>A piece was selected (<see cref="SelectedPiece"/>, <see cref="LegalTargets"/> updated).</summary>
        public event Action<Piece> PieceSelected;

        /// <summary>A piece was unselected.</summary>
        public event Action<Piece> PieceUnselected;

        /// <summary>A piece moved to (toX, toY); it is no longer selected.</summary>
        public event Action<Piece, int, int> PieceMoved;

        /// <summary>A piece was captured (followed by <see cref="PieceRemoved"/>).</summary>
        public event Action<Piece> PieceCaptured;

        /// <summary>A piece came onto the board.</summary>
        public event Action<Piece> PieceAdded;

        /// <summary>A piece left the board.</summary>
        public event Action<Piece> PieceRemoved;

        /// <summary>The board was reset: rebuild the pieces from <see cref="GetCurrentPieces"/> (no <see cref="PieceUnselected"/> is raised).</summary>
        public event Action BoardReset;

        #region Game events (marshalled to the view's thread)

        private void OnPieceSelected(Piece piece)
        {
            // Computed now (the selection's position), applied on the view's thread.
            var moves = _game.SelectedPiece == piece
                ? _game.SelectedPieceLegalMoves
                : piece.GetLegalMoves(_game.Board);
            _post(() =>
            {
                SelectedPiece = piece;
                _legalTargets = moves;
                _legalTargetsOwner = piece;
                PieceSelected?.Invoke(piece);
            });
        }

        private void OnPieceUnselected(Piece piece) => _post(() =>
        {
            if (SelectedPiece == piece) SelectedPiece = null;
            if (_legalTargetsOwner == piece) ClearLegalTargets();
            PieceUnselected?.Invoke(piece);
        });

        private void OnHangingPiecesChanged(IReadOnlyList<Piece> hanging)
        {
            // Snapshot now; the GameManager replaces the list on the next move.
            var snapshot = new List<Piece>(hanging);
            _post(() =>
            {
                _hanging.Clear();
                _hanging.AddRange(snapshot);
            });
        }

        private void OnPieceMoved(Piece piece, int toX, int toY) => _post(() =>
        {
            // Typically unselected after a move (the legal targets wait for PieceUnselected).
            if (SelectedPiece == piece) SelectedPiece = null;
            PieceMoved?.Invoke(piece, toX, toY);
        });

        private void OnPieceCaptured(Piece piece) => _post(() => PieceCaptured?.Invoke(piece));

        private void OnPieceAdded(Piece piece) => _post(() =>
        {
            // Not hanging until the next HangingPiecesChanged.
            _hanging.Remove(piece);
            PieceAdded?.Invoke(piece);
        });

        private void OnPieceRemoved(Piece piece) => _post(() =>
        {
            _hanging.Remove(piece);
            PieceRemoved?.Invoke(piece);
        });

        private void OnBoardReset() => _post(() =>
        {
            // A reset raises no PieceUnselected; the hanging pieces follow in HangingPiecesChanged.
            SelectedPiece = null;
            ClearLegalTargets();
            _hanging.Clear();
            BoardReset?.Invoke();
        });

        #endregion

        private void ClearLegalTargets()
        {
            _legalTargets = Array.Empty<(int x, int y)>();
            _legalTargetsOwner = null;
        }

        /// <summary>Unsubscribes from the game and clears the state. Call when the view is unloaded.</summary>
        public void Dispose()
        {
            _game.PieceSelected -= OnPieceSelected;
            _game.PieceUnselected -= OnPieceUnselected;
            _game.PieceMoved -= OnPieceMoved;
            _game.PieceCaptured -= OnPieceCaptured;
            _game.PieceAdded -= OnPieceAdded;
            _game.PieceRemoved -= OnPieceRemoved;
            _game.BoardReset -= OnBoardReset;
            _game.HangingPiecesChanged -= OnHangingPiecesChanged;

            SelectedPiece = null;
            ClearLegalTargets();
            _hanging.Clear();
        }
    }

    /// <summary>A hanging-piece ring (<see cref="BoardViewModel.HangingSquares"/>): the piece's square and colour.</summary>
    public readonly record struct HangingSquare(int X, int Y, PieceColor Color);
}

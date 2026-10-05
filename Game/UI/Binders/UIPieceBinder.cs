/* ----- ----- ----- ----- */
// UIPieceBinder.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/10/23
// Update Date: 2026/10/05
// Version: v1.2
/* ----- ----- ----- ----- */

using System;
using System.Collections.Generic;

using Chinese_Chess_v3.Game.Application.Boards;
using Chinese_Chess_v3.Game.Core.Pieces;
using Chinese_Chess_v3.Game.UI.Boards.Pieces;

namespace Chinese_Chess_v3.Game.UI.Binders
{
    /// <summary>
    /// Binds the board's view model (<see cref="BoardViewModel"/>) to UIPiece instances:
    /// keeps one UIPiece per <see cref="Piece"/> and updates their visual state on the view
    /// model's events (raised on the UI thread). The selection, move hints and hanging pieces
    /// are the view model's.
    /// Must be disposed to unsubscribe.
    /// </summary>
    public class UIPieceBinder : IDisposable
    {
        private readonly BoardViewModel _viewModel;
        public List<UIPiece> UIPieces { get; } = new();

        private readonly List<(Piece piece, UIPiece uiPiece)> _bindings = new();

        // Mapping from Piece model to UIPiece
        private readonly Dictionary<Piece, UIPiece> _pieceMap = new();

        public UIPieceBinder(BoardViewModel viewModel)
        {
            _viewModel = viewModel ?? throw new ArgumentNullException(nameof(viewModel));

            // subscribe
            _viewModel.PieceSelected += OnPieceSelected;
            _viewModel.PieceUnselected += OnPieceUnselected;
            _viewModel.PieceMoved += OnPieceMoved;
            _viewModel.PieceCaptured += OnPieceCaptured;
            _viewModel.PieceAdded += OnPieceAdded;
            _viewModel.PieceRemoved += OnPieceRemoved;
            _viewModel.BoardReset += OnBoardReset;

            // create initial set from current board
            foreach (var p in _viewModel.GetCurrentPieces())
                AddUIPieceFor(p);
        }

        private void AddUIPieceFor(Piece piece)
        {
            if (piece == null) return;
            if (_pieceMap.ContainsKey(piece)) return;

            var uiPiece = new UIPiece(piece);
            _pieceMap[piece] = uiPiece;
            UIPieces.Add(uiPiece);
        }

        private void RemoveUIPieceFor(Piece piece)
        {
            if (piece == null) return;
            if (!_pieceMap.TryGetValue(piece, out var uiPiece)) return;

            _pieceMap.Remove(piece);
            UIPieces.Remove(uiPiece);
            // Pieces are the short-lived UI elements: released when they leave the board.
            uiPiece.Dispose();
        }

        private void DisposeAllUIPieces()
        {
            foreach (var uiPiece in UIPieces)
                uiPiece.Dispose();
            _pieceMap.Clear();
            UIPieces.Clear();
        }

        #region View model event handlers (already on the UI thread)
        private void OnPieceSelected(Piece piece)
        {
            if (_pieceMap.TryGetValue(piece, out var ui)) ui.IsSelected = true;
        }

        private void OnPieceUnselected(Piece piece)
        {
            if (_pieceMap.TryGetValue(piece, out var ui)) ui.IsSelected = false;
        }

        private void OnPieceMoved(Piece piece, int toX, int toY)
        {
            if (_pieceMap.TryGetValue(piece, out var ui))
            {
                ui.TargetX = toX;
                ui.TargetY = toY;
                ui.IsSelected = false; // typically unselected after move
            }
        }

        private void OnPieceCaptured(Piece piece)
        {
            if (_pieceMap.TryGetValue(piece, out var ui))
            {
                ui.IsCaptured = true;
                // Optionally mark hidden or trigger captured animation
            }
        }

        private void OnPieceAdded(Piece piece) => AddUIPieceFor(piece);

        private void OnPieceRemoved(Piece piece) => RemoveUIPieceFor(piece);

        private void OnBoardReset()
        {
            // Clear existing UI pieces and recreate
            DisposeAllUIPieces();
            foreach (var p in _viewModel.GetCurrentPieces())
                AddUIPieceFor(p);
        }
        #endregion

        // Must be called when the screen is unloaded or switched, to avoid memory / event leaks.
        public void Dispose()
        {
            _viewModel.PieceSelected -= OnPieceSelected;
            _viewModel.PieceUnselected -= OnPieceUnselected;
            _viewModel.PieceMoved -= OnPieceMoved;
            _viewModel.PieceCaptured -= OnPieceCaptured;
            _viewModel.PieceAdded -= OnPieceAdded;
            _viewModel.PieceRemoved -= OnPieceRemoved;
            _viewModel.BoardReset -= OnBoardReset;

            DisposeAllUIPieces();
        }

        public List<UIPiece> GetUIPieces()
        {
            return UIPieces;
        }
        public void Bind(Piece piece, UIPiece uiPiece)
        {
            _bindings.Add((piece, uiPiece));
        }

        public void Unbind(Piece piece)
        {
            _bindings.RemoveAll(b => b.piece == piece);
        }

        public void UnbindAll()
        {
            foreach (var (piece, uiPiece) in _bindings)
            {
                // Optional: remove event listeners or reset the UI state
                uiPiece.IsSelected = false;
                uiPiece.IsHighlighted = false;
            }

            _bindings.Clear();
        }
    }
}

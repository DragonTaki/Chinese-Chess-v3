/* ----- ----- ----- ----- */
// UIPieceBinder.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/10/23
// Update Date: 2025/10/23
// Version: v1.0
/* ----- ----- ----- ----- */

using System;
using System.Collections.Generic;

using Chinese_Chess_v3.Game.Core;
using Chinese_Chess_v3.Game.Core.Pieces;
using Chinese_Chess_v3.Game.UI.Boards.Pieces;

using Engine.UI.Core.Interfaces;

namespace Chinese_Chess_v3.Game.UI.Binders
{
    /// <summary>
    /// Binds GameManager events to UIPiece instances.
    /// Responsible for creating UIPiece collection and updating UI state on events.
    /// Must be disposed to unsubscribe.
    /// </summary>
    public class UIPieceBinder : IDisposable
    {
        private readonly GameManager _gameManager;
        private readonly IUiContainer uiHost; // used for invoking on UI thread
        public List<UIPiece> UIPieces { get; } = new();
        private readonly List<(Piece piece, UIPiece uiPiece)> _bindings = new();

        // Mapping from Piece model to UIPiece
        private readonly Dictionary<Piece, UIPiece> pieceMap = new();

        public UIPieceBinder(GameManager gameManager, IUiContainer parentElement)
        {
            _gameManager = gameManager ?? throw new ArgumentNullException(nameof(gameManager));
            uiHost = parentElement ?? throw new ArgumentNullException(nameof(parentElement));

            // subscribe
            _gameManager.PieceSelected += OnPieceSelected;
            _gameManager.PieceUnselected += OnPieceUnselected;
            _gameManager.PieceMoved += OnPieceMoved;
            _gameManager.PieceCaptured += OnPieceCaptured;
            _gameManager.PieceAdded += OnPieceAdded;
            _gameManager.PieceRemoved += OnPieceRemoved;
            _gameManager.BoardReset += OnBoardReset;

            // create initial set from current board
            foreach (var p in _gameManager.GetCurrentPieces())
                AddUIPieceFor(p);
        }

        private void AddUIPieceFor(Piece piece)
        {
            if (piece == null) return;
            if (pieceMap.ContainsKey(piece)) return;

            var uiPiece = new UIPiece(piece);
            pieceMap[piece] = uiPiece;
            UIPieces.Add(uiPiece);
        }

        private void RemoveUIPieceFor(Piece piece)
        {
            if (piece == null) return;
            if (!pieceMap.TryGetValue(piece, out var uiPiece)) return;

            pieceMap.Remove(piece);
            UIPieces.Remove(uiPiece);
        }

        #region Event Handlers (marshal to UI thread)
        private void OnPieceSelected(Piece piece) => PostToUI(() =>
        {
            if (pieceMap.TryGetValue(piece, out var ui)) ui.IsSelected = true;
        });

        private void OnPieceUnselected(Piece piece) => PostToUI(() =>
        {
            if (pieceMap.TryGetValue(piece, out var ui)) ui.IsSelected = false;
        });

        private void OnPieceMoved(Piece piece, int toX, int toY) => PostToUI(() =>
        {
            if (pieceMap.TryGetValue(piece, out var ui))
            {
                ui.TargetX = toX;
                ui.TargetY = toY;
                ui.IsSelected = false; // typically unselected after move
            }
        });

        private void OnPieceCaptured(Piece piece) => PostToUI(() =>
        {
            if (pieceMap.TryGetValue(piece, out var ui))
            {
                ui.IsCaptured = true;
                // Optionally mark hidden or trigger captured animation
            }
        });

        private void OnPieceAdded(Piece piece) => PostToUI(() =>
        {
            AddUIPieceFor(piece);
        });

        private void OnPieceRemoved(Piece piece) => PostToUI(() =>
        {
            RemoveUIPieceFor(piece);
        });

        private void OnBoardReset() => PostToUI(() =>
        {
            // Clear existing UI pieces and recreate
            pieceMap.Clear();
            UIPieces.Clear();
            foreach (var p in _gameManager.GetCurrentPieces())
                AddUIPieceFor(p);
        });
        #endregion

        private void PostToUI(Action action)
        {
            if (uiHost.IsDisposed) return;
            uiHost.Post(action);
        }

        // 一定要在畫面卸載或切換時呼叫，避免記憶體 / 事件洩漏。
        public void Dispose()
        {
            _gameManager.PieceSelected -= OnPieceSelected;
            _gameManager.PieceUnselected -= OnPieceUnselected;
            _gameManager.PieceMoved -= OnPieceMoved;
            _gameManager.PieceCaptured -= OnPieceCaptured;
            _gameManager.PieceAdded -= OnPieceAdded;
            _gameManager.PieceRemoved -= OnPieceRemoved;
            _gameManager.BoardReset -= OnBoardReset;

            pieceMap.Clear();
            UIPieces.Clear();
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
                // 可選：移除事件監聽或重置UI狀態
                uiPiece.IsSelected = false;
                uiPiece.IsHighlighted = false;
            }

            _bindings.Clear();
        }
    }
}

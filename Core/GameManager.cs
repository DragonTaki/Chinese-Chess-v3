/* ----- ----- ----- ----- */
// GameManager.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/06
// Update Date: 2025/05/07
// Version: v1.1
/* ----- ----- ----- ----- */

using System;
using System.Collections.Generic;

using Chinese_Chess_v3.Configs;
using Chinese_Chess_v3.Core.Logging;

namespace Chinese_Chess_v3.Core
{
    public class GameManager
    {
        public Board Board { get; private set; }
        // Turn event
        public event Action<PlayerSide> TurnChanged;
        private PlayerSide currentTurn = PlayerSide.Red;
        public PlayerSide CurrentTurn
        {
            get => currentTurn;
            private set
            {
                if (currentTurn != value)
                {
                    currentTurn = value;
                    TurnChanged?.Invoke(currentTurn);
                }
            }
        }
        private Piece? selectedPiece;
        public Piece? SelectedPiece => selectedPiece;
        private static GameManager? instance;
        public static GameManager Instance => instance ??= new GameManager();

        public GameManager()
        {
            // Initialize the board
            Board = new Board();
            Board.Initialize();
            CurrentTurn = PlayerSide.Red;
            selectedPiece = null;
        }
        public List<Piece> GetCurrentPieces()
        {
            return Board.GetAllPieces();
        }
        public bool TryMove(int fromX, int fromY, int toX, int toY)
        {
            var piece = Board.GetPiece(fromX, fromY);
            if (piece == null || piece.Side != CurrentTurn)
                return false;

            if (piece.CanMoveTo(toX, toY, Board))
            {
                Board.MovePiece(fromX, fromY, toX, toY);
                SwitchTurn();
                return true;
            }

            return false;
        }

        public void HandleClick(int x, int y)
        {
            var clickedPiece = Board.GetPiece(x, y);
            AppLogger.Log(
                $"Current turn: {CurrentTurn}, holding: {(selectedPiece == null ? "null" : selectedPiece.Type.ToString())},\n" +
                $"clicked at ({x},{y}), on: {(clickedPiece == null ? "null" : clickedPiece.GetType().Name)}", LogLevel.DEBUG);

            // No selected piece, try to select one
            if (selectedPiece == null)
            {
                if (clickedPiece != null && clickedPiece.Side == CurrentTurn)
                {
                    selectedPiece = clickedPiece;
                    AppLogger.Log($"(Action) Selected {clickedPiece.Type} at ({x},{y})", LogLevel.DEBUG);
                }
                return;
            }

            // Has selected piece, but 2nd selection is own side
            if (clickedPiece != null && clickedPiece.Side == selectedPiece.Side)
            {
                if (clickedPiece == selectedPiece)
                {
                    AppLogger.Log($"(Action) Un-selected {selectedPiece.Type} at ({x},{y})", LogLevel.DEBUG);
                    selectedPiece = null;
                }
                else
                {
                    selectedPiece = clickedPiece;
                    AppLogger.Log($"(Action) Switched to {clickedPiece.Type} at ({x},{y})", LogLevel.DEBUG);
                }
                return;
            }

            // Has selected piece, try to move to 2nd selection
            if (selectedPiece.CanMoveTo(x, y, Board))
            {
                // If 2nd selection point has enemy piece
                if (clickedPiece != null && clickedPiece.Side != selectedPiece.Side)
                {
                    Board.RemovePiece(x, y);
                    AppLogger.Log($"(Action) Captured {clickedPiece.Type} at ({x},{y})", LogLevel.DEBUG);
                }

                Board.MovePiece(selectedPiece.Position.X, selectedPiece.Position.Y, x, y);
                AppLogger.Log($"(Action) Moved {selectedPiece.Type} to ({x},{y})", LogLevel.DEBUG);
                selectedPiece = null;
                SwitchTurn();
            }
            else
            {
                // If 2nd selection point is empty, unselected
                if (clickedPiece == null)
                {
                    AppLogger.Log($"(Action) Un-selected {selectedPiece.Type} at ({x},{y})", LogLevel.DEBUG);
                }
                // Invalid catch
                else
                {
                    AppLogger.Log($"(Action) Invalid move to ({x},{y})", LogLevel.DEBUG);
                }
                selectedPiece = null;
            }
        }

        private void SwitchTurn()
        {
            CurrentTurn = CurrentTurn == PlayerSide.Red ? PlayerSide.Black : PlayerSide.Red;
        }
    }
}
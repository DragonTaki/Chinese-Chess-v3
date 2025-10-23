/* ----- ----- ----- ----- */
// UIPiece.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/10/23
// Update Date: 2025/10/23
// Version: v1.0
/* ----- ----- ----- ----- */

using Chinese_Chess_v3.Game.Core;

using Engine.UI.Constants;
using Engine.UI.Core.Elements;

namespace Chinese_Chess_v3.Game.UI.Elements
{
    /// <summary>
    /// Represents the UI-layer wrapper for a logical chess piece (<see cref="Piece"/>).
    /// It contains only UI-related states such as selection or highlighting,
    /// while the game logic and position are defined in the <see cref="Piece"/> model.
    /// </summary>
    public class UIPiece : UIElement
    {
        /// <summary>
        /// Gets the underlying logical piece model associated with this UI element.
        /// </summary>
        public Piece PieceModel { get; }

        /// <summary>
        /// Gets or sets whether the piece is currently selected by the player.
        /// </summary>
        public bool IsSelected { get; set; } = false;

        /// <summary>
        /// Gets or sets whether this piece should be highlighted for visual feedback.
        /// </summary>
        public bool IsHighlighted { get; set; } = false;

        public bool IsCaptured { get; set; } = false;

        // screen target position (for animation) - board coordinates
        public int TargetX { get; set; }
        public int TargetY { get; set; }

        /// <summary>
        /// Initializes a new UI element that visually represents a <see cref="Piece"/>.
        /// </summary>
        /// <param name="pieceModel">The logical piece model this UI element represents.</param>
        public UIPiece(Piece pieceModel)
            : base(zIndex: 1, isPersistent: false, type: UIElementType.Piece)
        {
            PieceModel = pieceModel;
            TargetX = pieceModel.X;
            TargetY = pieceModel.Y;
        }

        /// <summary>
        /// Sync UI target position with the logical model (call this to snap).
        /// For smooth animation, update TargetX/TargetY and animation system will interpolate.
        /// </summary>
        public void SyncTargetToModel()
        {
            TargetX = PieceModel.X;
            TargetY = PieceModel.Y;
        }

        /// <summary>
        /// Quick method to clear UI state (used on board reset).
        /// </summary>
        public void ResetUIState()
        {
            IsSelected = false;
            IsHighlighted = false;
            IsCaptured = false;
            SyncTargetToModel();
        }
    }
}

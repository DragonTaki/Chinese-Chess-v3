/* ----- ----- ----- ----- */
// IDialogService.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/04
// Update Date: 2026/10/04
// Version: v1.0
/* ----- ----- ----- ----- */

using System;

namespace Chinese_Chess_v3.Game.Application.Services
{
    /// <summary>
    /// Shows the app's modal confirm dialogs, so game logic can ask the player without
    /// depending on the UI types that draw them.
    /// </summary>
    public interface IDialogService
    {
        /// <summary>
        /// Shows a confirm dialog over the current screen; it takes the input until one of
        /// its buttons is clicked.
        /// </summary>
        /// <param name="message">The text shown in the dialog.</param>
        /// <param name="type">Which buttons the dialog offers.</param>
        /// <param name="callback">
        /// Called when the dialog closes: with the clicked button, or with
        /// <see cref="ConfirmDialogResult.Cancel"/> when it is dismissed by a click outside it.
        /// </param>
        void ShowConfirm(string message, ConfirmDialogType type, Action<ConfirmDialogResult> callback);
    }
}

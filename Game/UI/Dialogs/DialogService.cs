/* ----- ----- ----- ----- */
// DialogService.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/04
// Update Date: 2026/10/04
// Version: v1.0
/* ----- ----- ----- ----- */

using System;

using Chinese_Chess_v3.Game.Application.Services;

namespace Chinese_Chess_v3.Game.UI.Dialogs
{
    /// <summary>
    /// The UI's <see cref="IDialogService"/>: shows this game's one confirm dialog
    /// (<see cref="UIConfirmDialog"/>) through <see cref="DialogManager"/>.
    /// </summary>
    public sealed class DialogService : IDialogService
    {
        public void ShowConfirm(string message, ConfirmDialogType type, Action<ConfirmDialogResult> callback) =>
            DialogManager.ShowConfirm(message, type, callback);
    }
}

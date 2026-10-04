/* ----- ----- ----- ----- */
// ConfirmDialogType.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/19
// Update Date: 2026/10/04
// Version: v1.1
/* ----- ----- ----- ----- */

namespace Chinese_Chess_v3.Game.Application.Services
{
    /// <summary>
    /// The kind of a confirm dialog (<see cref="IDialogService.ShowConfirm"/>): which buttons it offers.
    /// </summary>
    public enum ConfirmDialogType
    {
        Default,
        Ok,
        OkCancel,
        YesNo,
        YesNoCancel,
        /// <summary>The game-over dialog: 重新開始 / 回到主畫面 / 關閉.</summary>
        GameOver
    }

    /// <summary>
    /// The button a confirm dialog was closed with, passed to its callback.
    /// </summary>
    public enum ConfirmDialogResult
    {
        None,
        Ok,
        Cancel,
        Yes,
        No,
        /// <summary>重新開始 (the game-over dialog).</summary>
        Restart,
        /// <summary>回到主畫面 (the game-over dialog).</summary>
        ReturnToMain,
        /// <summary>關閉 (the game-over dialog): keeps the final board visible.</summary>
        Close
    }
}

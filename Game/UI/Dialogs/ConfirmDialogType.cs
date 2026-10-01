/* ----- ----- ----- ----- */
// ConfirmDialogType.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/19
// Update Date: 2025/05/19
// Version: v1.0
/* ----- ----- ----- ----- */

namespace Chinese_Chess_v3.Game.UI.Dialogs
{
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

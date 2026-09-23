/* ----- ----- ----- ----- */
// DialogManager.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/09/23
// Update Date: 2026/09/23
// Version: v1.0
/* ----- ----- ----- ----- */

using System;

using Engine.UI.Core.Infrastructure;

namespace Chinese_Chess_v3.Game.UI.Dialogs
{
    /// <summary>
    /// Thin app-specific wrapper over the generic
    /// <see cref="DialogManager{TDialog}"/>, specialized to this game's one
    /// confirm dialog. Keeps the original <c>ShowConfirm</c>/<c>HideConfirm</c>
    /// call sites (e.g. <c>UIMainMenuHandler</c>) unchanged.
    /// </summary>
    public static class DialogManager
    {
        public static void ShowConfirm(
            string message,
            ConfirmDialogType type,
            Action<ConfirmDialogResult> callback)
        {
            DialogManager<UIConfirmDialog>.ShowMask();
            DialogManager<UIConfirmDialog>.Dialog.Show(message, type, callback);
        }

        public static void HideConfirm() => DialogManager<UIConfirmDialog>.HideAll();
    }
}

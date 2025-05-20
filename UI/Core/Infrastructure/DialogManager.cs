/* ----- ----- ----- ----- */
// DialogManager.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/20
// Update Date: 2025/05/20
// Version: v1.0
/* ----- ----- ----- ----- */

using System;

using Chinese_Chess_v3.UI.Core.Base;
using Chinese_Chess_v3.UI.Core.Elements;
using Chinese_Chess_v3.UI.Dialog;
using Chinese_Chess_v3.UI.Utils;

namespace Chinese_Chess_v3.UI.Core.Infrastructure
{
    public class DialogManager : InitializableOnceBase<UIRootNode>
    {
        private static ConfirmDialog _confirmDialog;
        private static UIOverlayNode _overlay;

        public DialogManager() {}

        protected override void OnInit(UIRootNode root)
        {
            _overlay = UIElementUtils.GetOrCreateOverlay(root);
            if (_confirmDialog == null)
            {
                _confirmDialog = new ConfirmDialog(new ConfirmDialogRenderer());
                _overlay.AddChild(_confirmDialog);
            }
        }

        public static void ShowConfirm(
            string message,
            ConfirmDialogType type,
            Action<ConfirmDialogResult> callback)
        {
            _confirmDialog.Show(message, type, callback);
        }
    }
}

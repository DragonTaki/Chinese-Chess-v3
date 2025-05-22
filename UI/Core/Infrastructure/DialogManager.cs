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
        private static UIOverlayMask _overlayMask;
        private static UIOverlayNode _overlayNode;

        public DialogManager() { }

        protected override void OnInit(UIRootNode root)
        {
            _overlayNode = UIElementUtils.GetOrCreateOverlay(root);

            if (_confirmDialog == null)
            {
                _confirmDialog = new ConfirmDialog(new ConfirmDialogRenderer())
                {
                    ZIndex = int.MaxValue - 1
                };
                _overlayMask = new UIOverlayMask(_confirmDialog)
                {
                    ZIndex = int.MaxValue
                };

                _overlayNode.AddChild(_overlayMask);
                _overlayNode.AddChild(_confirmDialog);
            }
        }

        public static void ShowConfirm(
            string message,
            ConfirmDialogType type,
            Action<ConfirmDialogResult> callback)
        {
            _overlayMask.Show();
            _confirmDialog.Show(message, type, callback);
        }
        public static void HideConfirm()
        {
            _overlayMask?.Hide();
            _confirmDialog?.Hide();
        }
    }
}

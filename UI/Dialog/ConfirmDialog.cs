/* ----- ----- ----- ----- */
// ConfirmDialog.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/19
// Update Date: 2025/05/19
// Version: v1.0
/* ----- ----- ----- ----- */

using System;
using System.Collections.Generic;
using System.Drawing;

using Chinese_Chess_v3.UI.Core;
using Chinese_Chess_v3.UI.Elements;
using Chinese_Chess_v3.UI.Menu;
using Chinese_Chess_v3.UI.Utils;

using SharedLib.MathUtils;

namespace Chinese_Chess_v3.UI.Dialog
{
    public class ConfirmDialog : UIElement
    {
        private readonly UILabel messageLabel = new();
        private readonly List<UIButton<ConfirmDialogResult>> buttons = new();
        private readonly ConfirmDialogRenderer renderer;

#nullable enable
        private Action<ConfirmDialogResult>? onResult;
#nullable disable

        public ConfirmDialog(ConfirmDialogRenderer renderer)
        {
            this.renderer = renderer;
            IsVisible = false;
            IsEnabled = false;
        }

        public void Show(string message, ConfirmDialogType type, Action<ConfirmDialogResult> resultCallback)
        {
            onResult = resultCallback;
            buttons.Clear();
            Children.Clear();

            IsVisible = true;
            IsEnabled = true;

            Size = new Vector2F(300, 180);
            LocalPosition = new Vector2F(240, 150); // 可視窗居中

            messageLabel.Text = message;
            messageLabel.LocalPosition = new Vector2F(20, 30);
            messageLabel.Size = new Vector2F(260, 60);
            AddChild(messageLabel);

            AddButtons(type);
        }

        private void AddButtons(ConfirmDialogType type)
        {
            var entries = ConfirmDialogOptions.Create(type, result =>
            {
                onResult?.Invoke(result);
            });

            float totalWidth = entries.Count * 80 + (entries.Count - 1) * 10;
            float startX = (Size.X - totalWidth) / 2;

            for (int i = 0; i < entries.Count; i++)
            {
                var result = entries[i];
                
                var button = new UIButton<ConfirmDialogResult>(result);
                button.Size = new Vector2F(80, 40);
                button.LocalPosition = new Vector2F(startX + i * 90, 110);

                AddChild(button);
                buttons.Add(button);
            }
        }

        protected override void OnDraw(Graphics g)
        {
            if (!IsVisible) return;
            renderer.Draw(g, this);
        }
    }
}

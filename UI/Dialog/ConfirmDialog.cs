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
using System.Windows.Forms;
using Chinese_Chess_v3.UI.Constants;
using Chinese_Chess_v3.UI.Core.Elements;
using Chinese_Chess_v3.UI.Core.Infrastructure;
using SharedLib.Globals;
using SharedLib.MathUtils;

namespace Chinese_Chess_v3.UI.Dialog
{
    public class ConfirmDialog : UIElement
    {
        private readonly UILabel _messageLabel = new();
        private readonly List<UIButton<ConfirmDialogResult>> _buttons = new();
        private readonly ConfirmDialogRenderer _renderer;
        private float _maxDialogWidth;
        public float PaddingH { get; set; } = 24.0f;
        public float PaddingV { get; set; } = 16.0f;
        public bool ShowMaskEffect { get; set; } = true;

#nullable enable
        public Action<ConfirmDialogResult>? _onResult;
#nullable disable

        public ConfirmDialog(ConfirmDialogRenderer _renderer)
        {
            this._renderer = _renderer;
            _maxDialogWidth = GlobalWindow.Width * 2f / 3f;

            IsVisible = false;
            IsEnabled = false;
        }

        public void Show(string message, ConfirmDialogType type, Action<ConfirmDialogResult> resultCallback)
        {
            _onResult = resultCallback;
            _buttons.Clear();
            Children.Clear();
            var root = this.GetRoot();

            var gTmp = Graphics.FromHwnd(IntPtr.Zero);   // 只用來量字
            var textSize = gTmp.MeasureString(message, UILayoutStyles.MainMenu.Button.Font,
                            (int)_maxDialogWidth - (int)PaddingH * 2);
            gTmp.Dispose();

            float dlgW = MathF.Min(textSize.Width + PaddingH * 2, _maxDialogWidth);
            float dlgH = textSize.Height + PaddingV * 2 + 70;

            Size = new Vector2F(dlgW, dlgH);
            LocalPosition = GlobalWindow.Center - Size / 2f;  // Center the window

            _messageLabel.Text = message;
            _messageLabel.LocalPosition = new Vector2F(PaddingH, PaddingV);
            _messageLabel.Size = new Vector2F(dlgW - PaddingH * 2, textSize.Height);
            AddChild(_messageLabel);

            AddButtons(type);
            IsVisible = true;
            IsEnabled = true;
        }

        public void Hide()
        {
            IsVisible = false;
            IsEnabled = false;
        }

        private void AddButtons(ConfirmDialogType type)
        {
            var entries = ConfirmDialogOptions.Create(type, result =>
            {
                _onResult?.Invoke(result);
            });

            float totalWidth = entries.Count * 80 + (entries.Count - 1) * 10;
            float startX = (Size.X - totalWidth) / 2;

            for (int i = 0; i < entries.Count; i++)
            {
                var result = entries[i];

                var button = new UIButton<ConfirmDialogResult>(result);
                button.Size = new Vector2F(80, 40);
                button.LocalPosition = new Vector2F(startX + i * 90, 110);
                var originalAction = button.Action;
                button.Action = () =>
                {
                    originalAction?.Invoke();
                    DialogManager.HideConfirm();
                };

                AddChild(button);
                _buttons.Add(button);
            }
        }

        protected override void OnDraw(Graphics g)
        {
            _renderer.Draw(g, this);
        }
        
        protected override bool HandleMouseDown(MouseEventArgs e) => true;
        protected override bool HandleMouseMove(MouseEventArgs e) => true;
        protected override bool HandleMouseUp(MouseEventArgs e) => true;
        protected override bool HandleMouseWheel(MouseEventArgs e) => true;
        public override bool HandleMouseClick(MouseEventArgs e) => true;
    }
}

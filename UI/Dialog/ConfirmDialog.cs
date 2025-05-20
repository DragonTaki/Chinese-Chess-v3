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
using Chinese_Chess_v3.UI.Core;
using Chinese_Chess_v3.UI.Elements;
using SharedLib.Globals;
using SharedLib.MathUtils;

namespace Chinese_Chess_v3.UI.Dialog
{
    public class ConfirmDialog : UIElement
    {
        private readonly UILabel _messageLabel = new();
        private readonly List<UIButton<ConfirmDialogResult>> _buttons = new();
        private readonly ConfirmDialogRenderer _renderer;
        private UIElement _mask;
        private float _maxDialogWidth;
        public float PaddingH { get; set; } = 24.0f;
        public float PaddingV { get; set; } = 16.0f;
        public bool ShowMaskEffect { get; set; } = true;
        public Color MaskColor { get; set; } = Color.FromArgb(120, 0, 0, 0);

#nullable enable
        private Action<ConfirmDialogResult>? _onResult;
#nullable disable

        public ConfirmDialog(ConfirmDialogRenderer _renderer)
        {
            this._renderer = _renderer;
            _maxDialogWidth = GlobalWindow.Width * 2f / 3f;
            
            _mask = new UIMask(this);

            IsVisible = false;
            IsEnabled = false;
        }

        public void Show(string message, ConfirmDialogType type, Action<ConfirmDialogResult> resultCallback)
        {
            _onResult = resultCallback;
            _buttons.Clear();
            Children.Clear();
            var root = this.GetRoot();

            _mask.IsVisible = true;
            _mask.IsEnabled = true;
            AddChild(_mask);

            var gTmp = Graphics.FromHwnd(IntPtr.Zero);   // 只用來量字
            var textSize = gTmp.MeasureString(message, UILayoutStyles.MainMenu.Button.Font,
                            (int)_maxDialogWidth - (int)PaddingH * 2);
            gTmp.Dispose();

            float dlgW = MathF.Min(textSize.Width + PaddingH * 2, _maxDialogWidth);
            float dlgH = textSize.Height + PaddingV * 2 + 70;

            Size = new Vector2F(dlgW, dlgH);
            LocalPosition = GlobalWindow.Center - Size / 2f; // 可視窗居中

            _messageLabel.Text = message;
            _messageLabel.LocalPosition = new Vector2F(PaddingH, PaddingV);
            _messageLabel.Size = new Vector2F(dlgW - PaddingH * 2, textSize.Height);
            AddChild(_messageLabel);

            AddButtons(type);
            IsVisible = true;
            IsEnabled = true;
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

                AddChild(button);
                _buttons.Add(button);
            }
        }

        protected override void OnDraw(Graphics g)
        {
            if (!IsVisible) return;

            if (ShowMaskEffect)
                using (var brush = new SolidBrush(MaskColor))
                    g.FillRectangle(brush, 0, 0, GlobalWindow.Width, GlobalWindow.Height);

            _renderer.Draw(g, this);
        }
        private class UIMask : UIElement
        {
            private readonly ConfirmDialog _dialog;
            public UIMask(ConfirmDialog dlg) => _dialog = dlg;

            public override bool OnMouseDown(MouseEventArgs e)
            {
                // 點擊遮罩 = Cancel
                _dialog.IsVisible = false;
                _dialog.IsEnabled = false;
                _dialog._onResult?.Invoke(ConfirmDialogResult.Cancel);
                return true;   // 吞掉事件
            }

            protected override void OnDraw(Graphics g) { /* nothing, 由父層處理 */ }

            // 全畫面命中
            public override bool HitTest(PointF p) => true;
        }
    }
}

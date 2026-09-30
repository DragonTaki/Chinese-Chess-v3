/* ----- ----- ----- ----- */
// UIConfirmDialog.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/19
// Update Date: 2026/09/30
// Version: v1.1
/* ----- ----- ----- ----- */

using System;
using System.Collections.Generic;
using System.Drawing;
using Engine.Platform;

using Chinese_Chess_v3.Game.UI.Constants;

using Engine.Globals;
using Engine.Mathematics;
using Engine.UI.Core.Elements;
using Engine.UI.Core.Handlers;
using Engine.UI.Core.Interfaces;
using Engine.UI.Core.Renderers;
using Engine.UI.Dialogs;

namespace Chinese_Chess_v3.Game.UI.Dialogs
{
    public class UIConfirmDialog : UIElement, IUIDialog
    {
        private readonly UILabel _messageLabel;
        private readonly List<UIButton<ConfirmDialogResult>> _buttons = new();
        private readonly UIConfirmDialogRenderer _renderer;
        private float _maxDialogWidth;
        public float PaddingH { get; set; } = 24.0f;
        public float PaddingV { get; set; } = 16.0f;
        public bool ShowMaskEffect { get; set; } = true;

#nullable enable
        public Action<ConfirmDialogResult>? _onResult;
#nullable disable

        /// <param name="factory">
        /// Used to create the dialog's buttons. The dialog is constructed directly (not via
        /// UiFactory.Create*), so nothing else would ever set _factory - without it every
        /// Show() threw a NullReferenceException in AddButtons.
        /// </param>
        public UIConfirmDialog(UIConfirmDialogRenderer _renderer, IUiFactory factory)
        {
            this._renderer = _renderer;
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
            _maxDialogWidth = GlobalViewport.Size.X * 2f / 3f;

            // Created through the factory so it has a renderer (a bare `new UILabel()` has
            // none and drew nothing). Same font Show() measures the message with.
            _messageLabel = _factory.CreateElement<UILabel, UILabelHandler, UILabelRenderer>();
            _messageLabel.Font = MessageFont;
            _messageLabel.ForeColor = Color.Black;
            _messageLabel.TextAlign = ContentAlign.MiddleCenter;
            _messageLabel.WordWrap = true;

            IsVisible = false;
            IsEnabled = false;

            // Sized from its content and centered in the overlay layer
            // (see UILayoutSheet.Overlay.ConfirmDialog).
            LayoutRules.Apply(UILayoutSheet.Overlay.ConfirmDialog);
        }

        private static IFont MessageFont => UILayoutStyles.MainMenu.Button.Font;

        private const float ButtonHeight = 40f;
        private const float ButtonAreaHeight = 70f;

        /// <summary>
        /// Draws the dialog box and its buttons (UIConfirmDialogRenderer), then the children
        /// (the message label). The renderer was held but never called, so the dialog drew
        /// nothing at all.
        /// </summary>
        public override void Draw(IGraphics g)
        {
            if (DisableRender || IsDisposed)
                return;

            _renderer.Draw(g, this);
            base.Draw(g);
        }

        private string _message = string.Empty;

        /// <summary>
        /// The dialog's content size: the message wrapped to the maximum dialog width, plus
        /// padding and the button area.
        /// </summary>
        private Vector2F MeasureDialog(string message, out SizeF textSize)
        {
            using var gTmp = Engine.Platform.GraphicsBackend.Factory.CreateMeasurementContext();   // 只用來量字
            textSize = gTmp.MeasureString(message ?? string.Empty, MessageFont,
                            (int)_maxDialogWidth - (int)PaddingH * 2);

            float dlgW = MathF.Min(textSize.Width + PaddingH * 2, _maxDialogWidth);
            float dlgH = textSize.Height + PaddingV * 2 + ButtonAreaHeight;
            return new Vector2F(dlgW, dlgH);
        }

        /// <summary>Auto size: measured from the current message (see <see cref="MeasureDialog"/>).</summary>
        public override Vector2F MeasureIntrinsicSize(Vector2F available) => MeasureDialog(_message, out _);

        public void Show(string message, ConfirmDialogType type, Action<ConfirmDialogResult> resultCallback)
        {
            _onResult = resultCallback;

            // Release the previous buttons (Children.Clear() only dropped the list,
            // leaving them undisposed and still parented to this dialog).
            foreach (var old in _buttons)
                old.Dispose();
            _buttons.Clear();
            RemoveAllChild(includePersistent: true);

            _message = message;
            var dialogSize = MeasureDialog(message, out var textSize);
            float dlgW = dialogSize.X;

            // Declared size and position: the pre-layout fallback. The layout (Auto size =
            // MeasureIntrinsicSize, centered in the overlay) resolves to the same rect.
            Size = dialogSize;
            LocalPosition = GlobalViewport.Center - Size / 2f;  // Center the window

            _messageLabel.Text = message;
            _messageLabel.LocalPosition = new Vector2F(PaddingH, PaddingV);
            _messageLabel.Size = new Vector2F(dlgW - PaddingH * 2, textSize.Height);
            AddChild(_messageLabel);

            AddButtons(type, buttonY: PaddingV + textSize.Height + (ButtonAreaHeight - ButtonHeight) / 2f);
            IsVisible = true;
            IsEnabled = true;
        }

        public void Hide()
        {
            IsVisible = false;
            IsEnabled = false;
        }

        /// <summary>
        /// Called by <see cref="Engine.UI.Core.Elements.UIOverlayMask"/> when
        /// the user dismisses this dialog by clicking outside it.
        /// </summary>
        public void Cancel() => _onResult?.Invoke(ConfirmDialogResult.Cancel);

        /// <param name="buttonY">
        /// Row position, centered in the button area below the message. It was a fixed 110,
        /// which put the buttons below the bottom of a dialog with a one-line message.
        /// </param>
        private void AddButtons(ConfirmDialogType type, float buttonY)
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

                var button = _factory.CreateButton<ConfirmDialogResult>();

                button.Text = result.Label;
                button.Handler.Action = () => _onResult?.Invoke(result.Type);

                button.Size = new Vector2F(80, ButtonHeight);
                button.LocalPosition = new Vector2F(startX + i * 90, buttonY);
                var originalAction = button.Handler.Action;
                button.Handler.Action = () =>
                {
                    originalAction?.Invoke();
                    DialogManager.HideConfirm();
                };

                AddChild(button);
                _buttons.Add(button);
            }
        }
    }

    public class UIConfirmDialogHandler : UIHandler
    {
        internal override bool HandleMouseDown(IMouseEvent e) => true;
        internal override bool HandleMouseMove(IMouseEvent e) => true;
        internal override bool HandleMouseUp(IMouseEvent e) => true;
        internal override bool HandleMouseWheel(IMouseEvent e) => true;
        internal override bool HandleMouseClick(IMouseEvent e) => true;
    }
}

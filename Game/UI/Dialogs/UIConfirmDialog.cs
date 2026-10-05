/* ----- ----- ----- ----- */
// UIConfirmDialog.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/19
// Update Date: 2026/10/05
// Version: v1.3
/* ----- ----- ----- ----- */

using System;
using System.Collections.Generic;
using System.Drawing;

using Chinese_Chess_v3.Game.Application.Services;
using Chinese_Chess_v3.Game.UI.Constants;

using Engine.Globals;
using Engine.Mathematics;
using Engine.Platform;
using Engine.UI.Core.Elements;
using Engine.UI.Core.Handlers;
using Engine.UI.Core.Interfaces;
using Engine.UI.Core.Renderers;
using Engine.UI.Dialogs;
using Engine.UI.Widgets;

namespace Chinese_Chess_v3.Game.UI.Dialogs
{
    public class UIConfirmDialog : UIElement, IUIDialog
    {
        private readonly UILabel _messageLabel;
        private readonly List<UIButton<ConfirmDialogResult>> _buttons = new();
        private readonly UIConfirmDialogRenderer _renderer;
        public float PaddingH { get; set; } = 24.0f;
        public float PaddingV { get; set; } = 16.0f;
        public bool ShowMaskEffect { get; set; } = true;

        /// <summary>The callback of the dialog being shown (set by <see cref="Show"/>; the result of a button or a click outside).</summary>
#nullable enable
        private Action<ConfirmDialogResult>? _onResult;
#nullable disable

        /// <param name="_renderer">Draws the dialog box and its buttons.</param>
        /// <param name="factory">
        /// Used to create the dialog's buttons. The dialog is constructed directly (not via
        /// UiFactory.Create*), so nothing else would ever set _factory - without it every
        /// Show() threw a NullReferenceException in AddButtons.
        /// </param>
        public UIConfirmDialog(UIConfirmDialogRenderer _renderer, IUiFactory factory)
        {
            this._renderer = _renderer;
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));

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
        private const float ButtonGap = 10f;

        /// <summary>Button width: wide enough for the longest label (4 or more characters get the wide width).</summary>
        private const float ButtonWidth = 80f;
        private const float WideButtonWidth = 110f;

        /// <summary>The width of the current buttons in a row (set by <see cref="Show"/>); the dialog is at least this wide.</summary>
        private float _buttonRowWidth = 0f;

        private static float ButtonWidthFor(IReadOnlyList<ButtonEntry<ConfirmDialogResult>> entries)
        {
            foreach (var entry in entries)
            {
                if (entry.Label.Length > 3)
                    return WideButtonWidth;
            }
            return ButtonWidth;
        }

        /// <summary>The dialog is at most this fraction of the available width.</summary>
        private const float MaxWidthFraction = 2f / 3f;

        /// <summary>
        /// The maximum dialog width for <paramref name="availableWidth"/> (the overlay's width
        /// when the layout measures). Computed at measure time: it was computed once in the
        /// constructor from the viewport size, so it never followed the window once the UI
        /// area stopped being the fixed design size. Falls back to the UI area's width when
        /// the available width isn't known (the pre-layout measure in <see cref="Show"/>).
        /// </summary>
        private static float MaxDialogWidth(float availableWidth)
        {
            float width = float.IsFinite(availableWidth) && availableWidth > 0f
                ? availableWidth
                : GlobalViewport.Size.X;
            return width * MaxWidthFraction;
        }

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
        /// The dialog's content size: the message wrapped to the maximum dialog width for
        /// <paramref name="availableWidth"/>, plus padding and the button area.
        /// </summary>
        private Vector2F MeasureDialog(string message, float availableWidth, out SizeF textSize)
        {
            float maxDialogWidth = MaxDialogWidth(availableWidth);
            using var gTmp = GraphicsBackend.Factory.CreateMeasurementContext();   // only used to measure text
            textSize = gTmp.MeasureString(message ?? string.Empty, MessageFont,
                            (int)maxDialogWidth - (int)PaddingH * 2);

            float dlgW = MathF.Min(MathF.Max(textSize.Width + PaddingH * 2, _buttonRowWidth + PaddingH * 2), maxDialogWidth);
            float dlgH = textSize.Height + PaddingV * 2 + ButtonAreaHeight;
            return new Vector2F(dlgW, dlgH);
        }

        /// <summary>Auto size: measured from the current message (see <see cref="MeasureDialog"/>).</summary>
        public override Vector2F MeasureIntrinsicSize(Vector2F available) => MeasureDialog(_message, available.X, out _);

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
            var entries = ConfirmDialogOptions.Create(type, _ => { });
            _buttonRowWidth = entries.Count * ButtonWidthFor(entries) + (entries.Count - 1) * ButtonGap;
            // Measured against the whole UI area (the overlay covers it), like the layout does.
            var dialogSize = MeasureDialog(message, GlobalViewport.Size.X, out var textSize);
            float dlgW = dialogSize.X;

            // Declared size and position: the pre-layout fallback. The layout (Auto size =
            // MeasureIntrinsicSize, centered in the overlay, which covers the whole UI area)
            // resolves to the same rect - GlobalViewport.Center is the UI area's center.
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

        /// <param name="type">Which buttons to add (Ok, Yes/No, ...).</param>
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

            float buttonWidth = ButtonWidthFor(entries);
            float totalWidth = entries.Count * buttonWidth + (entries.Count - 1) * ButtonGap;
            float startX = (Size.X - totalWidth) / 2;

            for (int i = 0; i < entries.Count; i++)
            {
                var result = entries[i];

                var button = _factory.CreateButton<ConfirmDialogResult>();

                button.Text = result.Label;
                button.Handler.Action = () => _onResult?.Invoke(result.Type);

                button.Size = new Vector2F(buttonWidth, ButtonHeight);
                button.LocalPosition = new Vector2F(startX + i * (buttonWidth + ButtonGap), buttonY);
                var originalAction = button.Handler.Action;
                button.Handler.Action = () =>
                {
                    // Hide first, then the callback (as a click outside does, UIOverlayMask):
                    // a callback that shows another dialog must not have it hidden right away.
                    DialogManager.HideConfirm();
                    originalAction?.Invoke();
                };

                AddChild(button);
                _buttons.Add(button);
            }
        }
    }

    public class UIConfirmDialogHandler : UIHandler
    {
        protected override bool HandleMouseDown(IMouseEvent e) => true;
        protected override bool HandleMouseMove(IMouseEvent e) => true;
        protected override bool HandleMouseUp(IMouseEvent e) => true;
        protected override bool HandleMouseWheel(IMouseEvent e) => true;
        protected override bool HandleMouseClick(IMouseEvent e) => true;
    }
}

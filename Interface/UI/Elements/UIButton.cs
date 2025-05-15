/* ----- ----- ----- ----- */
// UIButton.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/14
// Update Date: 2025/05/14
// Version: v1.0
/* ----- ----- ----- ----- */

using System;

using Chinese_Chess_v3.Interface.UI.Core;

namespace Chinese_Chess_v3.Interface.UI.Elements
{
    public class UIButton : UIElement
    {
        public string Text { get; set; }
        public bool IsHighlighted { get; set; } = false;

#nullable enable
        public Action? OnClick { get; set; }
#nullable disable

        public UIButton(string text)
        {
            Text = text;
            OnClick = null;
        }

        public UIButton(Action onClick = null)
        {
            Text = null;
            OnClick = onClick;
        }

        public UIButton(string text, Action onClick = null)
        {
            Text = text;
            OnClick = onClick;
        }
    }
}
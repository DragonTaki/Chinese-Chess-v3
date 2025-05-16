/* ----- ----- ----- ----- */
// UIButton.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/14
// Update Date: 2025/05/14
// Version: v1.0
/* ----- ----- ----- ----- */

using System;
using System.Windows.Forms;

using Chinese_Chess_v3.UI.Core;
using Chinese_Chess_v3.UI.Input;
using Chinese_Chess_v3.UI.Menu;

namespace Chinese_Chess_v3.UI.Elements
{
    public class UIButton : UIElement
    {
        public string Text { get; protected set; }

#nullable enable
        public Action? Action { get; set; }
#nullable disable
        public bool IsHighlighted { get; set; } = false;

        protected override bool HandleMouseClick(MouseEventArgs e)
        {
            Action?.Invoke();
            return true;
        }
    }
    
    public class UIButton<TEnum> : UIButton where TEnum : Enum
    {
        public TEnum Type { get; }

        public UIButton(string text, TEnum type, Action action = null)
        {
            Text = text;
            Type = type;
            Action = action;
        }

        public UIButton(MenuEntry<TEnum> button)
            : this(button.Label, button.Type, button.OnClick) { }

        public UIButton(string text)
            : this(text, default, null) { }

    }
}
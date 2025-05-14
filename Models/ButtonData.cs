/* ----- ----- ----- ----- */
// ButtonData.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/14
// Update Date: 2025/05/14
// Version: v1.0
/* ----- ----- ----- ----- */

using System;

using SharedLib.MathUtils;

namespace Chinese_Chess_v3.Models
{
    public class ButtonData
    {
        public string Text { get; set; }
        public Bounds Bounds { get; set; }

        public Vector2F Position  // Original layout position (excluding scroll offset)
        {
            get => Bounds.Position;
            set => Bounds.Position = value;
        }

        public Vector2F Size
        {
            get => Bounds.Size;
            set => Bounds.Size = value;
        }
        
        public Vector2F RenderPosition { get; set; }  // The actual position to be drawn (including scroll offset)

        public bool IsHighlighted { get; set; } = false;

#nullable enable
        public Action? OnClick { get; set; }
#nullable disable

        public ButtonData(string text)
        {
            Text = text;
            Bounds = Bounds.Empty;
            OnClick = null;
        }

        public ButtonData(string text, Bounds bounds)
        {
            Text = text;
            Bounds = bounds;
            OnClick = null;
        }

        public ButtonData(string text, Action onClick)
        {
            Text = text;
            Bounds = Bounds.Empty;
            OnClick = onClick;
        }

        public ButtonData(Bounds bounds, Action onClick = null)
        {
            Text = null;
            Bounds = bounds;
            OnClick = onClick;
        }

        public ButtonData(string text, Bounds bounds, Action onClick = null)
        {
            Text = text;
            Bounds = bounds;
            OnClick = onClick;
        }

        public bool Contains(Vector2F p) => Bounds.Contains(p);
    }
}
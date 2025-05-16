/* ----- ----- ----- ----- */
// UIElementUtils.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/16
// Update Date: 2025/05/16
// Version: v1.0
/* ----- ----- ----- ----- */

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Chinese_Chess_v3.UI.Core;

namespace Chinese_Chess_v3.UI.Utils
{
    public static class UIElementUtils
    {
        /// <summary>
        /// Filters out the UI elements that are currently displayed within the specified clipping area (usually the viewport of a ScrollView).
        /// </summary>
        /// <typeparam name="T">Subtypes of UIElement</typeparam>
        /// <param name="elements">All UI elements to be inspected</param>
        /// <param name="clippingRect">The visible clipping area (usually the clipping rectangle of the container)</param>
        /// <returns>List of UI elements visible on the screen</returns>
        public static void UpdateVisibleState<T>(IEnumerable<T> elements, RectangleF clippingRect) where T : UIElement
        {
            foreach (var element in elements)
            {
                float y = element.LocalPosition.Current.Y;
                float h = element.Size.Y;
                element.IsEnabled =  y + h > clippingRect.Top && y < clippingRect.Bottom;
            }
        }
    }
}
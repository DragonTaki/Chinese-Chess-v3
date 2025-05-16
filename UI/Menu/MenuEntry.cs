/* ----- ----- ----- ----- */
// MenuEntry.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/16
// Update Date: 2025/05/16
// Version: v1.0
/* ----- ----- ----- ----- */

using System;

namespace Chinese_Chess_v3.UI.Menu
{
    /// <summary>
    /// Generic menu item, TEnum must be of Enum type.
    /// </summary>
    /// <typeparam name="TEnum">Enum type, indicating the type of this menu item.</typeparam>
    public class MenuEntry<TEnum> where TEnum : Enum
    {
        /// <summary>
        /// Menu display text
        /// </summary>
        public string Label { get; }
        
        /// <summary>
        /// Menu type (enum value)
        /// </summary>
        public TEnum Type { get; }

        /// <summary>
        /// Click event
        /// </summary>
        public Action OnClick { get; }

        public MenuEntry(string label, TEnum type, Action onClick)
        {
            Label = label;
            Type = type;
            OnClick = onClick;
        }
    }
}

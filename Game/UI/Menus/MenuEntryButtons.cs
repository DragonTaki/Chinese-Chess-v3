/* ----- ----- ----- ----- */
// MenuEntryButtons.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/05
// Update Date: 2026/10/05
// Version: v1.0
/* ----- ----- ----- ----- */

using System;

using Chinese_Chess_v3.Game.Application.Menus;

using Engine.UI.Widgets;

namespace Chinese_Chess_v3.Game.UI.Menus
{
    /// <summary>
    /// Turns the logic layer's <see cref="MenuEntry{TId}"/> into the engine's
    /// <see cref="ButtonEntry{TEnum}"/>, where a menu builds its buttons.
    /// </summary>
    public static class MenuEntryButtons
    {
        /// <summary>The button entry of <paramref name="entry"/>: same label, id and action.</summary>
        /// <param name="entry">The logic layer's menu entry.</param>
        /// <returns>The engine's button entry for it.</returns>
        public static ButtonEntry<TId> ToButtonEntry<TId>(this MenuEntry<TId> entry) where TId : Enum
        {
            ArgumentNullException.ThrowIfNull(entry);
            return new ButtonEntry<TId>(entry.Label, entry.Id, entry.Action);
        }
    }
}

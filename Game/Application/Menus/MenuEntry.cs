/* ----- ----- ----- ----- */
// MenuEntry.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/05
// Update Date: 2026/10/05
// Version: v1.0
/* ----- ----- ----- ----- */

using System;

namespace Chinese_Chess_v3.Game.Application.Menus
{
    /// <summary>
    /// One option of a menu, as the logic layer lists it: which option it is, the text shown
    /// for it and what choosing it does. The UI turns each entry into a button.
    /// </summary>
    /// <typeparam name="TId">The enum of the menu's options.</typeparam>
    public sealed class MenuEntry<TId> where TId : Enum
    {
        /// <summary>Which option this is.</summary>
        public TId Id { get; }

        /// <summary>The text shown for the option (its button text).</summary>
        public string Label { get; }

        /// <summary>What choosing the option does.</summary>
        public Action Action { get; }

        /// <param name="id">Which option this is.</param>
        /// <param name="label">The text shown for the option.</param>
        /// <param name="action">What choosing the option does.</param>
        public MenuEntry(TId id, string label, Action action)
        {
            Id = id;
            Label = label ?? throw new ArgumentNullException(nameof(label));
            Action = action ?? throw new ArgumentNullException(nameof(action));
        }
    }
}

/* ----- ----- ----- ----- */
// UICategoryListMenuHandler.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/01
// Update Date: 2026/10/01
// Version: v1.0
/* ----- ----- ----- ----- */

using System;
using System.Collections.Generic;
using System.Linq;

using Chinese_Chess_v3.Game.Core;
using Chinese_Chess_v3.Game.Core.Pgn;
using Chinese_Chess_v3.Game.UI.Menus.GameMenu;

using Engine.Logging;
using Engine.UI.Core.Handlers;
using Engine.UI.Core.Interfaces;
using Engine.UI.Core.Renderers;

using Microsoft.Extensions.DependencyInjection;

namespace Chinese_Chess_v3.Game.UI.Menus.CategoryListMenu
{
    /// <summary>
    /// The shared logic of a category list submenu
    /// (<see cref="UICategoryListMenu{TMenu, THandler, TRenderer, TItem}"/>): loads the items
    /// when the submenu is shown, keeps the category filter, and starts an item on the game
    /// screen when its button is clicked. A derived handler supplies the loading
    /// (<see cref="LoadItems"/>), the start itself (<see cref="StartOnBoard"/>) and its texts.
    /// <see cref="IScreen"/>: the main menu calls <see cref="OnEnter"/>/<see cref="OnExit"/>
    /// when it opens/closes the submenu.
    /// </summary>
    public abstract class UICategoryListMenuHandler<TMenu, THandler, TRenderer, TItem> : UIMenuHandler<TMenu, THandler, TRenderer>, IScreen
        where TMenu : UICategoryListMenu<TMenu, THandler, TRenderer, TItem>
        where THandler : UICategoryListMenuHandler<TMenu, THandler, TRenderer, TItem>
        where TRenderer : UIMenuRenderer<TMenu, THandler, TRenderer>
        where TItem : PgnGameFile
    {
        /// <summary>Categories the player switched off (all on by default; kept while the game runs).</summary>
        private readonly HashSet<string> _hiddenCategories = new(StringComparer.Ordinal);

        /// <summary>The items last loaded, in display order.</summary>
        public IReadOnlyList<TItem> Items { get; private set; } = Array.Empty<TItem>();

        protected UICategoryListMenuHandler() { }

        /// <summary>Log prefix without parentheses, e.g. <c>Endgame</c>.</summary>
        protected abstract string LogLabel { get; }

        /// <summary>The player's own folder of this kind (shown in the empty message and the log).</summary>
        public abstract string UserFolder { get; }

        /// <summary>Text shown when nothing was found.</summary>
        protected abstract string EmptyMessageText { get; }

        /// <summary>Loads both folders (built-in, then the player's), adding to <paramref name="warnings"/>.</summary>
        protected abstract IEnumerable<TItem> LoadItems(List<string> warnings);

        /// <summary>Sets <paramref name="item"/> up on the (already reset) game.</summary>
        /// <exception cref="FormatException">The item cannot be set up.</exception>
        protected abstract void StartOnBoard(GameManager gameManager, TItem item);

        /// <summary>
        /// Called at the end of <see cref="StartItem"/> (also when the item could not be set
        /// up), e.g. for a submenu on the game screen to close itself. Nothing by default.
        /// </summary>
        protected virtual void OnItemStarted(TItem item) { }

        /// <summary>Submenu opened: reload the files, so items added meanwhile appear.</summary>
        public void OnEnter() => Reload();

        public void OnExit() { }

        /// <summary>
        /// Loads both folders and rebuilds the buttons. Sorted by category, then file name
        /// (ordinal, as the loaders sort each folder), built-in before the player's own on a
        /// tie: the loaders return the two folders one after the other, so a category present
        /// in both would otherwise be split in two.
        /// </summary>
        public void Reload()
        {
            var warnings = new List<string>();
            Items = LoadItems(warnings)
                .OrderBy(p => p.Category ?? string.Empty, StringComparer.Ordinal)
                .ThenBy(p => p.FileName, StringComparer.Ordinal)
                .ThenBy(p => p.Origin)
                .ToList();

            AppLogger.Log($"({LogLabel}) Loaded {Items.Count} file(s), {warnings.Count} warning(s); user folder: {UserFolder}", LogLevel.DEBUG);

            Element.ShowItems(Items, IsCategoryShown, EmptyMessageText);
        }

        public bool IsCategoryShown(string category) => !_hiddenCategories.Contains(category ?? string.Empty);

        /// <summary>Category toggle clicked: hide its items if shown, show them if hidden.</summary>
        public void ToggleCategory(string category)
        {
            category ??= string.Empty;
            bool show = !IsCategoryShown(category);
            if (show)
                _hiddenCategories.Remove(category);
            else
                _hiddenCategories.Add(category);

            Element.SetCategoryShown(category, show);
        }

        /// <summary>
        /// Item clicked: switch to the game screen the same way a new game does (reset the
        /// game UI; already there for a submenu on the game screen), then set the item up
        /// (<see cref="StartOnBoard"/>), then <see cref="OnItemStarted"/>.
        /// <para>
        /// The board is drawn as for any game (red at the bottom); turning it so the side to
        /// move is at the bottom is phase C (docs/PLAN.md).
        /// </para>
        /// </summary>
        public void StartItem(TItem item)
        {
            Console.WriteLine($"{LogLabel}Menu: selected: {item}");

            var gameMenu = _navigationManager.Show<UIGameMenu, UIGameMenuHandler, UIGameMenuRenderer>();
            var gameManager = _factory.ServiceProvider.GetRequiredService<GameManager>();

            // Reset first (clears the log, restarts the game being played - UIBoard.OnReset ->
            // GameManager.Restart), then the item's position - the start's own log lines stay.
            gameMenu.ResetGameUI();

            try
            {
                StartOnBoard(gameManager, item);
            }
            catch (FormatException ex)
            {
                // Items from the loaders were already validated; the restarted previous game stays on the board.
                AppLogger.Log($"({LogLabel}) cannot start {item.FileName}: {ex.Message}", LogLevel.ERROR);
            }

            OnItemStarted(item);
        }
    }
}

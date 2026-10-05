/* ----- ----- ----- ----- */
// UICategoryListMenuHandler.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/01
// Update Date: 2026/10/05
// Version: v1.0
/* ----- ----- ----- ----- */

using System;
using System.Collections.Generic;

using Chinese_Chess_v3.Game.Application.Catalogs;
using Chinese_Chess_v3.Game.Application.Session;
using Chinese_Chess_v3.Game.Core;
using Chinese_Chess_v3.Game.Core.Pgn;

using Engine.Diagnostics;
using Engine.Logging;
using Engine.UI.Core.Handlers;
using Engine.UI.Core.Interfaces;
using Engine.UI.Core.Renderers;

using Microsoft.Extensions.DependencyInjection;

namespace Chinese_Chess_v3.Game.UI.Menus.CategoryListMenu
{
    /// <summary>
    /// Binds a category list submenu
    /// (<see cref="UICategoryListMenu{TMenu, THandler, TRenderer, TItem}"/>) to its list's model
    /// (<see cref="Model"/>, a <see cref="CategoryListModel{T}"/> of a catalog: loading, order,
    /// sections, the category filter): reloads it when the submenu is shown, passes the
    /// category toggles to it, and starts an item on the game screen when its button is
    /// clicked. A derived handler supplies the model, the start itself
    /// (<see cref="StartOnBoard"/>) and its texts.
    /// <see cref="IScreen"/>: the main menu calls <see cref="OnEnter"/>/<see cref="OnExit"/>
    /// when it opens/closes the submenu.
    /// </summary>
    public abstract class UICategoryListMenuHandler<TMenu, THandler, TRenderer, TItem> : UIMenuHandler<TMenu, THandler, TRenderer>, IScreen
        where TMenu : UICategoryListMenu<TMenu, THandler, TRenderer, TItem>
        where THandler : UICategoryListMenuHandler<TMenu, THandler, TRenderer, TItem>
        where TRenderer : UIMenuRenderer<TMenu, THandler, TRenderer>
        where TItem : PgnGameFile
    {
        /// <summary>The items last loaded, in display order (<see cref="CategoryListModel{T}.Items"/>).</summary>
        public IReadOnlyList<TItem> Items => Model.Items;

        protected UICategoryListMenuHandler() { }

        /// <summary>The list's model (the catalog's, registered in DI; it keeps the switched-off categories).</summary>
        protected abstract CategoryListModel<TItem> Model { get; }

        /// <summary>Log prefix without parentheses, e.g. <c>Endgame</c>.</summary>
        protected abstract string LogLabel { get; }

        /// <summary>The player's own folder of this kind (shown in the empty message and the log).</summary>
        public abstract string UserFolder { get; }

        /// <summary>Text shown when nothing was found.</summary>
        protected abstract string EmptyMessageText { get; }

        /// <summary>Sets <paramref name="item"/> up on the (already reset) game.</summary>
        /// <exception cref="FormatException">The item cannot be set up.</exception>
        protected abstract void StartOnBoard(GameManager gameManager, TItem item);

        /// <summary>
        /// Called at the end of <see cref="StartItem"/> (also when the item could not be set
        /// up), e.g. for a submenu on the game screen to close itself. Nothing by default.
        /// </summary>
        protected virtual void OnItemStarted(TItem item) { }

        /// <summary>Submenu opened: reload the files, so items added meanwhile appear.</summary>
        public virtual void OnEnter() => Reload();

        public void OnExit() { }

        /// <summary>
        /// Loads the list again (<see cref="CategoryListModel{T}.Reload"/>: both folders, sorted
        /// by section, category, then file name) and rebuilds the buttons.
        /// </summary>
        public void Reload()
        {
            var model = Model;
            var warnings = model.Reload();

            AppLogger.Log($"({LogLabel}) Loaded {model.Items.Count} file(s), {warnings.Count} warning(s); user folder: {UserFolder}", LogLevel.DEBUG);

            Element.ShowItems(model, EmptyMessageText);
        }

        /// <summary>Whether a category's items are shown (<see cref="CategoryListModel{T}.IsCategoryShown"/>).</summary>
        public bool IsCategoryShown(string section, string category) => Model.IsCategoryShown(section, category);

        /// <summary>Category toggle clicked: hide its items if shown, show them if hidden.</summary>
        public void ToggleCategory(string section, string category)
        {
            bool show = Model.ToggleCategory(section, category);
            Element.SetCategoryShown(section ?? string.Empty, category ?? string.Empty, show);
        }

        /// <summary>
        /// Item button clicked: starts it (<see cref="StartItem"/>). A derived handler may do
        /// something else in a mode of its own (the saved-game list's delete mode).
        /// </summary>
        public virtual void ClickItem(TItem item) => StartItem(item);

        /// <summary>
        /// Starts <paramref name="item"/> the way a new game is (<see cref="GameSession.TryStart"/>: game
        /// screen - already there for a submenu on the game screen -, restart and views reset,
        /// then the item set up with <see cref="StartOnBoard"/>; an item that cannot be set up
        /// is logged), then <see cref="OnItemStarted"/>.
        /// <para>
        /// The board is drawn as for any game (red at the bottom); turning it so the side to
        /// move is at the bottom is phase C.
        /// </para>
        /// </summary>
        public void StartItem(TItem item)
        {
            if (DebugOptions.ConsoleTrace)
                Console.WriteLine($"{LogLabel}Menu: selected: {item}");

            // Restart first (views reset, log cleared), then the item's position - the start's
            // own log lines stay.
            _factory.ServiceProvider.GetRequiredService<GameSession>()
                .TryStart(game => StartOnBoard(game, item), LogLabel, item.FileName);

            OnItemStarted(item);
        }
    }
}

/* ----- ----- ----- ----- */
// UISavedGameMenuHandler.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/01
// Update Date: 2026/10/05
// Version: v1.1
/* ----- ----- ----- ----- */

using System;

using Chinese_Chess_v3.Game.Application.Catalogs;
using Chinese_Chess_v3.Game.Application.Session;
using Chinese_Chess_v3.Game.Core.Saves;
using Chinese_Chess_v3.Game.UI.Constants;
using Chinese_Chess_v3.Game.UI.Menus.CategoryListMenu;

using Microsoft.Extensions.DependencyInjection;

namespace Chinese_Chess_v3.Game.UI.Menus.SavedGameMenu
{
    /// <summary>
    /// The saved-game list's binding (shared part:
    /// <see cref="UICategoryListMenuHandler{TMenu, THandler, TRenderer, TItem}"/>): lists the
    /// saves of the <see cref="SavedGameCatalog"/> (categories = mode folders) and loads the
    /// chosen one with <see cref="GameSession.LoadSavedGame"/>, then tells the game menu
    /// (<see cref="ItemStarted"/>) so it closes the list; in delete mode
    /// (<see cref="IsDeleteMode"/>) a clicked save is deleted after asking. Opened and closed by
    /// <c>UIGameMenuHandler</c>, which calls <see cref="UICategoryListMenuHandler{TMenu, THandler, TRenderer, TItem}.OnEnter"/>
    /// to reload the files each time.
    /// </summary>
    public class UISavedGameMenuHandler : UICategoryListMenuHandler<UISavedGameMenu, UISavedGameMenuHandler, UISavedGameMenuRenderer, SavedGame>
    {
        public UISavedGameMenuHandler() { }

        /// <summary>Called after a save was clicked and loaded (or could not be); the game menu closes the list.</summary>
        public Action ItemStarted { get; set; }

        /// <summary>The saves folder (<see cref="SavedGameCatalog.Folder"/>), created when the list loads when missing.</summary>
        public override string UserFolder => SavedGameCatalog.Folder;

        /// <summary>The saved-game catalog registered in DI.</summary>
        private SavedGameCatalog Catalog => _factory.ServiceProvider.GetRequiredService<SavedGameCatalog>();

        protected override CategoryListModel<SavedGame> Model => Catalog.List;

        protected override string LogLabel => "Load";

        protected override string EmptyMessageText => string.Format(GameMenuTexts.NoSavedGamesFormat, UserFolder);

        /// <summary>The save's start position and moves replayed, with its rules and clocks (<see cref="GameSession.LoadSavedGame"/>).</summary>
        protected override void StartInSession(GameSession session, SavedGame saved) =>
            session.LoadSavedGame(saved, LogLabel);

        protected override void OnItemStarted(SavedGame item) => ItemStarted?.Invoke();

        /// <summary>
        /// Whether the list is in delete mode: a clicked save asks to be deleted
        /// (<see cref="SavedGameCatalog.ConfirmDelete"/>) instead of being loaded. Off each time
        /// the list is opened.
        /// </summary>
        public bool IsDeleteMode { get; private set; }

        /// <summary>List opened: delete mode off, then the files reloaded.</summary>
        public override void OnEnter()
        {
            IsDeleteMode = false;
            base.OnEnter();
        }

        /// <summary>The delete-mode toggle clicked: switches delete mode on or off.</summary>
        public void ToggleDeleteMode()
        {
            IsDeleteMode = !IsDeleteMode;
            Element.SetDeleteMode(IsDeleteMode);
        }

        /// <summary>
        /// A save clicked: in delete mode, asks and deletes it, then reloads the list (delete
        /// mode stays on); otherwise loads it (<see cref="UICategoryListMenuHandler{TMenu, THandler, TRenderer, TItem}.StartItem"/>).
        /// </summary>
        public override void ClickItem(SavedGame item)
        {
            if (IsDeleteMode)
                Catalog.ConfirmDelete(item, Reload);
            else
                base.ClickItem(item);
        }
    }
}

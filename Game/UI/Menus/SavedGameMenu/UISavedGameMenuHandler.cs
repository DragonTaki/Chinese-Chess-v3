/* ----- ----- ----- ----- */
// UISavedGameMenuHandler.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/01
// Update Date: 2026/10/01
// Version: v1.0
/* ----- ----- ----- ----- */

using System;
using System.Collections.Generic;

using Chinese_Chess_v3.Game.Configs;
using Chinese_Chess_v3.Game.Core;
using Chinese_Chess_v3.Game.Core.Saves;
using Chinese_Chess_v3.Game.UI.Constants;
using Chinese_Chess_v3.Game.UI.Menus.CategoryListMenu;

namespace Chinese_Chess_v3.Game.UI.Menus.SavedGameMenu
{
    /// <summary>
    /// The saved-game list's logic (shared part:
    /// <see cref="UICategoryListMenuHandler{TMenu, THandler, TRenderer, TItem}"/>): loads every
    /// save with <see cref="GameSaveFiles.LoadAll"/> (categories = mode folders) and loads the
    /// chosen one with <see cref="GameManager.LoadSavedGame"/>, then tells the game menu
    /// (<see cref="ItemStarted"/>) so it closes the list. Opened and closed by
    /// <c>UIGameMenuHandler</c>, which calls <see cref="UICategoryListMenuHandler{TMenu, THandler, TRenderer, TItem}.OnEnter"/>
    /// to reload the files each time.
    /// </summary>
    public class UISavedGameMenuHandler : UICategoryListMenuHandler<UISavedGameMenu, UISavedGameMenuHandler, UISavedGameMenuRenderer, SavedGame>
    {
        public UISavedGameMenuHandler() { }

        /// <summary>Called after a save was clicked and loaded (or could not be); the game menu closes the list.</summary>
        public Action ItemStarted { get; set; }

        /// <summary>The saves folder (<see cref="SystemSettings.SavesFolder"/>), created by <see cref="GameSaveFiles.LoadAll"/> when missing.</summary>
        public override string UserFolder => SystemSettings.SavesFolder;

        protected override string LogLabel => "Load";

        protected override string EmptyMessageText => string.Format(GameMenuTexts.NoSavedGamesFormat, UserFolder);

        protected override IEnumerable<SavedGame> LoadItems(List<string> warnings) =>
            GameSaveFiles.LoadAll(warnings);

        /// <summary>The save's start position and moves replayed, with its rules and clocks (<see cref="GameManager.LoadSavedGame"/>).</summary>
        protected override void StartOnBoard(GameManager gameManager, SavedGame saved) =>
            gameManager.LoadSavedGame(saved);

        protected override void OnItemStarted(SavedGame item) => ItemStarted?.Invoke();
    }
}

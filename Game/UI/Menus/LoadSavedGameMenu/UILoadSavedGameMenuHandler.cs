/* ----- ----- ----- ----- */
// UILoadSavedGameMenuHandler.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/02
// Update Date: 2026/10/05
// Version: v1.0
/* ----- ----- ----- ----- */

using System;
using System.Collections.Generic;

using Chinese_Chess_v3.Game.Application.Catalogs;
using Chinese_Chess_v3.Game.Application.Session;
using Chinese_Chess_v3.Game.Core;
using Chinese_Chess_v3.Game.Core.Saves;

using Engine.Diagnostics;
using Engine.Logging;
using Engine.UI.Core.Handlers;
using Engine.UI.Core.Interfaces;

using Microsoft.Extensions.DependencyInjection;

namespace Chinese_Chess_v3.Game.UI.Menus.LoadSavedGameMenu
{
    /// <summary>
    /// The main menu's saved-game list's binding (讀取存檔): loads the player's saves with
    /// <see cref="SavedGameCatalog.LoadAll"/> (the same loader as the game screen's list), shows
    /// them grouped by mode, newest first within a group (<see cref="SavedGameCatalog.Group"/>),
    /// and starts a clicked save on the game screen the way the game screen's list does
    /// (<see cref="GameManager.LoadSavedGame"/>).
    /// <see cref="IScreen"/>: the main menu calls <see cref="OnEnter"/>/<see cref="OnExit"/>
    /// when it opens/closes the submenu.
    /// </summary>
    public class UILoadSavedGameMenuHandler : UIMenuHandler<UILoadSavedGameMenu, UILoadSavedGameMenuHandler, UILoadSavedGameMenuRenderer>, IScreen
    {
        public UILoadSavedGameMenuHandler() { }

        /// <summary>Submenu opened: reload the files, so games saved meanwhile appear.</summary>
        public void OnEnter() => Reload();

        public void OnExit() { }

        /// <summary>Loads every save (bad files skipped with a warning) and rebuilds the rows.</summary>
        public void Reload()
        {
            var warnings = new List<string>();
            var saves = SavedGameCatalog.LoadAll(warnings);
            foreach (var warning in warnings)
                AppLogger.Log($"(LoadMenu) {warning}", LogLevel.WARN);
            AppLogger.Log($"(LoadMenu) Loaded {saves.Count} save(s), {warnings.Count} warning(s); folder: {SavedGameCatalog.Folder}", LogLevel.DEBUG);

            Element.ShowGroups(SavedGameCatalog.Group(saves));
        }

        /// <summary>
        /// A save clicked: started the way a new game is (<see cref="GameSession.TryStart"/>:
        /// game screen, restart and views reset, then the save replayed with
        /// <see cref="GameManager.LoadSavedGame"/>; a bad save is logged) - the same steps as the
        /// game screen's list (<c>UICategoryListMenuHandler.StartItem</c>).
        /// </summary>
        public void StartSave(SavedGame saved)
        {
            ArgumentNullException.ThrowIfNull(saved);
            if (DebugOptions.ConsoleTrace)
                Console.WriteLine($"LoadMenu: selected: {saved}");

            _factory.ServiceProvider.GetRequiredService<GameSession>()
                .TryStart(game => game.LoadSavedGame(saved), "LoadMenu", saved.FileName);
        }
    }
}

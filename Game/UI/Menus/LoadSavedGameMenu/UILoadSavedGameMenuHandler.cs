/* ----- ----- ----- ----- */
// UILoadSavedGameMenuHandler.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/02
// Update Date: 2026/10/04
// Version: v1.0
/* ----- ----- ----- ----- */

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

using Chinese_Chess_v3.Game.Application.Session;
using Chinese_Chess_v3.Game.Configs;
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
    /// The main menu's saved-game list's logic (讀取存檔): loads the player's saves with
    /// <see cref="GameSaveFiles.LoadAll"/> (the same loader as the game screen's list), groups
    /// them by the mode each save records (<see cref="SavedGame.Mode"/>, the <c>[Event]</c>
    /// tag: 對局 / 殘局 / 開局, <see cref="SystemSettings.SaveModeName"/>) newest first within
    /// a group (<see cref="Group"/>), and starts a clicked save on the game screen the way the
    /// game screen's list does (<see cref="GameManager.LoadSavedGame"/>).
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
            var saves = GameSaveFiles.LoadAll(warnings);
            foreach (var warning in warnings)
                AppLogger.Log($"(LoadMenu) {warning}", LogLevel.WARN);
            AppLogger.Log($"(LoadMenu) Loaded {saves.Count} save(s), {warnings.Count} warning(s); folder: {SystemSettings.SavesFolder}", LogLevel.DEBUG);

            Element.ShowGroups(Group(saves));
        }

        /// <summary>
        /// <paramref name="saves"/> grouped by <see cref="SavedGame.Mode"/> in
        /// <see cref="GameMode"/> order (對局, 殘局, 開局), headed by
        /// <see cref="SystemSettings.SaveModeName"/>; empty groups left out. Within a group
        /// newest first by <see cref="SaveTime"/>, then file name (ordinal, descending: the
        /// later <c>_2</c> of two saves in one second first).
        /// </summary>
        public static IReadOnlyList<(string Header, IReadOnlyList<SavedGame> Saves)> Group(IEnumerable<SavedGame> saves)
        {
            ArgumentNullException.ThrowIfNull(saves);
            var list = saves.Where(s => s != null).ToList();
            var groups = new List<(string Header, IReadOnlyList<SavedGame> Saves)>();
            foreach (var mode in Enum.GetValues<GameMode>())
            {
                var inMode = list.Where(s => s.Mode == mode)
                    .OrderByDescending(SaveTime)
                    .ThenByDescending(s => s.FileName, StringComparer.Ordinal)
                    .ToList();
                if (inMode.Count > 0)
                    groups.Add((SystemSettings.SaveModeName(mode), inMode));
            }
            return groups;
        }

        /// <summary>
        /// When <paramref name="saved"/> was saved: the time stamp of its file name
        /// (<see cref="SystemSettings.TryParseSaveFileName"/>), else its <c>[Date]</c> tag
        /// (<c>yyyy.MM.dd</c>), else <see cref="DateTime.MinValue"/> (listed last).
        /// </summary>
        public static DateTime SaveTime(SavedGame saved)
        {
            if (saved == null)
                return DateTime.MinValue;
            if (SystemSettings.TryParseSaveFileName(saved.FileName, out _, out var time))
                return time;
            if (DateTime.TryParseExact(saved.Date, "yyyy.MM.dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
                return date;
            return DateTime.MinValue;
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

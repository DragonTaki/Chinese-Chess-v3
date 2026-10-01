/* ----- ----- ----- ----- */
// UIEndgameMenuHandler.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/01
// Update Date: 2026/10/01
// Version: v1.0
/* ----- ----- ----- ----- */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Chinese_Chess_v3.Game.Core;
using Chinese_Chess_v3.Game.Core.Endgames;
using Chinese_Chess_v3.Game.UI.Menus.GameMenu;

using Engine.Logging;
using Engine.UI.Core.Handlers;
using Engine.UI.Core.Interfaces;

using Microsoft.Extensions.DependencyInjection;

namespace Chinese_Chess_v3.Game.UI.Menus.EndgameMenu
{
    /// <summary>
    /// Loads the endgame puzzles when the submenu is shown, keeps the category filter and
    /// starts a puzzle when its button is clicked. <see cref="IScreen"/>: the main menu calls
    /// <see cref="OnEnter"/>/<see cref="OnExit"/> when it opens/closes this submenu.
    /// </summary>
    public class UIEndgameMenuHandler : UIMenuHandler<UIEndgameMenu, UIEndgameMenuHandler, UIEndgameMenuRenderer>, IScreen
    {
        /// <summary>Folder name of the game under the per-user application data folder.</summary>
        public const string AppDataFolderName = "Chinese-Chess-v3";

        /// <summary>The built-in puzzles: <c>Assets/Endgames</c> next to the game (copied there by the build).</summary>
        public static string BuiltInFolder =>
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "Endgames");

        /// <summary>
        /// The player's own puzzles: <c>Endgames</c> in the game's per-user application data
        /// folder (Windows: <c>%APPDATA%\Chinese-Chess-v3\Endgames</c>; macOS/Linux: under
        /// <c>~/.config</c>). Created by <see cref="EndgameLoader.LoadAll"/> when missing.
        /// </summary>
        public static string UserFolder =>
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), AppDataFolderName, "Endgames");

        /// <summary>Categories the player switched off (all on by default; kept while the game runs).</summary>
        private readonly HashSet<string> _hiddenCategories = new(StringComparer.Ordinal);

        /// <summary>The puzzles last loaded, in display order.</summary>
        public IReadOnlyList<EndgamePuzzle> Puzzles { get; private set; } = Array.Empty<EndgamePuzzle>();

        public UIEndgameMenuHandler() { }

        /// <summary>Submenu opened: reload the files, so puzzles added meanwhile appear.</summary>
        public void OnEnter() => Reload();

        public void OnExit() { }

        /// <summary>
        /// Loads both folders and rebuilds the buttons. Sorted by category, then file name
        /// (ordinal, as <see cref="EndgameLoader"/> sorts each folder), built-in before the
        /// player's own on a tie: the loader returns the two folders one after the other,
        /// so a category present in both would otherwise be split in two.
        /// </summary>
        public void Reload()
        {
            var warnings = new List<string>();
            Puzzles = EndgameLoader.LoadAll(BuiltInFolder, UserFolder, warnings)
                .OrderBy(p => p.Category ?? string.Empty, StringComparer.Ordinal)
                .ThenBy(p => p.FileName, StringComparer.Ordinal)
                .ThenBy(p => p.Origin)
                .ToList();

            AppLogger.Log($"(Endgame) Loaded {Puzzles.Count} puzzle(s), {warnings.Count} warning(s); user folder: {UserFolder}", LogLevel.DEBUG);

            Element.ShowPuzzles(Puzzles, IsCategoryShown, EmptyMessageText);
        }

        private string EmptyMessageText =>
            $"找不到殘局題目。\n可以把題目檔（.pgn）放到：\n{UserFolder}";

        public bool IsCategoryShown(string category) => !_hiddenCategories.Contains(category ?? string.Empty);

        /// <summary>Category toggle clicked: hide its puzzles if shown, show them if hidden.</summary>
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
        /// Puzzle clicked: switch to the game screen the same way a new game does (reset the
        /// game UI), then set the puzzle's position up.
        /// <para>
        /// The board is drawn as for any game (red at the bottom); turning it so the side to
        /// move is at the bottom is phase C (docs/PLAN.md).
        /// </para>
        /// </summary>
        public void StartPuzzle(EndgamePuzzle puzzle)
        {
            Console.WriteLine($"EndgameMenu: selected: {puzzle}");

            var gameMenu = _navigationManager.Show<UIGameMenu, UIGameMenuHandler, UIGameMenuRenderer>();
            var gameManager = _factory.ServiceProvider.GetRequiredService<GameManager>();

            // Reset first (clears the log, resets the board to the default position), then the
            // puzzle's position - StartEndgame's own log line stays.
            gameMenu.ResetGameUI();

            try
            {
                gameManager.StartEndgame(puzzle);
            }
            catch (FormatException ex)
            {
                // Puzzles from EndgameLoader were already validated; keep the default board.
                AppLogger.Log($"(Endgame) cannot start {puzzle.FileName}: {ex.Message}", LogLevel.ERROR);
            }
        }
    }
}

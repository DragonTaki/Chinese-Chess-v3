/* ----- ----- ----- ----- */
// EndgameCatalog.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/05
// Update Date: 2026/10/05
// Version: v1.0
/* ----- ----- ----- ----- */

using System.Collections.Generic;

using Chinese_Chess_v3.Game.Configs;
using Chinese_Chess_v3.Game.Core.Endgames;

namespace Chinese_Chess_v3.Game.Application.Catalogs
{
    /// <summary>
    /// The endgame puzzles (殘局闖關): the built-in folder and the player's own, loaded with
    /// <see cref="EndgameLoader.LoadAll"/> into <see cref="List"/> (by category, no sections).
    /// A single long-lived instance, so the switched-off categories are kept while the game runs.
    /// </summary>
    public sealed class EndgameCatalog
    {
        private readonly PlayerSettings _settings;

        /// <summary>Creates the catalog; nothing is loaded until <c>List.Reload</c>.</summary>
        /// <param name="settings">The player's settings (the user folder); null for the defaults.</param>
        public EndgameCatalog(PlayerSettings settings)
        {
            _settings = settings ?? PlayerSettings.Defaults;
            List = new CategoryListModel<EndgamePuzzle>(LoadAll);
        }

        /// <summary>The built-in puzzles (<see cref="SystemSettings.BuiltInEndgameFolder"/>).</summary>
        public static string BuiltInFolder => SystemSettings.BuiltInEndgameFolder;

        /// <summary>
        /// The player's own puzzles: <see cref="PlayerSettings.ResolvedEndgameUserFolder"/>
        /// (<c>[endgame] user_folder</c> in settings.ini; empty there means
        /// <see cref="SystemSettings.DefaultUserEndgameFolder"/>, <c>Endgames</c> in the
        /// game's per-user data folder). Created by <see cref="EndgameLoader.LoadAll"/> when missing.
        /// Read on every load, so a changed setting applies the next time the list is opened.
        /// </summary>
        public string UserFolder => _settings.ResolvedEndgameUserFolder;

        /// <summary>The puzzles as a category list.</summary>
        public CategoryListModel<EndgamePuzzle> List { get; }

        /// <summary>Loads both folders (built-in, then the player's), adding to <paramref name="warnings"/>.</summary>
        public IEnumerable<EndgamePuzzle> LoadAll(List<string> warnings) =>
            EndgameLoader.LoadAll(BuiltInFolder, UserFolder, warnings);
    }
}

/* ----- ----- ----- ----- */
// OpeningCatalog.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/05
// Update Date: 2026/10/05
// Version: v1.0
/* ----- ----- ----- ----- */

using System.Collections.Generic;

using Chinese_Chess_v3.Game.Configs;
using Chinese_Chess_v3.Game.Core.Openings;
using Chinese_Chess_v3.Game.Core.Players;

namespace Chinese_Chess_v3.Game.Application.Catalogs
{
    /// <summary>
    /// The openings (開局練習): the built-in folder and the player's own, loaded with
    /// <see cref="OpeningLoader.LoadAll"/> into <see cref="List"/>, in a 先手 and a 後手 section
    /// (<see cref="SectionOf"/>), each by category. A single long-lived instance, so the
    /// switched-off categories are kept while the game runs.
    /// </summary>
    public sealed class OpeningCatalog
    {
        /// <summary>The section of the openings Player1 (the first mover) practises.</summary>
        public const string FirstMoverSection = "先手";

        /// <summary>The section of the openings Player2 practises.</summary>
        public const string SecondMoverSection = "後手";

        private readonly PlayerSettings _settings;

        /// <summary>Creates the catalog; nothing is loaded until <c>List.Reload</c>.</summary>
        /// <param name="settings">The player's settings (the user folder); null for the defaults.</param>
        public OpeningCatalog(PlayerSettings settings)
        {
            _settings = settings ?? PlayerSettings.Defaults;
            List = new CategoryListModel<OpeningLine>(LoadAll, SectionOf);
        }

        /// <summary>The built-in openings (<see cref="SystemSettings.BuiltInOpeningFolder"/>).</summary>
        public static string BuiltInFolder => SystemSettings.BuiltInOpeningFolder;

        /// <summary>
        /// The player's own openings: <see cref="PlayerSettings.ResolvedOpeningUserFolder"/>
        /// (<c>[opening] user_folder</c> in settings.ini; empty there means
        /// <see cref="SystemSettings.DefaultUserOpeningFolder"/>, <c>Openings</c> in the
        /// game's per-user data folder). Created by <see cref="OpeningLoader.LoadAll"/> when missing.
        /// Read on every load, so a changed setting applies the next time the list is opened.
        /// </summary>
        public string UserFolder => _settings.ResolvedOpeningUserFolder;

        /// <summary>The openings as a category list with sections.</summary>
        public CategoryListModel<OpeningLine> List { get; }

        /// <summary>
        /// The section: <see cref="FirstMoverSection"/> (Player1) or
        /// <see cref="SecondMoverSection"/> (Player2), from the opening's <c>PlayerSide</c>.
        /// Ordinal order puts 先手 before 後手 (先 &lt; 後).
        /// </summary>
        public static string SectionOf(OpeningLine opening) =>
            opening.PlayerSide == PlayerSide.Player1 ? FirstMoverSection : SecondMoverSection;

        /// <summary>Loads both folders (built-in, then the player's), adding to <paramref name="warnings"/>.</summary>
        public IEnumerable<OpeningLine> LoadAll(List<string> warnings) =>
            OpeningLoader.LoadAll(BuiltInFolder, UserFolder, warnings);
    }
}

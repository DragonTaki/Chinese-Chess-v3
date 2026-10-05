/* ----- ----- ----- ----- */
// UIEndgameMenuHandler.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/01
// Update Date: 2026/10/05
// Version: v1.2
/* ----- ----- ----- ----- */

using System.Collections.Generic;

using Chinese_Chess_v3.Game.Application.Catalogs;
using Chinese_Chess_v3.Game.Application.Session;
using Chinese_Chess_v3.Game.Configs;
using Chinese_Chess_v3.Game.Core.Endgames;
using Chinese_Chess_v3.Game.UI.Menus.CategoryListMenu;

using Microsoft.Extensions.DependencyInjection;

namespace Chinese_Chess_v3.Game.UI.Menus.EndgameMenu
{
    /// <summary>
    /// The endgame submenu's binding (shared part:
    /// <see cref="UICategoryListMenuHandler{TMenu, THandler, TRenderer, TItem}"/>): lists the
    /// puzzles of the <see cref="EndgameCatalog"/> and starts one with
    /// <see cref="GameSession.StartEndgame"/>.
    /// </summary>
    public class UIEndgameMenuHandler : UICategoryListMenuHandler<UIEndgameMenu, UIEndgameMenuHandler, UIEndgameMenuRenderer, EndgamePuzzle>
    {
        /// <summary>Folder name of the game under the per-user application data folder (<see cref="SystemSettings.AppDataFolderName"/>).</summary>
        public const string AppDataFolderName = SystemSettings.AppDataFolderName;

        /// <summary>The built-in puzzles (<see cref="EndgameCatalog.BuiltInFolder"/>).</summary>
        public static string BuiltInFolder => EndgameCatalog.BuiltInFolder;

        /// <summary>The player's own puzzles (<see cref="EndgameCatalog.UserFolder"/>).</summary>
        public override string UserFolder => Catalog.UserFolder;

        /// <summary>The puzzles last loaded, in display order (<see cref="UICategoryListMenuHandler{TMenu, THandler, TRenderer, TItem}.Items"/>).</summary>
        public IReadOnlyList<EndgamePuzzle> Puzzles => Items;

        public UIEndgameMenuHandler() { }

        /// <summary>The endgame catalog registered in DI.</summary>
        private EndgameCatalog Catalog => _factory.ServiceProvider.GetRequiredService<EndgameCatalog>();

        protected override CategoryListModel<EndgamePuzzle> Model => Catalog.List;

        protected override string LogLabel => "Endgame";

        protected override string EmptyMessageText =>
            $"找不到殘局題目。\n可以把題目檔（.pgn）放到：\n{UserFolder}";

        /// <summary>The puzzle's position, with the side to move from its FEN; the solution is not played.</summary>
        protected override void StartInSession(GameSession session, EndgamePuzzle puzzle) =>
            session.StartEndgame(puzzle, LogLabel);

        /// <summary>Puzzle clicked (<see cref="UICategoryListMenuHandler{TMenu, THandler, TRenderer, TItem}.StartItem"/>).</summary>
        public void StartPuzzle(EndgamePuzzle puzzle) => StartItem(puzzle);
    }
}

/* ----- ----- ----- ----- */
// UIEndgameMenuHandler.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/01
// Update Date: 2026/10/01
// Version: v1.1
/* ----- ----- ----- ----- */

using System.Collections.Generic;

using Chinese_Chess_v3.Game.Configs;
using Chinese_Chess_v3.Game.Core;
using Chinese_Chess_v3.Game.Core.Endgames;
using Chinese_Chess_v3.Game.UI.Menus.CategoryListMenu;

using Microsoft.Extensions.DependencyInjection;

namespace Chinese_Chess_v3.Game.UI.Menus.EndgameMenu
{
    /// <summary>
    /// The endgame submenu's logic (shared part:
    /// <see cref="UICategoryListMenuHandler{TMenu, THandler, TRenderer, TItem}"/>): loads the
    /// puzzles with <see cref="EndgameLoader.LoadAll"/> and starts one with
    /// <see cref="GameManager.StartEndgame"/>.
    /// </summary>
    public class UIEndgameMenuHandler : UICategoryListMenuHandler<UIEndgameMenu, UIEndgameMenuHandler, UIEndgameMenuRenderer, EndgamePuzzle>
    {
        /// <summary>Folder name of the game under the per-user application data folder (<see cref="SystemSettings.AppDataFolderName"/>).</summary>
        public const string AppDataFolderName = SystemSettings.AppDataFolderName;

        /// <summary>The built-in puzzles (<see cref="SystemSettings.BuiltInEndgameFolder"/>).</summary>
        public static string BuiltInFolder => SystemSettings.BuiltInEndgameFolder;

        /// <summary>
        /// The player's own puzzles: <see cref="PlayerSettings.ResolvedEndgameUserFolder"/>
        /// (<c>[endgame] user_folder</c> in settings.ini; empty there means
        /// <see cref="SystemSettings.DefaultUserEndgameFolder"/>, <c>Endgames</c> in the
        /// game's per-user data folder). Created by <see cref="EndgameLoader.LoadAll"/> when missing.
        /// </summary>
        public override string UserFolder =>
            (_factory?.ServiceProvider.GetService<PlayerSettings>() ?? PlayerSettings.Defaults).ResolvedEndgameUserFolder;

        /// <summary>The puzzles last loaded, in display order (<see cref="UICategoryListMenuHandler{TMenu, THandler, TRenderer, TItem}.Items"/>).</summary>
        public IReadOnlyList<EndgamePuzzle> Puzzles => Items;

        public UIEndgameMenuHandler() { }

        protected override string LogLabel => "Endgame";

        protected override string EmptyMessageText =>
            $"找不到殘局題目。\n可以把題目檔（.pgn）放到：\n{UserFolder}";

        protected override IEnumerable<EndgamePuzzle> LoadItems(List<string> warnings) =>
            EndgameLoader.LoadAll(BuiltInFolder, UserFolder, warnings);

        /// <summary>The puzzle's position, with the side to move from its FEN; the solution is not played.</summary>
        protected override void StartOnBoard(GameManager gameManager, EndgamePuzzle puzzle) =>
            gameManager.StartEndgame(puzzle);

        /// <summary>Puzzle clicked (<see cref="UICategoryListMenuHandler{TMenu, THandler, TRenderer, TItem}.StartItem"/>).</summary>
        public void StartPuzzle(EndgamePuzzle puzzle) => StartItem(puzzle);
    }
}

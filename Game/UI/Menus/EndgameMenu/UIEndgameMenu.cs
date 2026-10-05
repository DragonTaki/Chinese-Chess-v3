/* ----- ----- ----- ----- */
// UIEndgameMenu.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/01
// Update Date: 2026/10/05
// Version: v1.1
/* ----- ----- ----- ----- */

using Chinese_Chess_v3.Game.Application.Catalogs;
using Chinese_Chess_v3.Game.Core.Endgames;
using Chinese_Chess_v3.Game.UI.Menus.CategoryListMenu;

namespace Chinese_Chess_v3.Game.UI.Menus.EndgameMenu
{
    /// <summary>
    /// The endgame challenge submenu (殘局闖關): a category list submenu
    /// (<see cref="UICategoryListMenu{TMenu, THandler, TRenderer, TItem}"/>: category toggles,
    /// then the puzzle buttons) whose buttons show the puzzle's name and its difficulty stars.
    /// </summary>
    public class UIEndgameMenu : UICategoryListMenu<UIEndgameMenu, UIEndgameMenuHandler, UIEndgameMenuRenderer, EndgamePuzzle>
    {
        public UIEndgameMenu() { }

        /// <summary>Two lines: the name, then the difficulty stars (e.g. <c>雙車錯殺</c> / <c>★★☆☆☆</c>).</summary>
        protected override string ItemButtonText(EndgamePuzzle puzzle) =>
            $"{puzzle.Title}\n{DifficultyStars(puzzle.Difficulty)}";

        /// <summary>Same as <see cref="UICategoryListMenu{TMenu, THandler, TRenderer, TItem}.ShowItems"/>.</summary>
        public void ShowPuzzles(CategoryListModel<EndgamePuzzle> puzzles, string emptyMessage) =>
            ShowItems(puzzles, emptyMessage);
    }
}

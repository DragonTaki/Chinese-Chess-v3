/* ----- ----- ----- ----- */
// UIOpeningMenu.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/01
// Update Date: 2026/10/01
// Version: v1.0
/* ----- ----- ----- ----- */

using Chinese_Chess_v3.Game.Core.Openings;
using Chinese_Chess_v3.Game.Core.Players;
using Chinese_Chess_v3.Game.UI.Constants;
using Chinese_Chess_v3.Game.UI.Menus.CategoryListMenu;

namespace Chinese_Chess_v3.Game.UI.Menus.OpeningMenu
{
    /// <summary>
    /// The opening practice submenu (開局練習, docs/OPENINGS.md): a category list submenu
    /// (<see cref="UICategoryListMenu{TMenu, THandler, TRenderer, TItem}"/>: category toggles,
    /// then the opening buttons, same layout as 殘局闖關, but in a 先手 and a 後手 section
    /// with a heading each) whose buttons show the opening's
    /// name, wrapped onto a second line when long, and difficulty stars only when the file
    /// sets <c>[Difficulty]</c>.
    /// </summary>
    public class UIOpeningMenu : UICategoryListMenu<UIOpeningMenu, UIOpeningMenuHandler, UIOpeningMenuRenderer, OpeningLine>
    {
        public UIOpeningMenu() { }

        /// <summary>
        /// The section: <c>先手</c> (Player1) or <c>後手</c> (Player2), from the opening's
        /// <c>PlayerSide</c>. Ordinal order puts 先手 before 後手 (先 &lt; 後).
        /// </summary>
        public override string SectionOf(OpeningLine opening) =>
            opening.PlayerSide == PlayerSide.Player1 ? "先手" : "後手";

        protected override string ItemButtonText(OpeningLine opening)
        {
            string title = WrapTitle(opening.Title, UILayoutStyles.CategoryListMenu.TitleLineLength);
            return opening.Difficulty is int difficulty ? $"{title}\n{DifficultyStars(difficulty)}" : title;
        }
    }
}

/* ----- ----- ----- ----- */
// UIOpeningMenu.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/01
// Update Date: 2026/10/01
// Version: v1.0
/* ----- ----- ----- ----- */

using Chinese_Chess_v3.Game.Core.Openings;
using Chinese_Chess_v3.Game.Core.Pieces;
using Chinese_Chess_v3.Game.Core.Players;
using Chinese_Chess_v3.Game.UI.Constants;
using Chinese_Chess_v3.Game.UI.Menus.CategoryListMenu;

namespace Chinese_Chess_v3.Game.UI.Menus.OpeningMenu
{
    /// <summary>
    /// The opening practice submenu (開局練習, docs/OPENINGS.md): a category list submenu
    /// (<see cref="UICategoryListMenu{TMenu, THandler, TRenderer, TItem}"/>: category toggles,
    /// then the opening buttons, same layout as 殘局闖關) whose buttons show the opening's
    /// name, wrapped onto a second line when long, and difficulty stars only when the file
    /// sets <c>[Difficulty]</c>.
    /// </summary>
    public class UIOpeningMenu : UICategoryListMenu<UIOpeningMenu, UIOpeningMenuHandler, UIOpeningMenuRenderer, OpeningLine>
    {
        public UIOpeningMenu() { }

        /// <summary>
        /// <c>先手{category}（執紅）</c> / <c>後手{category}（執黑）</c>: 先手／後手 from the
        /// opening's <c>PlayerSide</c> (Player1 / Player2), 紅／黑 from that player's colour in
        /// the opening's position. All 先手 groups sort before the 後手 ones (ordinal: 先 &lt; 後).
        /// </summary>
        public override string CategoryOf(OpeningLine opening)
        {
            string order = opening.PlayerSide == PlayerSide.Player1 ? "先手" : "後手";
            string color = opening.ColorOf(opening.PlayerSide) == PieceColor.Red ? "執紅" : "執黑";
            return $"{order}{opening.Category}（{color}）";
        }

        protected override string ItemButtonText(OpeningLine opening)
        {
            string title = WrapTitle(opening.Title, UILayoutStyles.CategoryListMenu.TitleLineLength);
            return opening.Difficulty is int difficulty ? $"{title}\n{DifficultyStars(difficulty)}" : title;
        }
    }
}

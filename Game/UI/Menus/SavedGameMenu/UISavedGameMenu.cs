/* ----- ----- ----- ----- */
// UISavedGameMenu.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/01
// Update Date: 2026/10/01
// Version: v1.0
/* ----- ----- ----- ----- */

using System.Globalization;
using System.Text;

using Chinese_Chess_v3.Game.Configs;
using Chinese_Chess_v3.Game.Core.Saves;
using Chinese_Chess_v3.Game.UI.Constants;
using Chinese_Chess_v3.Game.UI.Menus.CategoryListMenu;

using Engine.UI.Core.Interfaces;

namespace Chinese_Chess_v3.Game.UI.Menus.SavedGameMenu
{
    /// <summary>
    /// The saved-game list (載入, docs/PLAN.md in-game menu): a category list submenu
    /// (<see cref="UICategoryListMenu{TMenu, THandler, TRenderer, TItem}"/>, same layout as
    /// 殘局闖關 / 開局練習; categories = the mode folders 對局 / 殘局 / 開局) shown on the game
    /// screen by the game menu, in the board's place (<see cref="UILayoutSheet.GameScreen.SavedGameList"/>). A button shows the save's name, its
    /// date and its time, read from the file name (<see cref="SystemSettings.TryParseSaveFileName"/>);
    /// a file named otherwise shows its name wrapped like an opening's.
    /// </summary>
    public class UISavedGameMenu : UICategoryListMenu<UISavedGameMenu, UISavedGameMenuHandler, UISavedGameMenuRenderer, SavedGame>
    {
        public UISavedGameMenu() { }

        /// <summary>The category list layout, then placed in the board's area (<see cref="UILayoutSheet.GameScreen.SavedGameList"/>).</summary>
        protected override void OnInit(IUiFactory factory)
        {
            base.OnInit(factory);
            LayoutRules.Apply(UILayoutSheet.GameScreen.SavedGameList);
        }

        protected override string ItemButtonText(SavedGame saved)
        {
            if (!SystemSettings.TryParseSaveFileName(saved.Title, out string name, out var time))
                return WrapTitle(saved.Title, UILayoutStyles.CategoryListMenu.TitleLineLength);

            return $"{OneLine(name, UILayoutStyles.CategoryListMenu.TitleLineLength)}\n" +
                $"{time.ToString(GameMenuTexts.SavedGameDateFormat, CultureInfo.InvariantCulture)}\n" +
                $"{time.ToString(GameMenuTexts.SavedGameTimeFormat, CultureInfo.InvariantCulture)}";
        }

        /// <summary>
        /// <paramref name="text"/> cut to <paramref name="lineLength"/> characters (text
        /// elements) with <see cref="GameMenuTexts.Ellipsis"/> as the last one when longer.
        /// </summary>
        private static string OneLine(string text, int lineLength)
        {
            var info = new StringInfo(text ?? string.Empty);
            if (info.LengthInTextElements <= lineLength || lineLength < 2)
                return info.String;
            return new StringBuilder(info.SubstringByTextElements(0, lineLength - 1)).Append(GameMenuTexts.Ellipsis).ToString();
        }
    }
}

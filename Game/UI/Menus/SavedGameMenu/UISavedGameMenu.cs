/* ----- ----- ----- ----- */
// UISavedGameMenu.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/01
// Update Date: 2026/10/05
// Version: v1.2
/* ----- ----- ----- ----- */

using System.Globalization;
using System.Text;

using Chinese_Chess_v3.Game.Application.Catalogs;
using Chinese_Chess_v3.Game.Application.Texts;
using Chinese_Chess_v3.Game.Configs;
using Chinese_Chess_v3.Game.Core.Saves;
using Chinese_Chess_v3.Game.UI.Constants;
using Chinese_Chess_v3.Game.UI.Menus.CategoryListMenu;

using Engine.UI.Core.Elements;
using Engine.UI.Core.Interfaces;

namespace Chinese_Chess_v3.Game.UI.Menus.SavedGameMenu
{
    /// <summary>
    /// The saved-game list (載入): a category list submenu
    /// (<see cref="UICategoryListMenu{TMenu, THandler, TRenderer, TItem}"/>, same layout as
    /// 殘局闖關 / 開局練習; categories = the mode folders 對局 / 殘局 / 開局) shown on the game
    /// screen by the game menu, in the board's place (<see cref="UILayoutSheet.GameScreen.SavedGameList"/>). A button shows the save's name, its
    /// date and its time, read from the file name (<see cref="SystemSettings.TryParseSaveFileName"/>);
    /// a file named otherwise shows its name wrapped like an opening's.
    /// <para>
    /// After the category toggles comes the delete-mode toggle (刪除存檔 / 結束刪除, a category
    /// toggle's size): while it is on, a clicked save asks to be deleted instead of being loaded
    /// (the handler's <see cref="UISavedGameMenuHandler.IsDeleteMode"/>).
    /// </para>
    /// </summary>
    public class UISavedGameMenu : UICategoryListMenu<UISavedGameMenu, UISavedGameMenuHandler, UISavedGameMenuRenderer, SavedGame>
    {
        /// <summary>The delete-mode toggle of the current build (null while the list is empty).</summary>
        private UIButton _deleteToggle;

        public UISavedGameMenu() { }

        /// <summary>The delete-mode toggle, after the category toggles.</summary>
        protected override void AddCategoryRowButtons(UIButtonRow categoryRow)
        {
            _deleteToggle = CreateButton(UILayoutSheet.CategoryListMenu.CategoryButton, Handler.ToggleDeleteMode);
            _deleteToggle.Style = UILayoutStyles.CategoryListMenu.ButtonStyle;
            categoryRow.AddChild(_deleteToggle);
            SetDeleteMode(Handler.IsDeleteMode);
        }

        /// <summary>Shows the delete-mode toggle's label for <paramref name="on"/> (if the toggle is there).</summary>
        public void SetDeleteMode(bool on)
        {
            if (_deleteToggle == null || !Buttons.Contains(_deleteToggle))
                return;
            _deleteToggle.Text = MenuTexts.DeleteModeToggle(on);
        }

        /// <summary>The category list layout, then placed in the board's area (<see cref="UILayoutSheet.GameScreen.SavedGameList"/>).</summary>
        protected override void OnInit(IUiFactory factory)
        {
            base.OnInit(factory);
            LayoutRules.Apply(UILayoutSheet.GameScreen.SavedGameList);
        }

        protected override string ItemButtonText(SavedGame saved)
        {
            if (!SavedGameCatalog.TryGetNameAndTime(saved, out string name, out var time))
                return WrapTitle(saved.Title, UILayoutStyles.CategoryListMenu.TitleLineLength);

            return MenuTexts.SavedGameButton(OneLine(name, UILayoutStyles.CategoryListMenu.TitleLineLength), time);
        }

        /// <summary>
        /// <paramref name="text"/> cut to <paramref name="lineLength"/> characters (text
        /// elements) with <see cref="MenuTexts.Ellipsis"/> as the last one when longer.
        /// </summary>
        internal static string OneLine(string text, int lineLength)
        {
            var info = new StringInfo(text ?? string.Empty);
            if (info.LengthInTextElements <= lineLength || lineLength < 2)
                return info.String;
            return new StringBuilder(info.SubstringByTextElements(0, lineLength - 1)).Append(MenuTexts.Ellipsis).ToString();
        }
    }
}

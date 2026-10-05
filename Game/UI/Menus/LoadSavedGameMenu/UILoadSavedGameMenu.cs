/* ----- ----- ----- ----- */
// UILoadSavedGameMenu.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/02
// Update Date: 2026/10/05
// Version: v1.0
/* ----- ----- ----- ----- */

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

using Chinese_Chess_v3.Game.Application.Catalogs;
using Chinese_Chess_v3.Game.Core.Saves;
using Chinese_Chess_v3.Game.UI.Constants;
using Chinese_Chess_v3.Game.UI.Menus.SavedGameMenu;

using Engine.Styles;
using Engine.UI.Core.Elements;
using Engine.UI.Core.Handlers;
using Engine.UI.Core.Interfaces;
using Engine.UI.Core.Renderers;
using Engine.UI.Models;

namespace Chinese_Chess_v3.Game.UI.Menus.LoadSavedGameMenu
{
    /// <summary>
    /// The main menu's saved-game list (讀取存檔): the player's saved games - the same files
    /// as the game screen's list (<see cref="UISavedGameMenu"/>) - as a submenu column: per
    /// category a header, then one full-width button per save (name, date and time from the
    /// file name, <see cref="SavedGameCatalog.TryGetNameAndTime"/>), or a disabled
    /// 沒有存檔 row when there is none. The grouping and the order are the catalog's
    /// (<see cref="SavedGameCatalog.Group"/>); loading and starting a save the handler's
    /// (<see cref="UILoadSavedGameMenuHandler"/>).
    /// <para>
    /// The rows are rebuilt each time the submenu is shown (<see cref="ShowGroups"/>), so
    /// games saved meanwhile appear.
    /// </para>
    /// <para>
    /// Above the first header is the delete-mode toggle (刪除存檔 / 結束刪除): while it is on, a
    /// clicked save asks to be deleted instead of being loaded (the handler's
    /// <see cref="UILoadSavedGameMenuHandler.IsDeleteMode"/>).
    /// </para>
    /// </summary>
    public class UILoadSavedGameMenu : UIMenu<UILoadSavedGameMenu, UILoadSavedGameMenuHandler, UILoadSavedGameMenuRenderer>
    {
        /// <summary>The delete-mode toggle row of the current build (null while there is no save).</summary>
        private UIButton _deleteToggle;

        public UILoadSavedGameMenu() { }

        protected override void OnBeforeInit(IUiFactory factory)
        {
            // Only used by UIMenu's legacy stacking; the rows are laid out by the flex gaps.
            ButtonSpacing = UILayoutConstants.LoadSavedGameMenu.RowGap;
        }

        protected override void OnInit(IUiFactory factory)
        {
            // Declared sizes (also the pre-layout fallback), then the layout rules that
            // actually place the panel and its scroll container (see UILayoutSheet.LoadSavedGameMenu).
            Layout = UILayoutConstants.Submenu.Layout;
            ScrollContainer.Layout = UILayoutConstants.Submenu.ScrollContainer.Layout;

            LayoutRules.Apply(UILayoutSheet.LoadSavedGameMenu.Panel);
            ScrollContainer.LayoutRules.Apply(UILayoutSheet.LoadSavedGameMenu.ScrollContainer);
        }

        /// <summary>Nothing at init: the rows come with <see cref="ShowGroups"/> when the submenu is shown.</summary>
        protected override void BuildButtons() { }

        /// <summary>
        /// Replaces every row with the delete-mode toggle (刪除存檔 / 結束刪除, a header-height
        /// button) and a header per group of <paramref name="groups"/> (in the given order)
        /// followed by its saves' buttons, or with the disabled 沒有存檔 row when there is no save.
        /// </summary>
        /// <param name="groups">The categories in display order, each with its saves in display order.</param>
        public void ShowGroups(IReadOnlyList<(string Header, IReadOnlyList<SavedGame> Saves)> groups)
        {
            ArgumentNullException.ThrowIfNull(groups);
            ClearRows();
            _deleteToggle = null;

            bool any = groups.Any(group => group.Saves != null && group.Saves.Count > 0);
            if (any)
            {
                _deleteToggle = CreateRow(UILayoutSheet.LoadSavedGameMenu.Header, UILayoutStyles.LoadSavedGameMenu.ButtonStyle, Handler.ToggleDeleteMode);
                SetDeleteMode(Handler.IsDeleteMode);
            }

            foreach (var (header, saves) in groups)
            {
                if (saves == null || saves.Count == 0)
                    continue;

                var headerRow = CreateRow(UILayoutSheet.LoadSavedGameMenu.Header, UILayoutStyles.LoadSavedGameMenu.HeaderStyle, null);
                headerRow.Text = header;

                foreach (var saved in saves)
                {
                    var target = saved;
                    var button = CreateRow(UILayoutSheet.LoadSavedGameMenu.Item, UILayoutStyles.LoadSavedGameMenu.ButtonStyle, () => Handler.ClickSave(target));
                    button.Text = ButtonText(saved);
                }
            }

            if (!any)
            {
                // Disabled: no action. (Not IsEnabled = false: UIMenu reuses IsEnabled for
                // its scroll culling and would switch it back on.)
                var empty = CreateRow(UILayoutSheet.LoadSavedGameMenu.Item, UILayoutStyles.LoadSavedGameMenu.EmptyRowStyle, null);
                empty.Text = GameMenuTexts.NoSavedGamesRow;
            }

            Handler.UpdateScrollContentHeight();
        }

        /// <summary>Shows the delete-mode toggle's label for <paramref name="on"/> (if the toggle is there).</summary>
        public void SetDeleteMode(bool on)
        {
            if (_deleteToggle == null)
                return;
            _deleteToggle.Text = on ? GameMenuTexts.DeleteModeOn : GameMenuTexts.DeleteModeOff;
        }

        /// <summary>
        /// A save's one-line label: name, date and time read from the file name
        /// (<see cref="GameMenuTexts.SavedGameRowFormat"/>); a file named otherwise shows its
        /// title. Names are cut to <c>UILayoutStyles.LoadSavedGameMenu.NameLength</c>.
        /// </summary>
        public static string ButtonText(SavedGame saved)
        {
            ArgumentNullException.ThrowIfNull(saved);
            int length = UILayoutStyles.LoadSavedGameMenu.NameLength;
            if (!SavedGameCatalog.TryGetNameAndTime(saved, out string name, out var time))
                return UISavedGameMenu.OneLine(saved.Title, length);

            return string.Format(CultureInfo.InvariantCulture, GameMenuTexts.SavedGameRowFormat,
                UISavedGameMenu.OneLine(name, length),
                time.ToString(GameMenuTexts.SavedGameDateFormat, CultureInfo.InvariantCulture),
                time.ToString(GameMenuTexts.SavedGameTimeFormat, CultureInfo.InvariantCulture));
        }

        /// <summary>A row button in the scroll column; <paramref name="onClick"/> null = does nothing (header, 沒有存檔).</summary>
        private UIButton CreateRow(UILayoutStyle rules, IButtonDrawStyle style, Action onClick)
        {
            var button = _factory.CreateElement<UIButton, UIButtonHandler, UIButtonRenderer>();
            button.Handler.Action = onClick;
            button.Style = style;
            button.LayoutRules.Apply(rules);
            ScrollContainer.AddChild(button);
            Buttons.Add(button);
            return button;
        }

        /// <summary>Disposes every row (which also detaches it).</summary>
        private void ClearRows()
        {
            foreach (var button in Buttons)
                button.Dispose();
            Buttons.Clear();
        }
    }
}

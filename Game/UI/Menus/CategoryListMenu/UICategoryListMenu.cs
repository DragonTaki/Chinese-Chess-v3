/* ----- ----- ----- ----- */
// UICategoryListMenu.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/01
// Update Date: 2026/10/01
// Version: v1.0
/* ----- ----- ----- ----- */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Chinese_Chess_v3.Game.Core.Pgn;
using Chinese_Chess_v3.Game.UI.Constants;

using Engine.Platform;
using Engine.UI.Constants.Core;
using Engine.UI.Core.Elements;
using Engine.UI.Core.Handlers;
using Engine.UI.Core.Interfaces;
using Engine.UI.Core.Renderers;
using Engine.UI.Models;

namespace Chinese_Chess_v3.Game.UI.Menus.CategoryListMenu
{
    /// <summary>
    /// A submenu listing PGN game files by category (殘局闖關 <c>UIEndgameMenu</c>, 開局練習
    /// <c>UIOpeningMenu</c>): a row of category toggle buttons, then one button per item, both
    /// as wrapping rows of <c>UILayoutConstants.CategoryListMenu.Columns</c> equal-width
    /// buttons inside the scroll container. A centered message replaces them when no item was
    /// found. A derived menu only decides an item button's text (<see cref="ItemButtonText"/>);
    /// loading and starting items is its handler's (<see cref="UICategoryListMenuHandler{TMenu, THandler, TRenderer, TItem}"/>).
    /// <para>
    /// The buttons are rebuilt from the item list each time the submenu is shown (the
    /// handler's <c>OnEnter</c> loads the files), so files added while the game runs appear
    /// the next time it opens. A category toggle only hides or shows that category's buttons
    /// (<c>Display = None</c>), nothing is rebuilt.
    /// </para>
    /// </summary>
    /// <typeparam name="TItem">The kind of file listed.</typeparam>
    public abstract class UICategoryListMenu<TMenu, THandler, TRenderer, TItem> : UIMenu<TMenu, THandler, TRenderer>
        where TMenu : UICategoryListMenu<TMenu, THandler, TRenderer, TItem>
        where THandler : UICategoryListMenuHandler<TMenu, THandler, TRenderer, TItem>
        where TRenderer : UIMenuRenderer<TMenu, THandler, TRenderer>
        where TItem : PgnGameFile
    {
        /// <summary>The category toggle buttons' row (first block of the scroll content).</summary>
        internal UIButtonRow CategoryRow { get; private set; }

        /// <summary>The item buttons (second block of the scroll content).</summary>
        internal UIButtonRow ItemGrid { get; private set; }

        /// <summary>The "nothing found" message, displayed only while the list is empty.</summary>
        internal UILabel EmptyMessage { get; private set; }

        private readonly Dictionary<string, UIButton> _categoryButtons = new(StringComparer.Ordinal);
        private readonly List<(UIButton Button, TItem Item)> _itemButtons = new();

        protected UICategoryListMenu() { }

        /// <summary>The label of <paramref name="item"/>'s button (line breaks allowed).</summary>
        protected abstract string ItemButtonText(TItem item);

        protected override void OnBeforeInit(IUiFactory factory)
        {
            // Only used by UIMenu's legacy stacking; the rows are laid out by the flex gaps.
            ButtonSpacing = UILayoutConstants.CategoryListMenu.RowGap;
        }

        protected override void BuildUIObjects()
        {
            CategoryRow = _factory.CreateElement<UIButtonRow, UIButtonRowHandler, UIButtonRowRenderer>();
            CategoryRow.LayoutRules.Apply(UILayoutSheet.CategoryListMenu.CategoryRow);
            ScrollContainer.AddChild(CategoryRow);

            ItemGrid = _factory.CreateElement<UIButtonRow, UIButtonRowHandler, UIButtonRowRenderer>();
            ItemGrid.LayoutRules.Apply(UILayoutSheet.CategoryListMenu.ItemGrid);
            ScrollContainer.AddChild(ItemGrid);

            EmptyMessage = _factory.CreateElement<UILabel, UILabelHandler, UILabelRenderer>();
            EmptyMessage.Font = UILayoutStyles.CategoryListMenu.EmptyMessage.Font;
            EmptyMessage.ForeColor = UILayoutStyles.CategoryListMenu.EmptyMessage.Color;
            EmptyMessage.TextAlign = ContentAlign.MiddleCenter;
            EmptyMessage.WordWrap = true;
            EmptyMessage.LayoutRules.Apply(UILayoutSheet.CategoryListMenu.EmptyMessage);
            EmptyMessage.LayoutRules.Display = DisplayMode.None;
            AddChild(EmptyMessage);
        }

        protected override void OnInit(IUiFactory factory)
        {
            // Declared sizes (also the pre-layout fallback), then the layout rules that
            // actually place the panel and its scroll container (see UILayoutSheet.CategoryListMenu).
            Layout = UILayoutConstants.Submenu.Layout;
            ScrollContainer.Layout = UILayoutConstants.Submenu.ScrollContainer.Layout;

            LayoutRules.Apply(UILayoutSheet.CategoryListMenu.Panel);
            ScrollContainer.LayoutRules.Apply(UILayoutSheet.CategoryListMenu.ScrollContainer);
        }

        /// <summary>
        /// Nothing at init: the items are loaded when the submenu is shown
        /// (<see cref="ShowItems"/>).
        /// </summary>
        protected override void BuildButtons() { }

        /// <summary>
        /// Replaces all buttons with one toggle per category present in
        /// <paramref name="items"/> (in list order) and one button per item, or shows the
        /// empty message when there is none.
        /// </summary>
        /// <param name="items">The items in display order (category, then file name).</param>
        /// <param name="isCategoryShown">Whether a category's items are currently shown.</param>
        /// <param name="emptyMessage">Text shown when <paramref name="items"/> is empty.</param>
        public void ShowItems(IReadOnlyList<TItem> items, Func<string, bool> isCategoryShown, string emptyMessage)
        {
            ClearItemButtons();

            foreach (var category in items.Select(p => p.Category ?? string.Empty).Distinct(StringComparer.Ordinal))
            {
                string name = category;
                var button = CreateButton(UILayoutSheet.CategoryListMenu.CategoryButton, () => Handler.ToggleCategory(name));
                CategoryRow.AddChild(button);
                _categoryButtons[name] = button;
                ApplyCategoryState(name, isCategoryShown(name));
            }

            foreach (var item in items)
            {
                var target = item;
                var button = CreateButton(UILayoutSheet.CategoryListMenu.ItemButton, () => Handler.StartItem(target));
                button.Text = ItemButtonText(item);
                button.Style = UILayoutStyles.CategoryListMenu.ButtonStyle;
                button.LayoutRules.Display = isCategoryShown(item.Category ?? string.Empty) ? DisplayMode.Normal : DisplayMode.None;
                ItemGrid.AddChild(button);
                _itemButtons.Add((button, item));
            }

            bool empty = items.Count == 0;
            EmptyMessage.Text = empty ? emptyMessage : string.Empty;
            EmptyMessage.LayoutRules.Display = empty ? DisplayMode.Normal : DisplayMode.None;
            CategoryRow.LayoutRules.Display = empty ? DisplayMode.None : DisplayMode.Normal;
            ItemGrid.LayoutRules.Display = empty ? DisplayMode.None : DisplayMode.Normal;

            Handler.UpdateScrollContentHeight();
        }

        /// <summary>
        /// Shows or hides <paramref name="category"/>'s item buttons and updates its toggle.
        /// The rows re-layout on their own (a <c>Display</c> change invalidates the layout),
        /// and the scroll content height follows.
        /// </summary>
        public void SetCategoryShown(string category, bool shown)
        {
            ApplyCategoryState(category, shown);
            foreach (var (button, item) in _itemButtons)
                if (string.Equals(item.Category ?? string.Empty, category, StringComparison.Ordinal))
                    button.LayoutRules.Display = shown ? DisplayMode.Normal : DisplayMode.None;

            Handler.UpdateScrollContentHeight();
        }

        /// <summary>The toggle's label (on/off mark + name) and look for its state.</summary>
        private void ApplyCategoryState(string category, bool shown)
        {
            if (!_categoryButtons.TryGetValue(category, out var button))
                return;

            string name = category.Length == 0 ? UILayoutStyles.CategoryListMenu.UncategorizedName : category;
            string mark = shown ? UILayoutStyles.CategoryListMenu.CategoryOnMark : UILayoutStyles.CategoryListMenu.CategoryOffMark;
            button.Text = $"{mark} {name}";
            button.Style = shown ? UILayoutStyles.CategoryListMenu.ButtonStyle : UILayoutStyles.CategoryListMenu.CategoryOff.Style;
        }

        /// <summary>
        /// <paramref name="difficulty"/> filled stars, then empty stars up to
        /// <c>MaxDifficulty</c> (e.g. 3 -> ★★★☆☆). Out-of-range values are clamped.
        /// </summary>
        public static string DifficultyStars(int difficulty)
        {
            int max = UILayoutStyles.CategoryListMenu.MaxDifficulty;
            int filled = Math.Clamp(difficulty, 0, max);
            var text = new StringBuilder(max);
            for (int i = 0; i < max; i++)
                text.Append(i < filled ? UILayoutStyles.CategoryListMenu.StarFilled : UILayoutStyles.CategoryListMenu.StarEmpty);
            return text.ToString();
        }

        /// <summary>
        /// <paramref name="title"/> broken into lines of at most
        /// <paramref name="lineLength"/> characters (text elements, so a surrogate pair is
        /// never split), e.g. 順炮直車對橫車 -> 順炮直車對 / 橫車 at 5.
        /// </summary>
        public static string WrapTitle(string title, int lineLength)
        {
            if (string.IsNullOrEmpty(title) || lineLength < 1)
                return title ?? string.Empty;

            var text = new StringBuilder(title.Length + 4);
            var elements = System.Globalization.StringInfo.GetTextElementEnumerator(title);
            int count = 0;
            while (elements.MoveNext())
            {
                if (count > 0 && count % lineLength == 0)
                    text.Append('\n');
                text.Append(elements.GetTextElement());
                count++;
            }
            return text.ToString();
        }

        private UIButton CreateButton(UILayoutStyle rules, Action onClick)
        {
            var button = _factory.CreateElement<UIButton, UIButtonHandler, UIButtonRenderer>();
            button.Handler.Action = onClick;
            button.LayoutRules.Apply(rules);
            Buttons.Add(button);
            return button;
        }

        /// <summary>Disposes every category and item button (which also detaches it).</summary>
        private void ClearItemButtons()
        {
            foreach (var button in Buttons)
                button.Dispose();
            Buttons.Clear();
            _categoryButtons.Clear();
            _itemButtons.Clear();
        }
    }
}

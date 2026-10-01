/* ----- ----- ----- ----- */
// UIEndgameMenu.cs
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

using Chinese_Chess_v3.Game.Core.Endgames;
using Chinese_Chess_v3.Game.UI.Constants;

using Engine.Platform;
using Engine.UI.Constants.Core;
using Engine.UI.Core.Elements;
using Engine.UI.Core.Handlers;
using Engine.UI.Core.Interfaces;
using Engine.UI.Core.Renderers;
using Engine.UI.Models;

namespace Chinese_Chess_v3.Game.UI.Menus.EndgameMenu
{
    /// <summary>
    /// The endgame challenge submenu (殘局闖關, docs/ENDGAMES.md): a row of category toggle
    /// buttons, then one button per puzzle (name and difficulty stars), both as wrapping
    /// rows of <c>UILayoutConstants.EndgameMenu.Columns</c> equal-width buttons inside the
    /// scroll container. A centered message replaces them when no puzzle was found.
    /// <para>
    /// The buttons are rebuilt from the puzzle list each time the submenu is shown
    /// (<see cref="UIEndgameMenuHandler.OnEnter"/> loads the files), so puzzles added while
    /// the game runs appear the next time it opens. A category toggle only hides or shows
    /// that category's buttons (<c>Display = None</c>), nothing is rebuilt.
    /// </para>
    /// </summary>
    public class UIEndgameMenu : UIMenu<UIEndgameMenu, UIEndgameMenuHandler, UIEndgameMenuRenderer>
    {
        /// <summary>The category toggle buttons' row (first block of the scroll content).</summary>
        internal UIEndgameButtonRow CategoryRow { get; private set; }

        /// <summary>The puzzle buttons (second block of the scroll content).</summary>
        internal UIEndgameButtonRow PuzzleGrid { get; private set; }

        /// <summary>The "no puzzles found" message, displayed only while the list is empty.</summary>
        internal UILabel EmptyMessage { get; private set; }

        private readonly Dictionary<string, UIButton> _categoryButtons = new(StringComparer.Ordinal);
        private readonly List<(UIButton Button, EndgamePuzzle Puzzle)> _puzzleButtons = new();

        public UIEndgameMenu() { }

        protected override void OnBeforeInit(IUiFactory factory)
        {
            // Only used by UIMenu's legacy stacking; the rows are laid out by the flex gaps.
            ButtonSpacing = UILayoutConstants.EndgameMenu.RowGap;
        }

        protected override void BuildUIObjects()
        {
            CategoryRow = _factory.CreateElement<UIEndgameButtonRow, UIEndgameButtonRowHandler, UIEndgameButtonRowRenderer>();
            CategoryRow.LayoutRules.Apply(UILayoutSheet.EndgameMenu.CategoryRow);
            ScrollContainer.AddChild(CategoryRow);

            PuzzleGrid = _factory.CreateElement<UIEndgameButtonRow, UIEndgameButtonRowHandler, UIEndgameButtonRowRenderer>();
            PuzzleGrid.LayoutRules.Apply(UILayoutSheet.EndgameMenu.PuzzleGrid);
            ScrollContainer.AddChild(PuzzleGrid);

            EmptyMessage = _factory.CreateElement<UILabel, UILabelHandler, UILabelRenderer>();
            EmptyMessage.Font = UILayoutStyles.EndgameMenu.EmptyMessage.Font;
            EmptyMessage.ForeColor = UILayoutStyles.EndgameMenu.EmptyMessage.Color;
            EmptyMessage.TextAlign = ContentAlign.MiddleCenter;
            EmptyMessage.WordWrap = true;
            EmptyMessage.LayoutRules.Apply(UILayoutSheet.EndgameMenu.EmptyMessage);
            EmptyMessage.LayoutRules.Display = DisplayMode.None;
            AddChild(EmptyMessage);
        }

        protected override void OnInit(IUiFactory factory)
        {
            // Declared sizes (also the pre-layout fallback), then the layout rules that
            // actually place the panel and its scroll container (see UILayoutSheet.EndgameMenu).
            Layout = UILayoutConstants.Submenu.Layout;
            ScrollContainer.Layout = UILayoutConstants.Submenu.ScrollContainer.Layout;

            LayoutRules.Apply(UILayoutSheet.EndgameMenu.Panel);
            ScrollContainer.LayoutRules.Apply(UILayoutSheet.EndgameMenu.ScrollContainer);
        }

        /// <summary>
        /// Nothing at init: the puzzles are loaded when the submenu is shown
        /// (<see cref="ShowPuzzles"/>).
        /// </summary>
        protected override void BuildButtons() { }

        /// <summary>
        /// Replaces all buttons with one toggle per category present in
        /// <paramref name="puzzles"/> (in list order) and one button per puzzle, or shows the
        /// empty message when there is none.
        /// </summary>
        /// <param name="puzzles">The puzzles in display order (category, then file name).</param>
        /// <param name="isCategoryShown">Whether a category's puzzles are currently shown.</param>
        /// <param name="emptyMessage">Text shown when <paramref name="puzzles"/> is empty.</param>
        public void ShowPuzzles(IReadOnlyList<EndgamePuzzle> puzzles, Func<string, bool> isCategoryShown, string emptyMessage)
        {
            ClearPuzzleButtons();

            foreach (var category in puzzles.Select(p => p.Category ?? string.Empty).Distinct(StringComparer.Ordinal))
            {
                string name = category;
                var button = CreateButton(UILayoutSheet.EndgameMenu.CategoryButton, () => Handler.ToggleCategory(name));
                CategoryRow.AddChild(button);
                _categoryButtons[name] = button;
                ApplyCategoryState(name, isCategoryShown(name));
            }

            foreach (var puzzle in puzzles)
            {
                var target = puzzle;
                var button = CreateButton(UILayoutSheet.EndgameMenu.PuzzleButton, () => Handler.StartPuzzle(target));
                button.Text = $"{puzzle.Title}\n{DifficultyStars(puzzle.Difficulty)}";
                button.Style = UILayoutStyles.EndgameMenu.ButtonStyle;
                button.LayoutRules.Display = isCategoryShown(puzzle.Category ?? string.Empty) ? DisplayMode.Normal : DisplayMode.None;
                PuzzleGrid.AddChild(button);
                _puzzleButtons.Add((button, puzzle));
            }

            bool empty = puzzles.Count == 0;
            EmptyMessage.Text = empty ? emptyMessage : string.Empty;
            EmptyMessage.LayoutRules.Display = empty ? DisplayMode.Normal : DisplayMode.None;
            CategoryRow.LayoutRules.Display = empty ? DisplayMode.None : DisplayMode.Normal;
            PuzzleGrid.LayoutRules.Display = empty ? DisplayMode.None : DisplayMode.Normal;

            Handler.UpdateScrollContentHeight();
        }

        /// <summary>
        /// Shows or hides <paramref name="category"/>'s puzzle buttons and updates its toggle.
        /// The rows re-layout on their own (a <c>Display</c> change invalidates the layout),
        /// and the scroll content height follows.
        /// </summary>
        public void SetCategoryShown(string category, bool shown)
        {
            ApplyCategoryState(category, shown);
            foreach (var (button, puzzle) in _puzzleButtons)
                if (string.Equals(puzzle.Category ?? string.Empty, category, StringComparison.Ordinal))
                    button.LayoutRules.Display = shown ? DisplayMode.Normal : DisplayMode.None;

            Handler.UpdateScrollContentHeight();
        }

        /// <summary>The toggle's label (on/off mark + name) and look for its state.</summary>
        private void ApplyCategoryState(string category, bool shown)
        {
            if (!_categoryButtons.TryGetValue(category, out var button))
                return;

            string name = category.Length == 0 ? UILayoutStyles.EndgameMenu.UncategorizedName : category;
            string mark = shown ? UILayoutStyles.EndgameMenu.CategoryOnMark : UILayoutStyles.EndgameMenu.CategoryOffMark;
            button.Text = $"{mark} {name}";
            button.Style = shown ? UILayoutStyles.EndgameMenu.ButtonStyle : UILayoutStyles.EndgameMenu.CategoryOff.Style;
        }

        /// <summary>
        /// <paramref name="difficulty"/> filled stars, then empty stars up to
        /// <c>MaxDifficulty</c> (e.g. 3 -> ★★★☆☆). Out-of-range values are clamped.
        /// </summary>
        public static string DifficultyStars(int difficulty)
        {
            int max = UILayoutStyles.EndgameMenu.MaxDifficulty;
            int filled = Math.Clamp(difficulty, 0, max);
            var text = new StringBuilder(max);
            for (int i = 0; i < max; i++)
                text.Append(i < filled ? UILayoutStyles.EndgameMenu.StarFilled : UILayoutStyles.EndgameMenu.StarEmpty);
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

        /// <summary>Disposes every category and puzzle button (which also detaches it).</summary>
        private void ClearPuzzleButtons()
        {
            foreach (var button in Buttons)
                button.Dispose();
            Buttons.Clear();
            _categoryButtons.Clear();
            _puzzleButtons.Clear();
        }
    }
}

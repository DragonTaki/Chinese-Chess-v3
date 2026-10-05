/* ----- ----- ----- ----- */
// CategoryListModel.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/05
// Update Date: 2026/10/05
// Version: v1.0
/* ----- ----- ----- ----- */

using System;
using System.Collections.Generic;
using System.Linq;

using Chinese_Chess_v3.Game.Core.Pgn;

namespace Chinese_Chess_v3.Game.Application.Catalogs
{
    /// <summary>
    /// One section of a <see cref="CategoryListModel{T}"/>: its name (null for a list without
    /// sections), its categories in list order, and its items in display order.
    /// </summary>
    /// <typeparam name="T">The kind of file listed.</typeparam>
    /// <param name="Name">The section's name; null when the list has no sections.</param>
    /// <param name="Categories">The categories of the section's items, in list order (each once).</param>
    /// <param name="Items">The section's items in display order.</param>
    public sealed record CategoryListSection<T>(string Name, IReadOnlyList<string> Categories, IReadOnlyList<T> Items)
        where T : PgnGameFile;

    /// <summary>
    /// The state of a list of PGN game files shown by category (殘局闖關, 開局練習, the game
    /// screen's saved-game list): loads the items, sorts them (section, category, file name,
    /// built-in before the player's own), splits them into sections with their categories, and
    /// keeps which categories the player switched off.
    /// <para>
    /// Sections are optional (<c>sectionOf</c>): without them the list is one section named
    /// null. A category is identified by its section and name; the same category name in two
    /// sections is two independent toggles. A missing section or category counts as the empty
    /// string in those keys.
    /// </para>
    /// </summary>
    /// <typeparam name="T">The kind of file listed.</typeparam>
    public sealed class CategoryListModel<T> where T : PgnGameFile
    {
        private readonly Func<List<string>, IEnumerable<T>> _load;
        private readonly Func<T, string> _sectionOf;

        /// <summary>Categories (per section) the player switched off (all on by default; kept while the game runs).</summary>
        private readonly HashSet<(string Section, string Category)> _hiddenCategories = new();

        /// <summary>
        /// Creates the model; nothing is loaded until <see cref="Reload"/>.
        /// </summary>
        /// <param name="load">Loads every item (in any order), adding a line to the given list per skipped file.</param>
        /// <param name="sectionOf">
        /// The section an item is listed in, or null (the default) for a list without sections.
        /// Sections are shown in ordinal order of their names (開局練習: 先 &lt; 後, so 先手 before 後手).
        /// </param>
        public CategoryListModel(Func<List<string>, IEnumerable<T>> load, Func<T, string> sectionOf = null)
        {
            _load = load ?? throw new ArgumentNullException(nameof(load));
            _sectionOf = sectionOf;
        }

        /// <summary>The items last loaded, in display order.</summary>
        public IReadOnlyList<T> Items { get; private set; } = Array.Empty<T>();

        /// <summary>
        /// The items last loaded split into sections, in display order: one section named null
        /// when no item has a section; otherwise one per section name (an item without one goes
        /// to the section named empty). A section without items is not listed.
        /// </summary>
        public IReadOnlyList<CategoryListSection<T>> Sections { get; private set; } = Array.Empty<CategoryListSection<T>>();

        /// <summary>The category <paramref name="item"/>'s toggle is named after (its own <c>Category</c>; empty when none).</summary>
        public string CategoryOf(T item) => item?.Category ?? string.Empty;

        /// <summary>The section <paramref name="item"/> is listed in; null for a list without sections.</summary>
        public string SectionOf(T item) => _sectionOf?.Invoke(item);

        /// <summary>
        /// Loads the items again (files added meanwhile appear), sorts them (<see cref="Sort"/>)
        /// and splits them into <see cref="Sections"/>. The switched-off categories are kept.
        /// </summary>
        /// <returns>One line per file that was skipped.</returns>
        public IReadOnlyList<string> Reload()
        {
            var warnings = new List<string>();
            Items = Sort(_load(warnings) ?? Enumerable.Empty<T>());
            Sections = Split(Items);
            return warnings;
        }

        /// <summary>
        /// <paramref name="items"/> in display order: by section (none = empty), category, then
        /// file name (ordinal, as the loaders sort each folder), built-in before the player's
        /// own on a tie. The loaders return the two folders one after the other, so a category
        /// present in both would otherwise be split in two. Null items are left out.
        /// </summary>
        public IReadOnlyList<T> Sort(IEnumerable<T> items)
        {
            ArgumentNullException.ThrowIfNull(items);
            return items
                .Where(p => p != null)
                .OrderBy(p => SectionOf(p) ?? string.Empty, StringComparer.Ordinal)
                .ThenBy(CategoryOf, StringComparer.Ordinal)
                .ThenBy(p => p.FileName, StringComparer.Ordinal)
                .ThenBy(p => p.Origin)
                .ToList();
        }

        /// <summary>Whether <paramref name="category"/>'s items (of <paramref name="section"/>) are shown.</summary>
        public bool IsCategoryShown(string section, string category) =>
            !_hiddenCategories.Contains(Key(section, category));

        /// <summary>A category toggle clicked: hides its items if shown, shows them if hidden.</summary>
        /// <returns>Whether the category's items are shown now.</returns>
        public bool ToggleCategory(string section, string category)
        {
            var key = Key(section, category);
            bool show = _hiddenCategories.Contains(key);
            if (show)
                _hiddenCategories.Remove(key);
            else
                _hiddenCategories.Add(key);
            return show;
        }

        /// <summary>The section and category as a key: null is the empty string.</summary>
        private static (string Section, string Category) Key(string section, string category) =>
            (section ?? string.Empty, category ?? string.Empty);

        /// <summary>The sorted items split into sections (see <see cref="Sections"/>).</summary>
        private IReadOnlyList<CategoryListSection<T>> Split(IReadOnlyList<T> items)
        {
            if (!items.Any(i => SectionOf(i) != null))
                return items.Count == 0
                    ? Array.Empty<CategoryListSection<T>>()
                    : new[] { MakeSection(null, items) };

            return items
                .GroupBy(i => SectionOf(i) ?? string.Empty, StringComparer.Ordinal)
                .Select(g => MakeSection(g.Key, g.ToList()))
                .ToList();
        }

        private CategoryListSection<T> MakeSection(string name, IReadOnlyList<T> items) =>
            new(name, items.Select(CategoryOf).Distinct(StringComparer.Ordinal).ToList(), items);
    }
}

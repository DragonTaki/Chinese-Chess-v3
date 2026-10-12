/* ----- ----- ----- ----- */
// BoardGroups.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/12
// Update Date: 2026/10/12
// Version: v1.0
/* ----- ----- ----- ----- */

using System;
using System.Collections.Generic;

using Chinese_Chess_v3.Game.Application.Menus;
using Chinese_Chess_v3.Game.Application.Texts;
using Chinese_Chess_v3.Game.Core;

using Engine.Localization;

namespace Chinese_Chess_v3.Game.Application.MainMenu
{
    /// <summary>
    /// One board group of a game-kind submenu: a heading with up to two mode buttons beneath it,
    /// <see cref="Left"/> and <see cref="Right"/>. A null <see cref="Right"/> is a blank slot
    /// (no button, no placeholder).
    /// </summary>
    public sealed class BoardGroup
    {
        /// <summary>The group's heading text (a label, not a button).</summary>
        public string Title { get; }

        /// <summary>The left mode button.</summary>
        public MenuEntry<GameKind> Left { get; }

        /// <summary>The right mode button, or null when the right slot is blank.</summary>
        public MenuEntry<GameKind> Right { get; }

        /// <param name="title">The heading text.</param>
        /// <param name="left">The left mode button.</param>
        /// <param name="right">The right mode button, or null for a blank slot.</param>
        public BoardGroup(string title, MenuEntry<GameKind> left, MenuEntry<GameKind> right)
        {
            Title = title ?? throw new ArgumentNullException(nameof(title));
            Left = left ?? throw new ArgumentNullException(nameof(left));
            Right = right;
        }

        /// <summary>The group's buttons, left then right (the blank slot is skipped).</summary>
        public IEnumerable<MenuEntry<GameKind>> Entries
        {
            get
            {
                yield return Left;
                if (Right != null) yield return Right;
            }
        }
    }

    /// <summary>
    /// The three board groups shared by 單機遊戲, 人機對戰 and the Multiplayer panel's game-kinds
    /// page: 大盤 (傳統, 揭棋), 小盤 (暗棋半盤, 明棋半盤) and 其他 (三國半盤, blank).
    /// Every <see cref="GameKind"/> appears exactly once.
    /// </summary>
    public static class BoardGroups
    {
        /// <summary>Creates the groups; choosing a button calls <paramref name="choose"/> with its game kind.</summary>
        /// <param name="choose">Called with the chosen game kind.</param>
        /// <returns>The groups, in display order.</returns>
        public static IReadOnlyList<BoardGroup> Create(Action<GameKind> choose)
        {
            if (choose == null) throw new ArgumentNullException(nameof(choose));
            MenuEntry<GameKind> Entry(GameKind kind) =>
                new MenuEntry<GameKind>(kind, SettingsTexts.GameKindName(kind), () => choose(kind));

            return new List<BoardGroup>
            {
                new BoardGroup(Lang.Get("menu.board_group.full"), Entry(GameKind.Traditional), Entry(GameKind.Flip)),
                new BoardGroup(Lang.Get("menu.board_group.half"), Entry(GameKind.DarkHalf), Entry(GameKind.OpenHalf)),
                new BoardGroup(Lang.Get("menu.board_group.other"), Entry(GameKind.ThreeKingdoms), null),
            };
        }
    }
}

/* ----- ----- ----- ----- */
// HelpCatalog.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/14
// Update Date: 2026/10/14
// Version: v1.1
/* ----- ----- ----- ----- */

using System;
using System.Collections.Generic;

using Chinese_Chess_v3.Game.Application.MainMenu;
using Engine.Localization;

namespace Chinese_Chess_v3.Game.Application.Catalogs
{
    /// <summary>Stable content identities and localization keys in display order.</summary>
    public static class HelpCatalog
    {
        public sealed record Item(HelpMenuId Id, string ContentId, string LabelKey)
        {
            public string Label => Lang.Get(LabelKey);
        }

        public static IReadOnlyList<Item> Lessons { get; } = Array.AsReadOnly(new[]
        {
            new Item(HelpMenuId.Board, "board", "help.lesson.board"),
            new Item(HelpMenuId.GeneralAdvisorElephant, "general-advisor-elephant", "help.lesson.general-advisor-elephant"),
            new Item(HelpMenuId.RookHorseCannon, "rook-horse-cannon", "help.lesson.rook-horse-cannon"),
            new Item(HelpMenuId.Soldier, "soldier", "help.lesson.soldier"),
            new Item(HelpMenuId.CheckEscape, "check-escape", "help.lesson.check-escape"),
            new Item(HelpMenuId.Result, "result", "help.lesson.result"),
            new Item(HelpMenuId.MateIntro, "mate-intro", "help.lesson.mate-intro"),
        });

        public static IReadOnlyList<Item> ModeIntroductions { get; } = Array.AsReadOnly(new[]
        {
            new Item(HelpMenuId.Traditional, "traditional", "help.mode.traditional"),
            new Item(HelpMenuId.Jieqi, "jieqi", "help.mode.jieqi"),
            new Item(HelpMenuId.DarkHalf, "dark-half", "help.mode.dark-half"),
            new Item(HelpMenuId.ThreeKingdoms, "three-kingdoms", "help.mode.three-kingdoms"),
        });
    }
}

/* ----- ----- ----- ----- */
// HelpMenuModel.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/14
// Update Date: 2026/10/14
// Version: v1.2
/* ----- ----- ----- ----- */

using System.Collections.Generic;

using Chinese_Chess_v3.Game.Application.Menus;

namespace Chinese_Chess_v3.Game.Application.MainMenu
{
    public enum HelpMenuTab { Lessons, ModeIntroductions }

    public enum HelpMenuId
    {
        Board, GeneralAdvisorElephant, RookHorseCannon,
        Soldier, CheckEscape, Result, MateIntro, Traditional, Jieqi, DarkHalf, ThreeKingdoms,
    }

    /// <summary>The complete menu state passed to the view.</summary>
    public sealed record HelpMenuModel(HelpMenuTab Tab, IReadOnlyList<string> Tabs, IReadOnlyList<MenuEntry<HelpMenuId>> Entries);
}

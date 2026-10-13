/* ----- ----- ----- ----- */
// PreGameSetup.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/13
// Update Date: 2026/10/13
// Version: v1.0
/* ----- ----- ----- ----- */

using System.Collections.Generic;

using Chinese_Chess_v3.Game.Application.Settings;
using Chinese_Chess_v3.Game.Application.Texts;
using Chinese_Chess_v3.Game.Core;

namespace Chinese_Chess_v3.Game.Application.MainMenu
{
    /// <summary>A button of the pre-game settings page's fixed bottom row.</summary>
    public enum PreGameAction
    {
        /// <summary>Back to the board-mode list.</summary>
        Back,

        /// <summary>Restore this mode's settings to their defaults.</summary>
        Reset,

        /// <summary>Start the game.</summary>
        Start,
    }

    /// <summary>
    /// What the pre-game settings pages show (author decision 22, 2026-10-13): the rules split into pages, and the
    /// bottom row that stays fixed under the scrolling page.
    /// </summary>
    public static class PreGameSetup
    {
        /// <summary>Index of the computer options page among <see cref="SettingsMenuContent.ComputerPages"/>.</summary>
        public const int ComputerPageIndex = 2;

        /// <summary>One mode's pages for the local game: 規則 and 計時.</summary>
        public static IReadOnlyList<SettingsMenuPage> PagesFor(GameKind kind) => SettingsMenuContent.PreGamePages(kind);

        /// <summary>The computer game's pages: 規則, 計時, 電腦.</summary>
        public static IReadOnlyList<SettingsMenuPage> ComputerPages => SettingsMenuContent.ComputerPages;

        /// <summary>The bottom row, left to right: 返回, 重置, 開始遊戲.</summary>
        public static IReadOnlyList<(PreGameAction Action, string Text)> BottomRow => new[]
        {
            (PreGameAction.Back, AiSetupTexts.Back),
            (PreGameAction.Reset, AiSetupTexts.Reset),
            (PreGameAction.Start, AiSetupTexts.Start),
        };
    }
}

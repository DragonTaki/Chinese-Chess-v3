/* ----- ----- ----- ----- */
// MenuTexts.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/05
// Update Date: 2026/10/05
// Version: v1.0
/* ----- ----- ----- ----- */

namespace Chinese_Chess_v3.Game.Application.Texts
{
    /// <summary>
    /// Texts of the main menu and its submenus used by the logic layer (the main menu's
    /// presenter and option lists): entries named elsewhere too and the confirm dialog
    /// messages. The screens' own texts stay in <c>GameMenuTexts</c> (UI).
    /// </summary>
    public static class MenuTexts
    {
        // ----- Main menu -----

        /// <summary>Main menu entry and title of the rules screen (local games only; a network game does not use these rules).</summary>
        public const string LocalRuleSettings = "單機規則設定";

        /// <summary>Main menu entry and title of the general settings screen.</summary>
        public const string GameSettings = "遊戲設定";

        /// <summary>離開遊戲: asks before the application closes (yes = exit).</summary>
        public const string ConfirmExit = "確認要離開遊戲嗎？";

        // ----- New-game menu -----

        /// <summary>A new-game mode whose game cannot be started yet (揭棋大盤, 三國半盤); <paramref name="mode"/> = its button text.</summary>
        public static string NewGameModeUnavailable(string mode) => $"「{mode}」尚未完成，目前無法開始。";
    }
}

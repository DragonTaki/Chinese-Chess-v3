/* ----- ----- ----- ----- */
// UINewGameMenuType.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/16
// Update Date: 2025/05/16
// Version: v1.0
/* ----- ----- ----- ----- */

namespace Chinese_Chess_v3.Game.UI.Menus.NewGameMenu
{
    public enum UINewGameMenuType
    {
        Default,
        Traditional,       // 傳統大盤 (Traditional full board)
        FlipChess,         // 揭棋大盤 (Flip chess, full board)
        DarkHalf,          // 暗棋半盤 (Dark chess, half board)
        OpenHalf,          // 明棋半盤 (Open chess, half board)
        ThreeKingdomsHalf  // 三國半盤 (Three Kingdoms, half board)
    }
}

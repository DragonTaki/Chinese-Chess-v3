/* ----- ----- ----- ----- */
// MainMenuButtonList.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/14
// Update Date: 2025/05/14
// Version: v1.0
/* ----- ----- ----- ----- */

using System.Collections.Generic;
using Chinese_Chess_v3.Models;

namespace Chinese_Chess_v3.Data
{
    public static class MainMenuButtonList
    {
        public static List<ButtonData> CreateDefaultButtons()
        {
            return new List<ButtonData>
            {
                new ButtonData("開新一局"),
                new ButtonData("遊戲設定"),
                new ButtonData("規則設定"),
                new ButtonData("教學／幫助"),
                new ButtonData("讀取存檔"),
                new ButtonData("離開遊戲")
            };
        }
    }
}
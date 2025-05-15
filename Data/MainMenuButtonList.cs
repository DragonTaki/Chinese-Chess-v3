/* ----- ----- ----- ----- */
// MainMenuButtonList.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/14
// Update Date: 2025/05/14
// Version: v1.0
/* ----- ----- ----- ----- */

using System.Collections.Generic;

namespace Chinese_Chess_v3.Data
{
    public static class MainMenuButtonList
    {
        public static List<string> CreateDefaultButtonLabels()
        {
            return new List<string>
            {
                "開新一局",
                "遊戲設定",
                "規則設定",
                "教學／幫助",
                "讀取存檔",
                "離開遊戲"
            };
        }
    }
}
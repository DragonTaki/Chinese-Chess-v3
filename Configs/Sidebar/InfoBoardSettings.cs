/* ----- ----- ----- ----- */
// InfoBoardSettings.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/07
// Update Date: 2025/05/07
// Version: v1.0
/* ----- ----- ----- ----- */

using System.Drawing;

using Chinese_Chess_v3.Utils;

namespace Chinese_Chess_v3.Configs.Sidebar
{
    public static class InfoBoardSettings
    {
        public static readonly Font NameFont = StyleHelper.GetFont("MoeLI", 24, FontStyle.Bold);
        public static readonly Font TimerFont = StyleHelper.GetFont("Consolas", 16, FontStyle.Bold);
        public static readonly Color RedSideBackgroundColor = StyleHelper.GetColor("#E83015");  // #E83015
        public static readonly Color BlackSideBackgroundColor = StyleHelper.GetColor("#1C1C1C");  // #1C1C1C
        public static readonly Color TextColor = StyleHelper.GetColor(Color.White);
        
    }
}
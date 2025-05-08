/* ----- ----- ----- ----- */
// LoggerSettings.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/06
// Update Date: 2025/05/06
// Version: v1.0
/* ----- ----- ----- ----- */

using System.Drawing;

using Chinese_Chess_v3.Utils;

namespace Chinese_Chess_v3.Configs.Sidebar
{
    public static class LoggerSettings
    {
        public static readonly Font Font = StyleHelper.GetFont("Consolas", 12);
        public static readonly Color BackgroundColor = StyleHelper.GetColor("#1C1C1C");  // #1C1C1C
        public static readonly Color TextColor = StyleHelper.GetColor(Color.White);
        
    }
}
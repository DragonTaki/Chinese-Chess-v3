/* ----- ----- ----- ----- */
// UIInfoBoardSettings.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/07
// Update Date: 2026/09/24
// Version: v1.1
/* ----- ----- ----- ----- */

using System.Drawing;

using Engine.Platform;
using Engine.Styles;

namespace Chinese_Chess_v3.Game.UI.Sidebars.InfoBoards
{
    public static class UIInfoBoardSettings
    {
        public static readonly IFont NameFont = StyleHelper.GetFont("MoeLI", 24, FontStyleFlags.Bold);
        public static readonly IFont TimerFont = StyleHelper.GetFont("Consolas", 16, FontStyleFlags.Bold);
        public static readonly Color RedSideBackgroundColor = StyleHelper.GetColor("#E83015");  // #E83015
        public static readonly Color BlackSideBackgroundColor = StyleHelper.GetColor("#1C1C1C");  // #1C1C1C
        public static readonly Color TextColor = StyleHelper.GetColor(Color.White);

    }
}

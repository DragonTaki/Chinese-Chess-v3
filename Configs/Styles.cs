/* ----- ----- ----- ----- */
// Styles.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/13
// Update Date: 2025/05/13
// Version: v1.0
/* ----- ----- ----- ----- */

using System.Drawing;

using Chinese_Chess_v3.Utils;

namespace Chinese_Chess_v3.Configs
{
    public static class Styles
    {
        public static class MainMenu
        {
            public static class Button
            {
                public static readonly Font Font = StyleHelper.GetFont("NotoSerif", 36, FontStyle.Bold);
                public static readonly Brush TextBrush = StyleHelper.GetBrush("#FCFAF2", 1.0f);  // #FCFAF2
                public static class Border
                {
                    public static readonly Color OuterColor = StyleHelper.GetColor("#F9BF45", 0.85f);  // #F9BF45
                    public static readonly Color InnerColor = StyleHelper.GetColor("#F9BF45", 0.9f);  // #F9BF45
                    public const float OuterWidth = 4.0f;
                    public const float InnerWidth = 2.0f;
                    public const float Margin = 4.0f;
                    public const float CornerRadius = 6.0f;
                }
                public static class Background
                {
                    public static readonly Color TopColor = StyleHelper.GetColor("#FFFFFF", 0.25f);  // #FFFFFF
                    public static readonly Color BottomColor = StyleHelper.GetColor("#F0F0F0", 0.25f);  // #F0F0F0
                }
            }
        }
    }
}
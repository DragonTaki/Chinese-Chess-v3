/* ----- ----- ----- ----- */
// UILayoutStyles.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/13
// Update Date: 2025/05/13
// Version: v1.0
/* ----- ----- ----- ----- */

using System.Drawing;
using System.Drawing.Drawing2D;

using Engine.Styles;

namespace Chinese_Chess_v3.Game.UI.Constants
{
    public static class UILayoutStyles
    {
        public static class Overlay
        {
            public static class Dialog
            {
                public static class Border
                {
                    public const float CornerRadius = 12.0f;

                    public static BorderStyle BorderStyle = new BorderStyle
                    {
                        Width = 4.0f,
                        Color = StyleHelper.GetColor("#554236", 1.0f)  // #554236
                    };
                }
                public static class Background
                {
                    public static readonly Color Color = StyleHelper.GetColor("#FFFFFF", 1.0f);  // #FFFFFF
                    public static IBrushFactory BrushFactory =>
                        new SolidBrushFactory(Color);
                }

                public static IBoxDrawStyle Style = new InwardCornerDialogStyle
                {
                    BackgroundBrushFactory = Background.BrushFactory,
                    BorderStyle = Border.BorderStyle,
                    CornerRadius = Border.CornerRadius
                };
                public static class Button
                {
                    public static readonly Font Font = StyleHelper.GetFont("NotoSerif", 24, FontStyle.Bold);
                    public static readonly Brush TextBrush = StyleHelper.GetBrush("#000000", 1.0f);  // #000000
                    public static class Border
                    {
                        public const float CornerRadius = 8.0f;

                        public static BorderStyle BorderStyle = new BorderStyle
                        {
                            Width = 4.0f,
                            Color = StyleHelper.GetColor("#707C74", 1.0f)  // #707C74
                        };
                    }
                    public static class Background
                    {
                        public static readonly Color Color = StyleHelper.GetColor("#FFFFFF", 1.0f);  // #FFFFFF
                        public static IBrushFactory BrushFactory =>
                            new SolidBrushFactory(Color);
                    }

                    public static IButtonDrawStyle Style = new SingleBorderRoundedStyle
                    {
                        Font = Font,
                        TextBrush = TextBrush,
                        BackgroundBrushFactory = Background.BrushFactory,
                        BorderStyle = Border.BorderStyle,
                        CornerRadius = Border.CornerRadius
                    };
                }
            }
        }

        public static class MainMenu
        {
            public static class Button
            {
                public static readonly Font Font = StyleHelper.GetFont("NotoSerif", 36, FontStyle.Bold);
                public static readonly Brush TextBrush = StyleHelper.GetBrush("#FCFAF2", 1.0f);  // #FCFAF2
                public static class Border
                {
                    public const float Margin = 4.0f;
                    public const float CornerRadius = 6.0f;

                    public static BorderStyle Outer = new BorderStyle
                    {
                        Width = 4.0f,
                        Color = StyleHelper.GetColor("#F9BF45", 0.85f)  // #F9BF45
                    };

                    public static BorderStyle Inner = new BorderStyle
                    {
                        Width = 2.0f,
                        Color = StyleHelper.GetColor("#F9BF45", 0.9f)  // #F9BF45
                    };
                }
                public static class Background
                {
                    public static readonly Color TopColor = StyleHelper.GetColor("#FFFFFF", 0.25f);  // #FFFFFF
                    public static readonly Color BottomColor = StyleHelper.GetColor("#F0F0F0", 0.25f);  // #F0F0F0
                    public static IBrushFactory BrushFactory =>
                        new LinearGradientBrushFactory(TopColor, BottomColor, LinearGradientMode.Vertical);
                }

                public static IButtonDrawStyle Style = new DoubleBorderRoundedStyle
                {
                    Font = Font,
                    TextBrush = TextBrush,
                    BackgroundBrushFactory = Background.BrushFactory,
                    OuterBorder = Border.Outer,
                    InnerBorder = Border.Inner,
                    Margin = Border.Margin,
                    CornerRadius = Border.CornerRadius
                };
            }
        }
    }
}
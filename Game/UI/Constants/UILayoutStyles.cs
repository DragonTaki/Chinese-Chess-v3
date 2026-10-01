/* ----- ----- ----- ----- */
// UILayoutStyles.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/13
// Update Date: 2026/10/01
// Version: v2.1
/* ----- ----- ----- ----- */

using System.Drawing;

using Engine.Platform;
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
                    public static readonly IFont Font = StyleHelper.GetFont("NotoSerif", 24, FontStyleFlags.Bold);
                    public static readonly IBrush TextBrush = StyleHelper.GetBrush("#000000", 1.0f);  // #000000
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
                public static readonly IFont Font = StyleHelper.GetFont("NotoSerif", 36, FontStyleFlags.Bold);
                public static readonly IBrush TextBrush = StyleHelper.GetBrush("#FCFAF2", 1.0f);  // #FCFAF2
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
                        new LinearGradientBrushFactory(TopColor, BottomColor, GradientDirection.Vertical);
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

        /// <summary>
        /// The endgame challenge submenu (<c>UIEndgameMenu</c>). Its buttons are a quarter of
        /// the submenu width (<c>UILayoutConstants.EndgameMenu.ButtonWidth</c>, about 154), so
        /// they use a smaller font than the main menu's 36 and put the difficulty stars on a
        /// second line. Box look (borders, background) as the main menu buttons.
        /// <para>
        /// Glyphs: <see cref="StarFilled"/>/<see cref="StarEmpty"/> (U+2605/U+2606) and
        /// <see cref="CategoryOnMark"/>/<see cref="CategoryOffMark"/> (U+25CF/U+25CB) were
        /// checked in the cmap of the bundled <c>Assets/Font/NotoSerifCJKtc-Medium.otf</c>
        /// (and <c>MoeLI.ttf</c>): all present, no fallback font needed.
        /// </para>
        /// </summary>
        public static class EndgameMenu
        {
            /// <summary>Puzzle and category buttons; at this size about 5 CJK characters fit on a line.</summary>
            public static readonly IFont ButtonFont = StyleHelper.GetFont("NotoSerif", 18, FontStyleFlags.Bold);

            /// <summary>Difficulty stars: one filled star per level, empty ones up to <see cref="MaxDifficulty"/>.</summary>
            public const string StarFilled = "★";
            public const string StarEmpty = "☆";

            /// <summary>Difficulty is 1 to 5 (docs/ENDGAMES.md); the stars show it out of this many.</summary>
            public const int MaxDifficulty = 5;

            /// <summary>Category toggle labels: mark + category name.</summary>
            public const string CategoryOnMark = "●";
            public const string CategoryOffMark = "○";

            /// <summary>Shown for puzzles without a category (empty <c>EndgamePuzzle.Category</c>).</summary>
            public const string UncategorizedName = "未分類";

            /// <summary>A puzzle button, and a category toggle that is on.</summary>
            public static IButtonDrawStyle ButtonStyle = new DoubleBorderRoundedStyle
            {
                Font = ButtonFont,
                TextBrush = MainMenu.Button.TextBrush,
                BackgroundBrushFactory = MainMenu.Button.Background.BrushFactory,
                OuterBorder = MainMenu.Button.Border.Outer,
                InnerBorder = MainMenu.Button.Border.Inner,
                Margin = MainMenu.Button.Border.Margin,
                CornerRadius = MainMenu.Button.Border.CornerRadius
            };

            /// <summary>A category toggle that is off: same box, dimmed text and borders.</summary>
            public static class CategoryOff
            {
                public static readonly IBrush TextBrush = StyleHelper.GetBrush("#FCFAF2", 0.45f);  // #FCFAF2

                public static BorderStyle Outer = new BorderStyle
                {
                    Width = 4.0f,
                    Color = StyleHelper.GetColor("#F9BF45", 0.3f)  // #F9BF45
                };

                public static BorderStyle Inner = new BorderStyle
                {
                    Width = 2.0f,
                    Color = StyleHelper.GetColor("#F9BF45", 0.3f)  // #F9BF45
                };

                public static IButtonDrawStyle Style = new DoubleBorderRoundedStyle
                {
                    Font = ButtonFont,
                    TextBrush = TextBrush,
                    BackgroundBrushFactory = MainMenu.Button.Background.BrushFactory,
                    OuterBorder = Outer,
                    InnerBorder = Inner,
                    Margin = MainMenu.Button.Border.Margin,
                    CornerRadius = MainMenu.Button.Border.CornerRadius
                };
            }

            /// <summary>The message shown when no puzzle was found.</summary>
            public static class EmptyMessage
            {
                public static readonly IFont Font = StyleHelper.GetFont("NotoSerif", 20, FontStyleFlags.Bold);
                public static readonly Color Color = StyleHelper.GetColor("#FCFAF2", 1.0f);  // #FCFAF2
            }
        }
    }
}

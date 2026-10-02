/* ----- ----- ----- ----- */
// UILayoutStyles.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/13
// Update Date: 2026/10/02
// Version: v2.2
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

                    public static readonly BorderStyle BorderStyle = new BorderStyle
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

                public static readonly IBoxDrawStyle Style = new InwardCornerDialogStyle
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

                        public static readonly BorderStyle BorderStyle = new BorderStyle
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

                    public static readonly IButtonDrawStyle Style = new SingleBorderRoundedStyle
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

                    public static readonly BorderStyle Outer = new BorderStyle
                    {
                        Width = 4.0f,
                        Color = StyleHelper.GetColor("#F9BF45", 0.85f)  // #F9BF45
                    };

                    public static readonly BorderStyle Inner = new BorderStyle
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

                public static readonly IButtonDrawStyle Style = new DoubleBorderRoundedStyle
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
        /// The settings screens (<c>UISettingsMenu</c>). Box looks as the main menu buttons
        /// (gold borders) with the category menus' dimmed variant for what is not selected or
        /// only shows a value; fonts, colours and the dimmed section header are placeholders for
        /// the author to tune.
        /// </summary>
        public static class SettingsMenu
        {
            /// <summary>The save / back buttons.</summary>
            public static readonly IFont ButtonFont = StyleHelper.GetFont("NotoSerif", 24, FontStyleFlags.Bold);

            /// <summary>Section headers.</summary>
            public static readonly IFont HeaderFont = StyleHelper.GetFont("NotoSerif", 20, FontStyleFlags.Bold);

            /// <summary>A row's name.</summary>
            public static readonly IFont ItemFont = StyleHelper.GetFont("NotoSerif", 22, FontStyleFlags.Bold);

            /// <summary>Tab texts, value buttons and the text field.</summary>
            public static readonly IFont ValueFont = StyleHelper.GetFont("NotoSerif", 20, FontStyleFlags.Bold);

            /// <summary>Colour of a row's name (as the main menu buttons' text).</summary>
            public static readonly IBrush ItemTextBrush = MainMenu.Button.TextBrush;

            /// <summary>The save / back buttons.</summary>
            public static readonly IButtonDrawStyle ButtonStyle = new DoubleBorderRoundedStyle
            {
                Font = ButtonFont,
                TextBrush = MainMenu.Button.TextBrush,
                BackgroundBrushFactory = MainMenu.Button.Background.BrushFactory,
                OuterBorder = MainMenu.Button.Border.Outer,
                InnerBorder = MainMenu.Button.Border.Inner,
                Margin = MainMenu.Button.Border.Margin,
                CornerRadius = MainMenu.Button.Border.CornerRadius
            };

            /// <summary>A section header: a button box with dimmed borders and a smaller font (it does nothing when clicked).</summary>
            public static readonly IButtonDrawStyle HeaderStyle = new DoubleBorderRoundedStyle
            {
                Font = HeaderFont,
                TextBrush = MainMenu.Button.TextBrush,
                BackgroundBrushFactory = MainMenu.Button.Background.BrushFactory,
                OuterBorder = CategoryListMenu.CategoryOff.Outer,
                InnerBorder = CategoryListMenu.CategoryOff.Inner,
                Margin = MainMenu.Button.Border.Margin,
                CornerRadius = MainMenu.Button.Border.CornerRadius
            };

            /// <summary>A choice's button (a click steps to the next choice): bright borders.</summary>
            public static readonly IButtonDrawStyle ChoiceStyle = new DoubleBorderRoundedStyle
            {
                Font = ValueFont,
                TextBrush = MainMenu.Button.TextBrush,
                BackgroundBrushFactory = MainMenu.Button.Background.BrushFactory,
                OuterBorder = MainMenu.Button.Border.Outer,
                InnerBorder = MainMenu.Button.Border.Inner,
                Margin = MainMenu.Button.Border.Margin,
                CornerRadius = MainMenu.Button.Border.CornerRadius
            };

            /// <summary>A number's value (only shown for now, not clickable): dimmed borders.</summary>
            public static readonly IButtonDrawStyle NumberStyle = new DoubleBorderRoundedStyle
            {
                Font = ValueFont,
                TextBrush = MainMenu.Button.TextBrush,
                BackgroundBrushFactory = MainMenu.Button.Background.BrushFactory,
                OuterBorder = CategoryListMenu.CategoryOff.Outer,
                InnerBorder = CategoryListMenu.CategoryOff.Inner,
                Margin = MainMenu.Button.Border.Margin,
                CornerRadius = MainMenu.Button.Border.CornerRadius
            };

            /// <summary>The tab bar: unselected tabs dimmed, the selected one bright with a gold bar under it.</summary>
            public static readonly TabBarStyle TabBarStyle = new TabBarStyle
            {
                TabStyle = new DoubleBorderRoundedStyle
                {
                    Font = ValueFont,
                    TextBrush = CategoryListMenu.CategoryOff.TextBrush,
                    BackgroundBrushFactory = MainMenu.Button.Background.BrushFactory,
                    OuterBorder = CategoryListMenu.CategoryOff.Outer,
                    InnerBorder = CategoryListMenu.CategoryOff.Inner,
                    Margin = MainMenu.Button.Border.Margin,
                    CornerRadius = MainMenu.Button.Border.CornerRadius
                },
                SelectedTabStyle = ChoiceStyle,
                TabGap = UILayoutConstants.SettingsMenu.TabGap,
                IndicatorColor = StyleHelper.GetColor("#F9BF45", 1.0f),  // #F9BF45
                IndicatorHeight = UILayoutConstants.SettingsMenu.TabIndicatorHeight,
                IndicatorInset = UILayoutConstants.SettingsMenu.TabIndicatorInset,
            };

            /// <summary>A row's switch: gold track when on, faint white when off.</summary>
            public static readonly ToggleSwitchStyle ToggleStyle = new ToggleSwitchStyle
            {
                TrackOnColor = StyleHelper.GetColor("#F9BF45", 0.9f),   // #F9BF45
                TrackOffColor = StyleHelper.GetColor("#FFFFFF", 0.2f),  // #FFFFFF
                KnobColor = StyleHelper.GetColor("#FCFAF2", 1.0f),      // #FCFAF2
                BorderColor = StyleHelper.GetColor("#F9BF45", 0.6f),    // #F9BF45
                BorderWidth = 2.0f,
                KnobInset = UILayoutConstants.SettingsMenu.ToggleKnobInset,
                DisabledOpacity = 0.35f,
            };

            /// <summary>
            /// A choice's dropdown: the box looks like the choice buttons it replaced (bright gold
            /// borders; a faint gold fill while open); the open list is an almost opaque dark box (it is drawn over other
            /// rows) with a faint gold highlight under the mouse and the chosen option in gold.
            /// </summary>
            public static readonly DropdownStyle DropdownStyle = new DropdownStyle
            {
                Box = new DoubleBorderRoundedStyle
                {
                    BackgroundBrushFactory = MainMenu.Button.Background.BrushFactory,
                    OuterBorder = MainMenu.Button.Border.Outer,
                    InnerBorder = MainMenu.Button.Border.Inner,
                    Margin = MainMenu.Button.Border.Margin,
                    CornerRadius = MainMenu.Button.Border.CornerRadius
                },
                OpenBox = new DoubleBorderRoundedStyle
                {
                    BackgroundBrushFactory = new SolidBrushFactory(StyleHelper.GetColor("#F9BF45", 0.25f)),  // #F9BF45
                    OuterBorder = MainMenu.Button.Border.Outer,
                    InnerBorder = MainMenu.Button.Border.Inner,
                    Margin = MainMenu.Button.Border.Margin,
                    CornerRadius = MainMenu.Button.Border.CornerRadius
                },
                ListBox = new SingleBorderRoundedStyle
                {
                    CornerRadius = MainMenu.Button.Border.CornerRadius,
                    BorderStyle = MainMenu.Button.Border.Inner,
                    BackgroundBrushFactory = new SolidBrushFactory(StyleHelper.GetColor("#26221C", 0.96f)),  // #26221C
                },
                TextColor = StyleHelper.GetColor("#FCFAF2", 1.0f),          // #FCFAF2
                SelectedTextColor = StyleHelper.GetColor("#F9BF45", 1.0f),  // #F9BF45
                HoverColor = StyleHelper.GetColor("#F9BF45", 0.2f),         // #F9BF45
                ArrowColor = StyleHelper.GetColor("#F9BF45", 0.9f),         // #F9BF45
                ArrowWidth = UILayoutConstants.SettingsMenu.DropdownArrowWidth,
                ArrowInset = UILayoutConstants.SettingsMenu.DropdownArrowInset,
                TextInset = UILayoutConstants.SettingsMenu.ValueTextPaddingX,
                ItemHeight = UILayoutConstants.SettingsMenu.DropdownItemHeight,
                MaxVisibleItems = UILayoutConstants.SettingsMenu.DropdownMaxVisibleItems,
                ListGap = UILayoutConstants.SettingsMenu.DropdownListGap,
                ListPadding = UILayoutConstants.SettingsMenu.DropdownListPadding,
                ScrollBarColor = StyleHelper.GetColor("#F9BF45", 0.6f),     // #F9BF45
                ScrollBarWidth = UILayoutConstants.SettingsMenu.DropdownScrollBarWidth,
                DisabledOpacity = 0.4f,
            };

            /// <summary>A number's slider: faint white track, gold filled part and knob outline, the value in the row's text colour.</summary>
            public static readonly SliderStyle SliderStyle = new SliderStyle
            {
                TrackColor = StyleHelper.GetColor("#FFFFFF", 0.2f),        // #FFFFFF
                FillColor = StyleHelper.GetColor("#F9BF45", 0.9f),         // #F9BF45
                KnobColor = StyleHelper.GetColor("#FCFAF2", 1.0f),         // #FCFAF2
                KnobBorderColor = StyleHelper.GetColor("#F9BF45", 1.0f),   // #F9BF45
                KnobBorderWidth = 2.0f,
                TrackHeight = UILayoutConstants.SettingsMenu.SliderTrackHeight,
                KnobDiameter = UILayoutConstants.SettingsMenu.SliderKnobDiameter,
                LabelColor = StyleHelper.GetColor("#FCFAF2", 1.0f),        // #FCFAF2
                LabelWidth = UILayoutConstants.SettingsMenu.SliderLabelWidth,
                LabelGap = UILayoutConstants.SettingsMenu.SliderLabelGap,
                DisabledOpacity = 0.35f,
            };

            /// <summary>A row's text field: faint box with gold borders (brighter while typing).</summary>
            public static readonly TextFieldStyle TextFieldStyle = new TextFieldStyle
            {
                Box = new SingleBorderRoundedStyle
                {
                    CornerRadius = MainMenu.Button.Border.CornerRadius,
                    BorderStyle = CategoryListMenu.CategoryOff.Outer,
                    BackgroundBrushFactory = new SolidBrushFactory(StyleHelper.GetColor("#FFFFFF", 0.12f)),  // #FFFFFF
                },
                FocusedBox = new SingleBorderRoundedStyle
                {
                    CornerRadius = MainMenu.Button.Border.CornerRadius,
                    BorderStyle = MainMenu.Button.Border.Outer,
                    BackgroundBrushFactory = new SolidBrushFactory(StyleHelper.GetColor("#FFFFFF", 0.2f)),  // #FFFFFF
                },
                TextColor = StyleHelper.GetColor("#FCFAF2", 1.0f),         // #FCFAF2
                PlaceholderColor = StyleHelper.GetColor("#FCFAF2", 0.45f),  // #FCFAF2
                CaretColor = StyleHelper.GetColor("#FCFAF2", 1.0f),        // #FCFAF2
                CaretWidth = 2.0f,
                TextInset = UILayoutConstants.SettingsMenu.TextFieldInset,
                DisabledOpacity = 0.4f,
            };
        }

        /// <summary>
        /// The main menu's saved-game list (<c>UILoadSavedGameMenu</c>, 讀取存檔): the settings
        /// submenu's looks (button and section header), plus a dimmed row for 沒有存檔.
        /// Placeholders for the author to tune.
        /// </summary>
        public static class LoadSavedGameMenu
        {
            /// <summary>
            /// Characters of a save's name shown on its button (about what fits next to the
            /// date and time at <see cref="SettingsMenu.ButtonFont"/>); a longer one is cut with
            /// <c>GameMenuTexts.Ellipsis</c>.
            /// </summary>
            public const int NameLength = 14;

            /// <summary>A save's button.</summary>
            public static readonly IButtonDrawStyle ButtonStyle = SettingsMenu.ButtonStyle;

            /// <summary>A category header (it does nothing when clicked).</summary>
            public static readonly IButtonDrawStyle HeaderStyle = SettingsMenu.HeaderStyle;

            /// <summary>The 沒有存檔 row: a save's box with dimmed text and borders, like a category toggle that is off (it does nothing when clicked).</summary>
            public static readonly IButtonDrawStyle EmptyRowStyle = new DoubleBorderRoundedStyle
            {
                Font = SettingsMenu.ButtonFont,
                TextBrush = CategoryListMenu.CategoryOff.TextBrush,
                BackgroundBrushFactory = MainMenu.Button.Background.BrushFactory,
                OuterBorder = CategoryListMenu.CategoryOff.Outer,
                InnerBorder = CategoryListMenu.CategoryOff.Inner,
                Margin = MainMenu.Button.Border.Margin,
                CornerRadius = MainMenu.Button.Border.CornerRadius
            };
        }

        /// <summary>
        /// The category list submenus (<c>UICategoryListMenu</c>: 殘局闖關, 開局練習). Their
        /// buttons are a quarter of the submenu width
        /// (<c>UILayoutConstants.CategoryListMenu.ButtonWidth</c>, about 154), so they use a
        /// smaller font than the main menu's 36, wrap long names after
        /// <see cref="CategoryListMenu.TitleLineLength"/> characters and put the difficulty
        /// stars on their own line. Box look (borders, background) as the main menu buttons.
        /// <para>
        /// Glyphs: <see cref="StarFilled"/>/<see cref="StarEmpty"/> (U+2605/U+2606) and
        /// <see cref="CategoryOnMark"/>/<see cref="CategoryOffMark"/> (U+25CF/U+25CB) were
        /// checked in the cmap of the bundled <c>Assets/Font/NotoSerifCJKtc-Medium.otf</c>
        /// (and <c>MoeLI.ttf</c>): all present, no fallback font needed.
        /// </para>
        /// </summary>
        public static class CategoryListMenu
        {
            /// <summary>Item and category buttons; at this size about 5 CJK characters fit on a line.</summary>
            public static readonly IFont ButtonFont = StyleHelper.GetFont("NotoSerif", 18, FontStyleFlags.Bold);

            /// <summary>
            /// Characters per line of an item's name (about what fits in a button at
            /// <see cref="ButtonFont"/>); a longer name is broken into lines of this length.
            /// </summary>
            public const int TitleLineLength = 5;

            /// <summary>Difficulty stars: one filled star per level, empty ones up to <see cref="MaxDifficulty"/>.</summary>
            public const string StarFilled = "★";
            public const string StarEmpty = "☆";

            /// <summary>Difficulty is 1 to 5 (docs/ENDGAMES.md, docs/OPENINGS.md); the stars show it out of this many.</summary>
            public const int MaxDifficulty = 5;

            /// <summary>Category toggle labels: mark + category name.</summary>
            public const string CategoryOnMark = "●";
            public const string CategoryOffMark = "○";

            /// <summary>Shown for items without a category (empty <c>PgnGameFile.Category</c>).</summary>
            public const string UncategorizedName = "未分類";

            /// <summary>An item button, and a category toggle that is on.</summary>
            public static readonly IButtonDrawStyle ButtonStyle = new DoubleBorderRoundedStyle
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

                public static readonly BorderStyle Outer = new BorderStyle
                {
                    Width = 4.0f,
                    Color = StyleHelper.GetColor("#F9BF45", 0.3f)  // #F9BF45
                };

                public static readonly BorderStyle Inner = new BorderStyle
                {
                    Width = 2.0f,
                    Color = StyleHelper.GetColor("#F9BF45", 0.3f)  // #F9BF45
                };

                public static readonly IButtonDrawStyle Style = new DoubleBorderRoundedStyle
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

            /// <summary>A section heading (開局練習: 先手 / 後手), shown as <c>── 先手 ──</c>.</summary>
            public static class SectionHeading
            {
                public static readonly IFont Font = StyleHelper.GetFont("NotoSerif", 22, FontStyleFlags.Bold);
                public static readonly Color Color = StyleHelper.GetColor("#F9BF45", 1.0f);  // #F9BF45

                public const string Prefix = "── ";
                public const string Suffix = " ──";

                public static string Format(string section) => Prefix + section + Suffix;
            }

            /// <summary>The message shown when nothing was found.</summary>
            public static class EmptyMessage
            {
                public static readonly IFont Font = StyleHelper.GetFont("NotoSerif", 20, FontStyleFlags.Bold);
                public static readonly Color Color = StyleHelper.GetColor("#FCFAF2", 1.0f);  // #FCFAF2
            }
        }
    }
}

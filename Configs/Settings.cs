/* ----- ----- ----- ----- */
// Settings.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/06
// Update Date: 2025/05/06
// Version: v1.0
/* ----- ----- ----- ----- */

using System.Drawing;

using Chinese_Chess_v3.Utils;

using SharedLib.MathUtils;

namespace Chinese_Chess_v3.Configs
{
    public static class Settings
    {
        public static bool EnableDebugMode { get; set; } = true;
        public static string CurrentUser { get; set; } = "Player";
        
        /// <summary>
        /// Encapsulates menu related setting values.
        /// </summary>
        public class MainMenu
        {
            // Location start point
            public static readonly Vector2F Position = new Vector2F(0.0f, 0.0f);
            // Size
            public static Vector2F Size => new Vector2F(360.0f, Board.Size.Y);

            // Space between the edge of the form and the MainMenu object
            public const float Margin = 40.0f;
            
            /// <summary>
            /// Encapsulates MainMenu:Button related setting values.
            /// </summary>
            public class Button
            {
                // Location start point
                public static readonly Vector2F Position = MainMenu.Position + new Vector2F(Margin, Margin * 2);
                // Size
                public static Vector2F Size => new Vector2F(MainMenu.Size.X - Margin * 2.0f, 60.0f);
            }

        }

        /// <summary>
        /// Encapsulates Board related setting values.
        /// </summary>
        public class Board
        {
            // Location start point
            public static Vector2F Position => new Vector2F(MainMenu.Size.X, MainMenu.Position.Y);
            // Size
            public static readonly Vector2F Size = new Vector2F(
                (Constants.Board.Columns - 1) * Grid.Size + Margin * 2,
                (Constants.Board.Rows - 1) * Grid.Size + Margin * 2);

            // Space between the edge of the form and the Board
            public const float Margin = 60.0f;
            
            /// <summary>
            /// Encapsulates Board:Grid related setting values.
            /// </summary>
            public class Grid
            {
                // Location start point
                public static readonly Vector2F Position = Board.Position + Margin;
            
                // Distance between pieces
                public const float Size = 80.0f;
            
                // Board line width
                public const int LineWidth = 2;
            }
        }

        /// <summary>
        /// Encapsulates Sidebar related setting values.
        /// </summary>
        public static class Sidebar
        {
            // Location start point
            public static readonly Vector2F Position = new Vector2F(Board.Size.X, MainMenu.Position.Y);
            // Size
            public static readonly Vector2F Size = new Vector2F(360.0f, Board.Size.Y);

            // Space between the edge of the form and the sidebar object
            public const float Margin = 20.0f;

            /// <summary>
            /// Encapsulates Sidebar:Infoboard related setting values.
            /// </summary>
            public class Infoboard
            {
                // Location start point
                public static readonly Vector2F Position = Sidebar.Position + Margin;
                // Size
                public static readonly Vector2F Size = new Vector2F(Sidebar.Size.X - Margin * 2.0f, 160.0f);
            }

            /// <summary>
            /// Encapsulates Sidebar:Logger related setting values.
            /// </summary>
            public class Logger
            {
                // Location start point
                public static readonly Vector2F Position = new Vector2F(
                    Sidebar.Position.X + Margin,
                    Infoboard.Position.Y + Infoboard.Size.Y + Margin * 2.0f);
                // Size
                public static readonly Vector2F Size = new Vector2F(Sidebar.Size.X - Margin * 2.0f, 200.0f);
            }

            // Color
            public static readonly Color BackgroundColor = StyleHelper.GetColor("#0A0A0A");  // #0A0A0A
        }
    }
}
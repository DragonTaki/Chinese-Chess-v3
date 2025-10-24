/* ----- ----- ----- ----- */
// LoggerBoxRenderer.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/10/22
// Update Date: 2025/10/22
// Version: v1.0
/* ----- ----- ----- ----- */

using System;
using System.Drawing;
using System.Drawing.Drawing2D;

using Chinese_Chess_v3.Game.Constants.UI;

namespace Chinese_Chess_v3.Game.UI.Screens.Games.Sidebars.LoggerBoxes
{
    public class LoggerBoxRenderer
    {
        /// <summary>
        /// Width of the drawing canvas.
        /// </summary>
        private int _width;
        public int Width
        {
            get => _width;
            set
            {
                _width = Math.Max(value, 1);
            }
        }

        /// <summary>
        /// Height of the drawing canvas.
        /// </summary>
        private int _height;
        public int Height
        {
            get => _height;
            set
            {
                _height = Math.Max(value, 1);
            }
        }
        private LoggerBox _loggerBox;

        public LoggerBoxRenderer(LoggerBox loggerBox)
        {
            _loggerBox = loggerBox;
        }

        public void Draw(Graphics g)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            using var bgBrush = new SolidBrush(Color.Red);
            g.FillRectangle(bgBrush, UILayoutConstants.Sidebar.Logger.Layout);
            //Console.WriteLine(UILayoutConstants.Sidebar.Logger.Layout);
        }
    }
}

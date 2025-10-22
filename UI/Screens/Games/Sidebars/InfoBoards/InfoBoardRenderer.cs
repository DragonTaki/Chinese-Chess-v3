/* ----- ----- ----- ----- */
// InfoBoardRenderer.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/10/22
// Update Date: 2025/10/22
// Version: v1.0
/* ----- ----- ----- ----- */

using System;
using System.Drawing;
using System.Drawing.Drawing2D;

using Chinese_Chess_v3.Configs.Sidebar;
using Chinese_Chess_v3.Constants.UI;
using Chinese_Chess_v3.Models;
using Chinese_Chess_v3.Utils.GraphicsUtils;
using Chinese_Chess_v3.Utils.GraphicsUtils.GraphicsPaths;

namespace Chinese_Chess_v3.UI.Screens.Games.Sidebars.InfoBoards
{
    /// <summary>
    /// Responsible for drawing the InfoBoard visuals.
    /// </summary>
    public class InfoBoardRenderer
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
        private readonly InfoBoardHandler _handler;

        private readonly Font _nameFont = InfoBoardSettings.NameFont;
        private readonly Font _timerFont = InfoBoardSettings.TimerFont;

        public InfoBoardRenderer(InfoBoardHandler handler)
        {
            _handler = handler;
        }

        public void Draw(Graphics g)
        {
            GraphicsHelper.ApplyHighQualitySettings(g);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.Clear(Color.Transparent);

            DrawBackground(g);
            DrawPlayers(g);
        }

        public void RenderBackground(Graphics g)
        {
            using (GraphicsPath shieldPath = ShieldPath.Create(_width, _height))
            using (SolidBrush bgBrush = new SolidBrush(UILayoutConstants.Sidebar.BackgroundColor))
            {
                g.FillPath(bgBrush, shieldPath);
            }
        }

        private void DrawBackground(Graphics g)
        {
            // Fill left and right side based on current turn
            int w = _width;
            int h = _height;
            using (GraphicsPath fullShield = ShieldPath.Create(w, h))
            {
                Region leftRegion = new Region(fullShield);
                leftRegion.Intersect(new Rectangle(0, 0, w / 2, h));
                using (SolidBrush leftBrush = new SolidBrush(
                    _handler.CurrentTurn == PlayerSide.Black ? Color.Gold : Color.Gray))
                {
                    g.FillRegion(leftBrush, leftRegion);
                }

                Region rightRegion = new Region(fullShield);
                rightRegion.Intersect(new Rectangle(w / 2, 0, w / 2, h));
                using (SolidBrush rightBrush = new SolidBrush(
                    _handler.CurrentTurn == PlayerSide.Red ? Color.Gold : Color.LightCoral))
                {
                    g.FillRegion(rightBrush, rightRegion);
                }
            }
        }

        private void DrawPlayers(Graphics g)
        {
            int w = _width;
            int h = _height;

            DrawPlayerSection(g, 0, 0, w / 2, h, _handler.BlackPlayerName, _handler.BlackTime,
                              _handler.CurrentTurn == PlayerSide.Black);

            DrawPlayerSection(g, w / 2, 0, w / 2, h, _handler.RedPlayerName, _handler.RedTime,
                              _handler.CurrentTurn == PlayerSide.Red);
        }

        private void DrawPlayerSection(Graphics g, int x, int y, int width, int height,
                                       string playerName, TimeSpan time, bool isActive)
        {
            // Timer background
            Rectangle timerRect = new Rectangle(x + 20, y + 50, width - 40, 40);
            using (SolidBrush timerBgBrush = new SolidBrush(Color.DimGray))
                g.FillRectangle(timerBgBrush, timerRect);

            // Timer text
            using (SolidBrush timerTextBrush = new SolidBrush(isActive ? Color.Gold : Color.DeepSkyBlue))
            using (StringFormat timerFormat = new StringFormat
            { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
            {
                g.DrawString(time.ToString(@"mm\:ss"), _timerFont, timerTextBrush, timerRect, timerFormat);
            }

            // Player name
            using (SolidBrush nameBrush = new SolidBrush(Color.White))
            using (StringFormat nameFormat = new StringFormat
            { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Near })
            {
                g.DrawString(playerName, _nameFont, nameBrush, new Rectangle(x, y + 10, width, 30), nameFormat);
            }
        }
    }
}

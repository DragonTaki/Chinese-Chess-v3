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

using Chinese_Chess_v3.Game.Configs.Sidebar;
using Chinese_Chess_v3.Game.Constants.UI;
using Chinese_Chess_v3.Game.Models;

using Engine.GraphicsUtils;
using Engine.GraphicsUtils.GraphicsPaths;

namespace Chinese_Chess_v3.Game.UI.Screens.Games.Sidebars.InfoBoards
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
            //g.Clear(Color.Transparent);

            // Draw sidebar background
            using var bgBrush = new SolidBrush(Color.Red);
            //g.FillRectangle(bgBrush, UILayoutConstants.Sidebar.Infoboard.Layout);
            //Console.WriteLine(UILayoutConstants.Sidebar.Infoboard.Layout);

            // 背景底色
            RenderBackground(g);

            DrawShieldBackground(g);
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

        private void DrawShieldBackground(Graphics g)
        {
            float baseX = UILayoutConstants.Sidebar.Infoboard.Position.X;
            float baseY = UILayoutConstants.Sidebar.Infoboard.Position.Y;
            float width = UILayoutConstants.Sidebar.Infoboard.Size.X;
            float height = UILayoutConstants.Sidebar.Infoboard.Size.Y;
            int inset = 4;

            // 外層盾牌
            using GraphicsPath fullShield = ShieldPath.Create(width, height);
            fullShield.Transform(new Matrix(1, 0, 0, 1, baseX, baseY));

            // 左半背景
            using Region leftRegion = new Region(fullShield);
            leftRegion.Intersect(new RectangleF(baseX, baseY, width / 2f, height));
            using SolidBrush leftBrush = new SolidBrush(_handler.CurrentTurn == PlayerSide.Black ? Color.Gold : Color.Gray);
            g.FillRegion(leftBrush, leftRegion);

            // 右半背景
            using Region rightRegion = new Region(fullShield);
            rightRegion.Intersect(new RectangleF(baseX + width / 2f, baseY, width / 2f, height));
            using SolidBrush rightBrush = new SolidBrush(_handler.CurrentTurn == PlayerSide.Red ? Color.Gold : Color.LightCoral);
            g.FillRegion(rightBrush, rightRegion);

            // 內層盾牌
            using GraphicsPath innerShield = ShieldPath.Create(width - 2 * inset, height - 2 * inset);
            // 移到內層位置
            innerShield.Transform(new System.Drawing.Drawing2D.Matrix(1, 0, 0, 1, baseX + inset, baseY + inset));

            // 左半內層遮罩
            using Region leftOverlay = new Region(innerShield);
            float leftWidth = (_handler.CurrentTurn == PlayerSide.Black ? (width / 2f - inset) : width / 2f);
            leftOverlay.Intersect(new RectangleF(baseX + inset, baseY + inset, leftWidth, height - 2*inset));
            g.FillRegion(Brushes.Black, leftOverlay);

            // 右半內層遮罩
            using Region rightOverlay = new Region(innerShield);
            float rightX = (_handler.CurrentTurn == PlayerSide.Red ? baseX + width / 2f + inset : baseX + width / 2f);
            float rightWidth = (_handler.CurrentTurn == PlayerSide.Red ? width / 2f - inset : width / 2f);
            rightOverlay.Intersect(new RectangleF(rightX, baseY + inset, rightWidth, height - 2*inset));
            g.FillRegion(Brushes.DarkRed, rightOverlay);
            
            float centerX = baseX + width / 2f + inset / 2f;
            float startY = baseY;
            float endY = baseY + height - inset * 2;

            using (Pen centerLinePen = new Pen(Color.Gold, 4))
            {
                centerLinePen.Alignment = PenAlignment.Center;
                g.DrawLine(centerLinePen, centerX, startY, centerX, endY);
            }
        }

        private void DrawPlayers(Graphics g)
        {
            float baseX = UILayoutConstants.Sidebar.Infoboard.Position.X;
            float baseY = UILayoutConstants.Sidebar.Infoboard.Position.Y;
            float width = UILayoutConstants.Sidebar.Infoboard.Size.X;
            float height = UILayoutConstants.Sidebar.Infoboard.Size.Y;

            DrawPlayerSection(g, baseX, baseY, width / 2.0f, height, _handler.BlackPlayerName, _handler.BlackTime,
                              _handler.CurrentTurn == PlayerSide.Black);

            DrawPlayerSection(g, baseX + width / 2.0f, baseY, width / 2.0f, height, _handler.RedPlayerName, _handler.RedTime,
                              _handler.CurrentTurn == PlayerSide.Red);
        }

        private void DrawPlayerSection(Graphics g, float x, float y, float width, float height,
                                       string playerName, TimeSpan time, bool isActive)
        {
            // Timer background
            RectangleF timerRect = new RectangleF(x + 20.0f, y + 50.0f, width - 40.0f, 40.0f);
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
                g.DrawString(playerName, _nameFont, nameBrush, new RectangleF(x, y + 10.0f, width, 30.0f), nameFormat);
            }
        }
    }
}

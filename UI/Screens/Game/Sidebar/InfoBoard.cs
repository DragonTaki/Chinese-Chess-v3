/* ----- ----- ----- ----- */
// InfoBoard.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/07
// Update Date: 2025/05/07
// Version: v1.0
/* ----- ----- ----- ----- */

using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

using Chinese_Chess_v3.Configs.Sidebar;
using Chinese_Chess_v3.Core;
using Chinese_Chess_v3.UI.Constants;
using Chinese_Chess_v3.Models;
using Chinese_Chess_v3.Utils.GraphicsUtils;

namespace Chinese_Chess_v3.UI.Screens.Game.Sidebar
{
    public class InfoBoard : Control
    {
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string BlackPlayerName { get; set; } = "黑方玩家";
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string RedPlayerName { get; set; } = "紅方玩家";
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public TimeSpan BlackTime { get; set; } = TimeSpan.FromMinutes(5);
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public TimeSpan RedTime { get; set; } = TimeSpan.FromMinutes(5);
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]

        private readonly Font _nameFont = InfoBoardSettings.NameFont;
        private readonly Font _timerFont = InfoBoardSettings.TimerFont;
        private readonly Pen _glowPen = new Pen(Color.Gold, 3);

        public InfoBoard()
        {
            this.SetStyle(ControlStyles.AllPaintingInWmPaint |
                          ControlStyles.OptimizedDoubleBuffer |
                          ControlStyles.ResizeRedraw |
                          ControlStyles.UserPaint, true);
            this.Location = UILayoutConstants.Sidebar.Infoboard.Position.ToPoint();
            this.Size = UILayoutConstants.Sidebar.Infoboard.Size.ToSize();
            this.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            this.BackColor = Color.Transparent;
            GameManager.Instance.TurnChanged += OnTurnChanged;
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            GraphicsHelper.ApplyHighQualitySettings(g);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.Clear(this.BackColor);

            int w = this.Width;
            int h = this.Height;
            int inset = 4;

            var fullShield = GraphicsPaths.CreateShieldPath(w, h);
            var innerShield = GraphicsPaths.CreateShieldPath(w - inset * 2, h - inset * 2);
            innerShield.Transform(new Matrix(1, 0, 0, 1, inset, inset)); // move inward

            // Black side background
            Region leftRegion = new Region(fullShield);
            leftRegion.Intersect(new Rectangle(0, 0, w / 2, h));
            using (SolidBrush bgBrush = new SolidBrush(
                GameManager.Instance.CurrentTurn == PlayerSide.Black ? Color.Gold : Color.Gray))
            {
                g.FillRegion(bgBrush, leftRegion);
            }

            // Red side background
            Region rightRegion = new Region(fullShield);
            rightRegion.Intersect(new Rectangle(w / 2, 0, w / 2, h));
            using (SolidBrush bgBrush = new SolidBrush(
                GameManager.Instance.CurrentTurn == PlayerSide.Red ? Color.Gold : Color.LightCoral))
            {
                g.FillRegion(bgBrush, rightRegion);
            }

            // Draw inner shield overlays
            using (GraphicsPath leftInnerPath = GraphicsPaths.CreateShieldPath(w - inset * 2, h - inset * 2))
            using (GraphicsPath rightInnerPath = GraphicsPaths.CreateShieldPath(w - inset * 2, h - inset * 2))
            {
                leftInnerPath.Transform(new Matrix(1, 0, 0, 1, inset, inset));
                rightInnerPath.Transform(new Matrix(1, 0, 0, 1, inset, inset));

                bool isBlackTurn = GameManager.Instance.CurrentTurn == PlayerSide.Black;
                bool isRedTurn = GameManager.Instance.CurrentTurn == PlayerSide.Red;

                // 黑方內層（左半）
                Region leftOverlay = new Region(leftInnerPath);
                int leftWidth = isBlackTurn ? (w / 2 - 4) : (w / 2);
                leftOverlay.Intersect(new Rectangle(0, 0, leftWidth, h));
                using (SolidBrush leftBrush = new SolidBrush(Color.Black))
                    g.FillRegion(leftBrush, leftOverlay);

                // 紅方內層（右半）
                Region rightOverlay = new Region(rightInnerPath);
                int rightX = isRedTurn ? (w / 2 + 4) : (w / 2);
                int rightWidth = isRedTurn ? (w / 2 - 4) : (w / 2);
                rightOverlay.Intersect(new Rectangle(rightX, 0, rightWidth, h));
                using (SolidBrush rightBrush = new SolidBrush(Color.DarkRed))
                    g.FillRegion(rightBrush, rightOverlay);
            }

            // Draw player info
            DrawPlayerSection(g, 0, 0, w / 2, h, BlackPlayerName, BlackTime, GameManager.Instance.CurrentTurn == PlayerSide.Black);
            DrawPlayerSection(g, w / 2, 0, w / 2, h, RedPlayerName, RedTime, GameManager.Instance.CurrentTurn == PlayerSide.Red);
        }
        protected override void OnPaintBackground(PaintEventArgs e)
        {
            // 不呼叫 base.OnPaintBackground(e)，避免預設填滿矩形背景

            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            int w = this.Width;
            int h = this.Height;

            using (GraphicsPath shieldPath = GraphicsPaths.CreateShieldPath(w, h))
            using (SolidBrush bgBrush = new SolidBrush(UILayoutConstants.Sidebar.BackgroundColor))
            {
                g.FillPath(bgBrush, shieldPath); // 只填滿盾牌形狀，不畫整個矩形
            }
        }
        private void DrawPlayerSection(Graphics g, int x, int y, int width, int height,
                                    string playerName, TimeSpan time, bool isActive)
        {
            using (SolidBrush nameBrush = new SolidBrush(Color.White))
            using (StringFormat nameFormat = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Near })
                g.DrawString(playerName, _nameFont, nameBrush, new Rectangle(x, y + 10, width, 30), nameFormat);

            Rectangle timerRect = new Rectangle(x + 20, y + 50, width - 40, 40);
            using (SolidBrush timerBgBrush = new SolidBrush(Color.DimGray))
                g.FillRectangle(timerBgBrush, timerRect);

            using (SolidBrush timerTextBrush = new SolidBrush(isActive ? Color.Gold : Color.DeepSkyBlue))
            using (StringFormat timerFormat = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
                g.DrawString(time.ToString(@"mm\:ss"), _timerFont, timerTextBrush, timerRect, timerFormat);
        }
        protected void OnPaint_old(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            GraphicsHelper.ApplyHighQualitySettings(g);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.Clear(this.BackColor);

            int w = this.Width;
            int h = this.Height;

            // Info board: Shield shape
            using (GraphicsPath shieldPath = CreateShieldPath(w, h))
            {
                // Left side: Black player
                using (Region leftRegion = new Region(shieldPath))
                {
                    leftRegion.Intersect(new Rectangle(0, 0, w / 2, h));
                    g.SetClip(leftRegion, CombineMode.Replace);
                    using (GraphicsPath leftPath = (GraphicsPath)shieldPath.Clone())
                    {
                        DrawPlayerSection(g, 0, 0, w / 2, h, Color.Black, BlackPlayerName, BlackTime,
                                          GameManager.Instance.CurrentTurn == PlayerSide.Black, leftPath);
                    }
                    g.ResetClip();
                }

                // Right side: Red player
                using (Region rightRegion = new Region(shieldPath))
                {
                    rightRegion.Intersect(new Rectangle(w / 2, 0, w / 2, h));
                    g.SetClip(rightRegion, CombineMode.Replace);
                    using (GraphicsPath rightPath = (GraphicsPath)shieldPath.Clone())
                    {
                        DrawPlayerSection(g, w / 2, 0, w / 2, h, Color.DarkRed, RedPlayerName, RedTime,
                                          GameManager.Instance.CurrentTurn == PlayerSide.Red, rightPath);
                    }
                    g.ResetClip();
                }

                g.ResetClip();
            }
        }

        private void DrawPlayerSection(Graphics g, int x, int y, int width, int height,
                                       Color backgroundColor, string playerName, TimeSpan time,
                                       bool isActive, GraphicsPath borderPath)
        {
            // Background
            using (SolidBrush bgBrush = new SolidBrush(backgroundColor))
                g.FillRectangle(bgBrush, x, y, width, height);

            // Draw active glow outline
            if (isActive)
            {
                using (Pen fixPen = new Pen(this.BackColor, 5))
                {
                    fixPen.Alignment = PenAlignment.Center;
                    g.DrawPath(fixPen, borderPath);
                }

                using (Pen activePen = new Pen(Color.Gold, 4))
                {
                    activePen.Alignment = PenAlignment.Inset;
                    g.DrawPath(activePen, borderPath);
                }

                // 🔴 中線獨立繪製
                int centerStartX = this.Width / 2 - 2;
                int centerEndX = this.Width / 2 + 2;
                int extension = 2;
                int startY = y - extension;
                int endY = y + height + extension;

                using (Pen centerLinePen = new Pen(Color.Gold, 4))
                {
                    // 移除 alignment，讓線條置中畫在 path 上
                    centerLinePen.Alignment = PenAlignment.Center;

                    if (GameManager.Instance.CurrentTurn == PlayerSide.Black)
                    {
                        g.DrawLine(centerLinePen, centerStartX, startY, centerStartX, endY);
                    }
                    else
                    {
                        g.DrawLine(centerLinePen, centerEndX, startY, centerEndX, endY);
                    }
                }
            }
            else
            {
                // Draw inactive borders with different colors
                using (Pen inactivePenBlack = new Pen(Color.Gray, 4), inactivePenRed = new Pen(Color.LightCoral, 4))
                {
                    // Left side: Black player
                    if (backgroundColor == Color.Black)
                    {
                        inactivePenBlack.Alignment = PenAlignment.Inset;
                        g.DrawPath(inactivePenBlack, borderPath);
                    }
                    // Right side: Red player
                    else
                    {
                        inactivePenRed.Alignment = PenAlignment.Inset;
                        g.DrawPath(inactivePenRed, borderPath);
                    }
                }
            }

            // Player name
            using (SolidBrush nameBrush = new SolidBrush(Color.White))
            using (StringFormat nameFormat = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Near })
                g.DrawString(playerName, _nameFont, nameBrush, new Rectangle(x, y + 10, width, 30), nameFormat);

            // Timer background
            Rectangle timerRect = new Rectangle(x + 20, y + 50, width - 40, 40);
            using (SolidBrush timerBgBrush = new SolidBrush(Color.DimGray))
                g.FillRectangle(timerBgBrush, timerRect);

            // Timer text
            using (SolidBrush timerTextBrush = new SolidBrush(isActive ? Color.Gold : Color.DeepSkyBlue))
            using (StringFormat timerFormat = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
                g.DrawString(time.ToString(@"mm\:ss"), _timerFont, timerTextBrush, timerRect, timerFormat);
        }

        // Create shield shape
        private GraphicsPath CreateShieldPath(int width, int height)
        {
            GraphicsPath path = new GraphicsPath();

            int curveHeight = 20;
            int bottomPointHeight = 40;

            path.StartFigure();
            path.AddArc(0, 0, curveHeight * 2, curveHeight * 2, 180, 90); // 左上角
            path.AddLine(curveHeight, 0, width - curveHeight, 0);         // 上方直線
            path.AddArc(width - curveHeight * 2, 0, curveHeight * 2, curveHeight * 2, 270, 90); // 右上角
            path.AddLine(width, curveHeight, width, height - bottomPointHeight); // 右側直線

            path.AddBezier(width, height - bottomPointHeight,
                        width * 0.75f, height,
                        width * 0.25f, height,
                        0, height - bottomPointHeight); // 底部尖端

            path.AddLine(0, height - bottomPointHeight, 0, curveHeight); // 左側直線
            path.CloseFigure();

            return path;
        }
        private GraphicsPath ExtractSideFromShield(GraphicsPath fullPath, Rectangle sideRect)
        {
            using (Region sideRegion = new Region(sideRect))
            {
                sideRegion.Intersect(fullPath);
                return sideRegion.GetRegionScans(new Matrix()).Length > 0
                    ? RegionToPath(sideRegion)
                    : new GraphicsPath(); // fallback
            }
        }

        private GraphicsPath RegionToPath(Region region)
        {
            GraphicsPath path = new GraphicsPath();
            foreach (RectangleF rect in region.GetRegionScans(new Matrix()))
            {
                path.AddRectangle(rect);
            }
            return path;
        }
        private void OnTurnChanged(PlayerSide newTurn)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke((Action)(() => this.Invalidate()));
            }
            else
            {
                this.Invalidate();
            }
        }
    }
}

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
using Chinese_Chess_v3.Game.Models;

using Engine.Geometry;
using Engine.GraphicsUtils;
using Engine.GraphicsUtils.GraphicsPaths;
using Engine.UI.Core.Renderers;

namespace Chinese_Chess_v3.Game.UI.Screens.Games.Sidebars.InfoBoards
{
    /// <summary>
    /// Responsible for drawing the InfoBoard visuals.
    /// </summary>
    public class InfoBoardRenderer : UIContainerRenderer<InfoBoard, InfoBoardHandler, InfoBoardRenderer>
    {
        protected CompositeRenderer<InfoBoard, InfoBoardHandler, InfoBoardRenderer> _composite = new();

        public InfoBoardRenderer() { }
        
        protected override void AfterInit()
        {
            SetupRendererChildren();
        }

        private void SetupRendererChildren()
        {
            if (_composite.ListCount == 0)
            {
                _composite
                    .Add(new ClassicBoard());
            }
        }

        protected override void OnRender(Graphics g, InfoBoard element)
        {
            _composite.Render(g, element);
        }

        private class ClassicBoard : UIRenderer<InfoBoard, InfoBoardHandler, InfoBoardRenderer>
        {
            private InfoBoardHandler _handler;
            private LayoutF Layout;
            protected readonly Font _nameFont;
            protected readonly Font _timerFont;

            public ClassicBoard()
            {
                _nameFont = InfoBoardSettings.NameFont;
                _timerFont = InfoBoardSettings.TimerFont;
            }

            protected override void OnRender(Graphics g, InfoBoard element)
            {
                if (_handler == null)
                {
                    _handler = element.Handler;
                    Layout = element.Layout;
                }
                GraphicsHelper.ApplyHighQualitySettings(g);

                using var bgBrush = new SolidBrush(Color.Red);

                DrawShieldBackground(g);

                DrawPlayers(g);
            }

            private void DrawShieldBackground(Graphics g)
            {
                float baseX = Layout.X;
                float baseY = Layout.Y;
                float width = Layout.Width;
                float height = Layout.Height;
                int inset = 4;

                // 外層盾牌
                using GraphicsPath fullShield = ShieldPath.Create(width, height);
                fullShield.Transform(new Matrix(1, 0, 0, 1, baseX, baseY));

                PlayerSide currentTurn = _handler.GameManager.CurrentTurn;

                // 左半背景
                using Region leftRegion = new Region(fullShield);
                leftRegion.Intersect(new RectangleF(baseX, baseY, width / 2f, height));
                using SolidBrush leftBrush = new SolidBrush(currentTurn == PlayerSide.Black ? Color.Gold : Color.Gray);
                g.FillRegion(leftBrush, leftRegion);

                // 右半背景
                using Region rightRegion = new Region(fullShield);
                rightRegion.Intersect(new RectangleF(baseX + width / 2f, baseY, width / 2f, height));
                using SolidBrush rightBrush = new SolidBrush(currentTurn == PlayerSide.Red ? Color.Gold : Color.LightCoral);
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
                float baseX = Layout.X;
                float baseY = Layout.Y;
                float width = Layout.Width;
                float height = Layout.Height;

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
}

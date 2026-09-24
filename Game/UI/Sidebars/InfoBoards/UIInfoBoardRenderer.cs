/* ----- ----- ----- ----- */
// UIInfoBoardRenderer.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/10/22
// Update Date: 2026/09/24
// Version: v2.0
/* ----- ----- ----- ----- */

using System.Drawing;

using Chinese_Chess_v3.Game.Core.Players;

using Engine.Geometry;
using Engine.GraphicsUtils;
using Engine.GraphicsUtils.GraphicsPaths;
using Engine.Platform;
using Engine.UI.Core.Renderers;

namespace Chinese_Chess_v3.Game.UI.Sidebars.InfoBoards
{
    /// <summary>
    /// Responsible for drawing the InfoBoard visuals.
    /// </summary>
    public class UIInfoBoardRenderer : UIContainerRenderer<UIInfoBoard, UIInfoBoardHandler, UIInfoBoardRenderer>
    {
        protected CompositeRenderer<UIInfoBoard, UIInfoBoardHandler, UIInfoBoardRenderer> _composite = new();

        public UIInfoBoardRenderer() { }

        protected override void AfterInit()
        {
            SetupRendererChildren();
        }

        private void SetupRendererChildren()
        {
            if (_composite.ListCount == 0)
            {
                _composite
                    .Add(new ClassicInfoBoard());
            }
        }

        public override void OnRender(IGraphics g, UIInfoBoard element)
        {
            _composite.Render(g, element);
        }

        private class ClassicInfoBoard : UIRenderer<UIInfoBoard, UIInfoBoardHandler, UIInfoBoardRenderer>
        {
            private UIInfoBoardHandler _handler;
            private LayoutF Layout;
            protected readonly IFont _nameFont;
            protected readonly IFont _timerFont;

            public ClassicInfoBoard()
            {
                _nameFont = UIInfoBoardSettings.NameFont;
                _timerFont = UIInfoBoardSettings.TimerFont;
            }

            public override void OnRender(IGraphics g, UIInfoBoard element)
            {
                if (_handler == null)
                {
                    _handler = element.Handler;
                    Layout = element.Layout;
                }
                GraphicsHelper.ApplyHighQualitySettings(g);

                DrawShieldBackground(g, element);

                DrawPlayers(g, element);
            }

            private void DrawShieldBackground(IGraphics g, UIInfoBoard element)
            {
                float baseX = Layout.X;
                float baseY = Layout.Y;
                float width = Layout.Width;
                float height = Layout.Height;
                int inset = 4;

                // 外層盾牌
                using IGraphicsPath fullShield = ShieldPath.Create(width, height);
                using (var translate = GraphicsBackend.Factory.CreateMatrix())
                {
                    translate.Translate(baseX, baseY);
                    fullShield.Transform(translate);
                }

                PlayerSide currentTurn = element.GameManager.CurrentTurn;

                // 左半背景
                using IRegion leftRegion = GraphicsBackend.Factory.CreateRegion(fullShield);
                leftRegion.Intersect(new RectangleF(baseX, baseY, width / 2f, height));
                using IBrush leftBrush = GraphicsBackend.Factory.CreateSolidBrush(currentTurn == PlayerSide.Player2 ? Color.Gold : Color.Gray);
                g.FillRegion(leftBrush, leftRegion);

                // 右半背景
                using IRegion rightRegion = GraphicsBackend.Factory.CreateRegion(fullShield);
                rightRegion.Intersect(new RectangleF(baseX + width / 2f, baseY, width / 2f, height));
                using IBrush rightBrush = GraphicsBackend.Factory.CreateSolidBrush(currentTurn == PlayerSide.Player1 ? Color.Gold : Color.LightCoral);
                g.FillRegion(rightBrush, rightRegion);

                // 內層盾牌
                using IGraphicsPath innerShield = ShieldPath.Create(width - 2 * inset, height - 2 * inset);
                // 移到內層位置
                using (var innerTranslate = GraphicsBackend.Factory.CreateMatrix())
                {
                    innerTranslate.Translate(baseX + inset, baseY + inset);
                    innerShield.Transform(innerTranslate);
                }

                // 左半內層遮罩
                using IRegion leftOverlay = GraphicsBackend.Factory.CreateRegion(innerShield);
                float leftWidth = (element.GameManager.CurrentTurn == PlayerSide.Player2 ? (width / 2f - inset) : width / 2f);
                leftOverlay.Intersect(new RectangleF(baseX + inset, baseY + inset, leftWidth, height - 2*inset));
                using IBrush blackOverlayBrush = GraphicsBackend.Factory.CreateSolidBrush(Color.Black);
                g.FillRegion(blackOverlayBrush, leftOverlay);

                // 右半內層遮罩
                using IRegion rightOverlay = GraphicsBackend.Factory.CreateRegion(innerShield);
                float rightX = (element.GameManager.CurrentTurn == PlayerSide.Player1 ? baseX + width / 2f + inset : baseX + width / 2f);
                float rightWidth = (element.GameManager.CurrentTurn == PlayerSide.Player1 ? width / 2f - inset : width / 2f);
                rightOverlay.Intersect(new RectangleF(rightX, baseY + inset, rightWidth, height - 2*inset));
                using IBrush darkRedOverlayBrush = GraphicsBackend.Factory.CreateSolidBrush(Color.DarkRed);
                g.FillRegion(darkRedOverlayBrush, rightOverlay);

                float centerX = baseX + width / 2f + inset / 2f;
                float startY = baseY;
                float endY = baseY + height - inset * 2;

                using (IPen centerLinePen = GraphicsBackend.Factory.CreatePen(Color.Gold, 4))
                {
                    centerLinePen.Alignment = PenLineAlignment.Center;
                    g.DrawLine(centerLinePen, centerX, startY, centerX, endY);
                }
            }

            private void DrawPlayers(IGraphics g, UIInfoBoard element)
            {
                float baseX = Layout.X;
                float baseY = Layout.Y;
                float width = Layout.Width;
                float height = Layout.Height;

                DrawPlayerSection(g, baseX, baseY, width / 2.0f, height,
                    element.Player2Name,
                    element.GameManager.Player2.Timer.GetTotalTimeString(),
                    element.GameManager.Player2.Timer.GetStepTimeString(),
                    element.GameManager.CurrentTurn == PlayerSide.Player2);

                DrawPlayerSection(g, baseX + width / 2.0f, baseY, width / 2.0f, height,
                    element.Player1Name,
                    element.GameManager.Player1.Timer.GetTotalTimeString(),
                    element.GameManager.Player1.Timer.GetStepTimeString(),
                    element.GameManager.CurrentTurn == PlayerSide.Player1);
            }

            private void DrawPlayerSection(IGraphics g, float x, float y, float width, float height,
                string playerName, string totalTimeString, string stepTimeString, bool isActive)
            {
                // Timer background
                RectangleF totalTimerRect = new RectangleF(x + 20.0f, y + 50.0f, width - 40.0f, 40.0f);
                using (IBrush timerBgBrush = GraphicsBackend.Factory.CreateSolidBrush(Color.DimGray))
                    g.FillRectangle(timerBgBrush, totalTimerRect);

                // Timer text
                using (IBrush timerTextBrush = GraphicsBackend.Factory.CreateSolidBrush(isActive ? Color.Gold : Color.DeepSkyBlue))
                using (IStringFormat timerFormat = GraphicsBackend.Factory.CreateStringFormat())
                {
                    timerFormat.Alignment = TextAlign.Center;
                    timerFormat.LineAlignment = TextAlign.Center;
                    g.DrawString(totalTimeString, _timerFont, timerTextBrush, totalTimerRect, timerFormat);
                }

                // Timer background
                RectangleF stepTimerRect = new RectangleF(x + 20.0f, y + 95.0f, width - 40.0f, 40.0f);
                using (IBrush timerBgBrush = GraphicsBackend.Factory.CreateSolidBrush(Color.DimGray))
                    g.FillRectangle(timerBgBrush, stepTimerRect);

                // Timer text
                using (IBrush timerTextBrush = GraphicsBackend.Factory.CreateSolidBrush(isActive ? Color.Gold : Color.DeepSkyBlue))
                using (IStringFormat timerFormat = GraphicsBackend.Factory.CreateStringFormat())
                {
                    timerFormat.Alignment = TextAlign.Center;
                    timerFormat.LineAlignment = TextAlign.Center;
                    g.DrawString(stepTimeString, _timerFont, timerTextBrush, stepTimerRect, timerFormat);
                }

                // Player name
                using (IBrush nameBrush = GraphicsBackend.Factory.CreateSolidBrush(Color.White))
                using (IStringFormat nameFormat = GraphicsBackend.Factory.CreateStringFormat())
                {
                    nameFormat.Alignment = TextAlign.Center;
                    nameFormat.LineAlignment = TextAlign.Near;
                    g.DrawString(playerName, _nameFont, nameBrush, new RectangleF(x, y + 10.0f, width, 30.0f), nameFormat);
                }
            }
        }
    }
}

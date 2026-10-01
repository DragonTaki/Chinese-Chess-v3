/* ----- ----- ----- ----- */
// UIInfoBoardRenderer.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/10/22
// Update Date: 2026/10/01
// Version: v2.2
/* ----- ----- ----- ----- */

using System.Drawing;

using Chinese_Chess_v3.Game.Core.Pieces;
using Chinese_Chess_v3.Game.UI.Constants;
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
            // Bind the composite and its parts to this element (nothing else initializes it).
            _composite.Init(Element);
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
                    _handler = element.Handler;

                // Absolute bounds, every frame. It cached element.Layout - the parent-relative
                // position - once and drew there, which only matched the screen because the
                // info board's constant already contained the sidebar's own position; laid
                // out relative to the sidebar it would have been drawn at the window corner.
                Layout = element.GetCurrentAbsoluteBounds();
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

                // Outer shield
                using IGraphicsPath fullShield = ShieldPath.Create(width, height);
                using (var translate = GraphicsBackend.Factory.CreateMatrix())
                {
                    translate.Translate(baseX, baseY);
                    fullShield.Transform(translate);
                }

                PlayerSide currentTurn = element.GameManager.CurrentTurn;
                // Each half is coloured by the colour its player plays (fixed on the Full
                // board; on the dark-chess board decided by the first flip, neutral before).
                PieceColor leftColor = element.GameManager.ColorOf(PlayerSide.Player2);
                PieceColor rightColor = element.GameManager.ColorOf(PlayerSide.Player1);

                // Left-half background
                using IRegion leftRegion = GraphicsBackend.Factory.CreateRegion(fullShield);
                leftRegion.Intersect(new RectangleF(baseX, baseY, width / 2f, height));
                using IBrush leftBrush = GraphicsBackend.Factory.CreateSolidBrush(currentTurn == PlayerSide.Player2 ? Color.Gold : IdleColor(leftColor));
                g.FillRegion(leftBrush, leftRegion);

                // Right-half background
                using IRegion rightRegion = GraphicsBackend.Factory.CreateRegion(fullShield);
                rightRegion.Intersect(new RectangleF(baseX + width / 2f, baseY, width / 2f, height));
                using IBrush rightBrush = GraphicsBackend.Factory.CreateSolidBrush(currentTurn == PlayerSide.Player1 ? Color.Gold : IdleColor(rightColor));
                g.FillRegion(rightBrush, rightRegion);

                // Inner shield
                using IGraphicsPath innerShield = ShieldPath.Create(width - 2 * inset, height - 2 * inset);
                // Move to the inner position
                using (var innerTranslate = GraphicsBackend.Factory.CreateMatrix())
                {
                    innerTranslate.Translate(baseX + inset, baseY + inset);
                    innerShield.Transform(innerTranslate);
                }

                // Left-half inner overlay
                using IRegion leftOverlay = GraphicsBackend.Factory.CreateRegion(innerShield);
                float leftWidth = (element.GameManager.CurrentTurn == PlayerSide.Player2 ? (width / 2f - inset) : width / 2f);
                leftOverlay.Intersect(new RectangleF(baseX + inset, baseY + inset, leftWidth, height - 2*inset));
                using IBrush leftOverlayBrush = GraphicsBackend.Factory.CreateSolidBrush(OverlayColor(leftColor));
                g.FillRegion(leftOverlayBrush, leftOverlay);

                // Right-half inner overlay
                using IRegion rightOverlay = GraphicsBackend.Factory.CreateRegion(innerShield);
                float rightX = (element.GameManager.CurrentTurn == PlayerSide.Player1 ? baseX + width / 2f + inset : baseX + width / 2f);
                float rightWidth = (element.GameManager.CurrentTurn == PlayerSide.Player1 ? width / 2f - inset : width / 2f);
                rightOverlay.Intersect(new RectangleF(rightX, baseY + inset, rightWidth, height - 2*inset));
                using IBrush rightOverlayBrush = GraphicsBackend.Factory.CreateSolidBrush(OverlayColor(rightColor));
                g.FillRegion(rightOverlayBrush, rightOverlay);

                float centerX = baseX + width / 2f + inset / 2f;
                float startY = baseY;
                float endY = baseY + height - inset * 2;

                using (IPen centerLinePen = GraphicsBackend.Factory.CreatePen(Color.Gold, 4))
                {
                    centerLinePen.Alignment = PenLineAlignment.Center;
                    g.DrawLine(centerLinePen, centerX, startY, centerX, endY);
                }
            }

            /// <summary>
            /// The inner fill of a player's half: dark red / black by the colour the player
            /// plays (the colours the board always used: Player1 red, Player2 black); a
            /// neutral grey while a dark-chess game has not decided it yet.
            /// </summary>
            private static Color OverlayColor(PieceColor color) => color switch
            {
                PieceColor.Red => Color.DarkRed,
                PieceColor.Black => Color.Black,
                _ => Color.DimGray,
            };

            /// <summary>The outer rim of a player's half while it is not that player's turn (the active one is gold).</summary>
            private static Color IdleColor(PieceColor color) => color == PieceColor.Red ? Color.LightCoral : Color.Gray;

            /// <summary>
            /// The player's name, followed by 將軍 while that side is to move and in check
            /// (<c>GameManager.IsInCheck</c>; false once the game is over and on boards without check).
            /// </summary>
            private static string NameWithCheck(UIInfoBoard element, PlayerSide side)
            {
                string name = element.GetPlayerName(side);
                var game = element.GameManager;
                return game.IsInCheck && game.CurrentTurn == side ? name + GameMenuTexts.InCheckSuffix : name;
            }

            private void DrawPlayers(IGraphics g, UIInfoBoard element)
            {
                float baseX = Layout.X;
                float baseY = Layout.Y;
                float width = Layout.Width;
                float height = Layout.Height;

                DrawPlayerSection(g, baseX, baseY, width / 2.0f, height,
                    NameWithCheck(element, PlayerSide.Player2),
                    element.GameManager.Player2.Timer.GetTotalTimeString(),
                    element.GameManager.Player2.Timer.GetStepTimeString(),
                    element.GameManager.CurrentTurn == PlayerSide.Player2);

                DrawPlayerSection(g, baseX + width / 2.0f, baseY, width / 2.0f, height,
                    NameWithCheck(element, PlayerSide.Player1),
                    element.GameManager.Player1.Timer.GetTotalTimeString(),
                    element.GameManager.Player1.Timer.GetStepTimeString(),
                    element.GameManager.CurrentTurn == PlayerSide.Player1);
            }

            private void DrawPlayerSection(IGraphics g, float x, float y, float width, float height,
                string playerName, string totalTimeString, string stepTimeString, bool isActive)
            {
                // Total timer background
                RectangleF totalTimerRect = new RectangleF(x + 20.0f, y + 50.0f, width - 40.0f, 40.0f);
                using (IBrush timerBgBrush = GraphicsBackend.Factory.CreateSolidBrush(Color.DimGray))
                    g.FillRectangle(timerBgBrush, totalTimerRect);

                // Total timer text
                using (IBrush timerTextBrush = GraphicsBackend.Factory.CreateSolidBrush(isActive ? Color.Gold : Color.DeepSkyBlue))
                using (IStringFormat timerFormat = GraphicsBackend.Factory.CreateStringFormat())
                {
                    timerFormat.Alignment = TextAlign.Center;
                    timerFormat.LineAlignment = TextAlign.Center;
                    g.DrawString(totalTimeString, _timerFont, timerTextBrush, totalTimerRect, timerFormat);
                }

                // Step timer background
                RectangleF stepTimerRect = new RectangleF(x + 20.0f, y + 95.0f, width - 40.0f, 40.0f);
                using (IBrush timerBgBrush = GraphicsBackend.Factory.CreateSolidBrush(Color.DimGray))
                    g.FillRectangle(timerBgBrush, stepTimerRect);

                // Step timer text
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

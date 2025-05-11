/* ----- ----- ----- ----- */
// MainForm.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/06
// Update Date: 2025/05/06
// Version: v1.0
/* ----- ----- ----- ----- */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

using Chinese_Chess_v3.Core;
using Chinese_Chess_v3.Interface.Panels;
using Chinese_Chess_v3.Interface.Sidebar;
using Chinese_Chess_v3.Utils;
using Chinese_Chess_v3.Interface.Renderers;
using Chinese_Chess_v3.Configs.Board;
using Chinese_Chess_v3.Configs.Sidebar;

using StarAnimation.Controllers;

using SharedLib.RandomTable;
using SharedLib.Timing;

namespace Chinese_Chess_v3.Interface
{
    public class MainForm : Form
    {
        private readonly TimerManager timerManager = new TimerManager();
        public static RandomTable GlobalRandomTable;
        private MainRenderController starAnimation;
        private MainMenuPanel mainMenuPanel;
        private GameManager gameManager;
        private BoardRenderer boardRenderer;
        private PieceRenderer pieceRenderer;
        private SidebarRenderer sidebarRenderer;
        private LoggerBox loggerBox;
        // Class-level field to track the time each frame is drawn
        private Dictionary<RectangleF, DateTime> frameDrawTimes = new Dictionary<RectangleF, DateTime>();

        public MainForm()
        {
            this.Text = "Chinese Chess v3 - created by @DragonTaki";
            this.ClientSize = new Size(BoardConstants.BoardTotalWidth + SidebarSettings.SidebarWidth,
                                       BoardConstants.BoardTotalHeight);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.DoubleBuffered = true;
            FontManager.LoadFonts();

            GlobalRandomTable = new RandomTable(size: 10000, seed: 12345);

            GlobalTime.Timer = timerManager;
            timerManager.StartTimers();
            starAnimation = new MainRenderController(this.Width, this.Height, timerManager, GlobalRandomTable);
            timerManager.OnAnimationFrame += () =>
            {
                starAnimation.Update();
                this.Invalidate();
            };

            //mainMenuPanel = new MainMenuPanel();
            //this.Controls.Add(mainMenuPanel);
            //gameManager = GameManager.Instance;
            //boardRenderer = new BoardRenderer(); // Initialize the BoardRenderer
            //pieceRenderer = new PieceRenderer(); // Initialize the PieceRenderer
            //sidebarRenderer = new SidebarRenderer(); // Initialize the PieceRenderer

            this.Paint += Unified_Paint;
            //this.MouseClick += OnMouseClick;

            //loggerBox = new LoggerBox();
            //this.Controls.Add(loggerBox);
            //AppLogger.SetExternalLogger(loggerBox.AppendLog);
            //AppLogger.Log("System initialized", LogLevel.INIT);
            //AppLogger.Log("This is debug info", LogLevel.DEBUG);
            //AppLogger.LogWelcomeMessage();

            //InfoBoard infoBoard = new InfoBoard();
            //this.Controls.Add(infoBoard);
            this.Resize += (s, e) => starAnimation.Resize(this.Width, this.Height);
        }

        // Unified painting method
        private void Unified_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            starAnimation.Draw(g);

            //boardRenderer.DrawBoard(g);
            //var selectedPiece = gameManager.SelectedPiece;
            //pieceRenderer.DrawPieces(g, gameManager.GetCurrentPieces(), selectedPiece);
            //sidebarRenderer.DrawSidebar(g);
        }

        // Clicking method
        private void OnMouseClick(object sender, MouseEventArgs e)
        {
            // If click outside the board, no action
            if (e.X > BoardConstants.BoardTotalWidth || e.Y > BoardConstants.BoardTotalHeight)
                return;

            // Calculating which grid it is: the chess piece is placed on the intersection of the lines
            int x = (int)Math.Round((e.X - BoardSettings.BoardStartX) / (float)BoardSettings.GridSize);
            int y = (int)Math.Round((e.Y - BoardSettings.BoardStartY) / (float)BoardSettings.GridSize);

            if (x < 0 || x >= 9 || y < 0 || y >= 10)
                return;

            // Pass to GameManager for processing logic
            gameManager.HandleClick(x, y);

            this.Invalidate(); // Repaint on every click
        }
        public void SwitchToGameUI()
        {
            this.Controls.Clear();

            // 再重新載入 gameManager、boardRenderer、sidebarPanel 等
        }
    }
}
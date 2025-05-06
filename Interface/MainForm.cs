/* ----- ----- ----- ----- */
// MainForm.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/06
// Update Date: 2025/05/06
// Version: v1.0
/* ----- ----- ----- ----- */

using System;
using System.Drawing;
using System.Windows.Forms;

using Chinese_Chess_v3.Core;
using Chinese_Chess_v3.Core.Logging;
using Chinese_Chess_v3.Configs;
using Chinese_Chess_v3.Utils;

namespace Chinese_Chess_v3.Interface
{
    public class MainForm : Form
    {
        private GameManager game;
        private BoardRenderer boardRenderer;
        private PieceRenderer pieceRenderer;
        private SidebarRenderer sidebarRenderer;
        private LoggerBox loggerBox;

        public MainForm()
        {
            this.Text = "Chinese Chess v3 - created by @DragonTaki";
            this.ClientSize = new Size(BoardConstants.TotalWidth + SidebarSettings.Width,
                                       BoardConstants.TotalHeight);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.DoubleBuffered = true;

            game = new GameManager();
            FontManager.LoadFonts();
            boardRenderer = new BoardRenderer(); // Initialize the BoardRenderer
            pieceRenderer = new PieceRenderer(); // Initialize the PieceRenderer
            sidebarRenderer = new SidebarRenderer(); // Initialize the PieceRenderer

            this.Paint += Unified_Paint;

            loggerBox = new LoggerBox();
            this.Controls.Add(loggerBox);
            AppLogger.SetExternalLogger(loggerBox.AppendLog);
            AppLogger.Log("System initialized", LogLevel.INIT);
            AppLogger.Log("This is debug info", LogLevel.DEBUG);
            AppLogger.LogWelcomeMessage();
        }

        // Unified painting method
        private void Unified_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            boardRenderer.DrawBoard(g);
            pieceRenderer.DrawInitialPieces(g);
            sidebarRenderer.DrawSidebar(g);
        }
    }
}
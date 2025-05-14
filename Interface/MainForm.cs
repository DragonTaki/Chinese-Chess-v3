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
using System.Drawing;
using System.Windows.Forms;

using Chinese_Chess_v3.Configs;
using Chinese_Chess_v3.Core;
using Chinese_Chess_v3.Interface.Sidebar;
using Chinese_Chess_v3.Utils;
using Chinese_Chess_v3.Renderers;

using SharedLib.RandomTable;
using SharedLib.Timing;
using SharedLib.Globals;

using StarAnimation;
using Chinese_Chess_v3.Controllers;

namespace Chinese_Chess_v3.Interface
{
    public class MainForm : Form
    {
        private TimerManager timerManager = new TimerManager();
        public static RandomTable GlobalRandomTable;
        private StarAnimationApp starAnimationApp;
        private MainMenuController mainMenuController;
        private GameManager gameManager;
        private BoardRenderer boardRenderer;
        private PieceRenderer pieceRenderer;
        private SidebarRenderer sidebarRenderer;
        private LoggerBox loggerBox;
        // Class-level field to track the time each frame is drawn
        private Dictionary<RectangleF, DateTime> frameDrawTimes = new Dictionary<RectangleF, DateTime>();

        public MainForm()
        {
            // Initialization logic
            InitComponents();             // 控制項設置（滑鼠事件、大小設定等）
            InitTimer();                  // 建立更新用 Timer（或使用 Application.Idle 驅動）
            InitController();
            InitPanels();                 // 初始化所有面板（BoardPanel, MainMenuPanel 等）
            InitRenderers();              // 建立對應 renderer（MainMenuRenderer, PieceRenderer 等）

            //mainMenuPanel = new MainMenuPanel();
            //this.Controls.Add(mainMenuPanel);
            //gameManager = GameManager.Instance;
            //boardRenderer = new BoardRenderer(); // Initialize the BoardRenderer
            //pieceRenderer = new PieceRenderer(); // Initialize the PieceRenderer
            //sidebarRenderer = new SidebarRenderer(); // Initialize the PieceRenderer

            //this.MouseClick += OnMouseClick;

            //loggerBox = new LoggerBox();
            //this.Controls.Add(loggerBox);
            //AppLogger.SetExternalLogger(loggerBox.AppendLog);
            //AppLogger.Log("System initialized", LogLevel.INIT);
            //AppLogger.Log("This is debug info", LogLevel.DEBUG);
            //AppLogger.LogWelcomeMessage();

            //InfoBoard infoBoard = new InfoBoard();
            //this.Controls.Add(infoBoard);
        }
        private void InitComponents()
        {
            FontManager.LoadFonts();
            GlobalRandomTable = new RandomTable(size: 10000, seed: 12345);

            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.AllPaintingInWmPaint |
                        ControlStyles.UserPaint |
                        ControlStyles.OptimizedDoubleBuffer, true);

            this.Text = "Chinese Chess v3 - created by @DragonTaki";
            this.ClientSize = new Size((int)(Settings.MainMenu.Size.X + Settings.Board.Size.X + Settings.Sidebar.Size.X),
                                       (int)Settings.Board.Size.Y);
            this.StartPosition = FormStartPosition.CenterScreen;
            GlobalWindow.UpdateSize(Width, Height);

            this.MouseDown += OnMouseDown;
            this.MouseMove += OnMouseMove;
            this.MouseUp   += OnMouseUp;
            this.MouseWheel += OnMouseWheel;
            this.MouseClick += OnMouseClick;

            this.Resize += OnResize;
            this.KeyDown += OnKeyDown;
        }

        private void InitTimer()
        {
            timerManager = new TimerManager();
            GlobalTime.Timer = timerManager;
            timerManager.StartTimers();
            timerManager.OnAnimationFrame += () => {
                starAnimationApp.Update();
                this.Invalidate();
            };
            timerManager.StartTimers();
        }

        private void InitController()
        {
            mainMenuController = new MainMenuController(Width, Height);
            mainMenuController.Initialize();
        }

        private void InitPanels()
        {
            //mainMenuPanel = new MainMenuPanel();
            //boardPanel = new BoardPanel();
            //sidebarPanel = new SidebarPanel();
            //infoBoard = new InfoBoard();

            // 可以將 panel 放進 List<IUIPanel> panels 做統一更新處理
            //panels = new List<IUIPanel> { mainMenuPanel, boardPanel, sidebarPanel, infoBoard };
        }

        private void InitRenderers()
        {
            if (GlobalTime.Timer == null)
                throw new InvalidOperationException("GlobalTime.Timer must be initialized before creating renderers.");

            starAnimationApp = new StarAnimationApp();
            //boardRenderer = new BoardRenderer();
            //pieceRenderer = new PieceRenderer();
            //sidebarRenderer = new SidebarRenderer();

            // 若你的面板有 renderer 欄位，可以在這裡賦值
            //mainMenuPanel.SetRenderer(mainMenuRenderer);
            //boardPanel.SetRenderer(boardRenderer);
            //sidebarPanel.SetRenderer(sidebarRenderer);
        }

        // Rendering Process
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;
            starAnimationApp.Render(g);
            mainMenuController.Render(g);
/*
            foreach (var panel in panels)
            {
                panel.Render(g); // 呼叫 MainMenuPanel.Render 等，再呼叫 renderer.Draw()
            }
*/
            //mainMenuPanel.Render(e.Graphics);
            //boardPanel.Render(e.Graphics);
            //sidebarPanel.Render(e.Graphics);
            //gameMenuPanel.Render(e.Graphics);

            //boardRenderer.DrawBoard(g);
            //var selectedPiece = gameManager.SelectedPiece;
            //pieceRenderer.DrawPieces(g, gameManager.GetCurrentPieces(), selectedPiece);
            //sidebarRenderer.DrawSidebar(g);
        }

        // Update status (every frame/tick)
        private void UpdateGame()
        {
/*
            foreach (var panel in panels)
            {
                panel.Update(); // 面板內部更新滑動邏輯、動畫等
            }
*/
            //scrollContainer.Update();    // 慣性滑動與回彈
            //boardPanel.Update();
            //sidebarPanel.Update();
            Invalidate();                // 觸發重繪
        }

        // Event handling
        private void OnMouseDown(object sender, MouseEventArgs e)
        {
            //mainMenuController.OnMouseDown(e);
        }
        private void OnMouseMove(object sender, MouseEventArgs e)
        {
        }
        private void OnMouseUp(object sender, MouseEventArgs e)
        {
        }
        private void OnMouseWheel(object sender, MouseEventArgs e)
        {
        }

        // Clicking method
        private void OnMouseClick(object sender, MouseEventArgs e)
        {
            // If click outside the board, no action
            if (e.X > Settings.Board.Size.X || e.Y > Settings.Board.Size.Y)
                return;

            // Calculating which grid it is: the chess piece is placed on the intersection of the lines
            int x = (int)Math.Round((e.X - Settings.Board.Position.X) / (float)Settings.Board.Grid.Size);
            int y = (int)Math.Round((e.Y - Settings.Board.Position.Y) / (float)Settings.Board.Grid.Size);

            if (x < 0 || x >= 9 || y < 0 || y >= 10)
                return;

            // Pass to GameManager for processing logic
            gameManager.HandleClick(x, y);

            this.Invalidate(); // Repaint on every click
        }
        private void OnResize(object sender, EventArgs e)
        {
            GlobalWindow.UpdateSize(this.ClientSize.Width, this.ClientSize.Height);
            // 其他 Resize 處理邏輯放這裡
        }
        public interface IResizable
        {
            void OnResize(int width, int height);
        }
        private void OnKeyDown(object sender, EventArgs e)
        {
        }

        public void SwitchToGameUI()
        {
            this.Controls.Clear();

            // 再重新載入 gameManager、boardRenderer、sidebarPanel 等
        }
    }
}
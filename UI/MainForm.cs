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

using Chinese_Chess_v3.UI.Constants;
using Chinese_Chess_v3.UI.Core;
using Chinese_Chess_v3.UI.Input;
using Chinese_Chess_v3.UI.Screens.Menu;
using Chinese_Chess_v3.Utils;

using SharedLib.RandomTable;
using SharedLib.PhysicsUtils;
using SharedLib.Timing;
using SharedLib.Globals;

using StarAnimation;

namespace Chinese_Chess_v3.UI
{
    public class MainForm : Form
    {
        private TimerManager timerManager = new TimerManager();
        public static RandomTable GlobalRandomTable;
        private StarAnimationApp starAnimationApp;
        private MouseInputRouter inputRouter;
        private ScrollInputHandler scrollHandler = new ScrollInputHandler();
        private UIManager uiManager;
        private UIElement rootUI;
        // Class-level field to track the time each frame is drawn
        private Dictionary<RectangleF, DateTime> frameDrawTimes = new Dictionary<RectangleF, DateTime>();

        public MainForm()
        {
            // Initialization logic
            InitComponents();
            InitTimer();
            InitAppModules();
            InitRenderers();
            InitInputSystem();
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
            this.ClientSize = new Size((int)(UILayoutConstants.MainMenu.Size.X + UILayoutConstants.Board.Size.X + UILayoutConstants.Sidebar.Size.X),
                                       (int)UILayoutConstants.MainMenu.Size.Y);
            this.StartPosition = FormStartPosition.CenterScreen;
            GlobalWindow.UpdateSize(Width, Height);

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
                UpdateGame();
                this.Invalidate();
            };
            timerManager.StartTimers();
        }
        private void InitAppModules()
        {
            // 初始化動畫背景模組
            starAnimationApp = new StarAnimationApp();

            // 初始化 UI 根節點（例如主選單）
            rootUI = new MainMenu();
            uiManager = new UIManager(rootUI);
        }

        private void InitRenderers()
        {
            if (GlobalTime.Timer == null)
                throw new InvalidOperationException("`GlobalTime.Timer` must be initialized before creating renderers.");
            
            // Please using UI to call its renderer, only init extra renderer here
            // Main -> UIManager -> UIs -> Renderer
        }

        private void InitInputSystem()
        {
            inputRouter = new MouseInputRouter(rootUI, scrollHandler);
            /*
            var inputRouter = new MouseInputRouter(rootUI, new IInputHandler[]
            {
                new ScrollInputHandler(),
                new ClickInputHandler()
                // 可在這裡擴充更多 handler
            });*/

            this.MouseDown += inputRouter.OnMouseDown;
            this.MouseMove += inputRouter.OnMouseMove;
            this.MouseUp   += inputRouter.OnMouseUp;
            this.MouseWheel += inputRouter.OnMouseWheel;
            this.MouseClick += inputRouter.OnMouseClick;
        }

        // Rendering Process
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;
            starAnimationApp.Render(g);
            uiManager?.Draw(g);
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
            PhysicsRegistry.UpdateAll();
            uiManager?.Update();
            inputRouter.EndFrame();
            Invalidate();                // 觸發重繪
        }

        /// <summary>
        /// Mouse down event handler
        /// </summary>
        private void OnMouseDown(object sender, MouseEventArgs e)
        {
        }

        /// <summary>
        /// Mouse move event handler
        /// </summary>
        private void OnMouseMove(object sender, MouseEventArgs e)
        {
        }

        /// <summary>
        /// Mouse up event handler
        /// </summary>
        private void OnMouseUp(object sender, MouseEventArgs e)
        {
        }

        /// <summary>
        /// Mouse wheel event handler
        /// </summary>
        private void OnMouseWheel(object sender, MouseEventArgs e)
        {
        }

        /// <summary>
        /// Mouse click event handler
        /// </summary>
        private void OnMouseClick(object sender, MouseEventArgs e)
        {
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
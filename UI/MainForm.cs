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

using Chinese_Chess_v3.UI.Constants;
using Chinese_Chess_v3.UI.Core.Infrastructure;
using Chinese_Chess_v3.UI.Core.Elements;
using Chinese_Chess_v3.UI.Input;
using Chinese_Chess_v3.UI.Core.Interfaces;
using Chinese_Chess_v3.Utils;

using Microsoft.Extensions.DependencyInjection;

using SharedLib.PhysicsUtils;
using SharedLib.Timing;
using SharedLib.Globals;

using StarAnimation;

namespace Chinese_Chess_v3.UI
{
    public class MainForm : Form
    {
        private readonly TimerManager _timerMgr = new TimerManager();
        private readonly IServiceProvider _sp;
        private readonly UIInputManager _inputMgr;
        private readonly UIRootNode _rootCanvas;
        private NavigationManager _navigationManager;

        private StarAnimationApp _bgStar;


        public MainForm(IServiceProvider sp)
        {
            _sp = sp;

            // Initialization logic
            InitComponents();  // Create WinForms Designer
            InitWindow();

            var uiFactory = _sp.GetRequiredService<IUiFactory>();

            _rootCanvas = BuildUIRoot();

            _navigationManager = _sp.GetRequiredService<NavigationManager>();
            _navigationManager.Init(_rootCanvas);            
            _navigationManager.ShowMainMenu();

            var scrollHandler = _sp.GetRequiredService<IScrollInputHandler>();
            _inputMgr = new UIInputManager(_rootCanvas, scrollHandler);

            WireWinFormsMouseEvents();
            InitTimer();
        }

        private void InitComponents()
        {
            FontManager.LoadFonts();
        }

        private void InitWindow()
        {
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.AllPaintingInWmPaint |
                        ControlStyles.UserPaint |
                        ControlStyles.OptimizedDoubleBuffer, true);

            this.Text = "Chinese Chess v3 - created by @DragonTaki";
            this.ClientSize = new Size((int)(UILayoutConstants.MainMenu.Size.X + UILayoutConstants.Board.Size.X + UILayoutConstants.Sidebar.Size.X),
                                       (int)UILayoutConstants.MainMenu.Size.Y);
            this.StartPosition = FormStartPosition.CenterScreen;
            GlobalWindow.UpdateSize(Width, Height);
            _bgStar = new StarAnimationApp();
        }

        private UIRootNode BuildUIRoot()
        {
            var root = _sp.GetRequiredService<UIRootNode>();
            var dialogManager = _sp.GetRequiredService<DialogManager>();
            dialogManager.Init(root);

            return root;
        }

        private void WireWinFormsMouseEvents()
        {
            MouseDown += _inputMgr.ProcessMouseDown;
            MouseMove += _inputMgr.ProcessMouseMove;
            MouseUp += _inputMgr.ProcessMouseUp;
            MouseWheel += _inputMgr.ProcessMouseWheel;
            MouseClick += _inputMgr.ProcessMouseClick;
        }

        private void InitTimer()
        {
            GlobalTime.Timer = _timerMgr;
            _timerMgr.OnAnimationFrame += () =>
            {
                _bgStar.Update();
                _rootCanvas.Update();
                PhysicsRegistry.UpdateAll();
                _inputMgr.EndFrame();
                this.Invalidate();
            };
            _timerMgr.StartTimers();
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            _bgStar?.Render(e.Graphics);
            _rootCanvas?.Draw(e.Graphics);
        }
    }
}
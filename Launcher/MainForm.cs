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

using Microsoft.Extensions.DependencyInjection;

using Chinese_Chess_v3.Game.Constants.UI;
using Chinese_Chess_v3.Game.UI.Screens.Menus;

using Engine.Globals;
using Engine.Physics;
using Engine.Styles;
using Engine.Timing;
using Engine.UI.Core.Elements;
using Engine.UI.Core.Infrastructure;
using Engine.UI.Input;

using StarAnimation;

namespace Launcher
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
            _sp = sp ?? throw new ArgumentNullException(nameof(sp));

            // Initialization logic
            InitComponents();  // Create WinForms Designer
            InitWindow();

            _rootCanvas = UIInitializer.Initialize(_sp);
            _rootCanvas.MainForm = this;

            _navigationManager = _sp.GetRequiredService<NavigationManager>();
            _navigationManager.Init(_rootCanvas);
            _navigationManager.Show<MainMenu, MainMenuHandler, MainMenuRenderer>();

            var scrollHandler = _sp.GetRequiredService<IScrollInputHandler>();
            _inputMgr = new UIInputManager(_rootCanvas, scrollHandler);

            WireInputEvents();
            InitTimer();

            _bgStar = new StarAnimationApp();
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
            this.ClientSize = new Size(
                (int)(UILayoutConstants.MainMenu.Size.X + UILayoutConstants.Board.Size.X + UILayoutConstants.Sidebar.Size.X),
                (int)UILayoutConstants.MainMenu.Size.Y);
            this.StartPosition = FormStartPosition.CenterScreen;

            GlobalWindow.UpdateSize(Width, Height);
        }

        private void WireInputEvents()
        {
            MouseDown  += _inputMgr.ProcessMouseDown;
            MouseMove  += _inputMgr.ProcessMouseMove;
            MouseUp    += _inputMgr.ProcessMouseUp;
            MouseWheel += _inputMgr.ProcessMouseWheel;
            MouseClick += _inputMgr.ProcessMouseClick;
        }

        private void InitTimer()
        {
            GlobalTime.Timer = _timerMgr;
            _timerMgr.OnAnimationFrame += () =>
            {
                _bgStar?.Update();
                _rootCanvas?.Update();
                PhysicsRegistry.UpdateAll();
                _inputMgr?.EndFrame();
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

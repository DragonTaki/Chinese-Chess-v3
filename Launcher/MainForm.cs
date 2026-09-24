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

using Chinese_Chess_v3.Game.UI.Constants;
using Chinese_Chess_v3.Game.UI.Menus.MainMenu;

using Engine.Globals;
using Engine.Physics;
using Engine.Platform;
using Engine.Platform.WinForms;
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
            _rootCanvas.MainWindow = new WinFormsWindow(this);

            _navigationManager = _sp.GetRequiredService<NavigationManager>();
            _navigationManager.Init(_rootCanvas);
            _navigationManager.Show<UIMainMenu, UIMainMenuHandler, UIMainMenuRenderer>();

            var scrollHandler = _sp.GetRequiredService<IScrollInputHandler>();
            _inputMgr = new UIInputManager(_rootCanvas, scrollHandler);

            WireInputEvents();
            InitTimer();

            _bgStar = new StarAnimationApp();
        }

        private void InitComponents()
        {
            // FontManager.LoadFonts() now runs in Program.Main(), before
            // DefaultStyles.DefaultButtonStyle touches UILayoutStyles's
            // static constructor — see the comment there. Calling it again
            // here would just reload the same font files from disk.
        }

        private void InitWindow()
        {
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.AllPaintingInWmPaint |
                ControlStyles.UserPaint |
                ControlStyles.OptimizedDoubleBuffer, true);

            this.Text = "Chinese Chess v3 - created by @DragonTaki";

            // The window opens at a normal desktop size (1080p) rather than
            // the UI's own (smaller) DesignSize — content is scaled up to
            // fill it via GlobalViewport, same as any later resize. See
            // UILayoutConstants.DefaultWindowSize/MinimumWindowSize.
            this.ClientSize = new Size(
                (int)UILayoutConstants.DefaultWindowSize.X,
                (int)UILayoutConstants.DefaultWindowSize.Y);
            this.MinimumSize = new Size(
                (int)UILayoutConstants.MinimumWindowSize.X,
                (int)UILayoutConstants.MinimumWindowSize.Y);
            this.StartPosition = FormStartPosition.CenterScreen;

            GlobalWindow.UpdateSize(Width, Height);

            // UI content (MainMenu/Board/Sidebar/dialogs) is authored in its
            // own fixed DesignSize coordinate space; GlobalViewport maps
            // that onto whatever the actual window size is, uniformly (no
            // stretch) and letterboxed.
            GlobalViewport.DesignSize = UILayoutConstants.DesignSize;
            GlobalViewport.Recalculate(Width, Height);
            this.Resize += (_, _) =>
            {
                GlobalWindow.UpdateSize(Width, Height);
                GlobalViewport.Recalculate(Width, Height);
                _bgStar?.Resize(Width, Height);
            };
        }

        private void WireInputEvents()
        {
            var adapter = new WinFormsInputAdapter(_inputMgr);
            MouseDown  += adapter.ProcessMouseDown;
            MouseMove  += adapter.ProcessMouseMove;
            MouseUp    += adapter.ProcessMouseUp;
            MouseWheel += adapter.ProcessMouseWheel;
            MouseClick += adapter.ProcessMouseClick;
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

            using IGraphics g = new WinFormsGraphics(e.Graphics, ownsNative: false);

            // Background renders full-bleed in actual window pixels; UI
            // content renders inside the letterboxed/scaled viewport — see
            // GlobalViewport's doc comment for why these differ.
            _bgStar?.Render(g);

            g.PushTransform(GlobalViewport.Scale, GlobalViewport.Offset.X, GlobalViewport.Offset.Y);
            _rootCanvas?.Draw(g);
            g.PopTransform();
        }
    }
}

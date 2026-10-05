/* ----- ----- ----- ----- */
// MainForm.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/06
// Update Date: 2026/10/05
// Version: v1.1
/* ----- ----- ----- ----- */

using System;
using System.Drawing;
using System.Windows.Forms;

using Microsoft.Extensions.DependencyInjection;

using Chinese_Chess_v3.Composition;
using Chinese_Chess_v3.Game.Application.Session;
using Chinese_Chess_v3.Game.Configs;
using Chinese_Chess_v3.Game.UI.Constants;

using Engine.Diagnostics;
using Engine.Globals;
using Engine.Physics;
using Engine.Platform;
using Engine.Platform.WinForms;
using Engine.Styles;
using Engine.Timing;
using Engine.UI.Core.Elements;
using Engine.UI.Infrastructure;
using Engine.UI.Input;

using StarAnimation;

namespace Launcher
{
    public class MainForm : Form
    {
        private readonly WinFormsTimerProvider _timerMgr = new WinFormsTimerProvider();
        private readonly IServiceProvider _sp;
        private readonly UIInputManager _inputMgr;
        private readonly UIRootNode _rootCanvas;
        private NavigationManager _navigationManager;
        private readonly GameSession _gameSession;

        private StarAnimationApp _bgStar;

        public MainForm(IServiceProvider sp)
        {
            _sp = sp ?? throw new ArgumentNullException(nameof(sp));

            // Initialization logic
            InitComponents();  // Create WinForms Designer
            InitWindow();

            // Also initializes navigation and shows the main menu (once).
            _rootCanvas = UIInitializer.Initialize(_sp);
            _rootCanvas.MainWindow = new WinFormsWindow(this);

            _navigationManager = _sp.GetRequiredService<NavigationManager>();
            _gameSession = _sp.GetRequiredService<GameSession>();

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

            this.Text = SystemSettings.WindowTitle;

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

            // ClientSize, not Width/Height: those include the title bar and borders,
            // while painting and mouse coordinates are client-area based.
            GlobalWindow.UpdateSize(ClientSize.Width, ClientSize.Height);

            // UI content (MainMenu/Board/Sidebar/dialogs) is authored in its
            // own DesignSize coordinate space; GlobalViewport scales that
            // uniformly (no stretch) to the actual window size, and the UI
            // area covers the whole window (no letterbox - the layout
            // distributes the extra width/height).
            GlobalViewport.DesignSize = UILayoutConstants.DesignSize;
            GlobalViewport.Recalculate(ClientSize.Width, ClientSize.Height);
            this.Resize += (_, _) =>
            {
                GlobalWindow.UpdateSize(ClientSize.Width, ClientSize.Height);
                GlobalViewport.Recalculate(ClientSize.Width, ClientSize.Height);
                _bgStar?.Resize(ClientSize.Width, ClientSize.Height);
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
            // Keyboard (text fields): Windows-only like the rest of this form; CA1416 suppressed
            // so the new lines don't add to the build's platform warnings.
#pragma warning disable CA1416
            KeyPress   += adapter.ProcessKeyPress;
            KeyDown    += adapter.ProcessKeyDown;
#pragma warning restore CA1416

            // Losing focus mid-drag (e.g. Alt-Tab) means the MouseUp never arrives.
            Deactivate += (_, _) => _inputMgr?.CancelInput();
        }

        private void InitTimer()
        {
            GlobalTime.Timer = _timerMgr;
            _timerMgr.OnAnimationFrame += () =>
            {
                _bgStar?.Update();
                _rootCanvas?.Update();
                // Right after the UI update, where the board's update used to advance the clocks.
                _gameSession?.Tick();
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
            // content renders in design units scaled by GlobalViewport.Scale
            // (Offset is zero: the UI area covers the whole window too) — see
            // GlobalViewport's doc comment.
            _bgStar?.Render(g);

            g.PushTransform(GlobalViewport.Scale, GlobalViewport.Offset.X, GlobalViewport.Offset.Y);
            _rootCanvas?.Draw(g);
            g.PopTransform();

            // Debug readouts (FPS / network latency) over everything, when switched on.
            DebugOverlay.Draw(g);
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            // Stop frame ticks before the form's resources go away, and release the timer.
            _timerMgr.Dispose();
            base.OnFormClosed(e);
        }
    }
}

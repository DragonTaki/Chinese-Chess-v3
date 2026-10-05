/* ----- ----- ----- ----- */
// CrossPlatformApp.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/09/24
// Update Date: 2026/10/05
// Version: v1.2
/* ----- ----- ----- ----- */

using System;

using Microsoft.Extensions.DependencyInjection;

using SkiaSharp;
using Silk.NET.Input;
using Silk.NET.OpenGL;
using SilkWindowInterface = Silk.NET.Windowing.IWindow;

using Chinese_Chess_v3.Composition;
using Chinese_Chess_v3.Game.Application.Session;
using Chinese_Chess_v3.Game.UI.Constants;

using Engine.Diagnostics;
using Engine.UI.Diagnostics;
using Engine.Globals;
using Engine.Physics;
using Engine.Platform;
using Engine.Platform.Skia;
using Engine.Styles;
using Engine.Timing;
using Engine.UI.Core.Elements;
using Engine.UI.Infrastructure;
using Engine.UI.Input;

using StarAnimation;

namespace Launcher.Cross
{
    /// <summary>
    /// Cross-platform counterpart to <c>Launcher.MainForm</c> — same
    /// responsibilities (window setup, input wiring, the per-frame
    /// update/draw loop), driven by a Silk.NET window instead of a WinForms
    /// <c>Form</c> and drawing through a GPU-backed SkiaSharp surface
    /// instead of GDI+.
    /// </summary>
    public sealed class CrossPlatformApp : IDisposable
    {
        private readonly IServiceProvider _sp;
        private readonly SilkWindowInterface _window;
        private readonly ManualTimerProvider _timer = new ManualTimerProvider();

        private UIInputManager _inputMgr;
        private UIRootNode _rootCanvas;
        private NavigationManager _navigationManager;
        private GameSession _gameSession;
        private StarAnimationApp _bgStar;

        private IInputContext _inputContext;
        private SilkInputAdapter _inputAdapter;

        private GL _gl;
        private GRGlInterface _grGlInterface;
        private GRContext _grContext;

        public CrossPlatformApp(IServiceProvider sp, SilkWindowInterface window)
        {
            _sp = sp ?? throw new ArgumentNullException(nameof(sp));
            _window = window ?? throw new ArgumentNullException(nameof(window));

            _window.Load += OnLoad;
            _window.Update += OnUpdate;
            _window.Render += OnRender;
            _window.Resize += OnResize;
            _window.FramebufferResize += OnFramebufferResize;
            _window.Closing += OnClosing;
            _window.FocusChanged += OnFocusChanged;

            ApplyFrameRate(TimerSettings.GameAnimationFPS);
            TimerSettings.GameAnimationFpsChanged += ApplyFrameRate;
        }

        /// <summary>
        /// Runs the window's update and render loops at <paramref name="fps"/> (the player's
        /// <c>[display] fps</c>, via <see cref="TimerSettings.GameAnimationFPS"/>). With VSync on
        /// (Silk's default) the render rate is also capped by the display's refresh rate.
        /// </summary>
        private void ApplyFrameRate(int fps)
        {
            _window.FramesPerSecond = fps;
            _window.UpdatesPerSecond = fps;
        }

        private void OnLoad()
        {
            // GlobalWindow drives layout/bounds for things like the
            // StarAnimation background, and must be sized in the same pixel
            // space OnRender actually draws into — the physical framebuffer,
            // not the window's logical (point) size. On a HiDPI/Retina
            // display those differ by the display's scale factor; using the
            // wrong one here squeezes/misaligns everything relative to what
            // the GPU surface (sized from FramebufferSize in OnRender) shows.
            var fbSize = _window.FramebufferSize;
            GlobalWindow.UpdateSize(fbSize.X, fbSize.Y);
            UpdatePixelScale(fbSize);

            // The UI content itself (MainMenu/Board/Sidebar/dialogs — but
            // NOT the full-bleed StarAnimation background above, which keeps
            // using GlobalWindow directly) is authored in its own
            // DesignSize coordinate space. GlobalViewport scales that
            // uniformly (no stretch) to the actual window size, and the UI
            // area covers the whole window (no letterbox) — see
            // OnRender/SilkInputAdapter for the two halves (drawing and
            // hit-testing) of applying that mapping.
            GlobalViewport.DesignSize = UILayoutConstants.DesignSize;
            GlobalViewport.Recalculate(fbSize.X, fbSize.Y);

            _gl = GL.GetApi(_window);
            _grGlInterface = GRGlInterface.Create();
            _grContext = GRContext.CreateGl(_grGlInterface);

            _rootCanvas = UIInitializer.Initialize(_sp);
            _rootCanvas.MainWindow = new SilkWindow();

            _navigationManager = _sp.GetRequiredService<NavigationManager>();
            _gameSession = _sp.GetRequiredService<GameSession>();

            var scrollHandler = _sp.GetRequiredService<IScrollInputHandler>();
            _inputMgr = new UIInputManager(_rootCanvas, scrollHandler);

            _inputContext = _window.CreateInput();
            var keyboard = _inputContext.Keyboards.Count > 0 ? _inputContext.Keyboards[0] : null;
            _inputAdapter = new SilkInputAdapter(_inputMgr, _inputContext.Mice[0], _window, keyboard);

            _bgStar = new StarAnimationApp();
            _bgStar.Resize(GlobalWindow.LogicalWidth, GlobalWindow.LogicalHeight);

            GlobalTime.Timer = _timer;
            _timer.OnAnimationFrame += () =>
            {
                _bgStar?.Update();
                _rootCanvas?.Update();
                // Right after the UI update, where the board's update used to advance the clocks.
                _gameSession?.Tick();
                PhysicsRegistry.UpdateAll();
                _inputMgr?.EndFrame();
            };
            _timer.Start();
        }

        private void OnUpdate(double deltaSeconds) => _timer.Tick((float)deltaSeconds);

        private void OnFramebufferResize(Silk.NET.Maths.Vector2D<int> newSize)
        {
            GlobalWindow.UpdateSize(newSize.X, newSize.Y);
            UpdatePixelScale(newSize);
            GlobalViewport.Recalculate(newSize.X, newSize.Y);
            _bgStar?.Resize(GlobalWindow.LogicalWidth, GlobalWindow.LogicalHeight);
        }

        /// <summary>
        /// Framebuffer pixels per logical window unit (2 on Retina, 1 otherwise).
        /// </summary>
        private void UpdatePixelScale(Silk.NET.Maths.Vector2D<int> framebufferSize)
        {
            var logicalSize = _window.Size;
            if (logicalSize.X > 0)
                GlobalWindow.UpdatePixelScale(framebufferSize.X / (float)logicalSize.X);
        }

        /// <summary>
        /// Clamps the OS window to <see cref="UILayoutConstants.MinimumWindowSize"/>.
        /// Silk.NET/GLFW has no built-in "minimum size" constraint to set
        /// once, so this enforces it manually on every resize; the
        /// re-assignment below is idempotent once clamped, so it doesn't
        /// loop (Resize firing again with the already-clamped size is a
        /// no-op here).
        /// </summary>
        private void OnResize(Silk.NET.Maths.Vector2D<int> newSize)
        {
            int minWidth = (int)UILayoutConstants.MinimumWindowSize.X;
            int minHeight = (int)UILayoutConstants.MinimumWindowSize.Y;

            int clampedWidth = System.Math.Max(newSize.X, minWidth);
            int clampedHeight = System.Math.Max(newSize.Y, minHeight);

            if (clampedWidth != newSize.X || clampedHeight != newSize.Y)
                _window.Size = new Silk.NET.Maths.Vector2D<int>(clampedWidth, clampedHeight);
        }

        private void OnRender(double deltaSeconds)
        {
            var size = _window.FramebufferSize;
            if (size.X <= 0 || size.Y <= 0) return;

            int fbo = _gl.GetInteger(GLEnum.FramebufferBinding);
            var glInfo = new GRGlFramebufferInfo((uint)fbo, (uint)GLEnum.Rgba8);
            using var renderTarget = new GRBackendRenderTarget(size.X, size.Y, 0, 8, glInfo);
            using var surface = SKSurface.Create(_grContext, renderTarget, GRSurfaceOrigin.BottomLeft, SKColorType.Rgba8888);
            using IGraphics g = new SkiaGraphics(surface.Canvas);

            // Background renders full-bleed in logical units (scaled up to the
            // physical framebuffer by PixelScale, so star sizes/speeds/radii look
            // the same on Retina and non-Retina displays); UI content renders
            // in design units scaled by GlobalViewport.Scale (Offset is zero:
            // the UI area covers the whole window too) — see GlobalViewport's
            // doc comment.
            g.PushTransform(GlobalWindow.PixelScale, 0f, 0f);
            _bgStar?.Render(g);
            g.PopTransform();

            g.PushTransform(GlobalViewport.Scale, GlobalViewport.Offset.X, GlobalViewport.Offset.Y);
            _rootCanvas?.Draw(g);
            g.PopTransform();

            // Debug readouts (FPS / network latency) over everything, when switched on.
            DebugOverlay.Draw(g);

            surface.Canvas.Flush();
            _grContext.Flush();
        }

        // Losing focus mid-drag (e.g. Cmd-Tab) means the MouseUp never arrives.
        private void OnFocusChanged(bool focused)
        {
            if (!focused)
                _inputMgr?.CancelInput();
        }

        private void OnClosing()
        {
            TimerSettings.GameAnimationFpsChanged -= ApplyFrameRate;
            _timer.Stop();
            _grContext?.Dispose();
            _grGlInterface?.Dispose();
            _gl?.Dispose();
        }

        public void Dispose() => OnClosing();
    }
}

/* ----- ----- ----- ----- */
// CrossPlatformApp.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/09/24
// Update Date: 2026/09/24
// Version: v1.0
/* ----- ----- ----- ----- */

using System;

using Microsoft.Extensions.DependencyInjection;

using SkiaSharp;
using Silk.NET.Input;
using Silk.NET.OpenGL;
using SilkWindowInterface = Silk.NET.Windowing.IWindow;

using Chinese_Chess_v3.Game.UI.Constants;

using Engine.Globals;
using Engine.Physics;
using Engine.Platform;
using Engine.Platform.Skia;
using Engine.Styles;
using Engine.Timing;
using Engine.UI.Core.Elements;
using Engine.UI.Core.Infrastructure;
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
            _window.FramebufferResize += OnFramebufferResize;
            _window.Closing += OnClosing;
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

            _gl = GL.GetApi(_window);
            _grGlInterface = GRGlInterface.Create();
            _grContext = GRContext.CreateGl(_grGlInterface);

            _rootCanvas = UIInitializer.Initialize(_sp);
            _rootCanvas.MainWindow = new SilkWindow();

            _navigationManager = _sp.GetRequiredService<NavigationManager>();

            var scrollHandler = _sp.GetRequiredService<IScrollInputHandler>();
            _inputMgr = new UIInputManager(_rootCanvas, scrollHandler);

            _inputContext = _window.CreateInput();
            _inputAdapter = new SilkInputAdapter(_inputMgr, _inputContext.Mice[0], _window);

            _bgStar = new StarAnimationApp();
            _bgStar.Resize(fbSize.X, fbSize.Y);

            GlobalTime.Timer = _timer;
            _timer.OnAnimationFrame += () =>
            {
                _bgStar?.Update();
                _rootCanvas?.Update();
                PhysicsRegistry.UpdateAll();
                _inputMgr?.EndFrame();
            };
            _timer.Start();
        }

        private void OnUpdate(double deltaSeconds) => _timer.Tick((float)deltaSeconds);

        private void OnFramebufferResize(Silk.NET.Maths.Vector2D<int> newSize)
        {
            GlobalWindow.UpdateSize(newSize.X, newSize.Y);
            _bgStar?.Resize(newSize.X, newSize.Y);
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

            _bgStar?.Render(g);
            _rootCanvas?.Draw(g);

            surface.Canvas.Flush();
            _grContext.Flush();
        }

        private void OnClosing()
        {
            _timer.Stop();
            _grContext?.Dispose();
            _grGlInterface?.Dispose();
            _gl?.Dispose();
        }

        public void Dispose() => OnClosing();
    }
}

/* ----- ----- ----- ----- */
// UiFactory.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/17
// Update Date: 2025/05/17
// Version: v1.0
/* ----- ----- ----- ----- */

using System;
using System.Collections.Generic;

using Chinese_Chess_v3.UI.Elements;
using Chinese_Chess_v3.UI.Input;
using Chinese_Chess_v3.UI.Screens;
using Chinese_Chess_v3.UI.Screens.Game;
using Chinese_Chess_v3.UI.Screens.Menu;
using Microsoft.Extensions.DependencyInjection;

namespace Chinese_Chess_v3.UI.Core
{
    public class UiFactory : IUiFactory
    {
        private readonly IServiceProvider _sp;

        public UiFactory(IServiceProvider sp)
        {
            _sp = sp;
        }
        private readonly Dictionary<Type, Func<IUiFactory, UIElement>> _factories = new();
        private readonly Dictionary<Type, Func<IUiFactoryContext, UIElement>> _factoriesWithContext = new();

        public T Create<T>() where T : UIElement
        {
            var type = typeof(T);

            if (_factoriesWithContext.TryGetValue(type, out var contextFactory))
            {
                var ctx = new UiFactoryContext(_sp, this);
                return (T)contextFactory(ctx);
            }
            
            if (_factories.TryGetValue(type, out var factory))
                return (T)factory(this);
                
            throw new InvalidOperationException($"No factory registered for {type}");
        }

        public void RegisterFactory<T>(Func<IUiFactory, T> factory) where T : UIElement
        {
            _factories[typeof(T)] = factory;
        }
        public void RegisterFactory<T>(Func<IUiFactoryContext, T> factory) where T : UIElement
        {
            _factoriesWithContext[typeof(T)] = ctx => factory(ctx);
        }

        public void ClearCache<T>() where T : UIElement
        {
            _factories.Remove(typeof(T));
        }

        public void ClearAllCache()
        {
            _factories.Clear();
        }

        public UIScrollContainer CreateScrollContainer()
        {
            var scroll = _sp.GetRequiredService<IScrollInputHandler>();
            return new UIScrollContainer(scroll);
        }
        public T Resolve<T>() => _sp.GetRequiredService<T>();

        public MainMenu CreateMainMenu()
        {
            var menu = _sp.GetRequiredService<MainMenu>();
            var handler = _sp.GetRequiredService<MainMenuHandler>();
            var renderer = _sp.GetRequiredService<MainMenuRenderer>();

            handler.Init((this, menu));
            menu.Init((this, handler, renderer));

            return menu;
        }
        public GameMenu CreateGameMenu()
        {
            var menu = _sp.GetRequiredService<GameMenu>();
            var handler = _sp.GetRequiredService<GameMenuHandler>();
            var renderer = _sp.GetRequiredService<GameMenuRenderer>();

            handler.Init((this, menu));
            menu.Init((this, handler, renderer));

            return menu;
        }

        /*
        public NewGameMenu CreateNewGameMenu()
        {
            return new NewGameMenu(this); // 同上
        }
        public Tooltip CreateTooltip(string text)
        {
            return new Tooltip(text);
        }*/

        private class UiFactoryContext : IUiFactoryContext
        {
            public IServiceProvider ServiceProvider { get; }
            public IUiFactory UiFactory { get; }

            public UiFactoryContext(IServiceProvider sp, IUiFactory factory)
            {
                ServiceProvider = sp;
                UiFactory = factory;
            }
        }
    }
}

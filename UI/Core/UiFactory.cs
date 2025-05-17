/* ----- ----- ----- ----- */
// UiFactory.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/17
// Update Date: 2025/05/17
// Version: v1.0
/* ----- ----- ----- ----- */

using System;

using Chinese_Chess_v3.UI.Elements;
using Chinese_Chess_v3.UI.Input;
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

        public T Create<T>() where T : UIElement
        {
            return _sp.GetRequiredService<T>();
        }

        public UIScrollContainer CreateScrollContainer()
        {
            var scroll = _sp.GetRequiredService<IScrollInputHandler>();
            return new UIScrollContainer(scroll);
        }

        public MainMenu CreateMainMenu()
        {
            return new MainMenu(this); // 傳遞 factory 給 MainMenu 建立其他元件
        }
/*
        public SubMenu CreateSubMenu()
        {
            return new SubMenu(this); // 同上
        }

        public Tooltip CreateTooltip(string text)
        {
            return new Tooltip(text);
        }*/
    }
}

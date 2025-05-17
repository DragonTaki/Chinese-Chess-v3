/* ----- ----- ----- ----- */
// NewGameMenu.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/16
// Update Date: 2025/05/16
// Version: v1.0
/* ----- ----- ----- ----- */

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

using Chinese_Chess_v3.UI.Constants;
using Chinese_Chess_v3.UI.Core;
using Chinese_Chess_v3.UI.Elements;
using Chinese_Chess_v3.UI.Menu;
using Chinese_Chess_v3.UI.Utils;

using SharedLib.MathUtils;

namespace Chinese_Chess_v3.UI.Screens.Menu.Submenus
{
    public class LoadGameMenu : UIElement
    {
        private UIScrollContainer _scroll;
        private readonly LoadGameMenuRenderer renderer;
        private List<UIButton> buttons = new();
        public LoadGameMenu(IUiFactory factory)
        {
            _scroll = factory.CreateScrollContainer();
            
            LocalPosition = UILayoutConstants.Submenu.Position;
            Size = UILayoutConstants.Submenu.Size;

            renderer = new LoadGameMenuRenderer(this);

            _scroll.Layout = UILayoutConstants.Submenu.ScrollContainer.Layout;
            _scroll.BaseScrollY = -UILayoutConstants.Submenu.MarginY;
            _scroll.OverscrollLimit = UILayoutConstants.Submenu.MarginY;

            this.AddChild(_scroll);

            var menuEntries = new List<MenuEntry<NewGameMenuType>>
            {
                new MenuEntry<NewGameMenuType>("LoadGameMenu Option 1", NewGameMenuType.Traditional, () => Console.WriteLine("假選項1被點擊")),
                new MenuEntry<NewGameMenuType>("LoadGameMenu Option 2", NewGameMenuType.FlipChess, () => Console.WriteLine("假選項2被點擊")),
                new MenuEntry<NewGameMenuType>("LoadGameMenu Option 3", NewGameMenuType.FlipChess, () => Console.WriteLine("假選項3被點擊")),
                new MenuEntry<NewGameMenuType>("LoadGameMenu Option 4", NewGameMenuType.FlipChess, () => Console.WriteLine("假選項4被點擊")),
                new MenuEntry<NewGameMenuType>("LoadGameMenu Option 5", NewGameMenuType.FlipChess, () => Console.WriteLine("假選項5被點擊")),
                new MenuEntry<NewGameMenuType>("LoadGameMenu Option 6", NewGameMenuType.FlipChess, () => Console.WriteLine("假選項6被點擊")),
                new MenuEntry<NewGameMenuType>("LoadGameMenu Option 7", NewGameMenuType.FlipChess, () => Console.WriteLine("假選項7被點擊")),
                new MenuEntry<NewGameMenuType>("LoadGameMenu Option 8", NewGameMenuType.FlipChess, () => Console.WriteLine("假選項8被點擊")),
                new MenuEntry<NewGameMenuType>("LoadGameMenu Option 9", NewGameMenuType.FlipChess, () => Console.WriteLine("假選項9被點擊")),
            };

            for (int i = 0; i < menuEntries.Count; i++)
            {
                var entry = menuEntries[i];
                var button = new UIButton<NewGameMenuType>(entry);
                button.LocalPosition = UILayoutConstants.Submenu.Button.Position +
                    new Vector2F(0.0f, (UILayoutConstants.Submenu.Button.Size.Y + UILayoutConstants.Submenu.MarginY) * i);
                button.Size = UILayoutConstants.Submenu.Button.Size;

                _scroll.AddChild(button);
                buttons.Add(button);
            }

            _scroll.ContentHeight = buttons.Count * (UILayoutConstants.Submenu.Button.Size.Y + UILayoutConstants.Submenu.MarginY);
        }

        private void StartNewGame(NewGameMenuType selectedGamemode)
        {
            Console.WriteLine($"NewgameMenu: selected: {selectedGamemode}");
        }

        protected override void OnUpdate()
        {
            _scroll.Update();
        }
        
        protected override void OnDraw(Graphics g)
        {
            renderer.Draw(g);
        }

        public List<UIButton> Buttons => buttons;
        public List<UIButton> GetVisibleButtons()
        {
            UIElementUtils.UpdateVisibleState(buttons, GetAbsClipRect());
            return buttons.Where(b => b.IsEnabled).ToList();
        }

        public RectangleF GetAbsClipRect() => _scroll.GetAbsClippingRect();
    }
}

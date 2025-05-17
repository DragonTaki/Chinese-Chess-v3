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
    public class NewGameMenu : UIElement
    {
        private readonly UIScrollContainer _scroll;
        private readonly NewGameMenuRenderer renderer;
        private readonly NewGameMenuHandler handler;
        private readonly List<UIButton> buttons = new();
        private Action<NewGameMenuType> onStart;

        public NewGameMenu(IUiFactory factory)
        {
            _scroll = factory.CreateScrollContainer();

            LocalPosition = UILayoutConstants.Submenu.Position;
            Size = UILayoutConstants.Submenu.Size;

            renderer = new NewGameMenuRenderer(this);
        }

        private void BuildMenu()
        {
            _scroll.Layout = UILayoutConstants.Submenu.ScrollContainer.Layout;
            _scroll.BaseScrollY = -UILayoutConstants.Submenu.MarginY;
            _scroll.OverscrollLimit = UILayoutConstants.Submenu.MarginY;

            this.AddChild(_scroll);

            var menuEntries = NewGameMenuOptions.Create(onStart);

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
        
        public void Setup(Action<NewGameMenuType> onStartGame)
        {
            onStart = onStartGame;
            BuildMenu();
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

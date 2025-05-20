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
using Chinese_Chess_v3.UI.Core.Base;
using Chinese_Chess_v3.UI.Core.Elements;
using Chinese_Chess_v3.UI.Core.Interfaces;
using Chinese_Chess_v3.UI.Screens.Menu.Options;
using Chinese_Chess_v3.UI.Utils;

using SharedLib.MathUtils;

namespace Chinese_Chess_v3.UI.Screens.Menu.Submenus
{
    public class NewGameMenu
        : InitializableOnceElement<(IUiFactory factory, NewGameMenuHandler handler, NewGameMenuRenderer renderer)>
        , IScreen
    {
        private UIScrollContainer _scroll;
        private NewGameMenuRenderer _renderer;
        private NewGameMenuHandler _handler;
        private readonly List<UIButton> _buttons = new();

        public NewGameMenu() {}
        
        protected override void OnInit((IUiFactory factory, NewGameMenuHandler handler, NewGameMenuRenderer renderer) arg)
        {
            _scroll = arg.factory.CreateScrollContainer();
            _handler = arg.handler;
            _renderer = arg.renderer;

            LocalPosition = UILayoutConstants.Submenu.Position;
            Size = UILayoutConstants.Submenu.Size;

            BuildMenu();
        }

        private void BuildMenu()
        {
            _scroll.Layout = UILayoutConstants.Submenu.ScrollContainer.Layout;
            _scroll.BaseScrollY = -UILayoutConstants.Submenu.MarginY;
            _scroll.OverscrollLimit = UILayoutConstants.Submenu.MarginY;

            this.AddChild(_scroll);

            var menuEntries = NewGameMenuOptions.Create(_handler.StartNewGame);

            for (int i = 0; i < menuEntries.Count; i++)
            {
                var entry = menuEntries[i];
                var button = new UIButton<NewGameMenuType>(entry);
                button.LocalPosition = UILayoutConstants.Submenu.Button.Position +
                    new Vector2F(0.0f, (UILayoutConstants.Submenu.Button.Size.Y + UILayoutConstants.Submenu.MarginY) * i);
                button.Size = UILayoutConstants.Submenu.Button.Size;

                _scroll.AddChild(button);
                _buttons.Add(button);
            }

            _scroll.ContentHeight = _buttons.Count * (UILayoutConstants.Submenu.Button.Size.Y + UILayoutConstants.Submenu.MarginY);
        }

        public void OnEnter()
        {
            _handler.OnEnter();
        }
        
        public void OnExit()
        {
            _handler.OnExit();
        }

        protected override void OnUpdate()
        {
            _scroll.Update();
        }
        
        protected override void OnDraw(Graphics g)
        {
            _renderer.Draw(g);
        }

        public List<UIButton> Buttons => _buttons;
        public List<UIButton> GetVisibleButtons()
        {
            UIElementUtils.UpdateVisibleState(_buttons, GetAbsClipRect());
            return _buttons.Where(b => b.IsEnabled).ToList();
        }

        public RectangleF GetAbsClipRect() => _scroll.GetAbsClippingRect();
    }
}

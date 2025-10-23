/* ----- ----- ----- ----- */
// MainMenu.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/15
// Update Date: 2025/05/17
// Version: v1.1
/* ----- ----- ----- ----- */

using System.Collections.Generic;
using System.Drawing;
using System.Linq;

using Chinese_Chess_v3.Game.Constants.UI;
using Chinese_Chess_v3.Game.UI.Screens.Menus.Options;

using Engine.Mathematics;
using Engine.UI.Core.Base;
using Engine.UI.Core.Elements;
using Engine.UI.Core.Interfaces;
using Engine.UI.Utils;

namespace Chinese_Chess_v3.Game.UI.Screens.Menus
{
    public class MainMenu
        : InitializableOnceElement<(IUiFactory factory, MainMenuHandler handler, MainMenuRenderer renderer)>
        , IScreen
    {
        private UIScrollContainer _scroll;
        private MainMenuHandler _handler;
        private MainMenuRenderer _renderer;
        private readonly List<UIButton> _buttons = new();

        public MainMenu() {}
        protected override void OnInit((IUiFactory factory, MainMenuHandler handler, MainMenuRenderer renderer) arg)
        {
            _scroll = arg.factory.CreateScrollContainer();
            _handler = arg.handler;
            _renderer = arg.renderer;

            LocalPosition = UILayoutConstants.MainMenu.Position;
            Size = UILayoutConstants.MainMenu.Size;

            BuildMenu();
        }

        private void BuildMenu()
        {
            _scroll.Layout = UILayoutConstants.MainMenu.ScrollContainer.Layout;
            _scroll.BaseScrollY = -UILayoutConstants.MainMenu.Margin;
            _scroll.OverscrollLimit = UILayoutConstants.MainMenu.Margin;

            this.AddChild(_scroll);

            var menuEntries = MainMenuOptions.Create(_handler.SwitchSubmenu);

            for (int i = 0; i < menuEntries.Count; i++)
            {
                var entry = menuEntries[i];
                var button = new UIButton<MainMenuType>(entry);
                button.LocalPosition = UILayoutConstants.MainMenu.Button.Position +
                    new Vector2F(0.0f, (UILayoutConstants.MainMenu.Button.Size.Y + UILayoutConstants.MainMenu.Margin) * i);
                button.Size = UILayoutConstants.MainMenu.Button.Size;

                _scroll.AddChild(button);
                _buttons.Add(button);
            }

            _scroll.ContentHeight = _buttons.Count * (UILayoutConstants.MainMenu.Button.Size.Y + UILayoutConstants.MainMenu.Margin);
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

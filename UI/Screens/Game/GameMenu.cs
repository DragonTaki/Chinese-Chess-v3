/* ----- ----- ----- ----- */
// GameMenu.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/17
// Update Date: 2025/05/17
// Version: v1.0
/* ----- ----- ----- ----- */

using System.Collections.Generic;
using System.Drawing;
using System.Linq;

using Chinese_Chess_v3.UI.Constants;
using Chinese_Chess_v3.UI.Core;
using Chinese_Chess_v3.UI.Elements;
using Chinese_Chess_v3.UI.Menu;
using Chinese_Chess_v3.UI.Utils;

using SharedLib.MathUtils;

namespace Chinese_Chess_v3.UI.Screens.Game
{
    public class GameMenu
        : IInitializableOnceElement<(IUiFactory factory, GameMenuHandler handler, GameMenuRenderer renderer)>
        , IScreen
    {
        private UIScrollContainer _scroll;
        private GameMenuRenderer _renderer;
        private GameMenuHandler _handler;
        private readonly List<UIButton> _buttons = new();

        public GameMenu() {}
        protected override void OnInit((IUiFactory factory, GameMenuHandler handler, GameMenuRenderer renderer) arg)
        {
            _scroll = arg.factory.CreateScrollContainer();
            _handler = arg.handler;
            _renderer = arg.renderer;

            LocalPosition = UILayoutConstants.GameMenu.Position;
            Size = UILayoutConstants.GameMenu.Size;

            BuildMenu();
        }

        private void BuildMenu()
        {
            _scroll.Layout = UILayoutConstants.GameMenu.ScrollContainer.Layout;
            _scroll.BaseScrollY = -UILayoutConstants.GameMenu.Margin;
            _scroll.OverscrollLimit = UILayoutConstants.GameMenu.Margin;

            this.AddChild(_scroll);

            var menuEntries = GameMenuOptions.Create(_handler.GameMenuAction);

            for (int i = 0; i < menuEntries.Count; i++)
            {
                var entry = menuEntries[i];
                var button = new UIButton<GameMenuType>(entry);
                button.LocalPosition = UILayoutConstants.GameMenu.Button.Position +
                    new Vector2F(0.0f, (UILayoutConstants.GameMenu.Button.Size.Y + UILayoutConstants.GameMenu.Margin) * i);
                button.Size = UILayoutConstants.GameMenu.Button.Size;

                _scroll.AddChild(button);
                _buttons.Add(button);
            }

            _scroll.ContentHeight = _buttons.Count * (UILayoutConstants.GameMenu.Button.Size.Y + UILayoutConstants.GameMenu.Margin);
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

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

using Chinese_Chess_v3.UI.Constants;
using Chinese_Chess_v3.UI.Core;
using Chinese_Chess_v3.UI.Elements;
using Chinese_Chess_v3.UI.Menu;
using Chinese_Chess_v3.UI.Utils;

using SharedLib.MathUtils;

namespace Chinese_Chess_v3.UI.Screens.Menu
{
    public class MainMenu : UIElement, IScreen
    {
        private UIScrollContainer _scroll;
        private MainMenuRenderer renderer;
        private MainMenuHandler handler;
        private readonly List<UIButton> buttons = new();

        public MainMenu() {}
        public void Setup(IUiFactory factory, MainMenuHandler handler, MainMenuRenderer renderer)
        {
            _scroll = factory.CreateScrollContainer();
            this.handler = handler;
            this.renderer = renderer;

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

            var menuEntries = MainMenuOptions.Create(handler.SwitchSubmenu, handler.ExitApplication);

            for (int i = 0; i < menuEntries.Count; i++)
            {
                var entry = menuEntries[i];
                var button = new UIButton<MainMenuType>(entry);
                button.LocalPosition = UILayoutConstants.MainMenu.Button.Position +
                    new Vector2F(0.0f, (UILayoutConstants.MainMenu.Button.Size.Y + UILayoutConstants.MainMenu.Margin) * i);
                button.Size = UILayoutConstants.MainMenu.Button.Size;

                _scroll.AddChild(button);
                buttons.Add(button);
            }

            _scroll.ContentHeight = buttons.Count * (UILayoutConstants.MainMenu.Button.Size.Y + UILayoutConstants.MainMenu.Margin);
        }

        public void OnEnter()
        {
            handler.OnEnter();
        }
        
        public void OnExit()
        {
            handler.OnExit();
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

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
        private readonly LoadGameMenuRenderer renderer;
        private UIScrollContainer scroll;
        private List<UIButton> buttons = new();
        public LoadGameMenu()
        {
            LocalPosition = UILayoutConstants.Submenu.Position;
            Size = UILayoutConstants.Submenu.Size;

            renderer = new LoadGameMenuRenderer(this);

            scroll = new UIScrollContainer();
            scroll.LocalPosition = UILayoutConstants.Submenu.ScrollContainer.Position;
            scroll.Size = UILayoutConstants.Submenu.ScrollContainer.Size;
            scroll.BaseScrollY = -UILayoutConstants.Submenu.MarginY;
            scroll.OverscrollLimit = UILayoutConstants.Submenu.MarginY;

            this.AddChild(scroll);


            for (int i = 0; i < this.Children.Count; i++)
            {
                Console.WriteLine($"{this}[{i}]: {this.Children[i]}");
            }
            var menuEntries = NewGameMenuOptions.Create(StartNewGame);

            for (int i = 0; i < menuEntries.Count; i++)
            {
                var entry = menuEntries[i];
                var button = new UIButton<NewGameMenuType>(entry);
                button.LocalPosition = UILayoutConstants.Submenu.Button.Position +
                    new Vector2F(0.0f, (UILayoutConstants.Submenu.Button.Size.Y + UILayoutConstants.Submenu.MarginY) * i);
                button.Size = UILayoutConstants.Submenu.Button.Size;

                scroll.AddChild(button);
                buttons.Add(button);
            }

            scroll.ContentHeight = buttons.Count * (UILayoutConstants.Submenu.Button.Size.Y + UILayoutConstants.Submenu.MarginY);
        }

        private void StartNewGame(NewGameMenuType selectedGamemode)
        {
            Console.WriteLine($"NewgameMenu: selected: {selectedGamemode}");
        }

        protected override void OnUpdate()
        {
            scroll.Update();
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

        public RectangleF GetAbsClipRect() => scroll.GetAbsClippingRect();
    }
}

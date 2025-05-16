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
        private readonly NewGameMenuRenderer renderer;
        private UIScrollContainer scroll;
        private List<UIButton> buttons = new();
        public NewGameMenu()
        {
            LocalPosition = UILayoutConstants.SecondMenu.Position;
            Size = UILayoutConstants.SecondMenu.Size;

            renderer = new NewGameMenuRenderer(this);

            scroll = new UIScrollContainer();
            scroll.LocalPosition = UILayoutConstants.SecondMenu.ScrollContainer.Position;
            scroll.Size = UILayoutConstants.SecondMenu.ScrollContainer.Size;
            scroll.BaseScrollY = -UILayoutConstants.SecondMenu.MarginY;
            scroll.OverscrollLimit = UILayoutConstants.SecondMenu.MarginY;

            AddChild(scroll);

            var menuEntries = NewGameMenuOptions.Create(StartNewGame);

            for (int i = 0; i < menuEntries.Count; i++)
            {
                var entry = menuEntries[i];
                var button = new UIButton<NewGameMenuType>(entry);
                button.LocalPosition = UILayoutConstants.MainMenu.Button.Position +
                    new Vector2F(0.0f, (UILayoutConstants.MainMenu.Button.Size.Y + UILayoutConstants.MainMenu.Margin) * i);
                button.Size = UILayoutConstants.MainMenu.Button.Size;

                scroll.AddChild(button);
                buttons.Add(button);
            }

            scroll.ContentHeight = buttons.Count * (UILayoutConstants.MainMenu.Button.Size.Y + UILayoutConstants.MainMenu.Margin);
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
            UIElementUtils.UpdateVisibleState(buttons, GetClipRect());
            return buttons.Where(b => b.IsEnabled).ToList();
        }

        public RectangleF GetClipRect() => scroll.GetClippingRect();
    }
}

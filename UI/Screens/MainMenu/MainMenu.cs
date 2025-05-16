/* ----- ----- ----- ----- */
// MainMenu.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/15
// Update Date: 2025/05/15
// Version: v1.0
/* ----- ----- ----- ----- */

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Chinese_Chess_v3.UI.Constants;
using Chinese_Chess_v3.UI.Core;
using Chinese_Chess_v3.UI.Elements;
using Chinese_Chess_v3.UI.Menu;
using SharedLib.MathUtils;

namespace Chinese_Chess_v3.UI.Screens.MainMenu
{
    public class MainMenu : UIElement
    {
        private readonly MainMenuRenderer renderer;
        private UIScrollContainer scroll;
        private List<UIButton> buttons = new();
        private MainMenuType? currentSubmenu = null;

        public MainMenu()
        {
            LocalPosition = UILayoutConstants.MainMenu.Position;
            Size = UILayoutConstants.MainMenu.Size;

            renderer = new MainMenuRenderer(this);

            scroll = new UIScrollContainer();
            scroll.LocalPosition = UILayoutConstants.MainMenu.ScrollContainer.Position;
            scroll.Size = UILayoutConstants.MainMenu.ScrollContainer.Size;
            scroll.BaseScrollY = -UILayoutConstants.MainMenu.Margin;
            scroll.OverscrollLimit = UILayoutConstants.MainMenu.Margin;

            AddChild(scroll);

            var menuEntries = MainMenuOptions.Create(
                selectedMenu => SwitchSubmenu(selectedMenu),
                () => ExitApplication()
            );
            for (int i = 0; i < menuEntries.Count; i++)
            {
                var entry = menuEntries[i];
                var button = new UIButton<MainMenuType>(entry);
                button.LocalPosition = UILayoutConstants.MainMenu.Button.Position +
                    new Vector2F(0.0f, (UILayoutConstants.MainMenu.Button.Size.Y + UILayoutConstants.MainMenu.Margin) * i);
                button.Size = UILayoutConstants.MainMenu.Button.Size;

                scroll.AddChild(button);
                buttons.Add(button);
            }
            
            scroll.ContentHeight = buttons.Count * (UILayoutConstants.MainMenu.Button.Size.Y + UILayoutConstants.MainMenu.Margin);
        }

        public List<UIButton> GetVisibleButtons()
        {
            RectangleF view = scroll.GetClippingRect();
            return buttons.Where(button =>
            {
                //if (button == null) return false;

                float y = button.LocalPosition.Current.Y;
                float h = button.Size.Y;
                return y + h > view.Top && y < view.Bottom;
            }).ToList();
        }

        private void SwitchSubmenu(MainMenuType selectedMenu)
        {
            Console.WriteLine($"{currentSubmenu}-> selected: {selectedMenu}");
            if (currentSubmenu == selectedMenu)
            {
                // 點擊同一個submenu，關閉二級選單
                currentSubmenu = null;
                //HideSubmenu();
            }
            else
            {
                // 顯示新submenu
                currentSubmenu = selectedMenu;
                //ShowSubmenu(submenu);
            }
        }

        private void ExitApplication()
        {
            Application.Exit();
        }

        public override void Draw(Graphics g)
        {
            scroll.Update();
            renderer.Draw(g);
        }
        public List<UIButton> Buttons => GetVisibleButtons();
        public RectangleF GetClipRect() => scroll.GetClippingRect();
    }
}
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
using Chinese_Chess_v3.UI.Screens.Menu.Submenus;
using Chinese_Chess_v3.UI.Utils;
using SharedLib.MathUtils;

namespace Chinese_Chess_v3.UI.Screens.Menu
{
    public class MainMenu : UIElement
    {
        private readonly MainMenuRenderer renderer;
        private UIScrollContainer scroll;
        private List<UIButton> buttons = new();
        private MainMenuType? currentSubmenu = null;
        private Dictionary<MainMenuType, UIElement> submenus = new();

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

            this.AddChild(scroll);

            submenus[MainMenuType.NewGame] = CreateSubMenu<NewGameMenu>();
            submenus[MainMenuType.LoadGame] = CreateSubMenu<NewGameMenu>();
            submenus[MainMenuType.RuleSettings] = CreateSubMenu<NewGameMenu>();
            submenus[MainMenuType.Help] = CreateSubMenu<NewGameMenu>();
            submenus[MainMenuType.Settings] = CreateSubMenu<LoadGameMenu>();

            var menuEntries = MainMenuOptions.Create(SwitchSubmenu, ExitApplication);

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

        private UIElement CreateSubMenu<T>() where T : UIElement, new()
        {
            var menu = new T();
            menu.IsVisible = false;
            return menu;
        }

        private void SwitchSubmenu(MainMenuType selectedMenu)
        {
            Console.WriteLine($"[Switch] Showing submenu: {selectedMenu}, instance: {submenus[selectedMenu].GetHashCode()}");
            Console.WriteLine($"Mainmenu: {currentSubmenu}-> selected: {selectedMenu}");
            foreach (var kvp in submenus)
            {
                Console.WriteLine($"{kvp.Key}: {kvp.Value.IsVisible}, {kvp.Value.GetHashCode()}");
                for (int i=0;i< kvp.Value.Children.Count;i++)
                {
                    Console.WriteLine($"{kvp.Key} child [{i}], {kvp.Value.Children[i].GetHashCode()}");
                }
            }

            // Remove current submenu (if have)
            if (currentSubmenu.HasValue)
            {
                var oldMenu = submenus[currentSubmenu.Value];
                oldMenu.IsVisible = false;
                this.RemoveChild(oldMenu);
            }

            // Shift submenu
            if (currentSubmenu == selectedMenu)
            {
                // Clicked the same one, close it
                currentSubmenu = null;
            }
            else
            {
                // Clicked the new one, show it
                currentSubmenu = selectedMenu;
                submenus[currentSubmenu.Value].IsVisible = true;
                this.AddChild(submenus[currentSubmenu.Value]);
            }
        }

        private void ExitApplication()
        {
            Application.Exit();
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

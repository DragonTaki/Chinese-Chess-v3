/* ----- ----- ----- ----- */
// MainMenu.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/15
// Update Date: 2025/05/17
// Version: v1.1
/* ----- ----- ----- ----- */

using Chinese_Chess_v3.Game.Constants.UI;
using Chinese_Chess_v3.Game.UI.Screens.Menus.Options;

using Engine.Mathematics;
using Engine.UI.Core.Elements;
using Engine.UI.Core.Interfaces;

namespace Chinese_Chess_v3.Game.UI.Screens.Menus
{
    public class MainMenu : UIMenu<MainMenuHandler>, IScreen
    {
        public MainMenu() { }
        /*
        protected override void OnInit((IUiFactory factory, MainMenuHandler handler, MainMenuRenderer renderer) arg)  // OLD
        {
            _scroll = arg.factory.CreateScrollContainer();
            _handler = arg.handler;
            _renderer = arg.renderer;

            LocalPosition = UILayoutConstants.MainMenu.Position;
            Size = UILayoutConstants.MainMenu.Size;

            BuildMenu();
        }*/
        protected override void BuildButtons()
        {
            // 建立按鈕
            var menuEntries = MainMenuOptions.Create(_handler.SwitchSubmenu);
            Vector2F btnStartPos = UILayoutConstants.MainMenu.Button.Position; // 若未用可註解

            for (int i = 0; i < menuEntries.Count; i++)
            {
                var entry = menuEntries[i];
                var button = new UIButton<MainMenuType>(entry);

                // 設定文字與動作
                button.Text = entry.Label;
                button.Action = () => _handler.SwitchSubmenu(entry.Type);

                // 設定位置與大小
                button.Size = UILayoutConstants.MainMenu.Button.Size;
                button.LocalPosition = UILayoutConstants.MainMenu.Button.Position +
                    new Vector2F(0, i * (button.Size.Y + UILayoutConstants.MainMenu.Margin));

                // 加入 ScrollContainer 與 Buttons 列表
                ScrollContainer.AddChild(button);
                Buttons.Add(button);
            }
        }

/*
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
        }*/
    }
}

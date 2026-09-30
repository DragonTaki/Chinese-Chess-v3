/* ----- ----- ----- ----- */
// UIMainMenu.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/15
// Update Date: 2026/09/30
// Version: v1.3
/* ----- ----- ----- ----- */

using Chinese_Chess_v3.Game.UI.Constants;

using Engine.Mathematics;
using Engine.UI.Core.Elements;
using Engine.UI.Core.Handlers;
using Engine.UI.Core.Interfaces;
using Engine.UI.Core.Renderers;

namespace Chinese_Chess_v3.Game.UI.Menus.MainMenu
{
    public class UIMainMenu : UIMenu<UIMainMenu, UIMainMenuHandler, UIMainMenuRenderer>
    {
        public UIMainMenu() { }
        protected override void OnBeforeInit(IUiFactory factory)
        {
            ButtonSpacing = UILayoutConstants.MainMenu.Button.Spacing;
        }

        protected override void OnInit(IUiFactory factory)
        {
            // Declared sizes (also the pre-layout fallback), then the layout rules that
            // actually place the panel, its scroll container and its buttons.
            Layout = UILayoutConstants.MainMenu.Layout;
            ScrollContainer.Layout = UILayoutConstants.MainMenu.ScrollContainer.Layout;

            // The menu covers the whole root; its panel (outline, buttons) is the left
            // MainMenu.Size.X-wide column.
            MenuLayout.ApplyScreen(this, UILayoutConstants.MainMenu.Size.X);
            MenuLayout.ApplyScrollContainer(ScrollContainer,
                UILayoutConstants.MainMenu.Margin, UILayoutConstants.MainMenu.Margin, UILayoutConstants.MainMenu.Button.Spacing,
                UILayoutConstants.MainMenu.Size.X);
        }

        protected override void BuildButtons()
        {
            // 建立按鈕
            var menuEntries = UIMainMenuOptions.Create(Handler.SwitchSubmenu);
            Vector2F btnStartPos = UILayoutConstants.MainMenu.Button.Position; // 若未用可註解

            for (int i = 0; i < menuEntries.Count; i++)
            {
                var entry = menuEntries[i];
                var button = _factory.CreateElement<UIButton, UIButtonHandler, UIButtonRenderer>();

                button.Text = entry.Label;
                button.Handler.Action = () => Handler.SwitchSubmenu(entry.Type);

                // Stacked by the scroll container's flex column (Gap = Button.Spacing).
                button.Size = UILayoutConstants.MainMenu.Button.Size;
                MenuLayout.ApplyButton(button, UILayoutConstants.MainMenu.Button.Size.Y);

                ScrollContainer.AddChild(button);
                Buttons.Add(button);
            }
        }
    }
}

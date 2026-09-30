/* ----- ----- ----- ----- */
// UINewGameMenu.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/16
// Update Date: 2026/09/30
// Version: v1.1
/* ----- ----- ----- ----- */

using Chinese_Chess_v3.Game.UI.Constants;

using Engine.Mathematics;
using Engine.UI.Core.Elements;
using Engine.UI.Core.Handlers;
using Engine.UI.Core.Interfaces;
using Engine.UI.Core.Renderers;

namespace Chinese_Chess_v3.Game.UI.Menus.NewGameMenu
{
    public class UINewGameMenu : UIMenu<UINewGameMenu, UINewGameMenuHandler, UINewGameMenuRenderer>
    {
        public UINewGameMenu() { }

        protected override void OnBeforeInit(IUiFactory factory)
        {
            ButtonSpacing = UILayoutConstants.Submenu.Button.Spacing;
        }

        protected override void OnInit(IUiFactory factory)
        {
            // Declared sizes (also the pre-layout fallback), then the layout rules that
            // actually place the panel, its scroll container and its buttons.
            Layout = UILayoutConstants.Submenu.Layout;
            ScrollContainer.Layout = UILayoutConstants.Submenu.ScrollContainer.Layout;

            MenuLayout.ApplyPanel(this, UILayoutConstants.Submenu.Layout);
            MenuLayout.ApplyScrollContainer(ScrollContainer,
                UILayoutConstants.Submenu.MarginX, UILayoutConstants.Submenu.MarginY, UILayoutConstants.Submenu.Button.Spacing);
        }

        protected override void BuildButtons()
        {
            var menuEntries = UINewGameMenuOptions.Create(Handler.StartNewGame);

            for (int i = 0; i < menuEntries.Count; i++)
            {
                var entry = menuEntries[i];
                var button = _factory.CreateElement<UIButton, UIButtonHandler, UIButtonRenderer>();

                button.Text = entry.Label;
                button.Handler.Action = () => Handler.StartNewGame(entry.Type);

                // Stacked by the scroll container's flex column (Gap = Button.Spacing).
                button.Size = UILayoutConstants.Submenu.Button.Size;
                MenuLayout.ApplyButton(button, UILayoutConstants.Submenu.Button.Size.Y);

                ScrollContainer.AddChild(button);
                Buttons.Add(button);
            }
        }
    }
}

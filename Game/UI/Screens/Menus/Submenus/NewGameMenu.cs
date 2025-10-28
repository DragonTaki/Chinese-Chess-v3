/* ----- ----- ----- ----- */
// NewGameMenu.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/16
// Update Date: 2025/05/16
// Version: v1.0
/* ----- ----- ----- ----- */

using Chinese_Chess_v3.Game.Constants.UI;
using Chinese_Chess_v3.Game.UI.Screens.Menus.Options;

using Engine.Mathematics;
using Engine.UI.Core.Elements;
using Engine.UI.Core.Handlers;
using Engine.UI.Core.Interfaces;
using Engine.UI.Core.Renderers;

namespace Chinese_Chess_v3.Game.UI.Screens.Menus.Submenus
{
    public class NewGameMenu : UIMenu<NewGameMenu, NewGameMenuHandler, NewGameMenuRenderer>
    {
        public NewGameMenu() { }

        protected override void OnBeforeInit(IUiFactory factory)
        {
            ButtonSpacing = UILayoutConstants.Submenu.Button.Spacing;
        }

        protected override void OnInit(IUiFactory factory)
        {
            Layout = UILayoutConstants.Submenu.Layout;
            ScrollContainer.Layout = UILayoutConstants.Submenu.ScrollContainer.Layout;
        }

        protected override void BuildButtons()
        {
            var menuEntries = NewGameMenuOptions.Create(Handler.StartNewGame);

            for (int i = 0; i < menuEntries.Count; i++)
            {
                var entry = menuEntries[i];
                var button = _factory.CreateElement<UIButton, UIButtonHandler, UIButtonRenderer>();

                button.Text = entry.Label;
                button.Handler.Action = () => Handler.StartNewGame(entry.Type);

                button.LocalPosition = UILayoutConstants.Submenu.Button.Position +
                    new Vector2F(0.0f, (UILayoutConstants.Submenu.Button.Size.Y + ButtonSpacing) * i);
                button.Size = UILayoutConstants.Submenu.Button.Size;

                ScrollContainer.AddChild(button);
                Buttons.Add(button);
            }
        }
    }
}

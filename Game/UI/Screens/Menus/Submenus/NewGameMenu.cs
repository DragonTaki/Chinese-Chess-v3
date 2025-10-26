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
using Engine.UI.Core.Interfaces;

namespace Chinese_Chess_v3.Game.UI.Screens.Menus.Submenus
{
    public class NewGameMenu : UIMenu<NewGameMenuHandler>, IScreen
    {
        public NewGameMenu() {}

        protected override void OnInit((IUiFactory, NewGameMenuHandler) arg)
        {
            ButtonSpacing = UILayoutConstants.Submenu.Button.Spacing;

            base.OnInit(arg);

            Layout = UILayoutConstants.Submenu.Layout;
            ScrollContainer.Layout = UILayoutConstants.Submenu.ScrollContainer.Layout;
        }

        protected override void BuildButtons()
        {
            var menuEntries = NewGameMenuOptions.Create(_handler.StartNewGame);

            for (int i = 0; i < menuEntries.Count; i++)
            {
                var entry = menuEntries[i];
                var button = new UIButton<NewGameMenuType>(entry);
                button.LocalPosition = UILayoutConstants.Submenu.Button.Position +
                    new Vector2F(0.0f, (UILayoutConstants.Submenu.Button.Size.Y + ButtonSpacing) * i);
                button.Size = UILayoutConstants.Submenu.Button.Size;

                ScrollContainer.AddChild(button);
                Buttons.Add(button);
            }
        }
    }
}

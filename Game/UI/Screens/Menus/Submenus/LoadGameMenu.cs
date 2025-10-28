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

using Chinese_Chess_v3.Game.Constants.UI;
using Chinese_Chess_v3.Game.UI.Screens.Menus.Options;

using Engine.Mathematics;
using Engine.UI.Core.Elements;
using Engine.UI.Core.Handlers;
using Engine.UI.Core.Interfaces;
using Engine.UI.Core.Renderers;
using Engine.UI.Widgets;

namespace Chinese_Chess_v3.Game.UI.Screens.Menus.Submenus
{
    public class LoadGameMenu : UIMenu<LoadGameMenu, LoadGameMenuHandler, LoadGameMenuRenderer>
    {
        public LoadGameMenu() { }

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
            var menuEntries = new List<ButtonEntry<NewGameMenuType>>
            {
                new ButtonEntry<NewGameMenuType>("LoadGameMenu Option 1", NewGameMenuType.Traditional, () => Console.WriteLine("假選項1被點擊")),
                new ButtonEntry<NewGameMenuType>("LoadGameMenu Option 2", NewGameMenuType.FlipChess, () => Console.WriteLine("假選項2被點擊")),
                new ButtonEntry<NewGameMenuType>("LoadGameMenu Option 3", NewGameMenuType.FlipChess, () => Console.WriteLine("假選項3被點擊")),
                new ButtonEntry<NewGameMenuType>("LoadGameMenu Option 4", NewGameMenuType.FlipChess, () => Console.WriteLine("假選項4被點擊")),
                new ButtonEntry<NewGameMenuType>("LoadGameMenu Option 5", NewGameMenuType.FlipChess, () => Console.WriteLine("假選項5被點擊")),
                new ButtonEntry<NewGameMenuType>("LoadGameMenu Option 6", NewGameMenuType.FlipChess, () => Console.WriteLine("假選項6被點擊")),
                new ButtonEntry<NewGameMenuType>("LoadGameMenu Option 7", NewGameMenuType.FlipChess, () => Console.WriteLine("假選項7被點擊")),
                new ButtonEntry<NewGameMenuType>("LoadGameMenu Option 8", NewGameMenuType.FlipChess, () => Console.WriteLine("假選項8被點擊")),
                new ButtonEntry<NewGameMenuType>("LoadGameMenu Option 9", NewGameMenuType.FlipChess, () => Console.WriteLine("假選項9被點擊")),
            };

            for (int i = 0; i < menuEntries.Count; i++)
            {
                var entry = menuEntries[i];
                var button = _factory.CreateElement<UIButton, UIButtonHandler, UIButtonRenderer>();

                button.Text = entry.Label;
                button.Handler.Action = () => Handler.StartNewGame();

                button.LocalPosition = UILayoutConstants.Submenu.Button.Position +
                    new Vector2F(0.0f, (UILayoutConstants.Submenu.Button.Size.Y + ButtonSpacing) * i);
                button.Size = UILayoutConstants.Submenu.Button.Size;

                ScrollContainer.AddChild(button);
                Buttons.Add(button);
            }
        }
    }
}

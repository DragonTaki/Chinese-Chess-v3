/* ----- ----- ----- ----- */
// UILoadGameMenu.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/16
// Update Date: 2026/10/04
// Version: v1.1
/* ----- ----- ----- ----- */

using System;
using System.Collections.Generic;

using Chinese_Chess_v3.Game.UI.Constants;
using Chinese_Chess_v3.Game.UI.Menus.NewGameMenu;

using Engine.Diagnostics;
using Engine.Mathematics;
using Engine.UI.Core.Elements;
using Engine.UI.Core.Handlers;
using Engine.UI.Core.Interfaces;
using Engine.UI.Core.Renderers;
using Engine.UI.Widgets;

namespace Chinese_Chess_v3.Game.UI.Menus.LoadGameMenu
{
    public class UILoadGameMenu : UIMenu<UILoadGameMenu, UILoadGameMenuHandler, UILoadGameMenuRenderer>
    {
        public UILoadGameMenu() { }

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

            // See UILayoutSheet.Submenu.
            LayoutRules.Apply(UILayoutSheet.Submenu.Panel);
            ScrollContainer.LayoutRules.Apply(UILayoutSheet.Submenu.ScrollContainer);
        }

        protected override void BuildButtons()
        {
            // Placeholder entries (假選項): a click only prints a console trace
            // (DebugOptions.ConsoleTrace).
            var menuEntries = new List<ButtonEntry<UINewGameMenuType>>
            {
                new ButtonEntry<UINewGameMenuType>("UILoadGameMenu Option 1", UINewGameMenuType.Traditional, () => { if (DebugOptions.ConsoleTrace) Console.WriteLine("假選項1被點擊"); }),
                new ButtonEntry<UINewGameMenuType>("UILoadGameMenu Option 2", UINewGameMenuType.FlipChess, () => { if (DebugOptions.ConsoleTrace) Console.WriteLine("假選項2被點擊"); }),
                new ButtonEntry<UINewGameMenuType>("UILoadGameMenu Option 3", UINewGameMenuType.FlipChess, () => { if (DebugOptions.ConsoleTrace) Console.WriteLine("假選項3被點擊"); }),
                new ButtonEntry<UINewGameMenuType>("UILoadGameMenu Option 4", UINewGameMenuType.FlipChess, () => { if (DebugOptions.ConsoleTrace) Console.WriteLine("假選項4被點擊"); }),
                new ButtonEntry<UINewGameMenuType>("UILoadGameMenu Option 5", UINewGameMenuType.FlipChess, () => { if (DebugOptions.ConsoleTrace) Console.WriteLine("假選項5被點擊"); }),
                new ButtonEntry<UINewGameMenuType>("UILoadGameMenu Option 6", UINewGameMenuType.FlipChess, () => { if (DebugOptions.ConsoleTrace) Console.WriteLine("假選項6被點擊"); }),
                new ButtonEntry<UINewGameMenuType>("UILoadGameMenu Option 7", UINewGameMenuType.FlipChess, () => { if (DebugOptions.ConsoleTrace) Console.WriteLine("假選項7被點擊"); }),
                new ButtonEntry<UINewGameMenuType>("UILoadGameMenu Option 8", UINewGameMenuType.FlipChess, () => { if (DebugOptions.ConsoleTrace) Console.WriteLine("假選項8被點擊"); }),
                new ButtonEntry<UINewGameMenuType>("UILoadGameMenu Option 9", UINewGameMenuType.FlipChess, () => { if (DebugOptions.ConsoleTrace) Console.WriteLine("假選項9被點擊"); }),
            };

            for (int i = 0; i < menuEntries.Count; i++)
            {
                var entry = menuEntries[i];
                var button = _factory.CreateElement<UIButton, UIButtonHandler, UIButtonRenderer>();

                button.Text = entry.Label;
                button.Handler.Action = () => Handler.StartNewGame();

                // Stacked by the scroll container's flex column (Gap = Button.Spacing).
                button.Size = UILayoutConstants.Submenu.Button.Size;
                button.LayoutRules.Apply(UILayoutSheet.Submenu.Button);

                ScrollContainer.AddChild(button);
                Buttons.Add(button);
            }
        }
    }
}

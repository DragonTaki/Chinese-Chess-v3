/* ----- ----- ----- ----- */
// UIMainMenu.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/15
// Update Date: 2026/10/05
// Version: v1.4
/* ----- ----- ----- ----- */

using Chinese_Chess_v3.Game.Application.MainMenu;
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
            // column (see UILayoutSheet.MainMenu).
            LayoutRules.Apply(UILayoutSheet.MainMenu.Screen);
            PanelWidth = UILayoutSheet.MainMenu.PanelWidth;
            ScrollContainer.LayoutRules.Apply(UILayoutSheet.MainMenu.ScrollContainer);
        }

        protected override void BuildButtons()
        {
            // Create the buttons
            var menuEntries = MainMenuOptions.Create(Handler.SwitchSubmenu);
            Vector2F btnStartPos = UILayoutConstants.MainMenu.Button.Position; // TODO: btnStartPos is unused; remove it or use it

            for (int i = 0; i < menuEntries.Count; i++)
            {
                var entry = menuEntries[i].ToButtonEntry();
                var button = _factory.CreateElement<UIButton, UIButtonHandler, UIButtonRenderer>();

                button.Text = entry.Label;
                button.Handler.Action = entry.OnClick;

                // Stacked by the scroll container's flex column (Gap = Button.Spacing).
                button.Size = UILayoutConstants.MainMenu.Button.Size;
                button.LayoutRules.Apply(UILayoutSheet.MainMenu.Button);

                ScrollContainer.AddChild(button);
                Buttons.Add(button);
            }
        }
    }
}

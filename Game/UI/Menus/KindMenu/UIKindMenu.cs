/* ----- ----- ----- ----- */
// UIKindMenu.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/16
// Update Date: 2026/10/12
// Version: v1.4
/* ----- ----- ----- ----- */

using Chinese_Chess_v3.Game.Application.MainMenu;
using Chinese_Chess_v3.Game.UI.Constants;

using Engine.Mathematics;
using Engine.UI.Core.Elements;
using Engine.UI.Core.Handlers;
using Engine.UI.Core.Interfaces;
using Engine.UI.Core.Renderers;

namespace Chinese_Chess_v3.Game.UI.Menus.KindMenu
{
    /// <summary>The flow selected by a game-kind submenu.</summary>
    public enum KindMenuPurpose
    {
        SinglePlayer,
        VsComputer,
    }

    public class UIKindMenu : UIMenu<UIKindMenu, UIKindMenuHandler, UIKindMenuRenderer>
    {
        public UIKindMenu() { }

        /// <summary>The submenu's flow; set before it is first shown.</summary>
        public KindMenuPurpose Purpose { get; set; }

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
            var menuEntries = NewGameOptions.Create(Handler.ChooseKind);

            for (int i = 0; i < menuEntries.Count; i++)
            {
                var entry = menuEntries[i].ToButtonEntry();
                var button = _factory.CreateElement<UIButton, UIButtonHandler, UIButtonRenderer>();

                button.Text = entry.Label;
                button.Handler.Action = entry.OnClick;

                // Stacked by the scroll container's flex column (Gap = Button.Spacing).
                button.Size = UILayoutConstants.Submenu.Button.Size;
                button.LayoutRules.Apply(UILayoutSheet.Submenu.Button);

                ScrollContainer.AddChild(button);
                Buttons.Add(button);
            }
        }
    }
}

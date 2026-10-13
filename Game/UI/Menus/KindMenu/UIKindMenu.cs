/* ----- ----- ----- ----- */
// UIKindMenu.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/16
// Update Date: 2026/10/13
// Version: v1.6
/* ----- ----- ----- ----- */

using Chinese_Chess_v3.Game.Application.MainMenu;
using Chinese_Chess_v3.Game.UI.Constants;
using Chinese_Chess_v3.Game.UI.Menus.CategoryListMenu;

using Engine.Mathematics;
using Engine.Platform;
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

    public partial class UIKindMenu : UIMenu<UIKindMenu, UIKindMenuHandler, UIKindMenuRenderer>
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
            foreach (var group in NewGameOptions.Create(Handler.ChooseKind))
            {
                // Stacked by the scroll container's flex column (Gap = Button.Spacing).
                var block = _factory.CreateElement<UIButtonRow, UIButtonRowHandler, UIButtonRowRenderer>();
                block.LayoutRules.Apply(UILayoutSheet.Submenu.Group);
                ScrollContainer.AddChild(block);

                var heading = _factory.CreateElement<UILabel, UILabelHandler, UILabelRenderer>();
                heading.Font = UILayoutStyles.CategoryListMenu.SectionHeading.Font;
                heading.ForeColor = UILayoutStyles.CategoryListMenu.SectionHeading.Color;
                heading.TextAlign = ContentAlign.MiddleLeft;
                heading.Text = UILayoutStyles.CategoryListMenu.SectionHeading.Format(group.Title);
                heading.LayoutRules.Apply(UILayoutSheet.Submenu.GroupHeading);
                block.AddChild(heading);

                // Left and right mode button; a blank right slot gets no element.
                var row = _factory.CreateElement<UIButtonRow, UIButtonRowHandler, UIButtonRowRenderer>();
                row.LayoutRules.Apply(UILayoutSheet.Submenu.GroupRow);
                block.AddChild(row);

                foreach (var entry in group.Entries)
                {
                    var button = _factory.CreateElement<UIButton, UIButtonHandler, UIButtonRenderer>();
                    button.Text = entry.Label;
                    button.Handler.Action = entry.Action;
                    button.Size = UILayoutConstants.Submenu.Button.Size;
                    button.LayoutRules.Apply(UILayoutSheet.Submenu.GroupButton);
                    row.AddChild(button);
                    Buttons.Add(button);
                }
            }
        }
    }
}

/* ----- ----- ----- ----- */
// UIGameMenu.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/17
// Update Date: 2025/10/22
// Version: v1.1
/* ----- ----- ----- ----- */

using Chinese_Chess_v3.Game.UI.Constants;
using Chinese_Chess_v3.Game.UI.Boards;
using Chinese_Chess_v3.Game.UI.Sidebars;

using Engine.Mathematics;
using Engine.UI.Core.Elements;
using Engine.UI.Core.Handlers;
using Engine.UI.Core.Interfaces;
using Engine.UI.Core.Renderers;

namespace Chinese_Chess_v3.Game.UI.Menus.GameMenu
{
    public class UIGameMenu : UIMenu<UIGameMenu, UIGameMenuHandler, UIGameMenuRenderer>, IResettable
    {
        internal UIBoard ChessBoard { get; private set; }
        internal UISidebar Sidebar { get; private set; }

        public UIGameMenu() { }

        protected override void OnBeforeInit(IUiFactory factory)
        {
            ButtonSpacing = UILayoutConstants.GameMenu.Button.Spacing;
        }

        protected override void OnInit(IUiFactory factory)
        {
            Layout = UILayoutConstants.GameMenu.Layout;
            ScrollContainer.Layout = UILayoutConstants.GameMenu.ScrollContainer.Layout;
        }

        protected override void BuildButtons()
        {
            var menuEntries = UIGameMenuOptions.Create(Handler.UIGameMenuAction);

            for (int i = 0; i < menuEntries.Count; i++)
            {
                var entry = menuEntries[i];
                var button = _factory.CreateElement<UIButton, UIButtonHandler, UIButtonRenderer>();

                button.Text = entry.Label;
                button.Handler.Action = () => Handler.UIGameMenuAction(entry.Type);

                button.LocalPosition = UILayoutConstants.GameMenu.Button.Position +
                    new Vector2F(0.0f, (UILayoutConstants.GameMenu.Button.Size.Y + ButtonSpacing) * i);
                button.Size = UILayoutConstants.GameMenu.Button.Size;

                ScrollContainer.AddChild(button);
                Buttons.Add(button);
            }
        }
        protected override void BuildUIObjects()
        {
            if (ChessBoard == null)
                ChessBoard = _factory.CreateDIElement<UIBoard, UIBoardHandler, UIBoardRenderer>();
            if (!Children.Contains(ChessBoard))
                AddChild(ChessBoard);

            if (Sidebar == null)
                Sidebar = _factory.CreateDIElement<UISidebar, UISidebarHandler, UISidebarRenderer>();
            if (!Children.Contains(Sidebar))
                AddChild(Sidebar);
        }
        public void ResetGameUI() => Reset();
        
        protected override void OnAfterReset()
        {
            Renderer?.Invalidate();
        }
    }
}

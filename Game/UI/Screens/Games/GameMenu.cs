/* ----- ----- ----- ----- */
// GameMenu.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/17
// Update Date: 2025/10/22
// Version: v1.1
/* ----- ----- ----- ----- */

using Chinese_Chess_v3.Game.Constants.UI;
using Chinese_Chess_v3.Game.Core;
using Chinese_Chess_v3.Game.UI.Screens.Games.Boards;
using Chinese_Chess_v3.Game.UI.Screens.Games.Options;
using Chinese_Chess_v3.Game.UI.Screens.Games.Sidebars;

using Engine.Mathematics;
using Engine.UI.Core.Elements;
using Engine.UI.Core.Handlers;
using Engine.UI.Core.Interfaces;
using Engine.UI.Core.Renderers;

namespace Chinese_Chess_v3.Game.UI.Screens.Games
{
    public class GameMenu : UIMenu<GameMenu, GameMenuHandler, GameMenuRenderer>
    {
        internal ChessBoard ChessBoard { get; private set; }
        internal Sidebar Sidebar { get; private set; }

        public GameMenu() { }

        protected override void BeforeInit(IUiFactory factory)
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
            var menuEntries = GameMenuOptions.Create(Handler.GameMenuAction);

            for (int i = 0; i < menuEntries.Count; i++)
            {
                var entry = menuEntries[i];
                var button = _factory.CreateElement<UIButton, UIButtonHandler, UIButtonRenderer>();

                button.Text = entry.Label;
                button.Handler.Action = () => Handler.GameMenuAction(entry.Type);

                button.LocalPosition = UILayoutConstants.GameMenu.Button.Position +
                    new Vector2F(0.0f, (UILayoutConstants.GameMenu.Button.Size.Y + ButtonSpacing) * i);
                button.Size = UILayoutConstants.GameMenu.Button.Size;

                ScrollContainer.AddChild(button);
                Buttons.Add(button);
            }
        }
        protected override void BuildUIObjects()
        {
            // 1. 重置資料層
            ChessBoard?.GameManager.ResetBoardToDefault();

            // 2. 重置 UI
            if (ChessBoard == null)
                ChessBoard = _factory.CreateDIElement<ChessBoard, ChessBoardHandler, ChessBoardRenderer>();
            if (!Children.Contains(ChessBoard))
                AddChild(ChessBoard);

            if (Sidebar == null)
                Sidebar = _factory.CreateDIElement<Sidebar, SidebarHandler, SidebarRenderer>();
            if (!Children.Contains(Sidebar))
                AddChild(Sidebar);
        }
        public void ResetGameUI() => BuildUIObjects();
    }
}

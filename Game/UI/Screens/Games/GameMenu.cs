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
using Engine.UI.Core.Interfaces;

namespace Chinese_Chess_v3.Game.UI.Screens.Games
{
    public class GameMenu : UIMenu<GameMenuHandler>, IScreen
    {
        internal ChessBoard ChessBoard { get; private set; }
        internal Sidebar Sidebar { get; private set; }

        public GameMenu() {}

        protected override void BuildButtons()
        {
            var menuEntries = GameMenuOptions.Create(_handler.GameMenuAction);

            for (int i = 0; i < menuEntries.Count; i++)
            {
                var entry = menuEntries[i];
                var button = new UIButton<GameMenuType>(entry);
                button.LocalPosition = UILayoutConstants.GameMenu.Button.Position +
                    new Vector2F(0.0f, (UILayoutConstants.GameMenu.Button.Size.Y + UILayoutConstants.GameMenu.Margin) * i);
                button.Size = UILayoutConstants.GameMenu.Button.Size;

                ScrollContainer.AddChild(button);
                Buttons.Add(button);
            }
        }
        protected override void BuildUIObjects()
        {
            // 1. 重置資料層
            GameManager.Instance.ResetBoardToDefault();

            // 2. 重置 UI
            ChessBoard?.Dispose();
            ChessBoard = _factory.CreateScreen<ChessBoard, ChessBoardHandler>();
            AddChild(ChessBoard);

            Sidebar?.Dispose();
            Sidebar = _factory.CreateScreen<Sidebar, SidebarHandler>();
            AddChild(Sidebar);
        }
        public void ResetGameUI() => BuildUIObjects();

        // 方法來安全清理
        public void DisposeChildren()
        {
            ChessBoard?.Dispose();
            ChessBoard = null;

            Sidebar?.Dispose();
            Sidebar = null;
        }
    }
}

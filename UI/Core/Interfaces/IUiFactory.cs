/* ----- ----- ----- ----- */
// IUiFactory.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/17
// Update Date: 2025/05/17
// Version: v1.0
/* ----- ----- ----- ----- */

using System;

using Chinese_Chess_v3.UI.Core.Elements;
using Chinese_Chess_v3.UI.Core.Infrastructure;
using Chinese_Chess_v3.UI.Screens.Games;
using Chinese_Chess_v3.UI.Screens.Games.Boards;
using Chinese_Chess_v3.UI.Screens.Games.Sidebars;
using Chinese_Chess_v3.UI.Screens.Games.Sidebars.InfoBoards;
using Chinese_Chess_v3.UI.Screens.Games.Sidebars.LoggerBoxes;
using Chinese_Chess_v3.UI.Screens.Menus;

namespace Chinese_Chess_v3.UI.Core.Interfaces
{
    public interface IUiFactory
    {
        UIScrollContainer CreateScrollContainer();
        MainMenu CreateMainMenu();
        GameMenu CreateGameMenu();
        ChessBoard CreateChessBoard();
        Sidebar CreateSidebar();
        InfoBoard CreateInfoBoard();
        LoggerBox CreateLoggerBox();

        void RegisterFactory<T>(Func<IUiFactoryContext, T> factory) where T : UIElement;
        T Create<T>() where T : UIElement;
        T Resolve<T>();
    
        //SubMenu CreateSubMenu();
        //Tooltip CreateTooltip(string text);
    }
}
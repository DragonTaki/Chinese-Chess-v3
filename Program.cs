/* ----- ----- ----- ----- */
// Program.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/06
// Update Date: 2025/05/06
// Version: v1.0
/* ----- ----- ----- ----- */

using System;
using System.Windows.Forms;

using Microsoft.Extensions.DependencyInjection;

using Chinese_Chess_v3.Core;
using Chinese_Chess_v3.UI;
using Chinese_Chess_v3.UI.Core.Elements;
using Chinese_Chess_v3.UI.Core.Infrastructure;
using Chinese_Chess_v3.UI.Core.Interfaces;
using Chinese_Chess_v3.UI.Input;
using Chinese_Chess_v3.UI.Screens.Games;
using Chinese_Chess_v3.UI.Screens.Games.Boards;
using Chinese_Chess_v3.UI.Screens.Games.Sidebars;
using Chinese_Chess_v3.UI.Screens.Games.Sidebars.InfoBoards;
using Chinese_Chess_v3.UI.Screens.Games.Sidebars.LoggerBoxes;
using Chinese_Chess_v3.UI.Screens.Menus;
using Chinese_Chess_v3.UI.Screens.Menus.Submenus;

using SharedLib.RandomTable;

namespace Chinese_Chess_v3
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            // Setting DI services
            var services = new ServiceCollection();

            // WinForms window
            services.AddSingleton<IUiFactory, UiFactory>();
            services.AddSingleton<IScrollInputHandler, ScrollInputHandler>();

            services.AddSingleton<MainForm>();

            services.AddSingleton<RandomTable>(new RandomTable(size: 10000, seed: 12345));

            services.AddSingleton<NavigationManager>();
            services.AddSingleton<UIRootNode>();
            services.AddSingleton<DialogManager>();
            services.AddSingleton<GameManager>();

            services.AddSingleton<MainMenu>();
            services.AddSingleton<MainMenuHandler>();
            services.AddSingleton<MainMenuRenderer>();
            services.AddTransient<NewGameMenu>();
            services.AddSingleton<NewGameMenuHandler>();
            services.AddSingleton<NewGameMenuRenderer>();
            services.AddTransient<LoadGameMenu>();
            services.AddSingleton<LoadGameMenuHandler>();
            services.AddSingleton<LoadGameMenuRenderer>();

            services.AddSingleton<GameMenu>();
            services.AddSingleton<GameMenuHandler>();
            services.AddSingleton<GameMenuRenderer>();

            services.AddSingleton<ChessBoard>();
            services.AddSingleton<ChessBoardHandler>();
            services.AddSingleton<ChessBoardRenderer>();

            services.AddSingleton<Sidebar>();
            services.AddSingleton<SidebarHandler>();
            services.AddSingleton<SidebarRenderer>();
            services.AddSingleton<InfoBoard>();
            services.AddSingleton<InfoBoardHandler>();
            services.AddSingleton<InfoBoardRenderer>();
            services.AddSingleton<LoggerBox>();
            services.AddSingleton<LoggerBoxHandler>();
            services.AddSingleton<LoggerBoxRenderer>();

            var sp = services.BuildServiceProvider();

            // Run WinForms
            ApplicationConfiguration.Initialize();
            Application.Run(sp.GetRequiredService<MainForm>());
        }
    }
}

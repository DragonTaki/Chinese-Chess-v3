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

using Chinese_Chess_v3.UI;
using Chinese_Chess_v3.UI.Core;
using Chinese_Chess_v3.UI.Input;
using Chinese_Chess_v3.UI.Screens;
using Chinese_Chess_v3.UI.Screens.Menu;
using Chinese_Chess_v3.UI.Screens.Menu.Submenus;

using SharedLib.RandomTable;
using Chinese_Chess_v3.UI.Screens.Game;

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
            services.AddSingleton<MainMenu>();
            services.AddSingleton<MainMenuHandler>();
            services.AddSingleton<MainMenuRenderer>();
            services.AddTransient<NewGameMenu>();
            services.AddSingleton<NewGameMenuHandler>();
            services.AddSingleton<NewGameMenuRenderer>();
            services.AddTransient<LoadGameMenu>();
            services.AddSingleton<LoadGameMenuHandler>();
            services.AddSingleton<LoadGameMenuRenderer>();

            services.AddTransient<GameMenu>();
            services.AddSingleton<GameMenuHandler>();
            services.AddSingleton<GameMenuRenderer>();

            var sp = services.BuildServiceProvider();

            // Run WinForms
            ApplicationConfiguration.Initialize();
            Application.Run(sp.GetRequiredService<MainForm>());
        }
    }
}
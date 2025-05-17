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
using Chinese_Chess_v3.UI.Screens.Menu;
using Chinese_Chess_v3.UI.Screens.Menu.Submenus;

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

            services.AddSingleton<MainMenu>();
            services.AddTransient<NewGameMenu>();
            services.AddTransient<LoadGameMenu>();

            var sp = services.BuildServiceProvider();

            // Run WinForms
            ApplicationConfiguration.Initialize();
            Application.Run(sp.GetRequiredService<MainForm>());
        }
    }
}
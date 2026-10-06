/* ----- ----- ----- ----- */
// UIGameControlsHandler.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/06
// Update Date: 2026/10/06
// Version: v1.0
/* ----- ----- ----- ----- */

using System;

using Chinese_Chess_v3.Game.Application.GameScreen;

using Engine.Diagnostics;
using Engine.UI.Core.Handlers;

using Microsoft.Extensions.DependencyInjection;

namespace Chinese_Chess_v3.Game.UI.Sidebars.GameControls
{
    /// <summary>Sends each game control button to the <see cref="GameScreenPresenter"/>'s controls.</summary>
    public class UIGameControlsHandler : UIContainerHandler<UIGameControls, UIGameControlsHandler, UIGameControlsRenderer>
    {
        public UIGameControlsHandler() { }

        /// <summary>The game screen's decisions (the <see cref="GameScreenPresenter"/> registered in DI).</summary>
        private GameScreenPresenter Presenter => _factory.ServiceProvider.GetRequiredService<GameScreenPresenter>();

        /// <summary>A control button was clicked.</summary>
        public void OnControlSelected(GameControlOption option)
        {
            if (DebugOptions.ConsoleTrace)
                Console.WriteLine($"UIGameControls: selected: {option}");

            var controls = Presenter.Controls;
            switch (option)
            {
                case GameControlOption.Restart:
                    controls.Restart();
                    break;
                case GameControlOption.Undo:
                    controls.UndoRound();
                    break;
                case GameControlOption.Resign:
                    controls.ResignSideToMove();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(option), option, "Unknown game control");
            }
        }
    }
}

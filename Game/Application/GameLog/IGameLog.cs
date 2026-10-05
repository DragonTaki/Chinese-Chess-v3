/* ----- ----- ----- ----- */
// IGameLog.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/09/30
// Update Date: 2026/10/05
// Version: v1.1
/* ----- ----- ----- ----- */

namespace Chinese_Chess_v3.Game.Application.GameLog
{
    /// <summary>
    /// Receives the finished game-log lines (<see cref="GameLogComposer"/>). Declared in the
    /// logic layer so it never references a UI type; the sidebar's log box implements it and is
    /// handed to <see cref="GameLogComposer.SetLog"/>.
    /// </summary>
    public interface IGameLog
    {
        void AddMessage(string message);
    }
}

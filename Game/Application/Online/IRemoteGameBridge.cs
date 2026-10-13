/* ----- ----- ----- ----- */
// IRemoteGameBridge.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/13
// Update Date: 2026/10/13
// Version: v1.0
/* ----- ----- ----- ----- */

namespace Chinese_Chess_v3.Game.Application.Online
{
    /// <summary>The game screen's commands for a remote game, including its final board.</summary>
    public interface IRemoteGameBridge
    {
        bool IsActive { get; }
        void Resign();
        void ShowRoom();
        void Leave();
    }
}

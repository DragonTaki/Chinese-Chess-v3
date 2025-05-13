/* ----- ----- ----- ----- */
// Player.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/10
// Update Date: 2025/05/10
// Version: v1.0
/* ----- ----- ----- ----- */

using System;

using Chinese_Chess_v3.Timing;

namespace Chinese_Chess_v3.Models
{
    /// <summary>
    /// Represents a player in the game, including their timer.
    /// </summary>
    public class Player
    {
        public PlayerSide Side { get; }
        public PlayerTimer Timer { get; }

        public Player(PlayerSide side, TimeSpan initialTime)
        {
            Side = side;
            Timer = new PlayerTimer(initialTime);
        }
    }
}

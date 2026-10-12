/* ----- ----- ----- ----- */
// OnlineState.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/12
// Update Date: 2026/10/12
// Version: v1.0
/* ----- ----- ----- ----- */

namespace Chinese_Chess_v3.Game.Application.Online
{
    /// <summary>Where the <see cref="OnlineSession"/> is.</summary>
    public enum OnlineState
    {
        /// <summary>Not logged in (no connection).</summary>
        LoggedOut,

        /// <summary>A login is running (the login screen's, or the automatic one with the remembered login).</summary>
        LoggingIn,

        /// <summary>Logged in, not in a room (the online menu / lobby).</summary>
        LoggedIn,

        /// <summary>Seated in a room waiting for its game.</summary>
        InRoom,

        /// <summary>Playing a game.</summary>
        InGame,

        /// <summary>The connection of a room or game was lost; logging in again automatically while the seat is held.</summary>
        Reconnecting,

        /// <summary>The connection was lost outside a room (or a reconnect gave up); <see cref="OnlineSession.Reconnect"/> tries again.</summary>
        ConnectionLost,
    }
}

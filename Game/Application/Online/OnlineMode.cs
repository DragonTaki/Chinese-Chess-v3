/* ----- ----- ----- ----- */
// OnlineMode.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/12
// Update Date: 2026/10/13
// Version: v1.2
/* ----- ----- ----- ----- */

namespace Chinese_Chess_v3.Game.Application.Online
{
    /// <summary>
    /// The online menu's modes, chosen after the game kind (ONLINE-PLAY §8.1, like JJ象棋). Only
    /// <see cref="CustomRoom"/> is wired so far; the others are listed with the normal look and do
    /// nothing when chosen (author §8.26).
    /// </summary>
    public enum OnlineMode
    {
        /// <summary>排名賽 (not wired yet).</summary>
        Ranked,

        /// <summary>測評 (not wired yet).</summary>
        Evaluation,

        /// <summary>擂台 (not wired yet).</summary>
        Arena,

        /// <summary>自訂房間: the lobby (room list, room number, create).</summary>
        CustomRoom,
    }

    /// <summary>The page shown inside the Multiplayer submenu panel.</summary>
    public enum OnlineMenuPage
    {
        /// <summary>The choice between the online service and account-free LAN play.</summary>
        Connection,

        /// <summary>The login form, also shown while the remembered login connects.</summary>
        Login,

        /// <summary>選擇玩法: the game kinds.</summary>
        GameKinds,

        /// <summary>選擇模式: the modes of the chosen kind.</summary>
        Modes,

        /// <summary>The custom rooms' lobby (<see cref="LobbyPresenter"/>).</summary>
        Lobby,

        /// <summary>LAN nickname, nearby rooms and the current waiting room.</summary>
        Lan,
    }

    /// <summary>Who moves first in a custom room (the server's names: Host, Guest, Random).</summary>
    public enum OnlineFirstMover
    {
        /// <summary>The server draws.</summary>
        Random,

        /// <summary>The host.</summary>
        Host,

        /// <summary>The other player.</summary>
        Guest,
    }
}

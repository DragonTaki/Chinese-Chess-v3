/* ----- ----- ----- ----- */
// PacketType.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/11/01
// Update Date: 2026/10/07
// Version: v1.1
/* ----- ----- ----- ----- */

namespace Chinese_Chess_v3.Network
{
    /// <summary>
    /// The chess protocol's packet kinds. On the wire a packet's type is the member's name
    /// (<see cref="PacketTypeNames"/>); the names must match the server's PacketType values.
    /// </summary>
    public enum PacketType
    {
        NotDefined,       // Unknown or missing type

        // Auth
        AuthRequest,      // Authentication request
        AuthResponse,     // Authentication response

        // Room
        RoomList,         // The open rooms
        CreateRoom,       // Create a room
        JoinRoom,         // Join a room
        LeaveRoom,        // Leave a room
        Ready,            // Ready (or not) to start
        RoomState,        // A room's seats and settings

        // Chess game
        StartGame,        // Marks the start of a game
        EndGame,          // Marks the end of a game
        GameAction,       // A game action (e.g. a move)
        GameUpdate,       // The game state after an action
        TimerSync,        // Timer synchronization

        // Chat
        Chat,             // Chat message

        // Other
        Server,           // Server message
        Heartbeat,        // Heartbeat
        Error,            // Error message
    }
}

/* ----- ----- ----- ----- */
// RoomPayloads.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/07
// Update Date: 2026/10/07
// Version: v1.0
/* ----- ----- ----- ----- */

using System.Collections.Generic;

namespace Chinese_Chess_v3.Network.Protocol
{
    // The room packets' payloads, as the server (room.go) sends and takes them. Strings that name
    // an option (kind, mode, timer mode, first mover, state) are the server's names; the constants
    // below are the ones open so far.

    /// <summary>The option names the server accepts so far.</summary>
    public static class RoomNames
    {
        /// <summary>Game kind: traditional Chinese chess (the only one open so far).</summary>
        public const string KindTraditional = "Traditional";

        /// <summary>Mode: custom room (自訂房間; the only one open so far).</summary>
        public const string ModeCustom = "Custom";

        /// <summary>Timer mode: count down (局時, 步時, 加秒).</summary>
        public const string TimerCountDown = "CountDown";

        /// <summary>Timer mode: count up (only measuring).</summary>
        public const string TimerCountUp = "CountUp";

        /// <summary>First mover: the host.</summary>
        public const string FirstMoverHost = "Host";

        /// <summary>First mover: the guest.</summary>
        public const string FirstMoverGuest = "Guest";

        /// <summary>First mover: drawn by the server.</summary>
        public const string FirstMoverRandom = "Random";

        /// <summary>Room state: waiting for players / ready marks.</summary>
        public const string StateWaiting = "Waiting";

        /// <summary>Room state: a game is running.</summary>
        public const string StatePlaying = "Playing";
    }

    /// <summary>A room's clocks (the server checks the limits: 1-600 minutes, 1-3600 step seconds, 0-600 increment seconds).</summary>
    public sealed record TimerSettingsDto
    {
        /// <summary><see cref="RoomNames.TimerCountDown"/> or <see cref="RoomNames.TimerCountUp"/>.</summary>
        public string Mode { get; init; }

        /// <summary>The game time per side (局時), minutes.</summary>
        public int TotalMinutes { get; init; }

        /// <summary>The time per move (步時), seconds (used when <see cref="StepTimer"/> is on).</summary>
        public int StepSeconds { get; init; }

        /// <summary>Whether the step time applies.</summary>
        public bool StepTimer { get; init; }

        /// <summary>The time added after each move (加秒), seconds.</summary>
        public int IncrementSeconds { get; init; }
    }

    /// <summary>What a room is created with (CreateRoom's data; also part of RoomState). The server refuses unknown fields here.</summary>
    public sealed record RoomSettingsDto
    {
        /// <summary>The game kind (<see cref="RoomNames.KindTraditional"/>).</summary>
        public string Kind { get; init; }

        /// <summary>The mode (<see cref="RoomNames.ModeCustom"/>).</summary>
        public string Mode { get; init; }

        /// <summary>The rule switches (the client's Rules property names, camelCase); null for the defaults.</summary>
        public Dictionary<string, bool> Rules { get; init; }

        /// <summary>The clocks.</summary>
        public TimerSettingsDto Timer { get; init; }

        /// <summary><see cref="RoomNames.FirstMoverHost"/>, <see cref="RoomNames.FirstMoverGuest"/> or <see cref="RoomNames.FirstMoverRandom"/>.</summary>
        public string FirstMover { get; init; }

        /// <summary>Whether pausing is allowed.</summary>
        public bool AllowPause { get; init; }
    }

    /// <summary>One taken seat (a player of a RoomState, or of a StartGame in turn order).</summary>
    public sealed record SeatDto
    {
        /// <summary>The account id.</summary>
        public string Id { get; init; }

        /// <summary>The player's display name.</summary>
        public string Name { get; init; }

        /// <summary>Whether the player is ready (RoomState only).</summary>
        public bool Ready { get; init; }
    }

    /// <summary>A room as the clients see it (RoomState; one entry of RoomList).</summary>
    public sealed record RoomStateDto
    {
        /// <summary>The room number (房號, six digits).</summary>
        public string RoomId { get; init; }

        /// <summary>The settings it was created with.</summary>
        public RoomSettingsDto Settings { get; init; }

        /// <summary>The host's account id.</summary>
        public string HostId { get; init; }

        /// <summary>The seats in seat order; a free seat is null.</summary>
        public List<SeatDto> Seats { get; init; }

        /// <summary><see cref="RoomNames.StateWaiting"/> or <see cref="RoomNames.StatePlaying"/>.</summary>
        public string State { get; init; }
    }

    /// <summary>JoinRoom's data.</summary>
    public sealed record JoinRoomDto
    {
        /// <summary>The room number.</summary>
        public string RoomId { get; init; }
    }

    /// <summary>Ready's data.</summary>
    public sealed record ReadyDto
    {
        /// <summary>Ready (true) or not.</summary>
        public bool Ready { get; init; }
    }
}

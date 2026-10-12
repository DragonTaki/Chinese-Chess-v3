/* ----- ----- ----- ----- */
// ReconnectPolicy.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/12
// Update Date: 2026/10/12
// Version: v1.0
/* ----- ----- ----- ----- */

using System;

namespace Chinese_Chess_v3.Game.Application.Online
{
    /// <summary>
    /// When <see cref="OnlineSession"/> logs in again after losing the connection of a room or a
    /// game: the first tries come quickly (0.5 s, 1 s, 2 s after the previous one), then every 2 s,
    /// for as long as the server holds the seat: 15 s in a game (ONLINE-PLAY §8.8, §8.13; a
    /// count-down step clock may end it sooner), 5 s in a waiting room (§8.28). Outside a room it
    /// does not retry by itself.
    /// </summary>
    public static class ReconnectPolicy
    {
        /// <summary>How long a seat in a running game is held (the server's DisconnectGraceSeat).</summary>
        public static readonly TimeSpan GameSeatGrace = TimeSpan.FromSeconds(15);

        /// <summary>How long a seat in a waiting room is held (the server's seatGrace).</summary>
        public static readonly TimeSpan RoomSeatGrace = TimeSpan.FromSeconds(5);

        private static readonly TimeSpan[] FirstDelays =
        {
            TimeSpan.FromMilliseconds(500), TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(2),
        };

        private static readonly TimeSpan LaterDelay = TimeSpan.FromSeconds(2);

        /// <summary>The wait before try number <paramref name="attempt"/> (0 is the first), counted from the previous try's end (or the disconnect).</summary>
        /// <param name="attempt">The try's number, from 0.</param>
        /// <returns>The wait.</returns>
        public static TimeSpan DelayBefore(int attempt)
        {
            ArgumentOutOfRangeException.ThrowIfNegative(attempt);
            return attempt < FirstDelays.Length ? FirstDelays[attempt] : LaterDelay;
        }

        /// <summary>How long to keep trying after a disconnect.</summary>
        /// <param name="inGame">Whether the connection was lost during a game (else in a waiting room).</param>
        /// <returns>The limit.</returns>
        public static TimeSpan Limit(bool inGame) => inGame ? GameSeatGrace : RoomSeatGrace;
    }
}

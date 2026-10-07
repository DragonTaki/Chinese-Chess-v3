/* ----- ----- ----- ----- */
// Packet.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/11/01
// Update Date: 2026/10/07
// Version: v1.1
/* ----- ----- ----- ----- */

using System;

using NetClient;

namespace Chinese_Chess_v3.Network
{
    /// <summary>
    /// A chess protocol packet: a Net-Client <see cref="Envelope"/> whose type is read as a
    /// <see cref="PacketType"/>.
    /// </summary>
    public class Packet
    {
        public PacketType Type { get; set; }
        public string SenderId { get; set; } = "";
        public string RoomId { get; set; } = "";
        public string Token { get; set; } = "";    // Used for authentication
        public string Data { get; set; } = "";

        public static Packet Create(PacketType type, string senderId, string roomId, string data)
        {
            return new Packet
            {
                Type = type,
                SenderId = senderId,
                RoomId = roomId,
                Data = data,
                Token = ""
            };
        }

        /// <summary>The packet read from a received envelope (an unknown type becomes <see cref="PacketType.NotDefined"/>).</summary>
        /// <param name="envelope">The received envelope.</param>
        /// <returns>The packet.</returns>
        public static Packet FromEnvelope(Envelope envelope)
        {
            ArgumentNullException.ThrowIfNull(envelope);
            return new Packet
            {
                Type = PacketTypeNames.Parse(envelope.Type),
                SenderId = envelope.SenderId,
                RoomId = envelope.RoomId,
                Data = envelope.Data,
                Token = envelope.Token,
            };
        }

        /// <summary>The envelope to send for this packet.</summary>
        /// <returns>The envelope.</returns>
        public Envelope ToEnvelope()
            => Envelope.Create(PacketTypeNames.ToName(Type), SenderId, RoomId, Data, Token);
    }
}

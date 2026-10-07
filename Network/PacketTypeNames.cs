/* ----- ----- ----- ----- */
// PacketTypeNames.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/07
// Update Date: 2026/10/07
// Version: v1.0
/* ----- ----- ----- ----- */

using System;
using System.Collections.Generic;

namespace Chinese_Chess_v3.Network
{
    /// <summary>Maps <see cref="PacketType"/> to and from the type name in a packet on the wire.</summary>
    public static class PacketTypeNames
    {
        private static readonly Dictionary<string, PacketType> ByName = BuildByName();

        /// <summary>The wire name of <paramref name="type"/> (the member's name, e.g. "Heartbeat").</summary>
        /// <param name="type">A packet kind.</param>
        /// <returns>The name.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="type"/> is not a defined member.</exception>
        public static string ToName(PacketType type)
        {
            if (!Enum.IsDefined(type))
                throw new ArgumentOutOfRangeException(nameof(type), type, "Not a defined packet type.");
            return type.ToString();
        }

        /// <summary>
        /// The packet kind a wire name stands for (exact, case-sensitive). Never throws: null,
        /// unknown names and numbers give <see cref="PacketType.NotDefined"/>.
        /// </summary>
        /// <param name="name">The type name from a received packet.</param>
        /// <returns>The packet kind, or <see cref="PacketType.NotDefined"/>.</returns>
        public static PacketType Parse(string name)
            => name != null && ByName.TryGetValue(name, out PacketType type) ? type : PacketType.NotDefined;

        private static Dictionary<string, PacketType> BuildByName()
        {
            var byName = new Dictionary<string, PacketType>(StringComparer.Ordinal);
            foreach (PacketType type in Enum.GetValues<PacketType>())
                byName[type.ToString()] = type;
            return byName;
        }
    }
}

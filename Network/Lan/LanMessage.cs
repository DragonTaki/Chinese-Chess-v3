/* ----- ----- ----- ----- */
// LanMessage.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/12
// Update Date: 2026/10/12
// Version: v1.0
/* ----- ----- ----- ----- */

using System;
using System.Net;
using System.Text.Json;

using Chinese_Chess_v3.Network.Protocol;

namespace Chinese_Chess_v3.Network.Lan
{
    /// <summary>A LAN message; the host accepts intents and publishes validated room and game state.</summary>
    public sealed record LanMessage
    {
        public string Type { get; init; }
        public JsonElement Data { get; init; }

        public static LanMessage Create<T>(string type, T data) => new()
        {
            Type = type,
            Data = JsonSerializer.SerializeToElement(data, ProtocolJson.Options),
        };

        public T Read<T>() => Data.Deserialize<T>(ProtocolJson.Options);
    }

    /// <summary>A discovered room with its address taken from the UDP sender.</summary>
    public sealed record LanHost
    {
        public string RoomId { get; init; }
        public string Nickname { get; init; }
        public IPEndPoint Endpoint { get; init; }
    }

    /// <summary>LAN protocol identity and fixed ports, independent of the online service.</summary>
    public static class LanProtocol
    {
        public const string Identity = "Chinese-Chess-v3.LAN.1";
        public const int DiscoveryPort = 18081;
        public const int FirstGamePort = 18082;
        public const int LastGamePort = 18089;

        public static string ValidateNickname(string nickname)
        {
            ArgumentNullException.ThrowIfNull(nickname);
            nickname = nickname.Trim();
            if (nickname.Length is < 1 or > 32 || nickname.AsSpan().IndexOfAny('\r', '\n') >= 0)
                throw new ArgumentException("A nickname must contain 1 to 32 characters on one line.", nameof(nickname));
            return nickname;
        }
    }
}

/* ----- ----- ----- ----- */
// ProtocolJson.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/07
// Update Date: 2026/10/07
// Version: v1.0
/* ----- ----- ----- ----- */

using System.Text.Json;
using System.Text.Json.Serialization;

namespace Chinese_Chess_v3.Network.Protocol
{
    /// <summary>
    /// The JSON settings of every packet payload (a packet's <c>data</c>): camelCase names, unknown
    /// fields ignored (the server may add fields before the client knows them), names matched
    /// case-insensitively on read, null members left out on write.
    /// </summary>
    public static class ProtocolJson
    {
        /// <summary>The shared options (do not modify).</summary>
        public static readonly JsonSerializerOptions Options = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            WriteIndented = false,
        };

        /// <summary>The payload as JSON text.</summary>
        /// <typeparam name="T">The payload's type.</typeparam>
        /// <param name="value">The payload.</param>
        /// <returns>The JSON text.</returns>
        public static string Serialize<T>(T value) => JsonSerializer.Serialize(value, Options);

        /// <summary>Parses a payload. Never throws: invalid JSON, a wrong shape or <c>null</c> give false.</summary>
        /// <typeparam name="T">The payload's type.</typeparam>
        /// <param name="json">The packet's data.</param>
        /// <param name="value">The payload, or default.</param>
        /// <returns>True when the data held a payload.</returns>
        public static bool TryDeserialize<T>(string json, out T value)
        {
            value = default;
            if (string.IsNullOrWhiteSpace(json))
                return false;
            try
            {
                value = JsonSerializer.Deserialize<T>(json, Options);
                return value != null;
            }
            catch (JsonException)
            {
                return false;
            }
        }
    }
}

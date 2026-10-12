/* ----- ----- ----- ----- */
// SessionTokenClaims.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/12
// Update Date: 2026/10/12
// Version: v1.0
/* ----- ----- ----- ----- */

using System;
using System.Text;
using System.Text.Json;

namespace Chinese_Chess_v3.Game.Application.Online
{
    /// <summary>
    /// Reads the account id out of the session token (a JWT whose payload holds <c>userID</c>) so
    /// the client can find its own seat in a room. Not a check of the token: the signature is the
    /// server's business (the client is untrusted, ANTI-CHEAT.md), and nothing is decided from
    /// this id but what to show.
    /// </summary>
    public static class SessionTokenClaims
    {
        /// <summary>The account id in <paramref name="sessionToken"/>'s payload.</summary>
        /// <param name="sessionToken">The session token from the login.</param>
        /// <param name="accountId">The id, or null when the result is false.</param>
        /// <returns>Whether the token held one. Never throws.</returns>
        public static bool TryReadAccountId(string sessionToken, out string accountId)
        {
            accountId = null;
            if (string.IsNullOrEmpty(sessionToken))
                return false;

            string[] parts = sessionToken.Split('.');
            if (parts.Length != 3)
                return false;

            try
            {
                string payload = parts[1].Replace('-', '+').Replace('_', '/');
                payload = payload.PadRight(payload.Length + (4 - payload.Length % 4) % 4, '=');
                using JsonDocument doc = JsonDocument.Parse(Encoding.UTF8.GetString(Convert.FromBase64String(payload)));
                if (doc.RootElement.ValueKind == JsonValueKind.Object
                    && doc.RootElement.TryGetProperty("userID", out JsonElement id)
                    && id.ValueKind == JsonValueKind.String
                    && !string.IsNullOrEmpty(id.GetString()))
                {
                    accountId = id.GetString();
                    return true;
                }
            }
            catch (FormatException)
            {
            }
            catch (JsonException)
            {
            }
            return false;
        }
    }
}

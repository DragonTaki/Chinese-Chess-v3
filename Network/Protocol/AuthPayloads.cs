/* ----- ----- ----- ----- */
// AuthPayloads.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/12
// Update Date: 2026/10/12
// Version: v1.0
/* ----- ----- ----- ----- */

namespace Chinese_Chess_v3.Network.Protocol
{
    // The account packets' payloads after login, as the server (auth.go) takes them. The login's own
    // data is built by AuthManager.

    /// <summary>Logout's data: the login token to revoke (empty when the login was not remembered).</summary>
    public sealed record LogoutDto
    {
        /// <summary>The login token the server revokes, or empty / null for none.</summary>
        public string RefreshToken { get; init; }
    }
}

/* ----- ----- ----- ----- */
// AuthResult.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/07
// Update Date: 2026/10/07
// Version: v1.0
/* ----- ----- ----- ----- */

namespace Chinese_Chess_v3.Network
{
    /// <summary>
    /// How a login ended: accepted with the session token, or rejected with a reason code (the
    /// server's, e.g. "VersionMismatch", "MissingCredentials", "InvalidCredentials", or one of
    /// the client-side codes below).
    /// </summary>
    public sealed class AuthResult
    {
        /// <summary>The connection closed (or was replaced by a reconnect) before the login finished.</summary>
        public const string ReasonDisconnected = "Disconnected";

        /// <summary>The server could not be reached.</summary>
        public const string ReasonConnectFailed = "ConnectFailed";

        /// <summary>A connection was already open (disconnect first).</summary>
        public const string ReasonAlreadyConnected = "AlreadyConnected";

        private AuthResult(bool ok, string reason, string token)
        {
            Ok = ok;
            Reason = reason;
            Token = token;
        }

        /// <summary>Whether the server accepted the login.</summary>
        public bool Ok { get; }

        /// <summary>Why the login failed (a reason code); empty when it succeeded.</summary>
        public string Reason { get; }

        /// <summary>The session token the server issued; empty when the login failed.</summary>
        public string Token { get; }

        /// <summary>An accepted login.</summary>
        /// <param name="token">The issued session token.</param>
        /// <returns>The result.</returns>
        public static AuthResult Success(string token) => new(true, "", token ?? "");

        /// <summary>A rejected or abandoned login.</summary>
        /// <param name="reason">The reason code.</param>
        /// <returns>The result.</returns>
        public static AuthResult Failure(string reason) => new(false, reason ?? "", "");

        /// <inheritdoc/>
        public override string ToString() => Ok ? "Ok" : $"Failed ({Reason})";
    }
}

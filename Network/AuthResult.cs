/* ----- ----- ----- ----- */
// AuthResult.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/07
// Update Date: 2026/10/12
// Version: v1.1
/* ----- ----- ----- ----- */

namespace Chinese_Chess_v3.Network
{
    /// <summary>
    /// How a login ended: accepted with the session token (and, for a remembered or token login,
    /// the login token to use next time), or rejected with a reason code (the server's or a
    /// client-side one, see the constants below).
    /// </summary>
    public sealed class AuthResult
    {
        // ----- The server's reasons (an AuthResponse's data) -----

        /// <summary>The client's version is not the server's (update the game).</summary>
        public const string ReasonVersionMismatch = "VersionMismatch";

        /// <summary>The email or password was empty.</summary>
        public const string ReasonMissingCredentials = "MissingCredentials";

        /// <summary>The email or password is wrong.</summary>
        public const string ReasonInvalidCredentials = "InvalidCredentials";

        /// <summary>The account is logged in on a connection from too far away.</summary>
        public const string ReasonAlreadyLoggedIn = "AlreadyLoggedIn";

        /// <summary>The login token is unknown, expired, revoked or already spent (forget it; log in with the password).</summary>
        public const string ReasonInvalidLoginToken = "InvalidLoginToken";

        /// <summary>
        /// Not a login result: the AuthResponse a logged-in connection gets when a new login of its
        /// account (from the same place) replaced it; the server closes the connection after it.
        /// </summary>
        public const string ReplacedByNewLogin = "ReplacedByNewLogin";

        // ----- Client-side reasons -----

        /// <summary>The connection closed (or was replaced by a reconnect) before the login finished.</summary>
        public const string ReasonDisconnected = "Disconnected";

        /// <summary>The server could not be reached.</summary>
        public const string ReasonConnectFailed = "ConnectFailed";

        /// <summary>A connection was already open (disconnect first).</summary>
        public const string ReasonAlreadyConnected = "AlreadyConnected";

        private AuthResult(bool ok, string reason, string token, string refreshToken)
        {
            Ok = ok;
            Reason = reason;
            Token = token;
            RefreshToken = refreshToken;
        }

        /// <summary>Whether the server accepted the login.</summary>
        public bool Ok { get; }

        /// <summary>Why the login failed (a reason code); empty when it succeeded.</summary>
        public string Reason { get; }

        /// <summary>The session token the server issued; empty when the login failed.</summary>
        public string Token { get; }

        /// <summary>
        /// The login token to log in with next time, or null: set after a password login that
        /// asked to be remembered, and after every token login (the replacement of the token just
        /// used, which is now spent).
        /// </summary>
        public string RefreshToken { get; }

        /// <summary>An accepted login.</summary>
        /// <param name="token">The issued session token.</param>
        /// <param name="refreshToken">The login token for next time, or null / empty for none.</param>
        /// <returns>The result.</returns>
        public static AuthResult Success(string token, string refreshToken = null)
            => new(true, "", token ?? "", string.IsNullOrEmpty(refreshToken) ? null : refreshToken);

        /// <summary>A rejected or abandoned login.</summary>
        /// <param name="reason">The reason code.</param>
        /// <returns>The result.</returns>
        public static AuthResult Failure(string reason) => new(false, reason ?? "", "", null);

        /// <inheritdoc/>
        public override string ToString() => Ok ? "Ok" : $"Failed ({Reason})";
    }
}

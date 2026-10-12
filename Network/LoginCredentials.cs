/* ----- ----- ----- ----- */
// LoginCredentials.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/12
// Update Date: 2026/10/12
// Version: v1.0
/* ----- ----- ----- ----- */

using System;

namespace Chinese_Chess_v3.Network
{
    /// <summary>
    /// What a login proves the account with (the login's second step, after the version check):
    /// the email and password (optionally asking the server to remember the login), or a login
    /// token (refresh token) a remembered login was given. A login token is single-use: every
    /// accepted token login returns its replacement (<see cref="AuthResult.RefreshToken"/>), and
    /// sending a spent one again makes the server revoke every token of the account, so a client
    /// never runs two logins with the same token.
    /// </summary>
    public sealed class LoginCredentials
    {
        private LoginCredentials(string email, string password, bool remember, string refreshToken)
        {
            Email = email;
            Password = password;
            Remember = remember;
            RefreshToken = refreshToken;
        }

        /// <summary>The account's email (sent as the username); null for a token login.</summary>
        public string Email { get; }

        /// <summary>The account's password; null for a token login.</summary>
        public string Password { get; }

        /// <summary>Whether a password login asks for a login token to log in with next time.</summary>
        public bool Remember { get; }

        /// <summary>The login token of a token login; null for a password login.</summary>
        public string RefreshToken { get; }

        /// <summary>Whether this is a token login.</summary>
        public bool IsToken => RefreshToken != null;

        /// <summary>A login with the email and password.</summary>
        /// <param name="email">The account's email.</param>
        /// <param name="password">The account's password.</param>
        /// <param name="remember">Whether the server should return a login token for next time.</param>
        /// <returns>The credentials.</returns>
        public static LoginCredentials WithPassword(string email, string password, bool remember = false)
        {
            ArgumentNullException.ThrowIfNull(email);
            ArgumentNullException.ThrowIfNull(password);
            return new LoginCredentials(email, password, remember, null);
        }

        /// <summary>A login with a login token.</summary>
        /// <param name="refreshToken">The login token (not empty).</param>
        /// <returns>The credentials.</returns>
        public static LoginCredentials WithToken(string refreshToken)
        {
            ArgumentException.ThrowIfNullOrEmpty(refreshToken);
            return new LoginCredentials(null, null, false, refreshToken);
        }

        /// <inheritdoc/>
        public override string ToString() => IsToken ? "token login" : $"password login ({Email}{(Remember ? ", remember" : "")})";
    }
}

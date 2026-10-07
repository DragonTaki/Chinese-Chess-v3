/* ----- ----- ----- ----- */
// ServerEndpoint.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/07
// Update Date: 2026/10/07
// Version: v1.0
/* ----- ----- ----- ----- */

using System;

using NetClient.Transport;

namespace Chinese_Chess_v3.Network
{
    /// <summary>
    /// Where the chess server is and how its certificate is trusted. Not a player setting
    /// (author 2026-10-07): the address is built in here, and only a developer overrides it,
    /// through environment variables (the VS Code launch sets them for the local dev server).
    /// </summary>
    public static class ServerEndpoint
    {
        // There is no production server yet: the built-in address is the server's default listen
        // address (Chinese-Chess-v3-Server main.go DefaultListenAddr).

        /// <summary>The built-in server host.</summary>
        public const string DefaultHost = "127.0.0.1";

        /// <summary>The built-in server port.</summary>
        public const int DefaultPort = 8080;

        /// <summary>Overrides the host.</summary>
        public const string HostVariable = "CHESS_SERVER_HOST";

        /// <summary>Overrides the port (1-65535).</summary>
        public const string PortVariable = "CHESS_SERVER_PORT";

        /// <summary>
        /// When set, the server's certificate is pinned to this SHA-256 fingerprint (hex, colons
        /// allowed: a self-signed dev certificate); when unset, the system's certificate
        /// authorities decide.
        /// </summary>
        public const string CertSha256Variable = "CHESS_SERVER_CERT_SHA256";

        /// <summary>The server host: <see cref="HostVariable"/>, else <see cref="DefaultHost"/>.</summary>
        public static string Host
        {
            get
            {
                string value = Environment.GetEnvironmentVariable(HostVariable);
                return string.IsNullOrWhiteSpace(value) ? DefaultHost : value.Trim();
            }
        }

        /// <summary>The server port: <see cref="PortVariable"/>, else <see cref="DefaultPort"/> (also when the variable is not a valid port).</summary>
        public static int Port
        {
            get
            {
                string value = Environment.GetEnvironmentVariable(PortVariable);
                if (string.IsNullOrWhiteSpace(value))
                    return DefaultPort;
                if (int.TryParse(value.Trim(), out int port) && port >= 1 && port <= 65535)
                    return port;
                Console.WriteLine($"[Network] {PortVariable}={value} is not a port; using {DefaultPort}.");
                return DefaultPort;
            }
        }

        /// <summary>
        /// How the server's certificate is checked: pinned to <see cref="CertSha256Variable"/> when
        /// it is set, else <see cref="TlsCertificateValidation.SystemTrust"/> (also when the
        /// variable is not a fingerprint: that never trusts more than the system does).
        /// </summary>
        public static TlsCertificateValidation Validation
        {
            get
            {
                string value = Environment.GetEnvironmentVariable(CertSha256Variable);
                if (string.IsNullOrWhiteSpace(value))
                    return TlsCertificateValidation.SystemTrust;
                if (TlsCertificateValidation.TryParseFingerprint(value, out _))
                    return TlsCertificateValidation.PinnedSha256(value);
                Console.WriteLine($"[Network] {CertSha256Variable} is not a SHA-256 fingerprint; using the system's certificate authorities.");
                return TlsCertificateValidation.SystemTrust;
            }
        }

        /// <summary>The transport to the server: TLS 1.3 to <see cref="Host"/>:<see cref="Port"/>, checked by <see cref="Validation"/>.</summary>
        /// <returns>A new transport (the variables are read now).</returns>
        public static IStreamFactory CreateTransport() => new TlsStreamFactory(Host, Port, Validation);
    }
}

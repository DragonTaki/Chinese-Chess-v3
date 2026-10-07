/* ----- ----- ----- ----- */
// ConsoleNetLog.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/07
// Update Date: 2026/10/07
// Version: v1.0
/* ----- ----- ----- ----- */

using System;

using Engine.Diagnostics;

using NetClient;

namespace Chinese_Chess_v3.Network
{
    /// <summary>
    /// Writes the connection layer's diagnostics to the console: routine progress only when
    /// <see cref="DebugOptions.ConsoleTrace"/> is on, problems always.
    /// </summary>
    public sealed class ConsoleNetLog : INetLog
    {
        /// <inheritdoc/>
        public void Write(NetLogLevel level, string message)
        {
            if (level == NetLogLevel.Trace && !DebugOptions.ConsoleTrace)
                return;
            Console.WriteLine("[Network] " + message);
        }
    }
}

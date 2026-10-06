/* ----- ----- ----- ----- */
// CoreLogBridge.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/06
// Update Date: 2026/10/06
// Version: v1.0
/* ----- ----- ----- ----- */

using Chinese_Chess_v3.Game.Core;

using Engine.Logging;

namespace Chinese_Chess_v3.Game.Application.Services
{
    /// <summary>
    /// Sends the rules layer's log lines (<see cref="CoreLog"/>) to the engine's logger
    /// (<see cref="AppLogger"/>), at the matching level - the game client's choice of sink.
    /// </summary>
    public static class CoreLogBridge
    {
        /// <summary>Makes <see cref="AppLogger"/> the <see cref="CoreLog.Sink"/>.</summary>
        public static void Attach() => CoreLog.Sink = (message, level) => AppLogger.Log(message, ToLogLevel(level));

        /// <summary>The <see cref="AppLogger"/> level of a <see cref="CoreLogLevel"/>.</summary>
        public static LogLevel ToLogLevel(CoreLogLevel level) => level switch
        {
            CoreLogLevel.Debug => LogLevel.DEBUG,
            CoreLogLevel.Info => LogLevel.INFO,
            CoreLogLevel.Warn => LogLevel.WARN,
            CoreLogLevel.Error => LogLevel.ERROR,
            _ => LogLevel.INFO,
        };
    }
}

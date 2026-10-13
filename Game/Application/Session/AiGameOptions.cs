/* ----- ----- ----- ----- */
// AiGameOptions.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/12
// Update Date: 2026/10/13
// Version: v1.1
/* ----- ----- ----- ----- */

using Chinese_Chess_v3.Game.Ai;
using Chinese_Chess_v3.Game.Configs;

namespace Chinese_Chess_v3.Game.Application.Session
{
    /// <summary>The choices fixed when one game against the computer begins.</summary>
    /// <param name="Level">The computer's search effort.</param>
    /// <param name="PlayerSide">Who moves first, expressed as the human's side.</param>
    /// <param name="OpeningStyle">The opponent's opening category; null selects normal engine play.</param>
    /// <param name="UseConfiguredTimer">Uses the mode settings instead of the legacy count-up default.</param>
    public sealed record AiGameOptions(EngineLevel Level, AiPlayerSide PlayerSide, string OpeningStyle = null, bool UseConfiguredTimer = false);
}

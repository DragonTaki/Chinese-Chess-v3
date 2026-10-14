/* ----- ----- ----- ----- */
// AiSetupTexts.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/12
// Update Date: 2026/10/14
// Version: v1.3
/* ----- ----- ----- ----- */

using System;

using Chinese_Chess_v3.Game.Configs;

using Engine.Localization;

namespace Chinese_Chess_v3.Game.Application.Texts
{
    /// <summary>Finished strings for the computer-game setup panel.</summary>
    public static class AiSetupTexts
    {
        public static string Enabled => Lang.Get("ai.setup.enabled");
        public static string Title => Lang.Get("ai.setup.title");
        public static string Difficulty => Lang.Get("ai.setup.difficulty");
        public static string FirstMover => Lang.Get("ai.setup.first_mover");
        public static string AssignOpening => Lang.Get("ai.setup.assign_opening");
        public static string Opening => Lang.Get("ai.setup.opening");
        public static string Start => Lang.Get("ai.setup.start");
        public static string Back => Lang.Get("ai.setup.back");
        public static string Reset => Lang.Get("ai.setup.reset");
        public static string Side(AiPlayerSide side) => side switch
        {
            AiPlayerSide.First => Lang.Get("ai.setup.side.human_first"),
            AiPlayerSide.Second => Lang.Get("ai.setup.side.computer_first"),
            AiPlayerSide.Random => Lang.Get("ai.setup.side.random"),
            _ => throw new ArgumentOutOfRangeException(nameof(side)),
        };
    }
}

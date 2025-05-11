/* ----- ----- ----- ----- */
// TimerSettings.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/07
// Update Date: 2025/05/07
// Version: v1.0
/* ----- ----- ----- ----- */

namespace SharedLib.Timing
{
    public static class TimerSettings
    {
        private const int GameAnimationFPS = 60;
        public const int GameAnimationInterval = 1000 / GameAnimationFPS;
        public const int RedPlayerTimerInterval = 1000;
        public const int BlackPlayerTimerInterval = 1000;
        
    }
}
/* ----- ----- ----- ----- */
// IScreen.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/18
// Update Date: 2025/05/18
// Version: v1.0
/* ----- ----- ----- ----- */

namespace Chinese_Chess_v3.UI.Core.Interfaces
{
    public interface IScreen
    {
        void OnEnter();     // 畫面被切換進來
        void OnExit();      // 畫面即將被移除
    }
}

/* ----- ----- ----- ----- */
// UIInitializer.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/17
// Update Date: 2025/05/17
// Version: v1.0
/* ----- ----- ----- ----- */

using System;

using Microsoft.Extensions.DependencyInjection;

using Chinese_Chess_v3.UI.Core;
using Chinese_Chess_v3.UI.Input;

namespace Chinese_Chess_v3.UI.Bootstrap
{
    public static class UIInitializer
    {
        /// <summary>
        /// Create UI root, MouseInputRouter, UIInputManager, and connect them together.
        /// </summary>
        public static UIInputManager Create(IServiceProvider sp, out UIElement rootUI)
        {
            // 1) 構建 UI 樹根
            rootUI = new RootUIElement();   // ← 你的自訂 Root 元件

            // 2) 解析 DI 服務
            var scroll = sp.GetRequiredService<IScrollInputHandler>();

            // 3) 建立 MouseInputRouter（傳 root 與 scroll）
            var mouseRouter = new MouseInputRouter(rootUI, scroll);

            // 4) 組裝 UIInputManager
            var inputMgr = new UIInputManager();
            inputMgr.RegisterHandler(mouseRouter); // 先吃 Router
            inputMgr.RegisterHandler(scroll);      // 再吃 Scroll (可選)

            return inputMgr;
        }
    }
}
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
            // 1) Build the root of the UI tree
            rootUI = new RootUIElement();   // <- your custom root element

            // 2) Resolve DI services
            var scroll = sp.GetRequiredService<IScrollInputHandler>();

            // 3) Create the MouseInputRouter (pass root and scroll)
            var mouseRouter = new MouseInputRouter(rootUI, scroll);

            // 4) Assemble the UIInputManager
            var inputMgr = new UIInputManager();
            inputMgr.RegisterHandler(mouseRouter); // Router handles input first
            inputMgr.RegisterHandler(scroll);      // then Scroll (optional)

            return inputMgr;
        }
    }
}
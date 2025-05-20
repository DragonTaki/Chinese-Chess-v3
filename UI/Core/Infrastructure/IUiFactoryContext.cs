/* ----- ----- ----- ----- */
// IUiFactoryContext.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/18
// Update Date: 2025/05/18
// Version: v1.0
/* ----- ----- ----- ----- */

using System;

using Chinese_Chess_v3.UI.Core.Interfaces;

namespace Chinese_Chess_v3.UI.Core.Infrastructure
{
    public interface IUiFactoryContext
    {
        IServiceProvider ServiceProvider { get; }
        IUiFactory UiFactory { get; }
    }
}

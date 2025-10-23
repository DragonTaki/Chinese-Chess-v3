/* ----- ----- ----- ----- */
// IUiContainer.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/10/23
// Update Date: 2025/10/23
// Version: v1.0
/* ----- ----- ----- ----- */

using System;

namespace Chinese_Chess_v3.UI.Core.Interfaces
{
    public interface IUiContainer
    {
        bool IsDisposed { get; }
        void Post(Action action);
    }
}

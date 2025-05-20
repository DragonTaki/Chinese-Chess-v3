/* ----- ----- ----- ----- */
// IInitializableOnce.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/20
// Update Date: 2025/05/20
// Version: v1.0
/* ----- ----- ----- ----- */

namespace Chinese_Chess_v3.UI.Core
{
    public interface IInitializableOnce<TArg>
    {
        bool IsInitialized { get; }
        void Init(TArg arg);
    }
}

/* ----- ----- ----- ----- */
// InitializableOnceBase.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/20
// Update Date: 2025/05/20
// Version: v1.0
/* ----- ----- ----- ----- */

namespace Chinese_Chess_v3.UI.Core
{
    public abstract class InitializableOnceBase<TArg> : IInitializableOnce<TArg>
    {
        public bool IsInitialized { get; private set; }

        public void Init(TArg arg)
        {
            if (IsInitialized) return;
            IsInitialized = true;
            OnInit(arg);
        }

        protected abstract void OnInit(TArg arg);
    }
}

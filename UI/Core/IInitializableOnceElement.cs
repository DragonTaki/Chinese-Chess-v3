/* ----- ----- ----- ----- */
// IInitializableOnceElement.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/20
// Update Date: 2025/05/20
// Version: v1.0
/* ----- ----- ----- ----- */

namespace Chinese_Chess_v3.UI.Core
{
    public abstract class IInitializableOnceElement<TArg> : UIElement, IInitializableOnce<TArg>
    {
        private bool _isInitialized = false;

        public bool IsInitialized => _isInitialized;

        public void Init(TArg arg)
        {
            if (_isInitialized) return;
            _isInitialized = true;
            OnInit(arg);
        }

        protected abstract void OnInit(TArg arg);
    }
}

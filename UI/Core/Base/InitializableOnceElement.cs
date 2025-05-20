/* ----- ----- ----- ----- */
// InitializableOnceElement.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/20
// Update Date: 2025/05/20
// Version: v1.0
/* ----- ----- ----- ----- */

using Chinese_Chess_v3.UI.Core.Elements;
using Chinese_Chess_v3.UI.Core.Interfaces;

namespace Chinese_Chess_v3.UI.Core.Base
{
    public abstract class InitializableOnceElement<TArg> : UIElement, IInitializableOnce<TArg>
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

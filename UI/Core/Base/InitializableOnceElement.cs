/* ----- ----- ----- ----- */
// InitializableOnceElement.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/20
// Update Date: 2025/05/20
// Version: v1.0
/* ----- ----- ----- ----- */

using System;
using Chinese_Chess_v3.UI.Core.Elements;
using Chinese_Chess_v3.UI.Core.Interfaces;

namespace Chinese_Chess_v3.UI.Core.Base
{
    public abstract class InitializableOnceElement<TArg> : UIElement, IInitializableOnce<TArg>, IDisposable
    {
        private bool _isInitialized = false;

        public bool IsInitialized => _isInitialized;

        private bool _disposed = false;

        public void Init(TArg arg)
        {
            if (_isInitialized) return;
            _isInitialized = true;
            OnInit(arg);
        }

        protected abstract void OnInit(TArg arg);
        
        public virtual void Dispose()
        {
            if (_disposed)
                return;

            // 釋放子元件（UIElement已有 RemoveAllChild）
            RemoveAllChild(includePersistent: true);

            // 清除內部狀態
            _disposed = true;
        }
    }
}

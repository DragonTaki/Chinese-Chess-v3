/* ----- ----- ----- ----- */
// UILoggerBoxRenderer.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/10/22
// Update Date: 2026/10/12
// Version: v1.1
/* ----- ----- ----- ----- */

using Engine.Platform;
using Engine.UI.Core.Renderers;

namespace Chinese_Chess_v3.Game.UI.Sidebars.LoggerBoxes
{
    public class UILoggerBoxRenderer : UITextBoxRenderer<UILoggerBox, UILoggerBoxHandler, UILoggerBoxRenderer>
    {
        public UILoggerBoxRenderer() : base() { }

        public override void OnRender(IGraphics g, UILoggerBox element)
        {
            element.AppendPendingRecords();
            base.OnRender(g, element);
        }
    }
}

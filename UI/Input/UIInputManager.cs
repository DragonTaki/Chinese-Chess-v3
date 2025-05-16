/* ----- ----- ----- ----- */
// UIInputManager.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/16
// Update Date: 2025/05/16
// Version: v1.0
/* ----- ----- ----- ----- */

using System.Collections.Generic;
using System.Windows.Forms;
using Chinese_Chess_v3.UI.Core;

namespace Chinese_Chess_v3.UI.Input
{
    public class UIInputManager
    {
        public MouseInputRouter MouseRouter { get; }
        private readonly List<IInputHandler> generalHandlers = new();

        public UIInputManager(UIElement root)
        {
            MouseRouter = new MouseInputRouter(root);
            RegisterHandler(MouseRouter); // Add mouse router itself to the general processor
            RegisterHandler(ScrollInputHandler.Instance);
        }

        public void RegisterHandler(IInputHandler handler)
        {
            if (!generalHandlers.Contains(handler))
                generalHandlers.Add(handler);
        }

        public void UnregisterHandler(IInputHandler handler)
        {
            generalHandlers.Remove(handler);
        }

        public void ProcessMouseDown(object sender, MouseEventArgs e) => MouseRouter.OnMouseDown(e);
        public void ProcessMouseMove(object sender, MouseEventArgs e) => MouseRouter.OnMouseMove(e);
        public void ProcessMouseUp(object sender, MouseEventArgs e) => MouseRouter.OnMouseUp(e);
        public void ProcessMouseClick(object sender, MouseEventArgs e) => MouseRouter.OnMouseClick(e);
        public void ProcessMouseWheel(object sender, MouseEventArgs e) => MouseRouter.OnMouseWheel(e);

        public void EndFrame()
        {
            MouseRouter.EndFrame();
            foreach (var h in generalHandlers)
                h.EndFrame();
        }
    }
}

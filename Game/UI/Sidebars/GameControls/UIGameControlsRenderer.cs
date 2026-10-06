/* ----- ----- ----- ----- */
// UIGameControlsRenderer.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/06
// Update Date: 2026/10/06
// Version: v1.0
/* ----- ----- ----- ----- */

using Engine.Platform;
using Engine.Styles;
using Engine.UI.Core.Elements;
using Engine.UI.Core.Renderers;

namespace Chinese_Chess_v3.Game.UI.Sidebars.GameControls
{
    /// <summary>
    /// Draws the game control buttons through their <see cref="IButtonDrawStyle"/> (buttons draw
    /// nothing themselves; a menu draws its own the same way).
    /// </summary>
    public class UIGameControlsRenderer : UIContainerRenderer<UIGameControls, UIGameControlsHandler, UIGameControlsRenderer>
    {
        public UIGameControlsRenderer() : base() { }

        public override void OnRender(IGraphics g, UIGameControls element)
        {
            base.OnRender(g, element);
            foreach (var child in element.Children)
            {
                if (child is UIButton button && button.IsVisible)
                {
                    IButtonDrawStyle style = button.Style ?? DefaultStyles.DefaultButtonStyle;
                    style.Draw(g, button.Text, button.GetCurrentAbsolutePosition(), button.Size);
                }
            }
        }
    }
}

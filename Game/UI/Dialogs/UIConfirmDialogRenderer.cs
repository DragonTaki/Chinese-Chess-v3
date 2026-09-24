/* ----- ----- ----- ----- */
// UIConfirmDialogRenderer.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/19
// Update Date: 2026/09/24
// Version: v2.0
/* ----- ----- ----- ----- */

using System.Linq;

using Chinese_Chess_v3.Game.UI.Constants;

using Engine.Platform;
using Engine.Styles;
using Engine.UI.Core.Elements;

namespace Chinese_Chess_v3.Game.UI.Dialogs
{
    public class UIConfirmDialogRenderer
    {
        public void Draw(IGraphics g, UIConfirmDialog dialog)
        {
            var bounds = dialog.GetCurrentAbsoluteBounds();

            IBoxDrawStyle style = UILayoutStyles.Overlay.Dialog.Style;
            style.Draw(g, bounds);

            foreach (var child in dialog.Children.OfType<UIButton<ConfirmDialogResult>>())
            {
                DrawButton(g, child);
            }
        }

        private void DrawButton(IGraphics g, UIButton<ConfirmDialogResult> button)
        {
            IButtonDrawStyle style = UILayoutStyles.Overlay.Dialog.Button.Style;
            style.Draw(g, button.Text, button.GetCurrentAbsoluteBounds());
        }
    }
}

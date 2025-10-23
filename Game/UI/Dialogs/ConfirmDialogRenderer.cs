/* ----- ----- ----- ----- */
// ConfirmDialogRenderer.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/19
// Update Date: 2025/05/19
// Version: v1.0
/* ----- ----- ----- ----- */

using System.Drawing;
using System.Linq;

using Chinese_Chess_v3.Game.Constants.UI;

using Engine.Styles;
using Engine.UI.Core.Elements;
using Engine.UI.Dialogs;

namespace Chinese_Chess_v3.Game.UI.Dialogs
{
    public class ConfirmDialogRenderer
    {
        public void Draw(Graphics g, ConfirmDialog dialog)
        {
            var bounds = dialog.GetCurrentAbsoluteBounds();
            
            IBoxDrawStyle style = UILayoutStyles.Overlay.Dialog.Style;
            style.Draw(g, bounds);
            
            foreach (var child in dialog.Children.OfType<UIButton<ConfirmDialogResult>>())
            {
                DrawButton(g, child);
            }
        }
        
        private void DrawButton(Graphics g, UIButton<ConfirmDialogResult> button)
        {
            IButtonDrawStyle style = UILayoutStyles.Overlay.Dialog.Button.Style;
            style.Draw(g, button.Text, button.GetCurrentAbsoluteBounds());
        }
    }
}

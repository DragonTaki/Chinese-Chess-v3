/* ----- ----- ----- ----- */
// UIGameControls.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/06
// Update Date: 2026/10/06
// Version: v1.0
/* ----- ----- ----- ----- */

using Chinese_Chess_v3.Game.Application.GameScreen;
using Chinese_Chess_v3.Game.UI.Constants;

using Engine.UI.Core.Elements;
using Engine.UI.Core.Handlers;
using Engine.UI.Core.Interfaces;
using Engine.UI.Core.Renderers;

namespace Chinese_Chess_v3.Game.UI.Sidebars.GameControls
{
    /// <summary>
    /// The game controls in the sidebar, between the info board and the logger box (author
    /// 2026-10-05): one button per <see cref="GameControlOptions"/> entry (重新開始, 撤銷上步,
    /// 放棄對局), two per row. The buttons call the <see cref="GameScreenPresenter"/>'s controls
    /// (<see cref="UIGameControlsHandler"/>); <see cref="UIGameControlsRenderer"/> draws them.
    /// </summary>
    public class UIGameControls : UIContainer<UIGameControls, UIGameControlsHandler, UIGameControlsRenderer>
    {
        public UIGameControls() { }

        protected override void OnInit(IUiFactory factory)
        {
            LayoutRules.Apply(UILayoutSheet.GameScreen.GameControls);
        }

        protected override void BuildUIObjects()
        {
            if (Children.Count > 0)
                return;

            foreach (var entry in GameControlOptions.Create(Handler.OnControlSelected))
            {
                var button = _factory.CreateElement<UIButton, UIButtonHandler, UIButtonRenderer>();
                button.Text = entry.Label;
                button.Handler.Action = entry.Action;
                button.Style = UILayoutStyles.GameControls.Button.Style;
                button.Size = new Engine.Mathematics.Vector2F(
                    UILayoutConstants.Sidebar.GameControls.ButtonWidth, UILayoutConstants.Sidebar.GameControls.ButtonHeight);
                button.LayoutRules.Apply(UILayoutSheet.GameScreen.GameControlButton);
                AddChild(button);
            }
        }
    }
}

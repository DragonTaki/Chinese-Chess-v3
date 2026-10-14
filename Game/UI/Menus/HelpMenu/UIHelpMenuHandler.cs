/* ----- ----- ----- ----- */
// UIHelpMenuHandler.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/14
// Update Date: 2026/10/14
// Version: v1.0
/* ----- ----- ----- ----- */

using Chinese_Chess_v3.Game.Application.MainMenu;

using Engine.UI.Core.Handlers;
using Engine.UI.Core.Interfaces;

namespace Chinese_Chess_v3.Game.UI.Menus.HelpMenu
{
    public class UIHelpMenuHandler : UIMenuHandler<UIHelpMenu, UIHelpMenuHandler, UIHelpMenuRenderer>, IScreen
    {
        public HelpMenuPresenter Presenter { get; }

        public UIHelpMenuHandler(HelpMenuPresenter presenter) => Presenter = presenter;

        protected override void OnInit(IUiFactory factory)
        {
            Presenter.Changed += Element.RefreshPage;
        }

        public void OnEnter() => Presenter.Open();
        public void OnExit() { }
    }
}

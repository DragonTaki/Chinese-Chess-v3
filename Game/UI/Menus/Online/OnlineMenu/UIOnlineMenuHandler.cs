/* ----- ----- ----- ----- */
// UIOnlineMenuHandler.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/12
// Update Date: 2026/10/12
// Version: v1.0
/* ----- ----- ----- ----- */

using Chinese_Chess_v3.Game.Application.Online;

using Engine.UI.Core.Handlers;
using Engine.UI.Core.Interfaces;

using Microsoft.Extensions.DependencyInjection;

namespace Chinese_Chess_v3.Game.UI.Menus.Online.OnlineMenu
{
    /// <summary>
    /// Binds the <see cref="UIOnlineMenu"/> to the <see cref="OnlineMenuPresenter"/>, which makes
    /// the decisions (the pages, 登出, 重新連線, back): the buttons call its commands, the screen
    /// shows the page on <see cref="OnlineMenuPresenter.PageChanged"/> and redraws on
    /// <see cref="OnlineMenuPresenter.Changed"/>.
    /// <see cref="IScreen"/>: shown / left → <see cref="OnlineMenuPresenter.SetShown"/> (the lobby
    /// refreshes itself only while it is on display).
    /// </summary>
    public partial class UIOnlineMenuHandler : UIMenuHandler<UIOnlineMenu, UIOnlineMenuHandler, UIOnlineMenuRenderer>, IScreen
    {
        public UIOnlineMenuHandler() { }

        /// <summary>The online menu's decisions (the <see cref="OnlineMenuPresenter"/> registered in DI).</summary>
        internal OnlineMenuPresenter Presenter => _factory.ServiceProvider.GetRequiredService<OnlineMenuPresenter>();

        /// <summary>Subscribes to the presenters once: the screen (and this handler) is a single long-lived instance, like them.</summary>
        protected override void OnInit(IUiFactory factory)
        {
            var presenter = Presenter;
            presenter.Changed += Refresh;
            presenter.PageChanged += page =>
            {
                Element?.ShowPage(page);
                Refresh();
            };
            InitLobby();
        }

        /// <summary>重新連線.</summary>
        public void Reconnect() => Presenter.Reconnect();

        /// <summary>登出.</summary>
        public void Logout() => Presenter.Logout();

        /// <summary>返回: the previous page, or the main menu from the game kinds.</summary>
        public void Back() => Presenter.Back();

        public void OnEnter()
        {
            var presenter = Presenter;
            presenter.SetShown(true);
            Element.ShowPage(presenter.Page);
            Refresh();
        }

        public void OnExit() => Presenter.SetShown(false);

        /// <summary>Shows the presenter's state: the account, 重新連線, the error, and the lobby page's.</summary>
        private void Refresh()
        {
            var menu = Element;
            if (menu?.AccountLine == null)
                return;

            var presenter = Presenter;
            UIOnlineMenu.SetLine(menu.AccountLine, presenter.AccountText);
            UIOnlineMenu.SetShown(menu.ReconnectButton, presenter.CanReconnect);
            UIOnlineMenu.SetEnabled(menu.ReconnectButton, !presenter.IsBusy);
            UIOnlineMenu.SetLine(menu.ErrorLine, presenter.ErrorSentence);
            RefreshLobby();
            menu.RefreshLayout();
        }

        /// <summary>Subscribes to the lobby (nothing yet).</summary>
        partial void InitLobby();

        /// <summary>Shows the lobby's state (nothing yet).</summary>
        partial void RefreshLobby();
    }
}

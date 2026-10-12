/* ----- ----- ----- ----- */
// UIOnlinePanelHandler.cs
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

namespace Chinese_Chess_v3.Game.UI.Menus.Online.Panel
{
    /// <summary>
    /// Binds the <see cref="UIOnlinePanel"/> to the <see cref="OnlineMenuPresenter"/>, which makes
    /// the decisions (the pages, logout, reconnect, back): the buttons call its commands, the screen
    /// shows the page on <see cref="OnlineMenuPresenter.PageChanged"/> and redraws on
    /// <see cref="OnlineMenuPresenter.Changed"/>.
    /// <see cref="IScreen"/>: shown / left → <see cref="OnlineMenuPresenter.SetShown"/> (the lobby
    /// refreshes itself only while it is on display).
    /// </summary>
    public partial class UIOnlinePanelHandler : UIMenuHandler<UIOnlinePanel, UIOnlinePanelHandler, UIOnlinePanelRenderer>, IScreen
    {
        public UIOnlinePanelHandler() { }

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
                if (page == OnlineMenuPage.Login)
                    Login.Shown();
                Refresh();
            };
            InitLobby();
            InitLogin();
        }

        /// <summary>reconnect.</summary>
        public void Reconnect() => Presenter.Reconnect();

        /// <summary>logout.</summary>
        public void Logout() => Presenter.Logout();

        /// <summary>back: the previous page, or the main menu from the game kinds.</summary>
        public void Back()
        {
            if (Presenter.Page == OnlineMenuPage.Login)
                Login.Back();
            else
                Presenter.Back();
        }

        public void OnEnter()
        {
            var presenter = Presenter;
            presenter.SetShown(true);
            Element.ShowPage(presenter.Page);
            if (presenter.Page == OnlineMenuPage.Login)
                Login.Shown();
            Refresh();
        }

        public void OnExit() => Presenter.SetShown(false);

        /// <summary>Shows the presenter's state: the account, reconnect, the error, and the lobby page's.</summary>
        private void Refresh()
        {
            var menu = Element;
            if (menu?.AccountLine == null)
                return;

            var presenter = Presenter;
            UIOnlinePanel.SetLine(menu.AccountLine, presenter.AccountText);
            UIOnlinePanel.SetShown(menu.ReconnectButton, presenter.CanReconnect);
            UIOnlinePanel.SetEnabled(menu.ReconnectButton, !presenter.IsBusy);
            // On the lobby page the lobby's own error line may already show the same sentence.
            string error = presenter.ErrorSentence;
            if (presenter.Page == OnlineMenuPage.Lobby && error == Lobby.ErrorSentence)
                error = null;
            UIOnlinePanel.SetLine(menu.ErrorLine, error);
            RefreshLogin();
            RefreshLobby();
            menu.RefreshLayout();
        }

        /// <summary>Subscribes to the lobby (<c>UIOnlinePanelHandler.Lobby.cs</c>).</summary>
        partial void InitLobby();

        /// <summary>Shows the lobby's state (<c>UIOnlinePanelHandler.Lobby.cs</c>).</summary>
        partial void RefreshLobby();

        /// <summary>Subscribes to the login presenter.</summary>
        partial void InitLogin();

        /// <summary>Refreshes the login fields and state.</summary>
        partial void RefreshLogin();
    }
}

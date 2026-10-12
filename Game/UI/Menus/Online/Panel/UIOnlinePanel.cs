/* ----- ----- ----- ----- */
// UIOnlinePanel.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/12
// Update Date: 2026/10/12
// Version: v1.0
/* ----- ----- ----- ----- */

using Chinese_Chess_v3.Game.Application.Online;
using Chinese_Chess_v3.Game.UI.Constants;

using Engine.UI.Core.Elements;
using Engine.UI.Core.Interfaces;

namespace Chinese_Chess_v3.Game.UI.Menus.Online.Panel
{
    /// <summary>
    /// The Multiplayer submenu panel: the logged-in account's line, reconnect (only after the connection was lost
    /// here), logout and back, the error line, then one page at a time (<see cref="OnlineMenuPage"/>):
    /// login, kinds, modes, or the custom rooms' lobby. Every game kind and mode button has
    /// the normal look, wired or not (author §8.26). The handler (<see cref="UIOnlinePanelHandler"/>)
    /// binds it to the <c>OnlineMenuPresenter</c> (and the lobby page to the <c>LobbyPresenter</c>).
    /// </summary>
    public partial class UIOnlinePanel : UIOnlineScreen<UIOnlinePanel, UIOnlinePanelHandler, UIOnlinePanelRenderer>
    {
        /// <summary>Account (the logged-in account).</summary>
        internal UILabel AccountLine { get; private set; }

        /// <summary>reconnect: shown only while the presenter offers it.</summary>
        internal UIButton ReconnectButton { get; private set; }

        /// <summary>The last error (e.g. the connection was lost).</summary>
        internal UILabel ErrorLine { get; private set; }

        /// <summary>choose kind: the game kinds.</summary>
        internal UIOnlineGroup KindsPage { get; private set; }

        /// <summary>choose mode: the modes.</summary>
        internal UIOnlineGroup ModesPage { get; private set; }

        /// <summary>The custom rooms' lobby.</summary>
        internal UIOnlineGroup LobbyPage { get; private set; }

        /// <summary>The login form, including the remembered login's busy state.</summary>
        internal UIOnlineGroup LoginPage { get; private set; }

        /// <summary>The account and navigation shown only after login.</summary>
        private UIOnlineGroup _onlineNavigation;

        public UIOnlinePanel() { }

        protected override void OnInit(IUiFactory factory)
        {
            Layout = UILayoutConstants.Submenu.Layout;
            ScrollContainer.Layout = UILayoutConstants.Submenu.ScrollContainer.Layout;
            LayoutRules.Apply(UILayoutSheet.Submenu.Panel);
            ScrollContainer.LayoutRules.Apply(UILayoutSheet.OnlineMenu.PanelScrollContainer);
        }

        protected override void BuildButtons()
        {
            var presenter = Handler.Presenter;

            LoginPage = AddGroup(ScrollContainer, UILayoutSheet.OnlineMenu.Group);
            BuildLogin(LoginPage);

            _onlineNavigation = AddGroup(ScrollContainer, UILayoutSheet.OnlineMenu.Group);
            AccountLine = AddLine(_onlineNavigation, UILayoutStyles.OnlineMenu.LineColor);

            var navigation = AddGroup(_onlineNavigation, UILayoutSheet.OnlineMenu.ButtonRow);
            ReconnectButton = AddButton(navigation, UILayoutSheet.OnlineMenu.Button, presenter.ReconnectText, Handler.Reconnect);
            AddButton(navigation, UILayoutSheet.OnlineMenu.Button, presenter.LogoutText, Handler.Logout);
            AddButton(navigation, UILayoutSheet.OnlineMenu.Button, presenter.BackText, Handler.Back);

            ErrorLine = AddLine(_onlineNavigation, UILayoutStyles.OnlineMenu.ErrorColor);

            KindsPage = AddGroup(ScrollContainer, UILayoutSheet.OnlineMenu.Group);
            AddHeader(KindsPage, presenter.ChooseKindTitle);
            var kinds = AddGroup(KindsPage, UILayoutSheet.OnlineMenu.Choices);
            foreach (var entry in presenter.GameKinds())
                AddButton(kinds, UILayoutSheet.Submenu.Button, entry.Label, entry.Action);

            ModesPage = AddGroup(ScrollContainer, UILayoutSheet.OnlineMenu.Group);
            AddHeader(ModesPage, presenter.ChooseModeTitle);
            var modes = AddGroup(ModesPage, UILayoutSheet.OnlineMenu.Choices);
            foreach (var entry in presenter.Modes())
                AddButton(modes, UILayoutSheet.Submenu.Button, entry.Label, entry.Action);

            LobbyPage = AddGroup(ScrollContainer, UILayoutSheet.OnlineMenu.Group);
            BuildLobby(LobbyPage);

            ShowPage(presenter.Page);
        }

        /// <summary>Shows <paramref name="page"/> only.</summary>
        public void ShowPage(OnlineMenuPage page)
        {
            SetShown(LoginPage, page == OnlineMenuPage.Login);
            SetShown(_onlineNavigation, page != OnlineMenuPage.Login);
            SetShown(KindsPage, page == OnlineMenuPage.GameKinds);
            SetShown(ModesPage, page == OnlineMenuPage.Modes);
            SetShown(LobbyPage, page == OnlineMenuPage.Lobby);
            RefreshLayout();
        }

        /// <summary>Builds the lobby page's blocks into <paramref name="page"/> (<c>UIOnlinePanel.Lobby.cs</c>).</summary>
        partial void BuildLobby(UIOnlineGroup page);

        /// <summary>Builds the login form inside the same panel.</summary>
        partial void BuildLogin(UIOnlineGroup page);
    }
}

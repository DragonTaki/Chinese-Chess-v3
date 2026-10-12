/* ----- ----- ----- ----- */
// UIOnlineMenu.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/12
// Update Date: 2026/10/12
// Version: v1.0
/* ----- ----- ----- ----- */

using Chinese_Chess_v3.Game.Application.Online;
using Chinese_Chess_v3.Game.UI.Constants;

using Engine.UI.Core.Elements;

namespace Chinese_Chess_v3.Game.UI.Menus.Online.OnlineMenu
{
    /// <summary>
    /// 多人連線's menu: the logged-in account's line, 重新連線 (only after the connection was lost
    /// here), 登出 and 返回, the error line, then one page at a time (<see cref="OnlineMenuPage"/>):
    /// the game kinds, the modes, or the custom rooms' lobby. Every game kind and mode button has
    /// the normal look, wired or not (author §8.26). The handler (<see cref="UIOnlineMenuHandler"/>)
    /// binds it to the <c>OnlineMenuPresenter</c> (and the lobby page to the <c>LobbyPresenter</c>).
    /// </summary>
    public partial class UIOnlineMenu : UIOnlineScreen<UIOnlineMenu, UIOnlineMenuHandler, UIOnlineMenuRenderer>
    {
        /// <summary>帳號：… (the logged-in account).</summary>
        internal UILabel AccountLine { get; private set; }

        /// <summary>重新連線: shown only while the presenter offers it.</summary>
        internal UIButton ReconnectButton { get; private set; }

        /// <summary>The last error (e.g. the connection was lost).</summary>
        internal UILabel ErrorLine { get; private set; }

        /// <summary>選擇玩法: the game kinds.</summary>
        internal UIOnlineGroup KindsPage { get; private set; }

        /// <summary>選擇模式: the modes.</summary>
        internal UIOnlineGroup ModesPage { get; private set; }

        /// <summary>The custom rooms' lobby.</summary>
        internal UIOnlineGroup LobbyPage { get; private set; }

        public UIOnlineMenu() { }

        protected override void BuildButtons()
        {
            var presenter = Handler.Presenter;

            AccountLine = AddLine(ScrollContainer, UILayoutStyles.OnlineMenu.LineColor);

            var navigation = AddGroup(ScrollContainer, UILayoutSheet.OnlineMenu.ButtonRow);
            ReconnectButton = AddButton(navigation, UILayoutSheet.OnlineMenu.Button, presenter.ReconnectText, Handler.Reconnect);
            AddButton(navigation, UILayoutSheet.OnlineMenu.Button, presenter.LogoutText, Handler.Logout);
            AddButton(navigation, UILayoutSheet.OnlineMenu.Button, presenter.BackText, Handler.Back);

            ErrorLine = AddLine(ScrollContainer, UILayoutStyles.OnlineMenu.ErrorColor);

            KindsPage = AddGroup(ScrollContainer, UILayoutSheet.OnlineMenu.Group);
            AddHeader(KindsPage, presenter.ChooseKindTitle);
            var kinds = AddGroup(KindsPage, UILayoutSheet.OnlineMenu.ButtonRow);
            foreach (var entry in presenter.GameKinds())
                AddButton(kinds, UILayoutSheet.OnlineMenu.Button, entry.Label, entry.Action);

            ModesPage = AddGroup(ScrollContainer, UILayoutSheet.OnlineMenu.Group);
            AddHeader(ModesPage, presenter.ChooseModeTitle);
            var modes = AddGroup(ModesPage, UILayoutSheet.OnlineMenu.ButtonRow);
            foreach (var entry in presenter.Modes())
                AddButton(modes, UILayoutSheet.OnlineMenu.Button, entry.Label, entry.Action);

            LobbyPage = AddGroup(ScrollContainer, UILayoutSheet.OnlineMenu.Group);
            BuildLobby(LobbyPage);

            ShowPage(presenter.Page);
        }

        /// <summary>Shows <paramref name="page"/> only.</summary>
        public void ShowPage(OnlineMenuPage page)
        {
            SetShown(KindsPage, page == OnlineMenuPage.GameKinds);
            SetShown(ModesPage, page == OnlineMenuPage.Modes);
            SetShown(LobbyPage, page == OnlineMenuPage.Lobby);
            RefreshLayout();
        }

        /// <summary>Builds the lobby page's blocks into <paramref name="page"/> (nothing yet).</summary>
        partial void BuildLobby(UIOnlineGroup page);
    }
}

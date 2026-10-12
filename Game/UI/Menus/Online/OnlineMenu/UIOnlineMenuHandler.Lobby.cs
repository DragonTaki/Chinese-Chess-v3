/* ----- ----- ----- ----- */
// UIOnlineMenuHandler.Lobby.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/12
// Update Date: 2026/10/12
// Version: v1.0
/* ----- ----- ----- ----- */

using Chinese_Chess_v3.Game.Application.Online;
using Chinese_Chess_v3.Game.Application.Settings;
using Chinese_Chess_v3.Game.UI.Menus.SettingsMenu;

using Microsoft.Extensions.DependencyInjection;

namespace Chinese_Chess_v3.Game.UI.Menus.Online.OnlineMenu
{
    /// <summary>
    /// Binds the online menu's lobby page to the <see cref="LobbyPresenter"/>: the buttons and the
    /// room number field call its commands, the settings rows' edits go to it
    /// (<see cref="ISettingsRowEditor"/>), and the page redraws on <see cref="LobbyPresenter.Changed"/>.
    /// The presenter refreshes the list by itself while the page is on display
    /// (<see cref="OnlineMenuPresenter.SetShown"/>).
    /// </summary>
    public partial class UIOnlineMenuHandler : ISettingsRowEditor
    {
        /// <summary>The lobby's decisions (the <see cref="LobbyPresenter"/> registered in DI).</summary>
        internal LobbyPresenter Lobby => _factory.ServiceProvider.GetRequiredService<LobbyPresenter>();

        partial void InitLobby()
        {
            Lobby.Changed += Refresh;
        }

        /// <summary>Wires the lobby's controls the screen built to the presenter.</summary>
        internal void BindLobbyControls()
        {
            var lobby = Lobby;
            Element.RoomNumberField.Handler.TextChanged = text => lobby.RoomNumber = text;
            Element.RoomNumberField.Handler.Submitted = _ => JoinByNumber();
        }

        /// <summary>重新整理.</summary>
        public void RefreshRooms() => Lobby.Refresh();

        /// <summary>建立房間 with the editor's settings.</summary>
        public void CreateRoom() => Lobby.Create();

        /// <summary>加入 (button or Enter) with the room number field.</summary>
        public void JoinByNumber() => Lobby.JoinByNumber();

        /// <summary>Shows the lobby's state, while its page is the one on display.</summary>
        partial void RefreshLobby()
        {
            var menu = Element;
            if (menu?.RoomNumberField == null || Presenter.Page != OnlineMenuPage.Lobby)
                return;

            var lobby = Lobby;
            if (!menu.RoomNumberField.IsFocused)
                menu.RoomNumberField.Text = lobby.RoomNumber ?? string.Empty;
            UIOnlineMenu.SetLine(menu.NewRoomSummaryLine, lobby.NewRoomSummaryText);
            UIOnlineMenu.SetLine(menu.LobbyErrorLine, lobby.ErrorSentence);
            UIOnlineMenu.SetLine(menu.EmptyRoomsLine, lobby.EmptyText);
            menu.ShowRooms(lobby.Rooms, lobby.JoinText);
            menu.ShowRoomSettings(lobby.RoomSettingsSections, this);
            menu.RefreshRoomSettings(lobby.RoomSettingsValues);
        }

        #region ISettingsRowEditor (the new room's settings rows)

        public void SetToggle(SettingsToggleItem item, bool value) => Lobby.SetToggle(item, value);

        public void SetChoice(SettingsChoiceItem item, int index) => Lobby.SetChoice(item, index);

        public void SetNumber(SettingsNumberItem item, float value) => Lobby.SetNumber(item, value);

        public void SetInteger(SettingsIntegerItem item, int value) => Lobby.SetInteger(item, value);

        public void SetText(SettingsTextItem item, string value) => Lobby.SetText(item, value);

        /// <summary>Nothing to do: a room's settings are not saved, every edit is already applied.</summary>
        public void CommitEdit() { }

        #endregion
    }
}

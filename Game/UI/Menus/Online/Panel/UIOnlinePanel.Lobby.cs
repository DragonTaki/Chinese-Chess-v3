/* ----- ----- ----- ----- */
// UIOnlinePanel.Lobby.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/12
// Update Date: 2026/10/12
// Version: v1.0
/* ----- ----- ----- ----- */

using System;
using System.Collections.Generic;
using System.Text;

using Chinese_Chess_v3.Game.Application.Online;
using Chinese_Chess_v3.Game.Application.Settings;
using Chinese_Chess_v3.Game.Configs;
using Chinese_Chess_v3.Game.UI.Constants;
using Chinese_Chess_v3.Game.UI.Menus.SettingsMenu;

using Engine.UI.Core.Elements;

namespace Chinese_Chess_v3.Game.UI.Menus.Online.Panel
{
    /// <summary>
    /// The online menu's lobby page (custom rooms): refresh and create room with the new room's settings
    /// as one line, the room number field with its join, the lobby's error line, the waiting rooms
    /// (each its line, join while a seat is free, and its settings as one line) or no rooms,
    /// then the new room's settings editor - the local rule settings' rows
    /// (<see cref="SettingsRowBuilder"/>) over the lobby's own settings instance.
    /// </summary>
    public partial class UIOnlinePanel
    {
        /// <summary>The room number field.</summary>
        internal UITextField RoomNumberField { get; private set; }

        /// <summary>The new room's settings as one line.</summary>
        internal UILabel NewRoomSummaryLine { get; private set; }

        /// <summary>The lobby's error (a bad room number, the server's answer).</summary>
        internal UILabel LobbyErrorLine { get; private set; }

        /// <summary>no rooms.</summary>
        internal UILabel EmptyRoomsLine { get; private set; }

        /// <summary>The waiting rooms (rebuilt when the list changes).</summary>
        private UIOnlineGroup _roomList;

        /// <summary>The new room's settings rows (rebuilt when the game kind's editor changes).</summary>
        private UIOnlineGroup _roomSettings;

        /// <summary>Per settings row: refreshes its controls from the lobby's settings instance.</summary>
        private readonly List<Action<PlayerSettings>> _roomSettingsRefreshers = new();

        /// <summary>The editor shown in <see cref="_roomSettings"/>, to rebuild it only when it changes.</summary>
        private IReadOnlyList<SettingsMenuSection> _shownRoomSettings;

        /// <summary>The rooms shown in <see cref="_roomList"/> (text, summary, joinable), to rebuild it only when they change.</summary>
        private string _shownRoomsKey;

        partial void BuildLobby(UIOnlineGroup page)
        {
            var lobby = Handler.Lobby;

            AddHeader(page, lobby.TitleText);

            var actions = AddGroup(page, UILayoutSheet.OnlineMenu.ButtonRow);
            AddButton(actions, UILayoutSheet.OnlineMenu.Button, lobby.RefreshText, Handler.RefreshRooms);
            AddButton(actions, UILayoutSheet.OnlineMenu.Button, lobby.CreateText, Handler.CreateRoom);
            NewRoomSummaryLine = AddLine(page, UILayoutStyles.OnlineMenu.LineColor);

            var numberRow = AddRow(page, lobby.RoomNumberLabel);
            RoomNumberField = AddField(numberRow);
            RoomNumberField.MaxLength = LobbyPresenter.RoomNumberLength;
            AddButton(numberRow, UILayoutSheet.OnlineMenu.RowButton, lobby.JoinText, Handler.JoinByNumber);

            LobbyErrorLine = AddLine(page, UILayoutStyles.OnlineMenu.ErrorColor);

            AddHeader(page, lobby.RoomListTitle);
            EmptyRoomsLine = AddLine(page, UILayoutStyles.OnlineMenu.LineColor);
            _roomList = AddGroup(page, UILayoutSheet.OnlineMenu.Group);

            AddHeader(page, lobby.NewRoomTitle);
            _roomSettings = AddGroup(page, UILayoutSheet.OnlineMenu.Group);

            Handler.BindLobbyControls();
        }

        /// <summary>Shows <paramref name="rooms"/> (rebuilt only when they differ from the shown ones).</summary>
        /// <param name="rooms">The waiting rooms.</param>
        /// <param name="joinText">The join buttons' text.</param>
        public void ShowRooms(IReadOnlyList<LobbyRoomRow> rooms, string joinText)
        {
            var key = new StringBuilder();
            foreach (var room in rooms)
                key.Append(room.Text).Append('\u0001').Append(room.SummaryText).Append('\u0001').Append(room.CanJoin).Append('\u0002');
            if (key.ToString() == _shownRoomsKey)
                return;
            _shownRoomsKey = key.ToString();

            ClearGroup(_roomList);
            foreach (var room in rooms)
            {
                var block = AddGroup(_roomList, UILayoutSheet.OnlineMenu.LineGroup);
                SetLine(AddLine(block, UILayoutStyles.OnlineMenu.LineColor), room.Text);
                var row = AddRow(block, string.Empty);
                if (room.CanJoin)
                    AddButton(row, UILayoutSheet.OnlineMenu.RowButton, joinText, room.Join);
                SetLine(AddLine(block, UILayoutStyles.OnlineMenu.LineColor), room.SummaryText);
            }
        }

        /// <summary>
        /// Shows the new room's settings editor <paramref name="sections"/> (rebuilt only when it is
        /// another editor, i.e. another game kind's), its edits going to <paramref name="editor"/>.
        /// </summary>
        public void ShowRoomSettings(IReadOnlyList<SettingsMenuSection> sections, ISettingsRowEditor editor)
        {
            if (ReferenceEquals(sections, _shownRoomSettings))
                return;
            _shownRoomSettings = sections;

            ClearGroup(_roomSettings);
            _roomSettingsRefreshers.Clear();
            foreach (var section in sections)
            {
                if (section.Header != null)
                    AddHeader(_roomSettings, section.Header);
                foreach (var item in section.Items)
                    _roomSettings.AddChild(SettingsRowBuilder.Create(_factory, item, editor, _roomSettingsRefreshers));
            }
        }

        /// <summary>Updates every settings row (name, values, availability) from <paramref name="values"/>.</summary>
        public void RefreshRoomSettings(PlayerSettings values)
        {
            foreach (var refresh in _roomSettingsRefreshers)
                refresh(values);
        }
    }
}

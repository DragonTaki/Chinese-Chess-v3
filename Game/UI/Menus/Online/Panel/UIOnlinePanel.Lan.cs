/* ----- ----- ----- ----- */
// UIOnlinePanel.Lan.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/13
// Update Date: 2026/10/13
// Version: v1.1
/* ----- ----- ----- ----- */

using System;
using System.Collections.Generic;
using System.Linq;

using Chinese_Chess_v3.Game.Application.Online;
using Chinese_Chess_v3.Game.Application.Texts;
using Chinese_Chess_v3.Game.Configs;
using Chinese_Chess_v3.Game.UI.Constants;
using Chinese_Chess_v3.Game.UI.Menus.SettingsMenu;

using Engine.UI.Core.Elements;

namespace Chinese_Chess_v3.Game.UI.Menus.Online.Panel
{
    /// <summary>Account-free LAN discovery and waiting room using the existing submenu controls.</summary>
    public partial class UIOnlinePanel
    {
        internal UIOnlineGroup ConnectionPage { get; private set; }
        internal UIOnlineGroup LanPage { get; private set; }
        private UITextField _nicknameField;
        private UITextField _lanRoomNumberField;
        private UIButton _lanJoinButton;
        private UIOnlineGroup _lanRoomSettings;
        private UIButton _lanSearchButton;
        private UIButton _lanReadyButton;
        private UILabel _lanStatusLine;
        private UILabel _lanErrorLine;
        private UILabel _lanEmptyLine;
        private UILabel _lanRoomTitle;
        private UILabel _lanSeatsLine;
        private UILabel _lanSettingsLine;
        private UILabel _lanResultLine;
        private UIOnlineGroup _lanBrowser;
        private UIOnlineGroup _lanRoom;
        private UIOnlineGroup _lanHosts;
        private readonly List<Action<PlayerSettings>> _lanSettingsRefreshers = new();
        private string _shownLanHosts;

        partial void BuildLan()
        {
            var presenter = Handler.Presenter;
            var lan = presenter.Lan;
            ConnectionPage = AddGroup(ScrollContainer, UILayoutSheet.OnlineMenu.Group);
            AddHeader(ConnectionPage, presenter.LanText);
            AddButton(ConnectionPage, UILayoutSheet.Submenu.Button, lan.CreateText, presenter.CreateLanRoom);
            AddButton(ConnectionPage, UILayoutSheet.Submenu.Button, LanTexts.JoinRoom, presenter.ChooseLan);
            LanPage = AddGroup(ScrollContainer, UILayoutSheet.OnlineMenu.Group);
            AddHeader(LanPage, lan.TitleText);
            _lanStatusLine = AddLine(LanPage, UILayoutStyles.OnlineMenu.StatusColor);
            _lanErrorLine = AddLine(LanPage, UILayoutStyles.OnlineMenu.ErrorColor);
            _lanResultLine = AddLine(LanPage, UILayoutStyles.OnlineMenu.StatusColor);
            _lanBrowser = AddGroup(LanPage, UILayoutSheet.OnlineMenu.Group);
            _nicknameField = AddField(AddRow(_lanBrowser, lan.NicknameLabel));
            _nicknameField.MaxLength = PlayerNameSettings.PlayerNameMaxLength;
            var actions = AddGroup(_lanBrowser, UILayoutSheet.OnlineMenu.ButtonRow);
            _lanSearchButton = AddButton(actions, UILayoutSheet.OnlineMenu.Button, lan.SearchText, lan.Refresh);
            _lanRoomNumberField = AddField(AddRow(_lanBrowser, lan.RoomAddressLabel));
            _lanJoinButton = AddButton(_lanBrowser, UILayoutSheet.OnlineMenu.Button, lan.JoinText, lan.JoinByNumber);
            _lanRoomNumberField.Handler.TextChanged = text => lan.RoomAddress = text;
            _lanRoomNumberField.Handler.Submitted = _ => lan.JoinByNumber();
            AddHeader(_lanBrowser, lan.NearbyTitle);
            _lanEmptyLine = AddLine(_lanBrowser, UILayoutStyles.OnlineMenu.LineColor);
            _lanHosts = AddGroup(_lanBrowser, UILayoutSheet.OnlineMenu.Group);
            _lanRoom = AddGroup(LanPage, UILayoutSheet.OnlineMenu.Group);
            _lanRoomTitle = AddLine(_lanRoom, UILayoutStyles.OnlineMenu.StatusColor);
            var roomActions = AddGroup(_lanRoom, UILayoutSheet.OnlineMenu.ButtonRow);
            _lanReadyButton = AddButton(roomActions, UILayoutSheet.OnlineMenu.Button, lan.ReadyText, lan.ToggleReady);
            AddButton(roomActions, UILayoutSheet.OnlineMenu.Button, lan.LeaveText, lan.LeaveRoom);
            _lanSeatsLine = AddLine(_lanRoom, UILayoutStyles.OnlineMenu.LineColor);
            _lanSettingsLine = AddLine(_lanRoom, UILayoutStyles.OnlineMenu.LineColor);
            _lanRoomSettings = AddGroup(_lanRoom, UILayoutSheet.OnlineMenu.Group);
            AddHeader(_lanRoomSettings, presenter.ChooseKindTitle);
            foreach (var group in presenter.GroupedKinds())
            {
                AddGroupHeading(_lanRoomSettings, group.Title);
                var row = AddGroup(_lanRoomSettings, UILayoutSheet.Submenu.GroupRow);
                foreach (var entry in group.Entries)
                    AddButton(row, UILayoutSheet.Submenu.GroupButton, entry.Label, () => lan.ChooseKind(entry.Id));
            }
            AddHeader(_lanRoomSettings, presenter.ChooseModeTitle);
            foreach (var entry in presenter.Modes())
                AddButton(_lanRoomSettings, UILayoutSheet.Submenu.Button, entry.Label, () => lan.ChooseMode(entry.Id));
            AddHeader(_lanRoomSettings, lan.NewRoomTitle);
            foreach (var section in lan.SettingsSections)
            {
                if (section.Header != null)
                    AddHeader(_lanRoomSettings, section.Header);
                foreach (var item in section.Items)
                    _lanRoomSettings.AddChild(SettingsRowBuilder.Create(_factory, item, Handler, _lanSettingsRefreshers));
            }
            AddButton(LanPage, UILayoutSheet.OnlineMenu.Button, presenter.BackText, Handler.Back);
            _nicknameField.Handler.TextChanged = text => lan.Nickname = text;
            _nicknameField.Handler.Submitted = _ => lan.Refresh();
        }

        internal void ShowLanState(LanMenuPresenter lan)
        {
            SetShown(_lanBrowser, !lan.HasRoom);
            SetShown(_lanRoom, lan.HasRoom);
            SetShown(_lanRoomSettings, lan.CanEditSettings);
            SetLine(_lanStatusLine, lan.StatusSentence);
            SetLine(_lanErrorLine, lan.ErrorSentence);
            SetLine(_lanResultLine, lan.LastResultSentence);
            SetLine(_lanEmptyLine, lan.EmptyText);
            SetLine(_lanRoomTitle, lan.RoomTitle);
            SetLine(_lanSeatsLine, lan.SeatsText);
            SetLine(_lanSettingsLine, lan.RoomSummary);
            if (!_nicknameField.IsFocused)
                _nicknameField.Text = lan.Nickname ?? string.Empty;
            _nicknameField.IsEnabled = lan.CanCreate;
            SetEnabled(_lanSearchButton, !lan.IsBusy);
            if (!_lanRoomNumberField.IsFocused)
                _lanRoomNumberField.Text = lan.RoomAddress;
            _lanRoomNumberField.IsEnabled = !lan.IsBusy;
            SetEnabled(_lanJoinButton, !lan.IsBusy);
            _lanReadyButton.Text = lan.ReadyText;
            SetEnabled(_lanReadyButton, lan.CanReady);
            foreach (var refresh in _lanSettingsRefreshers)
                refresh(lan.RoomSettingsValues);
            var rooms = lan.Rooms;
            string key = string.Join("\n", rooms.Select(room => room.Key + "\t" + room.Text));
            if (key != _shownLanHosts)
            {
                _shownLanHosts = key;
                ClearGroup(_lanHosts);
                foreach (var room in rooms)
                {
                    var row = AddRow(_lanHosts, room.Text);
                    AddButton(row, UILayoutSheet.OnlineMenu.RowButton, lan.JoinText, room.Join);
                }
            }
            RefreshLayout();
        }
    }
}

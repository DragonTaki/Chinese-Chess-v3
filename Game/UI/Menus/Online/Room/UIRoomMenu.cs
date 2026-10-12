/* ----- ----- ----- ----- */
// UIRoomMenu.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/12
// Update Date: 2026/10/12
// Version: v1.0
/* ----- ----- ----- ----- */

using System.Collections.Generic;
using System.Drawing;

using Chinese_Chess_v3.Game.Application.Online;
using Chinese_Chess_v3.Game.UI.Constants;

using Engine.UI.Core.Elements;

namespace Chinese_Chess_v3.Game.UI.Menus.Online.Room
{
    /// <summary>
    /// An online room: its title (房間 123456), 準備 / 取消準備 and 離開房間, the status line
    /// (重新連線中…, 對局進行中), the last game's result and the error line, then the seats (this
    /// player's highlighted, a free one dimmed) and the room's settings. The online game board is
    /// not here yet: during the game this screen only says so. The handler
    /// (<see cref="UIRoomMenuHandler"/>) binds it to the <c>RoomPresenter</c>.
    /// </summary>
    public class UIRoomMenu : UIOnlineScreen<UIRoomMenu, UIRoomMenuHandler, UIRoomMenuRenderer>
    {
        /// <summary>The room's title (a header).</summary>
        internal UIButton Title { get; private set; }

        /// <summary>準備 / 取消準備 (dimmed when it cannot be pressed).</summary>
        internal UIButton ReadyButton { get; private set; }

        /// <summary>重新連線中…, 對局進行中.</summary>
        internal UILabel StatusLine { get; private set; }

        /// <summary>The last game's result.</summary>
        internal UILabel ResultLine { get; private set; }

        /// <summary>The server's last error here.</summary>
        internal UILabel ErrorLine { get; private set; }

        /// <summary>The room's settings, one sentence per line.</summary>
        internal UILabel SettingsLine { get; private set; }

        /// <summary>The seats (rebuilt on every update).</summary>
        private UIOnlineGroup _seats;

        public UIRoomMenu() { }

        protected override void BuildButtons()
        {
            var presenter = Handler.Presenter;

            Title = AddHeader(ScrollContainer, presenter.TitleText);

            var buttons = AddGroup(ScrollContainer, UILayoutSheet.OnlineMenu.ButtonRow);
            ReadyButton = AddButton(buttons, UILayoutSheet.OnlineMenu.Button, presenter.ReadyText, Handler.ToggleReady);
            AddButton(buttons, UILayoutSheet.OnlineMenu.Button, presenter.LeaveText, Handler.Leave);

            StatusLine = AddLine(ScrollContainer, UILayoutStyles.OnlineMenu.StatusColor);
            ResultLine = AddLine(ScrollContainer, UILayoutStyles.OnlineMenu.StatusColor);
            ErrorLine = AddLine(ScrollContainer, UILayoutStyles.OnlineMenu.ErrorColor);

            AddHeader(ScrollContainer, presenter.SeatsTitle);
            _seats = AddGroup(ScrollContainer, UILayoutSheet.OnlineMenu.LineGroup);

            AddHeader(ScrollContainer, presenter.SettingsTitle);
            SettingsLine = AddLine(ScrollContainer, UILayoutStyles.OnlineMenu.LineColor);
        }

        /// <summary>Replaces the seat lines with <paramref name="seats"/>.</summary>
        public void ShowSeats(IReadOnlyList<RoomSeatRow> seats)
        {
            ClearGroup(_seats);
            foreach (var seat in seats)
                SetLine(AddLine(_seats, SeatColor(seat)), seat.Text);
        }

        /// <summary>This player's seat highlighted, a free one dimmed, the others plain.</summary>
        private static Color SeatColor(RoomSeatRow seat) =>
            seat.IsMe ? UILayoutStyles.OnlineMenu.StatusColor
            : seat.IsEmpty ? UILayoutStyles.OnlineMenu.EmptySeatColor
            : UILayoutStyles.OnlineMenu.LineColor;
    }
}

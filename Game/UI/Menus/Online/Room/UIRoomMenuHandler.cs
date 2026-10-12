/* ----- ----- ----- ----- */
// UIRoomMenuHandler.cs
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

namespace Chinese_Chess_v3.Game.UI.Menus.Online.Room
{
    /// <summary>
    /// Binds the <see cref="UIRoomMenu"/> to the <see cref="RoomPresenter"/>, which makes the
    /// decisions (ready, leaving - asking first during the game -, the seats and sentences): the
    /// buttons call its commands and the screen redraws on <see cref="RoomPresenter.Changed"/>.
    /// </summary>
    public class UIRoomMenuHandler : UIMenuHandler<UIRoomMenu, UIRoomMenuHandler, UIRoomMenuRenderer>, IScreen
    {
        public UIRoomMenuHandler() { }

        /// <summary>The room screen's decisions (the <see cref="RoomPresenter"/> registered in DI).</summary>
        internal RoomPresenter Presenter => _factory.ServiceProvider.GetRequiredService<RoomPresenter>();

        /// <summary>Subscribes to the presenter once: the screen (and this handler) is a single long-lived instance, like the presenter.</summary>
        protected override void OnInit(IUiFactory factory)
        {
            Presenter.Changed += Refresh;
        }

        /// <summary>準備 / 取消準備.</summary>
        public void ToggleReady() => Presenter.ToggleReady();

        /// <summary>離開房間.</summary>
        public void Leave() => Presenter.Leave();

        public void OnEnter() => Refresh();

        public void OnExit() { }

        /// <summary>Shows the presenter's state: title, ready button, lines, seats and settings.</summary>
        private void Refresh()
        {
            var menu = Element;
            if (menu?.Title == null)
                return;

            var presenter = Presenter;
            menu.Title.Text = presenter.TitleText;
            menu.ReadyButton.Text = presenter.ReadyText;
            UIRoomMenu.SetEnabled(menu.ReadyButton, presenter.CanToggleReady);
            UIRoomMenu.SetLine(menu.StatusLine, presenter.StatusSentence);
            UIRoomMenu.SetLine(menu.ResultLine, presenter.LastResultSentence);
            UIRoomMenu.SetLine(menu.ErrorLine, presenter.ErrorSentence);
            menu.ShowSeats(presenter.Seats);
            UIRoomMenu.SetLine(menu.SettingsLine, string.Join("\n", presenter.SettingsSummary));
            menu.RefreshLayout();
        }
    }
}

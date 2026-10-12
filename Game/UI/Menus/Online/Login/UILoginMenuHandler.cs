/* ----- ----- ----- ----- */
// UILoginMenuHandler.cs
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

namespace Chinese_Chess_v3.Game.UI.Menus.Online.Login
{
    /// <summary>
    /// Binds the <see cref="UILoginMenu"/> to the <see cref="LoginPresenter"/>, which makes the
    /// decisions (logging in, the busy state, the sentences): the fields and the switch write the
    /// presenter's <see cref="LoginPresenter.Account"/>, <see cref="LoginPresenter.Password"/> and
    /// <see cref="LoginPresenter.Remember"/>; 登入 and Enter call <see cref="LoginPresenter.Submit"/>;
    /// the screen redraws on <see cref="LoginPresenter.Changed"/>.
    /// <see cref="IScreen"/>: shown → <see cref="LoginPresenter.Shown"/>.
    /// </summary>
    public class UILoginMenuHandler : UIMenuHandler<UILoginMenu, UILoginMenuHandler, UILoginMenuRenderer>, IScreen
    {
        public UILoginMenuHandler() { }

        /// <summary>The login screen's decisions (the <see cref="LoginPresenter"/> registered in DI).</summary>
        internal LoginPresenter Presenter => _factory.ServiceProvider.GetRequiredService<LoginPresenter>();

        /// <summary>Subscribes to the presenter once: the screen (and this handler) is a single long-lived instance, like the presenter.</summary>
        protected override void OnInit(IUiFactory factory)
        {
            Presenter.Changed += Refresh;
        }

        /// <summary>Wires the controls the screen built to the presenter.</summary>
        internal void BindControls()
        {
            var presenter = Presenter;
            Element.AccountField.Handler.TextChanged = text => presenter.Account = text;
            Element.AccountField.Handler.Submitted = _ => Submit();
            Element.PasswordField.Handler.TextChanged = text => presenter.Password = text;
            Element.PasswordField.Handler.Submitted = _ => Submit();
            Element.RememberSwitch.Handler.ValueChanged = on => presenter.Remember = on;
        }

        /// <summary>登入 (button or Enter).</summary>
        public void Submit() => Presenter.Submit();

        /// <summary>返回: back to the main menu.</summary>
        public void Back() => Presenter.Back();

        public void OnEnter()
        {
            Presenter.Shown();
            Refresh();
        }

        public void OnExit() { }

        /// <summary>Shows the presenter's state: the fields (not while typing in them), busy, status and error.</summary>
        private void Refresh()
        {
            var menu = Element;
            if (menu?.AccountField == null)
                return;

            var presenter = Presenter;
            if (!menu.AccountField.IsFocused)
                menu.AccountField.Text = presenter.Account ?? string.Empty;
            if (!menu.PasswordField.IsFocused)
                menu.PasswordField.Text = presenter.Password ?? string.Empty;
            menu.RememberSwitch.IsOn = presenter.Remember;

            bool idle = !presenter.IsBusy;
            menu.AccountField.IsEnabled = idle;
            menu.PasswordField.IsEnabled = idle;
            menu.RememberSwitch.IsEnabled = idle;
            UILoginMenu.SetEnabled(menu.SubmitButton, idle);

            UILoginMenu.SetLine(menu.StatusLine, presenter.StatusSentence);
            UILoginMenu.SetLine(menu.ErrorLine, presenter.ErrorSentence);
            menu.RefreshLayout();
        }
    }
}

/* ----- ----- ----- ----- */
// UIOnlinePanelHandler.Login.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/12
// Update Date: 2026/10/13
// Version: v1.2
/* ----- ----- ----- ----- */

using Chinese_Chess_v3.Game.Application.Online;

using Engine.UI.Core.Interfaces;

using Microsoft.Extensions.DependencyInjection;

namespace Chinese_Chess_v3.Game.UI.Menus.Online.Panel
{
    /// <summary>
    /// Binds the <see cref="UIOnlinePanel"/> to the <see cref="LoginPresenter"/>, which makes the
    /// decisions (logging in, the busy state, the sentences): the fields and the checkbox write the
    /// presenter's <see cref="LoginPresenter.Account"/>, <see cref="LoginPresenter.Password"/> and
    /// <see cref="LoginPresenter.Remember"/>; login and Enter call <see cref="LoginPresenter.Submit"/>;
    /// the screen redraws on <see cref="LoginPresenter.Changed"/>.
    /// <see cref="IScreen"/>: shown → <see cref="LoginPresenter.Shown"/>.
    /// </summary>
    public partial class UIOnlinePanelHandler
    {
        /// <summary>The login screen's decisions (the <see cref="LoginPresenter"/> registered in DI).</summary>
        internal LoginPresenter Login => _factory.ServiceProvider.GetRequiredService<LoginPresenter>();

        /// <summary>Subscribes to the presenter once: the screen (and this handler) is a single long-lived instance, like the presenter.</summary>
        partial void InitLogin()
        {
            Login.Changed += Refresh;
        }

        /// <summary>Wires the controls the screen built to the presenter.</summary>
        internal void BindLoginControls()
        {
            var presenter = Login;
            Element.AccountField.Handler.TextChanged = text => presenter.Account = text;
            Element.AccountField.Handler.Submitted = _ => Submit();
            Element.PasswordField.Handler.TextChanged = text => presenter.Password = text;
            Element.PasswordField.Handler.Submitted = _ => Submit();
            Element.RememberCheckbox.Changed += on => presenter.Remember = on;
        }

        /// <summary>login (button or Enter).</summary>
        public void Submit() => Login.Submit();

        /// <summary>Shows the presenter's state: the fields (not while typing in them), busy, status and error.</summary>
        partial void RefreshLogin()
        {
            var menu = Element;
            if (menu?.AccountField == null || !Presenter.IsFirstPage || Presenter.IsLoggedIn)
                return;

            var presenter = Login;
            if (!menu.AccountField.IsFocused)
                menu.AccountField.Text = presenter.Account ?? string.Empty;
            if (!menu.PasswordField.IsFocused)
                menu.PasswordField.Text = presenter.Password ?? string.Empty;
            menu.RememberCheckbox.IsChecked = presenter.Remember;

            bool idle = !presenter.IsBusy;
            menu.AccountField.IsEnabled = idle;
            menu.PasswordField.IsEnabled = idle;
            menu.RememberCheckbox.IsEnabled = idle;
            UIOnlinePanel.SetEnabled(menu.SubmitButton, idle);

            UIOnlinePanel.SetLine(menu.StatusLine, presenter.StatusSentence);
            UIOnlinePanel.SetLine(menu.LoginErrorLine, presenter.ErrorSentence);
            menu.RefreshLayout();
        }
    }
}

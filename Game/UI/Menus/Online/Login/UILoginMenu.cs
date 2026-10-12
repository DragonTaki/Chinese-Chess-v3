/* ----- ----- ----- ----- */
// UILoginMenu.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/12
// Update Date: 2026/10/12
// Version: v1.0
/* ----- ----- ----- ----- */

using Chinese_Chess_v3.Game.UI.Constants;

using Engine.UI.Core.Elements;

namespace Chinese_Chess_v3.Game.UI.Menus.Online.Login
{
    /// <summary>
    /// 多人連線's login screen: the title, the account field, the masked password field (Tab moves
    /// from the account to the password, Enter in either logs in), the 記住帳密 switch, 登入 and
    /// 返回, then the status line (連線中…) and the error line. The handler
    /// (<see cref="UILoginMenuHandler"/>) binds it to the <c>LoginPresenter</c>.
    /// </summary>
    public class UILoginMenu : UIOnlineScreen<UILoginMenu, UILoginMenuHandler, UILoginMenuRenderer>
    {
        /// <summary>The account (email) field.</summary>
        internal UITextField AccountField { get; private set; }

        /// <summary>The password field (masked).</summary>
        internal UITextField PasswordField { get; private set; }

        /// <summary>The 記住帳密 switch.</summary>
        internal UIToggleSwitch RememberSwitch { get; private set; }

        /// <summary>The 登入 button (dimmed while a login runs).</summary>
        internal UIButton SubmitButton { get; private set; }

        /// <summary>連線中… while a login runs.</summary>
        internal UILabel StatusLine { get; private set; }

        /// <summary>Why the last login failed.</summary>
        internal UILabel ErrorLine { get; private set; }

        public UILoginMenu() { }

        protected override void BuildButtons()
        {
            var presenter = Handler.Presenter;

            AddHeader(ScrollContainer, presenter.TitleText);

            AccountField = AddField(AddRow(ScrollContainer, presenter.AccountLabel));
            PasswordField = AddField(AddRow(ScrollContainer, presenter.PasswordLabel), masked: true);
            AccountField.NextFocus = PasswordField;
            RememberSwitch = AddToggle(AddRow(ScrollContainer, presenter.RememberLabel));

            var buttons = AddGroup(ScrollContainer, UILayoutSheet.OnlineMenu.ButtonRow);
            SubmitButton = AddButton(buttons, UILayoutSheet.OnlineMenu.Button, presenter.SubmitText, Handler.Submit);
            AddButton(buttons, UILayoutSheet.OnlineMenu.Button, presenter.BackText, Handler.Back);

            StatusLine = AddLine(ScrollContainer, UILayoutStyles.OnlineMenu.StatusColor);
            ErrorLine = AddLine(ScrollContainer, UILayoutStyles.OnlineMenu.ErrorColor);

            Handler.BindControls();
        }
    }
}

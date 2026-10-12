/* ----- ----- ----- ----- */
// UIOnlinePanel.Login.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/12
// Update Date: 2026/10/12
// Version: v1.0
/* ----- ----- ----- ----- */

using Chinese_Chess_v3.Game.UI.Constants;

using Engine.UI.Core.Elements;

namespace Chinese_Chess_v3.Game.UI.Menus.Online.Panel
{
    /// <summary>
    /// Multiplayer's login screen: the title, the account field, the masked password field (Tab moves
    /// from the account to the password, Enter in either logs in), the remember login switch, login and
    /// back, then the status line (connecting) and the error line. The handler
    /// (<see cref="UIOnlinePanelHandler"/>) binds it to the <c>LoginPresenter</c>.
    /// </summary>
    public partial class UIOnlinePanel
    {
        /// <summary>The account (email) field.</summary>
        internal UITextField AccountField { get; private set; }

        /// <summary>The password field (masked).</summary>
        internal UITextField PasswordField { get; private set; }

        /// <summary>The remember login switch.</summary>
        internal UIToggleSwitch RememberSwitch { get; private set; }

        /// <summary>The login button (dimmed while a login runs).</summary>
        internal UIButton SubmitButton { get; private set; }

        /// <summary>connecting while a login runs.</summary>
        internal UILabel StatusLine { get; private set; }

        /// <summary>Why the last login failed.</summary>
        internal UILabel LoginErrorLine { get; private set; }

        partial void BuildLogin(UIOnlineGroup page)
        {
            var presenter = Handler.Login;

            AddHeader(page, presenter.TitleText);

            AccountField = AddField(AddRow(page, presenter.AccountLabel));
            PasswordField = AddField(AddRow(page, presenter.PasswordLabel), masked: true);
            AccountField.NextFocus = PasswordField;
            RememberSwitch = AddToggle(AddRow(page, presenter.RememberLabel));

            var buttons = AddGroup(page, UILayoutSheet.OnlineMenu.ButtonRow);
            SubmitButton = AddButton(buttons, UILayoutSheet.OnlineMenu.Button, presenter.SubmitText, Handler.Submit);
            AddButton(buttons, UILayoutSheet.OnlineMenu.Button, presenter.BackText, Handler.Back);

            StatusLine = AddLine(page, UILayoutStyles.OnlineMenu.StatusColor);
            LoginErrorLine = AddLine(page, UILayoutStyles.OnlineMenu.ErrorColor);

            Handler.BindLoginControls();
        }
    }
}

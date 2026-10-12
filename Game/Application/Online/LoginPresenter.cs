/* ----- ----- ----- ----- */
// LoginPresenter.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/12
// Update Date: 2026/10/12
// Version: v1.0
/* ----- ----- ----- ----- */

using System;

using Chinese_Chess_v3.Game.Application.Texts;

namespace Chinese_Chess_v3.Game.Application.Online
{
    /// <summary>
    /// The login screen's decisions, apart from the view that draws it: the account (email),
    /// password and 記住帳密 fields, 登入, the busy state (連線中… while a login runs, also the
    /// automatic one with the remembered login) and the error sentence. The view binds its fields
    /// to <see cref="Account"/>, <see cref="Password"/> and <see cref="Remember"/>, calls
    /// <see cref="Submit"/> (button or Enter) and <see cref="Back"/>, and redraws on <see cref="Changed"/>.
    /// </summary>
    public sealed class LoginPresenter
    {
        private readonly OnlineSession _session;

        /// <summary>Creates the presenter (a single long-lived instance, like the screen it drives).</summary>
        /// <param name="session">The online session.</param>
        public LoginPresenter(OnlineSession session)
        {
            _session = session ?? throw new ArgumentNullException(nameof(session));
            _session.StateChanged += () => Changed?.Invoke();
            _session.ErrorRaised += _ => Changed?.Invoke();
        }

        // ----- Texts -----

        /// <summary>The screen's title.</summary>
        public string TitleText => OnlineTexts.Login;

        /// <summary>The account field's label.</summary>
        public string AccountLabel => OnlineTexts.Account;

        /// <summary>The password field's label (the view masks the field).</summary>
        public string PasswordLabel => OnlineTexts.Password;

        /// <summary>The 記住帳密 check box's text.</summary>
        public string RememberLabel => OnlineTexts.RememberLogin;

        /// <summary>The submit button's text.</summary>
        public string SubmitText => OnlineTexts.Login;

        /// <summary>The back button's text.</summary>
        public string BackText => OnlineTexts.Back;

        // ----- State -----

        /// <summary>The account field (the email).</summary>
        public string Account { get; set; } = "";

        /// <summary>The password field.</summary>
        public string Password { get; set; } = "";

        /// <summary>記住帳密: log in automatically next time.</summary>
        public bool Remember { get; set; }

        /// <summary>Whether a login runs (the fields and 登入 are disabled; <see cref="StatusSentence"/> says so).</summary>
        public bool IsBusy => _session.IsBusy;

        /// <summary>連線中… while a login runs, otherwise null.</summary>
        public string StatusSentence => IsBusy ? _session.StatusSentence ?? OnlineTexts.Connecting : null;

        /// <summary>Why the last login failed (or the connection was lost), or null.</summary>
        public string ErrorSentence => IsBusy ? null : _session.LastError;

        /// <summary>The view redraws (busy, status or error changed).</summary>
        public event Action Changed;

        // ----- Commands -----

        /// <summary>The screen is shown: fills the account of a remembered login when the field is empty.</summary>
        public void Shown()
        {
            if (string.IsNullOrEmpty(Account) && _session.RememberedAccount is string account)
                Account = account;
            Changed?.Invoke();
        }

        /// <summary>登入 (button or Enter in a field): logs in, unless busy; an empty field gives an error sentence.</summary>
        public void Submit()
        {
            if (IsBusy)
                return;
            _session.Login(Account, Password, Remember);
            Changed?.Invoke();
        }

        /// <summary>Back to the main menu (stops a running login first).</summary>
        public void Back()
        {
            _session.CancelLogin();
            _session.LeaveToMainMenu();
        }
    }
}

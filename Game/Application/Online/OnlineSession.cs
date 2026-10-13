/* ----- ----- ----- ----- */
// OnlineSession.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/12
// Update Date: 2026/10/13
// Version: v1.3
/* ----- ----- ----- ----- */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

using Chinese_Chess_v3.Game.Application.Services;
using Chinese_Chess_v3.Game.Application.Texts;
using Chinese_Chess_v3.Network;
using Chinese_Chess_v3.Network.Protocol;

using Engine.Logging;
using Engine.Security;
using Engine.Timing;

namespace Chinese_Chess_v3.Game.Application.Online
{
    /// <summary>
    /// Online play's flow (one long-lived instance): logging in (with the email and password, or
    /// automatically with the remembered login token), the online menu, the lobby and the room,
    /// losing and restoring the connection, logging out. It owns the online state the screens
    /// show: <see cref="State"/>, the account, the current room (<see cref="Room"/>), the current
    /// game's start (<see cref="Game"/>) and the last error sentence (<see cref="LastError"/>).
    /// <para>
    /// Threads: everything here runs on the UI thread. Every <see cref="OnlineChannel"/> event (raised
    /// on a network thread) and every finished login is re-posted through
    /// <see cref="IMainThreadQueue"/>, so this class's state changes and its events are only ever
    /// seen on the UI thread; its methods must be called there too.
    /// </para>
    /// <para>
    /// Logins never overlap (<see cref="IsBusy"/>): a login token is single-use, and the server
    /// takes a second login with the same token as theft and revokes every token of the account.
    /// Every accepted token login returns the token's replacement, which is stored at once.
    /// </para>
    /// <para>
    /// Lost connection (not asked for): in a room or a game the session logs in again by itself
    /// (<see cref="ReconnectPolicy"/>: while the server holds the seat), with the login token, or the
    /// password kept in memory when the login was not remembered; it stops at a reason that cannot
    /// change by retrying (wrong password, invalid token, logged in elsewhere, old version) and shows
    /// it. Outside a room it does not retry: the state is <see cref="OnlineState.ConnectionLost"/> and
    /// <see cref="Reconnect"/> tries again. A replacement by a new login elsewhere is never retried.
    /// </para>
    /// The server is authoritative: the room, the game and its result are only what the server
    /// reports.
    /// </summary>
    public sealed class OnlineSession : IDisposable
    {
        /// <summary>The credential store key of the remembered login token.</summary>
        public const string LoginTokenKey = "online.loginToken";

        /// <summary>The credential store key of the remembered login's account (its email, shown while no room has named it).</summary>
        public const string LoginAccountKey = "online.loginAccount";

        /// <summary>How long after a successful automatic reconnect the room / game may still arrive before the seat counts as lost.</summary>
        private static readonly TimeSpan ReconnectSettleTime = TimeSpan.FromSeconds(1);

        /// <summary>How long after a logout the connection is closed from this side if the server has not closed it.</summary>
        private static readonly TimeSpan LogoutCloseTime = TimeSpan.FromSeconds(2);

        // Why a login runs: decides what its result does.
        private enum LoginPurpose
        {
            Password,         // The login screen's
            RememberedToken,  // Entering 多人連線 with a remembered login
            AutoReconnect,    // The connection of a room or game was lost
            ManualReconnect,  // Reconnect() after the connection was lost outside a room
        }

        private readonly OnlineChannel _channel;
        private readonly IMainThreadQueue _mainThread;
        private readonly ICredentialStore _credentials;
        private readonly INavigator _navigator;
        private readonly IDialogService _dialogs;

        // Handlers subscribed to the channel (kept to unsubscribe).
        private readonly Action<IReadOnlyList<RoomStateDto>> _onRoomList;
        private readonly Action<RoomStateDto> _onRoomState;
        private readonly Action<StartGameDto> _onGameStarted;
        private readonly Action<GameUpdateDto> _onGameUpdated;
        private readonly Action<int, string> _onActionRejected;
        private readonly Action<ClocksDto> _onTimerSynced;
        private readonly Action<EndGameDto> _onGameEnded;
        private readonly Action<string, string> _onServerError;
        private readonly Action _onReplaced;
        private readonly Action _onLoggedOut;
        private readonly Action _onDisconnected;

        // Bumped by every login started and by every cancel / logout: a scheduled step or a login
        // result of an older generation is stale.
        private int _generation;
        private bool _loginRunning;

        // The newest login token (null: the login is not remembered) and, for a reconnect without
        // one, the password login of this session (memory only).
        private string _refreshToken;
        private LoginCredentials _passwordLogin;

        // Whether the room / game arrived since the running (or last) login started.
        private bool _roomSinceLogin;
        private bool _gameSinceLogin;

        // The automatic reconnect.
        private Stopwatch _reconnectClock;
        private TimeSpan _reconnectLimit;
        private int _reconnectAttempt;

        // The game this client left (= resigned): its EndGame is no news.
        private string _leftGameId;

        private bool _disposed;

        /// <summary>Creates the session (subscribes to <paramref name="channel"/> until <see cref="Dispose"/>).</summary>
        /// <param name="channel">The connection to the server.</param>
        /// <param name="mainThread">Where the channel's events are re-posted (drained on the UI thread).</param>
        /// <param name="credentials">Keeps the remembered login token.</param>
        /// <param name="navigator">Switches between the online screens.</param>
        /// <param name="dialogs">Tells the player about a replacement by another login or a result missed while away.</param>
        public OnlineSession(OnlineChannel channel, IMainThreadQueue mainThread, ICredentialStore credentials, INavigator navigator, IDialogService dialogs)
        {
            _channel = channel ?? throw new ArgumentNullException(nameof(channel));
            _mainThread = mainThread ?? throw new ArgumentNullException(nameof(mainThread));
            _credentials = credentials ?? throw new ArgumentNullException(nameof(credentials));
            _navigator = navigator ?? throw new ArgumentNullException(nameof(navigator));
            _dialogs = dialogs ?? throw new ArgumentNullException(nameof(dialogs));

            _onRoomList = rooms => Post(() => HandleRoomList(rooms));
            _onRoomState = room => Post(() => HandleRoomState(room));
            _onGameStarted = start => Post(() => HandleGameStarted(start));
            _onGameUpdated = update => Post(() => GameUpdated?.Invoke(update));
            _onActionRejected = (seq, reason) => Post(() => ActionRejected?.Invoke(seq, reason));
            _onTimerSynced = clocks => Post(() => TimerSynced?.Invoke(clocks));
            _onGameEnded = end => Post(() => HandleGameEnded(end));
            _onServerError = (code, detail) => Post(() => HandleServerError(code, detail));
            _onReplaced = () => Post(HandleReplacedByNewLogin);
            _onLoggedOut = () => Post(HandleLoggedOut);
            _onDisconnected = () => Post(HandleDisconnected);

            _channel.RoomList += _onRoomList;
            _channel.RoomState += _onRoomState;
            _channel.GameStarted += _onGameStarted;
            _channel.GameUpdated += _onGameUpdated;
            _channel.ActionRejected += _onActionRejected;
            _channel.TimerSynced += _onTimerSynced;
            _channel.GameEnded += _onGameEnded;
            _channel.ServerError += _onServerError;
            _channel.ReplacedByNewLogin += _onReplaced;
            _channel.LoggedOut += _onLoggedOut;
            _channel.Disconnected += _onDisconnected;
        }

        // ----- State -----

        /// <summary>Where the session is.</summary>
        public OnlineState State { get; private set; } = OnlineState.LoggedOut;

        /// <summary>Whether a login is running (the login screen is busy; no other login may start).</summary>
        public bool IsBusy => _loginRunning || State == OnlineState.LoggingIn || State == OnlineState.Reconnecting;

        /// <summary>The account's name: the server's name for it once a room has shown it, before that the email logged in with; null when logged out.</summary>
        public string Username { get; private set; }

        /// <summary>The account's id (from the session token), used to find this player's seat; null when unknown.</summary>
        public string AccountId { get; private set; }

        /// <summary>The room this player is seated in (also during its game and while reconnecting), or null.</summary>
        public RoomStateDto Room { get; private set; }

        /// <summary>The current game's start (StartGame; resumed after a reconnect), or null outside a game.</summary>
        public StartGameDto Game { get; private set; }

        /// <summary>The last game's result, or null.</summary>
        public EndGameDto LastGameEnd { get; private set; }

        /// <summary>The sentence of <see cref="LastGameEnd"/>, or null.</summary>
        public string LastGameEndSentence { get; private set; }

        /// <summary>The waiting rooms of the last room list.</summary>
        public IReadOnlyList<RoomStateDto> Rooms { get; private set; } = Array.Empty<RoomStateDto>();

        /// <summary>The last error sentence (a failed login, a lost connection, a server error), or null.</summary>
        public string LastError { get; private set; }

        /// <summary>What the session is doing, for a busy screen (連線中…, 重新連線中…), or null.</summary>
        public string StatusSentence { get; private set; }

        /// <summary>Whether a login is remembered (the next <see cref="EnterMultiplayer"/> logs in by itself).</summary>
        public bool HasRememberedLogin => _credentials.TryRead(LoginTokenKey, out string token) && !string.IsNullOrEmpty(token);

        /// <summary>The account remembered with the login token (its email), or null.</summary>
        public string RememberedAccount => _credentials.TryRead(LoginAccountKey, out string account) ? account : null;

        /// <summary>This player's seat in <see cref="Room"/>, or null.</summary>
        public SeatDto MySeat => FindMySeat(Room);

        /// <summary>Whether this player hosts <see cref="Room"/>.</summary>
        public bool IsHost => Room != null && AccountId != null && Room.HostId == AccountId;

        /// <summary>Whether <see cref="Reconnect"/> can do anything (the connection was lost outside a room).</summary>
        public bool CanReconnect => State == OnlineState.ConnectionLost && !_loginRunning;

        // ----- Events (all on the UI thread) -----

        /// <summary><see cref="State"/> (or the status / error sentences with it) changed.</summary>
        public event Action StateChanged;

        /// <summary><see cref="Room"/> changed (null: no room any more).</summary>
        public event Action<RoomStateDto> RoomChanged;

        /// <summary><see cref="Rooms"/> was refreshed.</summary>
        public event Action<IReadOnlyList<RoomStateDto>> RoomListChanged;

        /// <summary>A game started or was resumed (<see cref="Game"/>).</summary>
        public event Action<StartGameDto> GameStarted;

        /// <summary>A move was applied.</summary>
        public event Action<GameUpdateDto> GameUpdated;

        /// <summary>One of this player's actions was refused: its seq and the reason code.</summary>
        public event Action<int, string> ActionRejected;

        /// <summary>The server's clocks.</summary>
        public event Action<ClocksDto> TimerSynced;

        /// <summary>A game ended: the result and its sentence (also a result missed while away).</summary>
        public event Action<EndGameDto, string> GameEnded;

        /// <summary>An error sentence to show (<see cref="LastError"/>): a failed login, a lost connection, a server error.</summary>
        public event Action<string> ErrorRaised;

        /// <summary>The page to open when the main menu next enters; consumed once by its presenter.</summary>
        public OnlineMenuPage? PendingPanelPage { get; private set; }

        /// <summary>The main menu is on display and can consume the requested panel page.</summary>
        public event Action PanelRequested;

        /// <summary>The panel's back command closes the submenu.</summary>
        public event Action PanelClosed;

        /// <summary>Consumes the pending page, including a request made while another screen was leaving.</summary>
        public OnlineMenuPage? TakePanelPage()
        {
            OnlineMenuPage? page = PendingPanelPage;
            PendingPanelPage = null;
            return page;
        }

        /// <summary>Returns to the main menu with the Multiplayer panel on the requested page.</summary>
        /// <param name="page">The panel page.</param>
        public void ShowPanel(OnlineMenuPage page)
        {
            PendingPanelPage = page;
            _navigator.Show(ScreenId.MainMenu);
            PanelRequested?.Invoke();
        }

        // ----- Entering and logging in -----

        /// <summary>
        /// Opens the connection choice before entering the online service or account-free LAN play.
        /// </summary>
        public void EnterMultiplayer() => ShowPanel(OnlineMenuPage.Connection);

        /// <summary>Enters the selected online service: a current or remembered login skips password entry; a restored seat opens its room.</summary>
        public void EnterOnline()
        {
            switch (State)
            {
                case OnlineState.LoggedIn:
                    ShowPanel(OnlineMenuPage.GameKinds);
                    return;
                case OnlineState.ConnectionLost:
                    ShowPanel(OnlineMenuPage.Lobby);
                    return;
                case OnlineState.InRoom:
                case OnlineState.InGame:
                case OnlineState.Reconnecting:
                    _navigator.Show(ScreenId.Room);
                    return;
                case OnlineState.LoggingIn:
                    ShowPanel(OnlineMenuPage.Login);
                    return;
            }

            SetError(null, raise: false);
            if (!_loginRunning && _credentials.TryRead(LoginTokenKey, out string token) && !string.IsNullOrEmpty(token))
            {
                Username = RememberedAccount;
                StartLogin(LoginCredentials.WithToken(token), LoginPurpose.RememberedToken);
            }
            ShowPanel(OnlineMenuPage.Login);
        }

        /// <summary>
        /// The login screen's 登入: logs in with the email and password; <paramref name="remember"/>
        /// keeps the returned login token for the automatic login (otherwise any remembered one is
        /// forgotten). Success opens the panel's kinds; failure → <see cref="LastError"/>.
        /// </summary>
        /// <param name="email">The account's email.</param>
        /// <param name="password">The password.</param>
        /// <param name="remember">記住帳密.</param>
        /// <returns>Whether the login started (false: busy, already logged in, or a field is empty: <see cref="LastError"/>).</returns>
        public bool Login(string email, string password, bool remember)
        {
            if (_loginRunning || (State != OnlineState.LoggedOut && State != OnlineState.ConnectionLost))
                return false;
            email = email?.Trim() ?? "";
            if (email.Length == 0 || string.IsNullOrEmpty(password))
            {
                SetError(OnlineTexts.MissingCredentials);
                StateChanged?.Invoke();
                return false;
            }

            Username = email;
            StartLogin(LoginCredentials.WithPassword(email, password, remember), LoginPurpose.Password);
            return true;
        }

        /// <summary>Stops a running login (the login screen's back while 連線中…): not logged in afterwards.</summary>
        public void CancelLogin()
        {
            if (State != OnlineState.LoggingIn)
                return;
            _generation++;
            StatusSentence = null;
            SetState(OnlineState.LoggedOut);
            _channel.Disconnect();
        }

        /// <summary>重新連線 after the connection was lost outside a room (<see cref="CanReconnect"/>).</summary>
        public void Reconnect()
        {
            if (!CanReconnect)
                return;
            LoginCredentials login = ReconnectLogin();
            if (login == null)
            {
                EndToLogin(null);
                return;
            }
            StartLogin(login, LoginPurpose.ManualReconnect);
        }

        /// <summary>
        /// 登出: the server revokes the login token, takes this player out of its room at once (a
        /// running game is resigned) and closes the connection; the remembered login is forgotten;
        /// back to the Multiplayer panel's login page.
        /// </summary>
        public void Logout()
        {
            string token = _refreshToken;
            if (token == null && _credentials.TryRead(LoginTokenKey, out string stored))
                token = stored;

            int generation = ++_generation;
            if (Game != null)
                _leftGameId = Game.GameId;
            if (_channel.Network.IsConnected)
                _channel.Logout(token);

            ForgetLogin();
            _passwordLogin = null;
            Username = null;
            AccountId = null;
            ClearRoom();
            StatusSentence = null;
            SetError(null, raise: false);
            SetState(OnlineState.LoggedOut);
            ShowPanel(OnlineMenuPage.Login);

            // The server closes the connection after its reply; close it here if it does not.
            Schedule(LogoutCloseTime, () =>
            {
                if (generation == _generation && State == OnlineState.LoggedOut && !_loginRunning)
                    _channel.Disconnect();
            });
        }

        /// <summary>Closes the Multiplayer submenu, staying logged in.</summary>
        public void LeaveToMainMenu()
        {
            PendingPanelPage = null;
            _navigator.Show(ScreenId.MainMenu);
            PanelClosed?.Invoke();
        }

        // ----- Lobby and room -----

        /// <summary>Asks for the waiting rooms (<see cref="RoomListChanged"/>).</summary>
        /// <returns>True when the request was sent.</returns>
        public bool RequestRoomList() => State == OnlineState.LoggedIn && _channel.RequestRoomList();

        /// <summary>Creates a custom room and takes its first seat (then <see cref="RoomChanged"/> and the room screen, or <see cref="ErrorRaised"/>); clears <see cref="LastError"/>.</summary>
        /// <param name="settings">The room's settings.</param>
        /// <returns>True when the request was sent.</returns>
        public bool CreateRoom(RoomSettingsDto settings)
        {
            ArgumentNullException.ThrowIfNull(settings);
            if (State != OnlineState.LoggedIn)
                return false;
            SetError(null, raise: false);
            return _channel.CreateRoom(settings);
        }

        /// <summary>Takes a seat in the room <paramref name="roomId"/> (then the room screen, or <see cref="ErrorRaised"/>); clears <see cref="LastError"/>.</summary>
        /// <param name="roomId">The room number.</param>
        /// <returns>True when the request was sent.</returns>
        public bool JoinRoom(string roomId)
        {
            ArgumentNullException.ThrowIfNull(roomId);
            if (State != OnlineState.LoggedIn)
                return false;
            SetError(null, raise: false);
            return _channel.JoinRoom(roomId);
        }

        /// <summary>Sets this player's ready mark (the game starts when every seat is taken and ready).</summary>
        /// <param name="ready">Ready or not.</param>
        /// <returns>True when the request was sent.</returns>
        public bool SetReady(bool ready) => State == OnlineState.InRoom && _channel.SetReady(ready);

        /// <summary>Leaves the room (during its game this resigns it) and goes back to the lobby.</summary>
        public void LeaveRoom()
        {
            if (State != OnlineState.InRoom && State != OnlineState.InGame)
                return;
            if (State == OnlineState.InGame && Game != null)
                _leftGameId = Game.GameId;
            _channel.LeaveRoom();
            ClearRoom();
            SetState(OnlineState.LoggedIn);
            ShowPanel(OnlineMenuPage.Lobby);
        }

        /// <summary>Asks to move a piece (FEN coordinates); see <see cref="OnlineChannel.SendMove"/>.</summary>
        /// <returns>The action's seq, or -1 when it was not sent.</returns>
        public int SendMove(int fromX, int fromY, int toX, int toY) =>
            State == OnlineState.InGame ? _channel.SendMove(fromX, fromY, toX, toY) : -1;

        /// <summary>Resigns the running game.</summary>
        /// <returns>The action's seq, or -1 when it was not sent.</returns>
        public int Resign() => State == OnlineState.InGame ? _channel.Resign() : -1;

        /// <summary>Stops listening to the channel.</summary>
        public void Dispose()
        {
            if (_disposed)
                return;
            _disposed = true;
            _channel.RoomList -= _onRoomList;
            _channel.RoomState -= _onRoomState;
            _channel.GameStarted -= _onGameStarted;
            _channel.GameUpdated -= _onGameUpdated;
            _channel.ActionRejected -= _onActionRejected;
            _channel.TimerSynced -= _onTimerSynced;
            _channel.GameEnded -= _onGameEnded;
            _channel.ServerError -= _onServerError;
            _channel.ReplacedByNewLogin -= _onReplaced;
            _channel.LoggedOut -= _onLoggedOut;
            _channel.Disconnected -= _onDisconnected;
        }

        // ----- Login -----

        /// <summary>Starts a login (never two at once); its result comes back on the UI thread.</summary>
        private void StartLogin(LoginCredentials login, LoginPurpose purpose)
        {
            int generation = ++_generation;
            _loginRunning = true;
            _roomSinceLogin = false;
            _gameSinceLogin = false;

            if (purpose != LoginPurpose.AutoReconnect)
            {
                StatusSentence = OnlineTexts.Connecting;
                SetState(OnlineState.LoggingIn, force: true);
            }

            // A connection still open (e.g. a replaced one not yet closed) would refuse the login.
            if (_channel.Network.IsConnected)
                _channel.Disconnect();

            Task<AuthResult> task;
            try
            {
                task = _channel.LoginAsync(login);
            }
            catch (Exception ex)
            {
                AppLogger.Log($"(Online) Login could not start: {ex.Message}", LogLevel.ERROR);
                task = Task.FromResult(AuthResult.Failure(AuthResult.ReasonConnectFailed));
            }

            task.ContinueWith(t =>
            {
                AuthResult result = t.Status == TaskStatus.RanToCompletion && t.Result != null
                    ? t.Result
                    : AuthResult.Failure(AuthResult.ReasonConnectFailed);
                Post(() => HandleLoginFinished(generation, login, purpose, result));
            }, TaskScheduler.Default);
        }

        private void HandleLoginFinished(int generation, LoginCredentials login, LoginPurpose purpose, AuthResult result)
        {
            _loginRunning = false;

            if (generation != _generation)
            {
                // Cancelled or logged out meanwhile. A token login that went through spent the
                // stored token: keep its replacement unless the login was forgotten since.
                if (result.Ok)
                {
                    if (result.RefreshToken != null && login.IsToken
                        && _credentials.TryRead(LoginTokenKey, out string stored) && stored == login.RefreshToken)
                        _credentials.Write(LoginTokenKey, result.RefreshToken);
                    _channel.Disconnect();
                }
                StateChanged?.Invoke();  // IsBusy changed
                return;
            }

            if (result.Ok)
                HandleLoginSucceeded(login, purpose, result);
            else
                HandleLoginFailed(purpose, result.Reason);
        }

        private void HandleLoginSucceeded(LoginCredentials login, LoginPurpose purpose, AuthResult result)
        {
            AccountId = SessionTokenClaims.TryReadAccountId(result.Token, out string id) ? id : null;
            StatusSentence = null;
            SetError(null, raise: false);

            if (!login.IsToken)
            {
                // Kept in memory only: a reconnect without a login token logs in with it.
                _passwordLogin = LoginCredentials.WithPassword(login.Email, login.Password);
                if (!login.Remember)
                {
                    _refreshToken = null;
                    ForgetLogin();
                }
                else if (result.RefreshToken != null)
                {
                    _credentials.Write(LoginAccountKey, login.Email);
                }
            }

            // Every token login spends its token: the replacement is the one to use from now on.
            if (result.RefreshToken != null)
            {
                _refreshToken = result.RefreshToken;
                if (!_credentials.Write(LoginTokenKey, result.RefreshToken))
                    AppLogger.Log("(Online) The login token could not be stored; the next start asks for the password", LogLevel.WARN);
            }

            AppLogger.Log($"(Online) Logged in ({login}, {purpose})", LogLevel.INFO);

            // The room / game the server restores may have arrived before this result.
            OnlineState state = _gameSinceLogin && Game != null ? OnlineState.InGame
                : _roomSinceLogin && Room != null ? OnlineState.InRoom
                : OnlineState.LoggedIn;
            SetState(state, force: true);

            if (state != OnlineState.LoggedIn)
            {
                _navigator.Show(state == OnlineState.InGame ? ScreenId.Game : ScreenId.Room);
                return;
            }

            if (purpose != LoginPurpose.AutoReconnect)
            {
                ClearRoom();
                ShowPanel(purpose == LoginPurpose.ManualReconnect ? OnlineMenuPage.Lobby : OnlineMenuPage.GameKinds);
                return;
            }

            // A reconnect whose seat is gone gets no room: give the server's packets a moment.
            int generation = _generation;
            Schedule(ReconnectSettleTime, () =>
            {
                if (generation != _generation || State != OnlineState.LoggedIn || _roomSinceLogin)
                    return;
                ClearRoom();
                SetError(OnlineTexts.RoomLostAfterDisconnect);
                StateChanged?.Invoke();
                ShowPanel(OnlineMenuPage.Lobby);
            });
        }

        private void HandleLoginFailed(LoginPurpose purpose, string reason)
        {
            AppLogger.Log($"(Online) Login failed ({purpose}): {reason}", LogLevel.WARN);

            if (reason == AuthResult.ReasonInvalidLoginToken)
            {
                _refreshToken = null;
                ForgetLogin();
            }

            switch (purpose)
            {
                case LoginPurpose.AutoReconnect:
                    if (IsFinal(reason) || ReconnectLogin() == null)
                        EndToLogin(reason);
                    else
                        ScheduleReconnectAttempt();
                    return;

                case LoginPurpose.ManualReconnect:
                    if (IsFinal(reason) || ReconnectLogin() == null)
                    {
                        EndToLogin(reason);
                        return;
                    }
                    StatusSentence = null;
                    SetError(OnlineTexts.LoginFailure(reason));
                    SetState(OnlineState.ConnectionLost, force: true);
                    return;

                default:
                    StatusSentence = null;
                    Username = null;
                    SetError(OnlineTexts.LoginFailure(reason));
                    SetState(OnlineState.LoggedOut, force: true);
                    return;
            }
        }

        /// <summary>Whether a login failure cannot change by trying again.</summary>
        private static bool IsFinal(string reason) =>
            reason == AuthResult.ReasonInvalidCredentials
            || reason == AuthResult.ReasonInvalidLoginToken
            || reason == AuthResult.ReasonAlreadyLoggedIn
            || reason == AuthResult.ReasonVersionMismatch
            || reason == AuthResult.ReasonMissingCredentials;

        /// <summary>The login a reconnect uses: the newest login token, else the password of this session; null when neither.</summary>
        private LoginCredentials ReconnectLogin()
        {
            if (_refreshToken != null)
                return LoginCredentials.WithToken(_refreshToken);
            return _passwordLogin;
        }

        /// <summary>Gives up on the session: logged out, the login screen with the reason (null: none).</summary>
        private void EndToLogin(string reason)
        {
            ClearRoom();
            StatusSentence = null;
            AccountId = null;
            SetError(reason == null ? null : OnlineTexts.LoginFailure(reason));
            SetState(OnlineState.LoggedOut, force: true);
            ShowPanel(OnlineMenuPage.Login);
        }

        /// <summary>Forgets the remembered login (the token and its account) on this machine.</summary>
        private void ForgetLogin()
        {
            _credentials.Delete(LoginTokenKey);
            _credentials.Delete(LoginAccountKey);
        }

        // ----- Automatic reconnect -----

        private void BeginAutoReconnect()
        {
            bool inGame = State == OnlineState.InGame;
            _reconnectLimit = ReconnectPolicy.Limit(inGame);
            _reconnectClock = Stopwatch.StartNew();
            _reconnectAttempt = 0;
            StatusSentence = OnlineTexts.Reconnecting;
            AppLogger.Log($"(Online) Connection lost {(inGame ? "in a game" : "in a room")}; reconnecting for {_reconnectLimit.TotalSeconds:0} s", LogLevel.WARN);
            SetState(OnlineState.Reconnecting);
            if (ReconnectLogin() == null)
            {
                EndToLogin(null);
                return;
            }
            ScheduleReconnectAttempt();
        }

        private void ScheduleReconnectAttempt()
        {
            TimeSpan delay = ReconnectPolicy.DelayBefore(_reconnectAttempt);
            if (_reconnectClock.Elapsed + delay > _reconnectLimit)
            {
                GiveUpReconnect();
                return;
            }

            int generation = _generation;
            Schedule(delay, () =>
            {
                if (generation != _generation || State != OnlineState.Reconnecting || _loginRunning)
                    return;
                _reconnectAttempt++;
                LoginCredentials login = ReconnectLogin();
                if (login == null)
                {
                    EndToLogin(null);
                    return;
                }
                StartLogin(login, LoginPurpose.AutoReconnect);
            });
        }

        /// <summary>The seat is no longer held: the room is gone; the lobby panel offers 重新連線.</summary>
        private void GiveUpReconnect()
        {
            AppLogger.Log("(Online) Reconnect gave up: the seat is no longer held", LogLevel.WARN);
            ClearRoom();
            StatusSentence = null;
            SetError(OnlineTexts.RoomLostAfterDisconnect);
            SetState(OnlineState.ConnectionLost, force: true);
            ShowPanel(OnlineMenuPage.Lobby);
        }

        // ----- Channel events (on the UI thread) -----

        private void HandleRoomList(IReadOnlyList<RoomStateDto> rooms)
        {
            Rooms = rooms ?? Array.Empty<RoomStateDto>();
            RoomListChanged?.Invoke(Rooms);
        }

        private void HandleRoomState(RoomStateDto room)
        {
            if (!IsConnectedState && State != OnlineState.LoggingIn && State != OnlineState.Reconnecting)
                return;
            if (room == null)
                return;

            SeatDto mine = FindMySeat(room);
            if (mine == null && AccountId != null)
                return;  // Not this player's room (any more)

            if (mine != null && !string.IsNullOrEmpty(mine.Name))
                Username = mine.Name;

            Room = room;
            _roomSinceLogin = true;
            RoomChanged?.Invoke(room);

            if (State == OnlineState.LoggedIn)
            {
                SetState(OnlineState.InRoom);
                _navigator.Show(ScreenId.Room);
            }
        }

        private void HandleGameStarted(StartGameDto start)
        {
            if (!IsConnectedState && State != OnlineState.LoggingIn && State != OnlineState.Reconnecting)
                return;
            if (start == null)
                return;

            Game = start;
            _gameSinceLogin = true;
            _leftGameId = null;

            bool wasLoggedIn = State == OnlineState.LoggedIn;
            if (wasLoggedIn || State == OnlineState.InRoom)
                SetState(OnlineState.InGame);
            GameStarted?.Invoke(start);
        }

        private void HandleGameEnded(EndGameDto end)
        {
            if (end == null || State == OnlineState.LoggedOut)
                return;

            bool sameGame = Game != null && (string.IsNullOrEmpty(end.GameId) || end.GameId == Game.GameId);
            string sentence = sameGame ? GameOverSentence(Game, end) : OnlineTexts.MissedGameOver;
            bool left = _leftGameId != null && (string.IsNullOrEmpty(end.GameId) || end.GameId == _leftGameId);
            bool missed = State != OnlineState.InGame && !left;

            LastGameEnd = end;
            LastGameEndSentence = sentence;
            _leftGameId = null;
            if (State == OnlineState.InGame)
                SetState(OnlineState.InRoom);
            GameEnded?.Invoke(end, sentence);

            // A result that came after a new login (the game ended while away): nothing else shows it.
            if (missed)
                _dialogs.ShowConfirm(sentence, ConfirmDialogType.Ok, _ => { });
        }

        private void HandleServerError(string code, string detail)
        {
            AppLogger.Log($"(Online) Server error {code}{(string.IsNullOrEmpty(detail) ? "" : ": " + detail)}", LogLevel.WARN);
            SetError(OnlineTexts.ServerError(code));
        }

        private void HandleReplacedByNewLogin()
        {
            if (State == OnlineState.LoggedOut)
                return;
            AppLogger.Log("(Online) Replaced by a new login of this account elsewhere", LogLevel.WARN);
            _generation++;
            ClearRoom();
            StatusSentence = null;
            SetError(OnlineTexts.ReplacedByNewLogin, raise: false);
            SetState(OnlineState.LoggedOut, force: true);
            ShowPanel(OnlineMenuPage.Login);
            _dialogs.ShowConfirm(OnlineTexts.ReplacedByNewLogin, ConfirmDialogType.Ok, _ => { });
            ErrorRaised?.Invoke(LastError);
        }

        private void HandleLoggedOut()
        {
            // The server closes the connection too; nothing of this session is left.
            if (State == OnlineState.LoggedOut && !_loginRunning)
                _channel.Disconnect();
        }

        private void HandleDisconnected()
        {
            switch (State)
            {
                case OnlineState.InRoom:
                case OnlineState.InGame:
                    BeginAutoReconnect();
                    break;
                case OnlineState.LoggedIn:
                    AppLogger.Log("(Online) Connection lost outside a room", LogLevel.WARN);
                    SetError(OnlineTexts.ConnectionLost);
                    SetState(OnlineState.ConnectionLost);
                    break;
                default:
                    // Logged out, or a login's own failure (its result handles it).
                    break;
            }
        }

        // ----- Helpers -----

        private bool IsConnectedState =>
            State == OnlineState.LoggedIn || State == OnlineState.InRoom || State == OnlineState.InGame;

        private SeatDto FindMySeat(RoomStateDto room)
        {
            if (room?.Seats == null)
                return null;
            foreach (SeatDto seat in room.Seats)
            {
                if (seat != null && AccountId != null && seat.Id == AccountId)
                    return seat;
            }
            return null;
        }

        /// <summary>The result's sentence, with the players' names of <paramref name="game"/>.</summary>
        private static string GameOverSentence(StartGameDto game, EndGameDto end)
        {
            string NameOf(int side) =>
                game.Players != null && side >= 1 && side <= game.Players.Count ? game.Players[side - 1]?.Name : null;

            if (end.Winner == 0)
                return OnlineTexts.GameOver(0, null, null, end.Reason);
            int loser = end.Winner == 1 ? 2 : 1;
            return OnlineTexts.GameOver(end.Winner, NameOf(end.Winner), NameOf(loser), end.Reason);
        }

        private void ClearRoom()
        {
            bool hadRoom = Room != null;
            Room = null;
            Game = null;
            if (hadRoom)
                RoomChanged?.Invoke(null);
        }

        private void SetState(OnlineState state, bool force = false)
        {
            if (State == state && !force)
                return;
            State = state;
            StateChanged?.Invoke();
        }

        private void SetError(string sentence, bool raise = true)
        {
            LastError = sentence;
            if (raise && sentence != null)
                ErrorRaised?.Invoke(sentence);
        }

        private void Post(Action action) => _mainThread.Post(action);

        /// <summary>Runs <paramref name="action"/> on the UI thread after <paramref name="delay"/>.</summary>
        private void Schedule(TimeSpan delay, Action action) =>
            Task.Delay(delay).ContinueWith(_ => Post(action), TaskScheduler.Default);
    }
}

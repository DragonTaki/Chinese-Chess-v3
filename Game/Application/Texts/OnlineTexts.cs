/* ----- ----- ----- ----- */
// OnlineTexts.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/12
// Update Date: 2026/10/13
// Version: v1.1
/* ----- ----- ----- ----- */

using System;

using Chinese_Chess_v3.Game.Application.Online;
using Chinese_Chess_v3.Game.Application.Settings;
using Chinese_Chess_v3.Game.Configs;
using Chinese_Chess_v3.Network;

using Engine.Localization;

namespace Chinese_Chess_v3.Game.Application.Texts
{
    /// <summary>
    /// Texts of online play (多人連線): the login screen, the online menu (game kinds, modes, lobby),
    /// the room, and every sentence the online session reports (login failures, connection
    /// problems, the server's error codes, a game's result). Every text comes from the language
    /// files (<see cref="Lang"/>, keys <c>online.*</c>); this class only picks the key and fills in
    /// the parameters.
    /// </summary>
    public static class OnlineTexts
    {
        public static string Unavailable => Lang.Get("online.game_unavailable");
        public static string MoveRejected => Lang.Get("online.move_rejected");
        public static string OutOfSync => Lang.Get("online.out_of_sync");
        public static string AwaySuffix => Lang.Get("online.away_suffix");

        // ----- Login screen -----

        /// <summary>The login screen's title and its submit button.</summary>
        public static string Login => Lang.Get("online.login");

        /// <summary>The account field's label (the account is its email).</summary>
        public static string Account => Lang.Get("online.account");

        /// <summary>The password field's label.</summary>
        public static string Password => Lang.Get("online.password");

        /// <summary>The 記住帳密 check box (log in automatically next time).</summary>
        public static string RememberLogin => Lang.Get("online.remember_login");

        /// <summary>Shown while a login (or a reconnect's) is running.</summary>
        public static string Connecting => Lang.Get("online.connecting");

        /// <summary>Shown while the connection of a room or game is being restored.</summary>
        public static string Reconnecting => Lang.Get("online.reconnecting");

        /// <summary>The 重新連線 action after the connection was lost outside a room.</summary>
        public static string Reconnect => Lang.Get("online.reconnect");

        /// <summary>The login screen's way back to the main menu.</summary>
        public static string Back => Lang.Get("online.back");

        // ----- Login failures and connection problems -----

        /// <summary>The account field or the password is empty (checked before anything is sent).</summary>
        public static string MissingCredentials => Lang.Get("online.error.missing_credentials");

        /// <summary>
        /// The sentence for a login's failure reason (<see cref="AuthResult.Reason"/>): the server's
        /// codes and the client-side ones; an unknown code is shown inside <see cref="UnknownError"/>.
        /// </summary>
        /// <param name="reason">The reason code.</param>
        /// <returns>The sentence.</returns>
        public static string LoginFailure(string reason) => reason switch
        {
            AuthResult.ReasonInvalidCredentials => Lang.Get("online.error.invalid_credentials"),
            AuthResult.ReasonVersionMismatch => Lang.Get("online.error.version_mismatch"),
            AuthResult.ReasonMissingCredentials => MissingCredentials,
            AuthResult.ReasonAlreadyLoggedIn => Lang.Get("online.error.already_logged_in"),
            AuthResult.ReasonInvalidLoginToken => Lang.Get("online.error.invalid_login_token"),
            AuthResult.ReasonConnectFailed => ConnectFailed,
            AuthResult.ReasonDisconnected => ConnectionLost,
            AuthResult.ReasonAlreadyConnected => ConnectionLost,
            _ => UnknownError(reason),
        };

        /// <summary>The server could not be reached.</summary>
        public static string ConnectFailed => Lang.Get("online.error.connect_failed");

        /// <summary>The connection ended unexpectedly (and was not restored).</summary>
        public static string ConnectionLost => Lang.Get("online.error.connection_lost");

        /// <summary>The connection was lost for longer than the seat is held: the room (or game) is gone.</summary>
        public static string RoomLostAfterDisconnect => Lang.Get("online.error.room_lost");

        /// <summary>A new login of this account elsewhere replaced this connection (no automatic reconnect).</summary>
        public static string ReplacedByNewLogin => Lang.Get("online.error.replaced_by_new_login");

        /// <summary>An error without its own sentence; <paramref name="code"/> = its code.</summary>
        /// <param name="code">The code.</param>
        /// <returns>The sentence.</returns>
        public static string UnknownError(string code) => Lang.Get("online.error.unknown", code ?? "");

        /// <summary>
        /// The sentence for a server error code (an Error packet's code, see the server's room.go
        /// and game.go); an unknown code is shown inside <see cref="UnknownError"/>.
        /// </summary>
        /// <param name="code">The error code.</param>
        /// <returns>The sentence.</returns>
        public static string ServerError(string code) => code switch
        {
            "InvalidData" => Lang.Get("online.server_error.invalid_data"),
            "KindNotOpen" => Lang.Get("online.server_error.kind_not_open"),
            "ModeNotOpen" => Lang.Get("online.server_error.mode_not_open"),
            "InvalidSettings" => Lang.Get("online.server_error.invalid_settings"),
            "AlreadyInRoom" => Lang.Get("online.server_error.already_in_room"),
            "NotInRoom" => Lang.Get("online.server_error.not_in_room"),
            "RoomNotFound" => Lang.Get("online.server_error.room_not_found"),
            "RoomFull" => Lang.Get("online.server_error.room_full"),
            "RoomNotWaiting" => Lang.Get("online.server_error.room_not_waiting"),
            "NoGame" => Lang.Get("online.server_error.no_game"),
            "NotYourTurn" => Lang.Get("online.server_error.not_your_turn"),
            "RulesUnavailable" => Lang.Get("online.server_error.rules_unavailable"),
            _ => UnknownError(code),
        };

        // ----- Online menu: game kinds and modes -----

        /// <summary>The game kind page's title.</summary>
        public static string ChooseGameKind => Lang.Get("online.choose_kind");

        /// <summary>The mode page's title.</summary>
        public static string ChooseMode => Lang.Get("online.choose_mode");

        /// <summary>A mode's name (the custom room is the only one wired so far, author §8.26: all look the same).</summary>
        /// <param name="mode">The mode.</param>
        /// <returns>Its name.</returns>
        public static string ModeName(OnlineMode mode) => mode switch
        {
            OnlineMode.Ranked => Lang.Get("online.mode.ranked"),
            OnlineMode.Evaluation => Lang.Get("online.mode.evaluation"),
            OnlineMode.Arena => Lang.Get("online.mode.arena"),
            OnlineMode.CustomRoom => Lang.Get("online.mode.custom_room"),
            _ => throw new ArgumentOutOfRangeException(nameof(mode), mode, "Unknown online mode"),
        };

        /// <summary>The 登出 action.</summary>
        public static string Logout => Lang.Get("online.logout");

        /// <summary>The logged-in account shown on the online menu; <paramref name="name"/> = its name.</summary>
        /// <param name="name">The account's name.</param>
        /// <returns>The sentence.</returns>
        public static string LoggedInAs(string name) => Lang.Get("online.logged_in_as", name ?? "");

        // ----- Lobby (the custom room page of the online menu) -----

        /// <summary>The 重新整理 action (the list also refreshes itself).</summary>
        public static string Refresh => Lang.Get("online.refresh");

        /// <summary>The 建立房間 action.</summary>
        public static string CreateRoom => Lang.Get("online.create_room");

        /// <summary>The 加入 action (by room number or from the list).</summary>
        public static string Join => Lang.Get("online.join");

        /// <summary>The room number field's label.</summary>
        public static string RoomNumber => Lang.Get("online.room_number");

        /// <summary>The list's text when there is no waiting room.</summary>
        public static string NoRooms => Lang.Get("online.no_rooms");

        /// <summary>The header above the waiting rooms.</summary>
        public static string RoomList => Lang.Get("online.room_list");

        /// <summary>The header above a new room's settings.</summary>
        public static string NewRoomSettings => Lang.Get("online.new_room_settings");

        /// <summary>A room settings editor's rule section header (the room's rules, not the local ones).</summary>
        public static string SectionRules => Lang.Get("online.section_rules");

        /// <summary>A room settings editor's clock section header.</summary>
        public static string SectionTimer => Lang.Get("online.section_timer");

        /// <summary>A room settings editor's own section header (first mover, pausing).</summary>
        public static string SectionRoom => Lang.Get("online.section_room");

        /// <summary>A room number that is not six digits.</summary>
        public static string InvalidRoomNumber => Lang.Get("online.error.invalid_room_number");

        /// <summary>A room's title; <paramref name="roomId"/> = its number.</summary>
        /// <param name="roomId">The room number.</param>
        /// <returns>The title.</returns>
        public static string RoomTitle(string roomId) => Lang.Get("online.room_title", roomId ?? "");

        /// <summary>How many seats of a room are taken; e.g. 1/2.</summary>
        /// <param name="taken">The taken seats.</param>
        /// <param name="total">All seats.</param>
        /// <returns>The text.</returns>
        public static string SeatCount(int taken, int total) => Lang.Get("online.seat_count", taken, total);

        /// <summary>A lobby row: the room's title, its host, its seats; e.g. 房間 123456　Alice　1/2.</summary>
        /// <param name="title">The room's title.</param>
        /// <param name="host">The host's name.</param>
        /// <param name="seats">The seat count text.</param>
        /// <returns>The row.</returns>
        public static string LobbyRow(string title, string host, string seats) => Lang.Get("online.lobby_row", title, host ?? "", seats);

        // ----- Room -----

        /// <summary>The 準備 toggle (pressed while ready).</summary>
        public static string Ready => Lang.Get("online.ready");

        /// <summary>The 取消準備 toggle text while ready.</summary>
        public static string CancelReady => Lang.Get("online.cancel_ready");

        /// <summary>The 離開房間 action.</summary>
        public static string LeaveRoom => Lang.Get("online.leave_room");

        /// <summary>Asks before leaving a room during its game (leaving resigns it).</summary>
        public static string ConfirmLeaveDuringGame => Lang.Get("online.confirm_leave_during_game");

        /// <summary>The host mark of a seat.</summary>
        public static string Host => Lang.Get("online.host");

        /// <summary>A free seat.</summary>
        public static string WaitingForPlayer => Lang.Get("online.waiting_for_player");

        /// <summary>A seat whose player's connection ended (held for its return).</summary>
        public static string Away => Lang.Get("online.away");

        /// <summary>A seat's ready mark.</summary>
        public static string IsReady => Lang.Get("online.is_ready");

        /// <summary>A seat that is not ready yet.</summary>
        public static string NotReady => Lang.Get("online.not_ready");

        /// <summary>The room's state while its game runs.</summary>
        public static string GameInProgress => Lang.Get("online.game_in_progress");

        /// <summary>A seat's line: the name and its marks (host, ready, away), e.g. Alice（房主、準備）.</summary>
        /// <param name="name">The player's name.</param>
        /// <param name="marks">The marks, already joined (empty for none).</param>
        /// <returns>The line.</returns>
        public static string SeatLine(string name, string marks) =>
            string.IsNullOrEmpty(marks) ? name ?? "" : Lang.Get("online.seat_line", name ?? "", marks);

        /// <summary>The room settings' first-mover line value.</summary>
        /// <param name="firstMover">The server's first-mover name (Host, Guest, Random).</param>
        /// <returns>The value text.</returns>
        public static string FirstMoverName(string firstMover) => firstMover switch
        {
            "Host" => Lang.Get("online.first_mover.host"),
            "Guest" => Lang.Get("online.first_mover.guest"),
            "Random" => Lang.Get("online.first_mover.random"),
            _ => firstMover ?? "",
        };

        /// <summary>The header above a room's seats.</summary>
        public static string Seats => Lang.Get("online.seats");

        /// <summary>The header above a room's settings.</summary>
        public static string RoomSettings => Lang.Get("online.room_settings");

        /// <summary>The room settings' first-mover line name.</summary>
        public static string FirstMover => Lang.Get("online.first_mover");

        /// <summary>The room settings' pause line name.</summary>
        public static string AllowPause => Lang.Get("online.allow_pause");

        // ----- Game result -----

        /// <summary>
        /// A game's result: who won and how, from the loser's side. <paramref name="winnerName"/>
        /// null or <paramref name="loserName"/> null are taken as unknown players.
        /// </summary>
        /// <param name="winner">The winner's player number (0: a draw).</param>
        /// <param name="winnerName">The winner's name.</param>
        /// <param name="loserName">The loser's name.</param>
        /// <param name="reason">The server's end reason (e.g. Resign, TimeUp, Disconnect, Checkmate).</param>
        /// <returns>The sentence.</returns>
        public static string GameOver(int winner, string winnerName, string loserName, string reason)
        {
            if (winner == 0)
                return Lang.Get("online.game_over.draw");
            return Lang.Get("online.game_over.win", winnerName ?? "", EndReason(reason, loserName ?? ""));
        }

        /// <summary>A game that ended while this player was away, when it is no longer known which game it was.</summary>
        public static string MissedGameOver => Lang.Get("online.game_over.missed");

        /// <summary>What ended a game, said of the loser; an unknown reason shows its code.</summary>
        /// <param name="reason">The server's end reason.</param>
        /// <param name="loserName">The loser's name.</param>
        /// <returns>The text.</returns>
        public static string EndReason(string reason, string loserName) => reason switch
        {
            "Checkmate" => Lang.Get("online.end_reason.checkmate", loserName),
            "Stalemate" => Lang.Get("online.end_reason.stalemate", loserName),
            "TimeUp" => Lang.Get("online.end_reason.time_up", loserName),
            "Resign" => Lang.Get("online.end_reason.resign", loserName),
            "Disconnect" => Lang.Get("online.end_reason.disconnect", loserName),
            "NoPiecesLeft" => Lang.Get("online.end_reason.no_pieces_left", loserName),
            _ => Lang.Get("online.end_reason.other", loserName, reason ?? ""),
        };

        // ----- Room settings summary -----

        /// <summary>A rule switch's line of a room's settings, e.g. 象眼可被塞：開.</summary>
        /// <param name="option">The rule option.</param>
        /// <param name="on">Its value.</param>
        /// <returns>The line.</returns>
        public static string RuleLine(RuleOption option, bool on) =>
            SettingsTexts.SettingText(SettingsMenuContent.OptionName(option), on ? SettingsTexts.On : SettingsTexts.Off);
    }
}

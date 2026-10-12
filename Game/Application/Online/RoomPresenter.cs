/* ----- ----- ----- ----- */
// RoomPresenter.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/12
// Update Date: 2026/10/12
// Version: v1.0
/* ----- ----- ----- ----- */

using System;
using System.Collections.Generic;

using Chinese_Chess_v3.Game.Application.Services;
using Chinese_Chess_v3.Game.Application.Texts;
using Chinese_Chess_v3.Network.Protocol;

using Engine.Localization;

namespace Chinese_Chess_v3.Game.Application.Online
{
    /// <summary>One seat of the room, as the view shows it.</summary>
    public sealed class RoomSeatRow
    {
        /// <summary>Creates a row.</summary>
        public RoomSeatRow(string text, bool isEmpty, bool isMe, bool isHost, bool isReady, bool isAway)
        {
            Text = text;
            IsEmpty = isEmpty;
            IsMe = isMe;
            IsHost = isHost;
            IsReady = isReady;
            IsAway = isAway;
        }

        /// <summary>The seat's line: the player's name and marks (房主, 準備, 斷線中), or 等待玩家… when free.</summary>
        public string Text { get; }

        /// <summary>Whether the seat is free.</summary>
        public bool IsEmpty { get; }

        /// <summary>Whether this player sits here.</summary>
        public bool IsMe { get; }

        /// <summary>Whether the room's host sits here.</summary>
        public bool IsHost { get; }

        /// <summary>Whether the player is ready.</summary>
        public bool IsReady { get; }

        /// <summary>Whether the player's connection ended (the seat is held for its return).</summary>
        public bool IsAway { get; }
    }

    /// <summary>
    /// The room screen's decisions: the seats (host, ready, away), 準備 / 取消準備, 離開房間 (asking
    /// first during the game: leaving resigns it), the room's settings as sentences, the status
    /// (重新連線中…, 對局進行中) and the server's errors as sentences. Everything comes from the
    /// <see cref="OnlineSession"/>; the view redraws on <see cref="Changed"/>.
    /// </summary>
    public sealed class RoomPresenter
    {
        private readonly OnlineSession _session;
        private readonly IDialogService _dialogs;
        private string _error;

        /// <summary>Creates the presenter (a single long-lived instance, like the screen it drives).</summary>
        /// <param name="session">The online session.</param>
        /// <param name="dialogs">Asks before leaving during a game.</param>
        public RoomPresenter(OnlineSession session, IDialogService dialogs)
        {
            _session = session ?? throw new ArgumentNullException(nameof(session));
            _dialogs = dialogs ?? throw new ArgumentNullException(nameof(dialogs));
            _session.RoomChanged += _ =>
            {
                _error = null;
                Changed?.Invoke();
            };
            _session.StateChanged += () => Changed?.Invoke();
            _session.GameStarted += _ => Changed?.Invoke();
            _session.GameEnded += (_, _) => Changed?.Invoke();
            _session.ErrorRaised += sentence =>
            {
                if (_session.Room != null)
                {
                    _error = sentence;
                    Changed?.Invoke();
                }
            };
        }

        // ----- Texts -----

        /// <summary>The room's title (房間 123456), or empty without a room.</summary>
        public string TitleText => _session.Room == null ? "" : OnlineTexts.RoomTitle(_session.Room.RoomId);

        /// <summary>The ready toggle's text: 準備, or 取消準備 while ready.</summary>
        public string ReadyText => IsReady ? OnlineTexts.CancelReady : OnlineTexts.Ready;

        /// <summary>The 離開房間 button's text.</summary>
        public string LeaveText => OnlineTexts.LeaveRoom;

        // ----- State -----

        /// <summary>The seats in seat order.</summary>
        public IReadOnlyList<RoomSeatRow> Seats
        {
            get
            {
                var rows = new List<RoomSeatRow>();
                RoomStateDto room = _session.Room;
                if (room?.Seats == null)
                    return rows;
                SeatDto mine = _session.MySeat;
                foreach (SeatDto seat in room.Seats)
                {
                    if (seat == null)
                    {
                        rows.Add(new RoomSeatRow(OnlineTexts.WaitingForPlayer, true, false, false, false, false));
                        continue;
                    }
                    bool isHost = seat.Id == room.HostId;
                    var marks = new List<string>();
                    if (isHost)
                        marks.Add(OnlineTexts.Host);
                    if (seat.Away)
                        marks.Add(OnlineTexts.Away);
                    else if (room.State == RoomNames.StateWaiting)
                        marks.Add(seat.Ready ? OnlineTexts.IsReady : OnlineTexts.NotReady);
                    rows.Add(new RoomSeatRow(OnlineTexts.SeatLine(seat.Name, Lang.JoinList(marks)), false,
                        ReferenceEquals(seat, mine), isHost, seat.Ready, seat.Away));
                }
                return rows;
            }
        }

        /// <summary>The room's settings, one sentence per line.</summary>
        public IReadOnlyList<string> SettingsSummary => OnlineRoomSettings.Summary(_session.Room?.Settings);

        /// <summary>Whether this player is marked ready.</summary>
        public bool IsReady => _session.MySeat?.Ready == true;

        /// <summary>Whether 準備 can be pressed (in a waiting room, connected).</summary>
        public bool CanToggleReady => _session.State == OnlineState.InRoom && _session.Room?.State == RoomNames.StateWaiting;

        /// <summary>Whether this player hosts the room.</summary>
        public bool IsHost => _session.IsHost;

        /// <summary>重新連線中… while the connection is restored, 對局進行中 during the game, otherwise null.</summary>
        public string StatusSentence => _session.State switch
        {
            OnlineState.Reconnecting => _session.StatusSentence,
            OnlineState.InGame => OnlineTexts.GameInProgress,
            _ => null,
        };

        /// <summary>The last game's result, or null.</summary>
        public string LastResultSentence => _session.LastGameEndSentence;

        /// <summary>The server's last error here as a sentence, or null.</summary>
        public string ErrorSentence => _error;

        /// <summary>The view redraws.</summary>
        public event Action Changed;

        // ----- Commands -----

        /// <summary>準備 / 取消準備.</summary>
        public void ToggleReady()
        {
            if (!CanToggleReady)
                return;
            _error = null;
            _session.SetReady(!IsReady);
        }

        /// <summary>離開房間: during the game asks first (leaving resigns it); back to the lobby.</summary>
        public void Leave()
        {
            if (_session.State == OnlineState.InGame)
            {
                _dialogs.ShowConfirm(OnlineTexts.ConfirmLeaveDuringGame, ConfirmDialogType.YesNo, result =>
                {
                    if (result == ConfirmDialogResult.Yes)
                        _session.LeaveRoom();
                });
                return;
            }
            _session.LeaveRoom();
        }
    }
}

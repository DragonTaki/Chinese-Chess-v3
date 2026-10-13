/* ----- ----- ----- ----- */
// OnlineGameBridge.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/13
// Update Date: 2026/10/13
// Version: v1.0
/* ----- ----- ----- ----- */

using System;

using Chinese_Chess_v3.Game.Application.GameLog;
using Chinese_Chess_v3.Game.Application.InfoBoards;
using Chinese_Chess_v3.Game.Application.Services;
using Chinese_Chess_v3.Game.Application.Session;
using Chinese_Chess_v3.Game.Application.Texts;
using Chinese_Chess_v3.Game.Core;
using Chinese_Chess_v3.Network.Protocol;

namespace Chinese_Chess_v3.Game.Application.Online
{
    /// <summary>Connects the server-authoritative online session to the game screen.</summary>
    public sealed class OnlineGameBridge : RemoteGameBridge
    {
        private readonly OnlineSession _online;
        private readonly INavigator _navigator;
        private readonly InfoBoardViewModel _info;

        public OnlineGameBridge(OnlineSession online, GameSession session, GameLogComposer log,
            INavigator navigator, InfoBoardViewModel info) : base(session, log)
        {
            _online = online ?? throw new ArgumentNullException(nameof(online));
            _navigator = navigator ?? throw new ArgumentNullException(nameof(navigator));
            _info = info ?? throw new ArgumentNullException(nameof(info));
            _online.GameStarted += Start;
            _online.GameUpdated += Apply;
            _online.ActionRejected += Reject;
            _online.TimerSynced += Sync;
            _online.GameEnded += EndOnline;
        }

        public string EndSentence { get; private set; }
        protected override string RejectedSentence => OnlineTexts.MoveRejected;
        protected override string OutOfSyncSentence => OnlineTexts.OutOfSync;
        protected override string DisconnectedSentence => OnlineTexts.ConnectionLost;

        protected override void Start(StartGameDto start)
        {
            EndSentence = null;
            _info.SetAway(null);
            base.Start(start);
        }

        protected override int SendMove(MoveRequest request) =>
            _online.SendMove(request.FromX, request.FromY, request.ToX, request.ToY);

        protected override int SendResign() => _online.Resign();

        protected override void Sync(ClocksDto clocks)
        {
            if (IsActive && clocks != null)
                _info.SetAway(clocks.Away);
            base.Sync(clocks);
        }

        private void Reject(int seq, string reason)
        {
            if (!IsActive)
                return;
            _session.Game.IsInputLocked = false;
            _log.Write(RejectedSentence);
        }

        private void EndOnline(EndGameDto end, string sentence)
        {
            if (!IsActive || (!string.IsNullOrEmpty(end.GameId) && end.GameId != _online.Game?.GameId))
                return;
            EndSentence = sentence;
            End(end);
        }

        public override void ShowRoom()
        {
            if (IsActive && _session.Game.IsGameOver)
                _navigator.Show(ScreenId.Room);
        }

        protected override void LeaveSource()
        {
            _info.SetAway(null);
            _online.LeaveRoom();
            _online.ShowPanel(OnlineMenuPage.Modes);
        }

        protected override void Unsubscribe()
        {
            _online.GameStarted -= Start;
            _online.GameUpdated -= Apply;
            _online.ActionRejected -= Reject;
            _online.TimerSynced -= Sync;
            _online.GameEnded -= EndOnline;
        }
    }
}

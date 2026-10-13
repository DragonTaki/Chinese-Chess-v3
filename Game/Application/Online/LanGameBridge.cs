/* ----- ----- ----- ----- */
// LanGameBridge.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/13
// Update Date: 2026/10/13
// Version: v1.1
/* ----- ----- ----- ----- */

using System;

using Chinese_Chess_v3.Game.Application.GameLog;
using Chinese_Chess_v3.Game.Application.Session;
using Chinese_Chess_v3.Game.Core;

namespace Chinese_Chess_v3.Game.Application.Online
{
    /// <summary>Routes LAN authority updates and board intents through the existing remote game APIs.</summary>
    public sealed class LanGameBridge : RemoteGameBridge
    {
        private readonly LanSession _lan;

        public LanGameBridge(LanSession lan, GameSession session, GameLogComposer log) : base(session, log)
        {
            _lan = lan ?? throw new ArgumentNullException(nameof(lan));
            _lan.GameStarted += Start;
            _lan.GameUpdated += Apply;
            _lan.TimerSynced += Sync;
            _lan.GameEnded += End;
        }

        protected override int SendMove(MoveRequest request) =>
            _lan.SendMove(request.FromX, request.FromY, request.ToX, request.ToY);

        protected override int SendResign() => _lan.Resign();
        protected override void LeaveSource() => _lan.Leave();

        protected override void Unsubscribe()
        {
            _lan.GameStarted -= Start;
            _lan.GameUpdated -= Apply;
            _lan.TimerSynced -= Sync;
            _lan.GameEnded -= End;
        }
    }
}

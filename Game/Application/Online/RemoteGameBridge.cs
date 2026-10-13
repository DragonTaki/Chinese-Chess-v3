/* ----- ----- ----- ----- */
// RemoteGameBridge.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/13
// Update Date: 2026/10/13
// Version: v1.1
/* ----- ----- ----- ----- */

using System;
using System.Linq;

using Chinese_Chess_v3.Game.Application.GameLog;
using Chinese_Chess_v3.Game.Application.Session;
using Chinese_Chess_v3.Game.Application.Texts;
using Chinese_Chess_v3.Game.Core;
using Chinese_Chess_v3.Game.Core.Players;
using Chinese_Chess_v3.Network.Protocol;

namespace Chinese_Chess_v3.Game.Application.Online
{
    /// <summary>Routes remote authority updates and board intents through the existing remote game APIs.</summary>
    public abstract class RemoteGameBridge : IRemoteGameBridge, IDisposable
    {
        private int _ply;
        protected readonly GameSession _session;
        protected readonly GameLogComposer _log;
        private GameManager _attachedGame;
        private bool _disposed;

        protected RemoteGameBridge(GameSession session, GameLogComposer log)
        {
            _session = session ?? throw new ArgumentNullException(nameof(session));
            _log = log ?? throw new ArgumentNullException(nameof(log));
        }

        protected abstract int SendMove(MoveRequest request);
        protected abstract int SendResign();
        protected abstract void LeaveSource();
        protected abstract void Unsubscribe();
        protected virtual string RejectedSentence => LanTexts.MoveRejected;
        protected virtual string OutOfSyncSentence => LanTexts.OutOfSync;
        protected virtual string DisconnectedSentence => LanTexts.Disconnected;

        /// <summary>True while the displayed game belongs to a remote room, including its final board.</summary>
        public bool IsActive { get; private set; }

        /// <summary>The menu reopens its remote room when a finished game's restart is chosen.</summary>
        public event Action RoomRequested;

        /// <summary>Resigns the local seat, even when the other player is to move.</summary>
        public void Resign()
        {
            if (!IsActive || _session.Game.IsGameOver)
                return;
            _session.Game.IsInputLocked = true;
            if (SendResign() < 0)
                _session.Game.IsInputLocked = false;
        }

        /// <summary>Returns to the waiting room instead of restarting a network game locally.</summary>
        public virtual void ShowRoom()
        {
            if (!IsActive || !_session.Game.IsGameOver)
                return;
            RoomRequested?.Invoke();
        }

        /// <summary>Leaves the remote room and stops routing subsequent packets to this board.</summary>
        public virtual void Leave()
        {
            IsActive = false;
            if (_attachedGame != null)
                _attachedGame.IsInputLocked = false;
            LeaveSource();
        }

        public void Dispose()
        {
            if (_disposed)
                return;
            _disposed = true;
            Unsubscribe();
            if (_attachedGame != null)
                _attachedGame.MoveRequested -= Request;
        }

        protected virtual void Start(StartGameDto start)
        {
            IsActive = true;
            _ply = start.Resumed ? start.Ply ?? start.Moves?.Count ?? 0 : 0;
            _session.Start(game => game.StartRemote(OnlinePosition.PiecesFrom(start.Position),
                OnlineRoomSettings.RulesFrom(start), (PlayerSide)start.YourSide,
                (PlayerSide)start.Position.ToMove, start.Players.Select(player => player.Name).ToArray()));
            if (_attachedGame == null)
            {
                _attachedGame = _session.Game;
                _attachedGame.MoveRequested += Request;
            }
            Sync(start.Clocks);
        }

        private void Request(MoveRequest request)
        {
            if (!IsActive || _session.Game.IsGameOver || _session.Game.IsInputLocked)
                return;
            // Host updates can arrive synchronously from SendMove, so lock before sending.
            _session.Game.IsInputLocked = true;
            if (SendMove(request) < 0)
                _session.Game.IsInputLocked = false;
        }

        protected virtual void Apply(GameUpdateDto update)
        {
            if (!IsActive)
                return;
            var game = _session.Game;
            if (update.IsRejected)
            {
                game.IsInputLocked = false;
                _log.Write(RejectedSentence);
                return;
            }
            if (update.Ply <= _ply)
                return;
            if (update.Ply != _ply + 1 || update.From?.Length != 2 || update.To?.Length != 2
                || !game.ApplyRemoteMove(update.From[0], update.From[1], update.To[0], update.To[1]))
            {
                LoseSynchronization();
                return;
            }
            _ply = update.Ply;
            MoveApplied(update, game.LastMove);
            Sync(update.Clocks);
            game.IsInputLocked = false;
            if (!OnlinePosition.Matches(game, update.Position))
                LoseSynchronization();
        }

        protected virtual void MoveApplied(GameUpdateDto update, MoveRecord move) { }

        protected virtual void Sync(ClocksDto clocks)
        {
            if (!IsActive || clocks == null || _session.Game.IsGameOver)
                return;
            var elapsed = OnlinePosition.ClocksFrom(clocks, out PlayerSide active);
            _session.Game.SyncClocks(elapsed, active);
        }

        protected void End(EndGameDto end)
        {
            if (!IsActive)
                return;
            var result = OnlinePosition.EndFrom(end);
            _session.Game.EndRemote(result.Winner, result.Reason);
            if (end.Reason == "Disconnect")
                _log.Write(DisconnectedSentence);
        }

        private void LoseSynchronization()
        {
            _log.Write(OutOfSyncSentence);
            _session.Game.EndRemote(PlayerSide.None, GameOverReason.Disconnect);
            Leave();
        }
    }
}

/* ----- ----- ----- ----- */
// RemoteHistory.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/13
// Update Date: 2026/10/13
// Version: v1.0
/* ----- ----- ----- ----- */

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using Chinese_Chess_v3.Game.Application.Texts;
using Chinese_Chess_v3.Game.Core;
using Chinese_Chess_v3.Network.Protocol;

using Engine.Timing;

namespace Chinese_Chess_v3.Game.Application.Online
{
    /// <summary>Server-ordered records independent of the current board and its undo history.</summary>
    public sealed class RemoteHistory
    {
        private const int BatchSize = 8;
        private readonly IMainThreadQueue _queue;
        private readonly SortedDictionary<int, MoveEntryDto> _moves = new();
        private readonly IReadOnlyDictionary<int, MoveEntryDto> _movesView;
        private int _generation;

        public RemoteHistory(IMainThreadQueue queue)
        {
            _queue = queue ?? throw new ArgumentNullException(nameof(queue));
            _movesView = new ReadOnlyDictionary<int, MoveEntryDto>(_moves);
        }

        /// <summary>Records keyed by server ply, including live moves received during synchronization.</summary>
        public IReadOnlyDictionary<int, MoveEntryDto> Moves => _movesView;

        /// <summary>Whether another history batch is pending.</summary>
        public bool IsSynchronizing { get; private set; }

        /// <summary>A newly inserted record's server ply and finished log sentence.</summary>
        public event Action<int, string> MoveAdded;

        /// <summary>Invalidates queued batches without changing the history already displayed.</summary>
        public void Cancel()
        {
            _generation++;
            IsSynchronizing = false;
        }

        /// <summary>Clears the previous game's records and starts bounded batches for a resumed game.</summary>
        public void Start(StartGameDto start)
        {
            ArgumentNullException.ThrowIfNull(start);
            Cancel();
            _moves.Clear();
            int count = Math.Min(start.Ply ?? start.Moves?.Count ?? 0, start.Moves?.Count ?? 0);
            if (!start.Resumed || count <= 0)
                return;
            IsSynchronizing = true;
            int generation = _generation;
            int index = 0;
            void Batch()
            {
                if (generation != _generation)
                    return;
                int end = Math.Min(index + BatchSize, count);
                while (index < end)
                {
                    int ply = ++index;
                    Add(ply, start.Moves[ply - 1]);
                    if (generation != _generation)
                        return;
                }
                IsSynchronizing = index < count;
                if (IsSynchronizing)
                    _queue.Post(Batch);
            }
            _queue.Post(Batch);
        }

        /// <summary>Inserts an authoritative live move immediately, using local notation only when omitted.</summary>
        public void Apply(GameUpdateDto update, MoveRecord local)
        {
            ArgumentNullException.ThrowIfNull(update);
            ArgumentNullException.ThrowIfNull(local);
            Add(update.Ply, new MoveEntryDto
            {
                From = update.From,
                To = update.To,
                Notation = string.IsNullOrEmpty(update.Record?.Notation) ? local.Notation : update.Record.Notation,
            });
        }

        private void Add(int ply, MoveEntryDto move)
        {
            if (move == null || !_moves.TryAdd(ply, move))
                return;
            MoveAdded?.Invoke(ply, string.IsNullOrEmpty(move.Notation) ? null : GameTexts.RemoteMoveLine(ply, move.Notation));
        }
    }
}

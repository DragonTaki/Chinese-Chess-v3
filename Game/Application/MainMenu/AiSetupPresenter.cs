/* ----- ----- ----- ----- */
// AiSetupPresenter.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/12
// Update Date: 2026/10/13
// Version: v1.1
/* ----- ----- ----- ----- */

using System;
using System.Collections.Generic;
using System.Linq;

using Chinese_Chess_v3.Game.Ai;
using Chinese_Chess_v3.Game.Application.Catalogs;
using Chinese_Chess_v3.Game.Application.Session;
using Chinese_Chess_v3.Game.Application.Texts;
using Chinese_Chess_v3.Game.Configs;
using Chinese_Chess_v3.Game.Core;
using Chinese_Chess_v3.Game.Core.Openings;
using PlayerNumber = Chinese_Chess_v3.Game.Core.Players.PlayerSide;

using Engine.Logging;

namespace Chinese_Chess_v3.Game.Application.MainMenu
{
    /// <summary>Edits the next computer game's options; only Start enters the game screen.</summary>
    public sealed class AiSetupPresenter
    {
        private readonly GameSession _session;
        private readonly AiSettings _defaults;
        private readonly Func<IEnumerable<OpeningLine>> _load;
        private AiOpeningBook _book;
        private GameKind _kind;

        /// <summary>Creates the setup flow; the optional loader supports in-memory repertoires.</summary>
        public AiSetupPresenter(GameSession session, AiSettings defaults, OpeningCatalog catalog,
            Func<IEnumerable<OpeningLine>> load = null)
        {
            _session = session ?? throw new ArgumentNullException(nameof(session));
            _defaults = defaults ?? throw new ArgumentNullException(nameof(defaults));
            ArgumentNullException.ThrowIfNull(catalog);
            _load = load ?? (() =>
            {
                var warnings = new List<string>();
                var lines = catalog.LoadAll(warnings).ToArray();
                foreach (var warning in warnings) AppLogger.Log(warning, LogLevel.WARN);
                return lines;
            });
        }

        public EngineLevel Level { get; set; }
        public AiPlayerSide PlayerSide { get; private set; }
        public bool AssignOpening { get; private set; }
        public string OpeningStyle { get; set; }
        public IReadOnlyList<string> OpeningStyles { get; private set; } = Array.Empty<string>();
        public string Title => AiSetupTexts.Title;
        public string DifficultyLabel => AiSetupTexts.Difficulty;
        public string FirstMoverLabel => AiSetupTexts.FirstMover;
        public string AssignOpeningLabel => AiSetupTexts.AssignOpening;
        public string OpeningLabel => AiSetupTexts.Opening;
        public string StartText => AiSetupTexts.Start;
        public string BackText => AiSetupTexts.Back;

        /// <summary>Loads options for a supported kind without creating or restarting a game.</summary>
        public bool Prepare(GameKind kind)
        {
            if (!GameSession.CanStartVsComputer(kind)) return false;
            _kind = kind;
            _book = new AiOpeningBook(_load());
            Level = _defaults.Level;
            AssignOpening = false;
            SetPlayerSide(_defaults.PlayerSide);
            return true;
        }

        /// <summary>Restricts named repertoires to the opponent's side; random requires both sides.</summary>
        public void SetPlayerSide(AiPlayerSide side)
        {
            if (!Enum.IsDefined(side)) throw new ArgumentOutOfRangeException(nameof(side));
            PlayerSide = side;
            if (_book == null) return;
            OpeningStyles = side switch
            {
                AiPlayerSide.First => _book.StylesFor(PlayerNumber.Player2),
                AiPlayerSide.Second => _book.StylesFor(PlayerNumber.Player1),
                _ => _book.StylesFor(PlayerNumber.Player1)
                    .Intersect(_book.StylesFor(PlayerNumber.Player2), StringComparer.Ordinal).ToArray(),
            };
            if (!OpeningStyles.Contains(OpeningStyle)) OpeningStyle = OpeningStyles.FirstOrDefault();
            if (OpeningStyles.Count == 0) AssignOpening = false;
        }

        public void SetAssignOpening(bool assign) => AssignOpening = assign && OpeningStyles.Count > 0;

        /// <summary>Captures the choices and begins the game; invalid selections leave the form open.</summary>
        public bool Start()
        {
            if (_book == null || !Enum.IsDefined(Level) || (AssignOpening && !OpeningStyles.Contains(OpeningStyle)))
                return false;
            _session.StartVsComputer(_kind, new AiGameOptions(Level, PlayerSide, AssignOpening ? OpeningStyle : null, UseConfiguredTimer: true), _book);
            return true;
        }
    }
}

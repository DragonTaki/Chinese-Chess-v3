/* ----- ----- ----- ----- */
// AiSetupPresenter.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/12
// Update Date: 2026/10/14
// Version: v1.5
/* ----- ----- ----- ----- */

using System;
using System.Collections.Generic;
using System.Linq;

using Chinese_Chess_v3.Game.Ai;
using Chinese_Chess_v3.Game.Application.Catalogs;
using Chinese_Chess_v3.Game.Application.Session;
using Chinese_Chess_v3.Game.Application.Settings;
using Chinese_Chess_v3.Game.Application.Texts;
using Chinese_Chess_v3.Game.Configs;
using Chinese_Chess_v3.Game.Core;
using Chinese_Chess_v3.Game.Core.Openings;
using PlayerNumber = Chinese_Chess_v3.Game.Core.Players.PlayerSide;

using Engine.Logging;

namespace Chinese_Chess_v3.Game.Application.MainMenu
{
    /// <summary>Edits the next local game's computer options; only Start enters the game screen.</summary>
    public sealed class AiSetupPresenter
    {
        private readonly GameSession _session;
        private readonly AiSettings _defaults;
        private readonly Func<IEnumerable<OpeningLine>> _load;
        private AiOpeningBook _book;
        private GameKind _kind;
        private bool _prepared;

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
                var lines = catalog.LoadAll(warnings).Where(line => line.Origin == Core.Pgn.PgnOrigin.BuiltIn).ToArray();
                foreach (var warning in warnings) AppLogger.Log(warning, LogLevel.WARN);
                return lines;
            });
        }

        public EngineLevel Level { get; set; }

        /// <summary>Whether the prepared local game uses a computer opponent.</summary>
        public bool Enabled { get; set; } = true;

        /// <summary>The computer game's clock mode; count-up unless the player changed it.</summary>
        public Core.Players.TimerMode TimerMode { get; set; } = Core.Players.TimerMode.CountUp;

        /// <summary>The opening name to save: empty when 指定電腦開局 is unchecked.</summary>
        public string PersistedOpening => AssignOpening && !string.IsNullOrEmpty(OpeningStyle) ? OpeningStyle : string.Empty;
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
        public string ResetText => AiSetupTexts.Reset;

        /// <summary>Loads options for a supported kind without creating or restarting a game.</summary>
        public bool Prepare(GameKind kind)
        {
            if (!GameSession.CanStartNew(kind)) return false;
            _kind = kind;
            _prepared = true;
            OpeningStyles = Array.Empty<string>();
            _book = null;
            Load(_defaults);
            return true;
        }

        /// <summary>Takes every choice from the saved settings (the opening name is empty when none was assigned).</summary>
        public void Load(AiSettings settings)
        {
            ArgumentNullException.ThrowIfNull(settings);
            Enabled = settings.Enabled;
            Level = settings.Level;
            TimerMode = settings.TimerMode;
            OpeningStyle = settings.OpeningName;
            AssignOpening = !string.IsNullOrEmpty(OpeningStyle);
            SetPlayerSide(settings.PlayerSide);
        }

        /// <summary>
        /// 重置: every setting of the computer game's pages (and the assigned opening) returns to its default and is
        /// saved; the form then follows the restored values.
        /// </summary>
        public void ResetToDefaults(SettingsScreenModel model)
        {
            ArgumentNullException.ThrowIfNull(model);
            model.ResetToDefaults();
            Load(model.Live.Ai);
        }

        /// <summary>Follows the settings rows (strength, side, clock) without touching the opening choice.</summary>
        public void SyncChoices(AiSettings settings)
        {
            ArgumentNullException.ThrowIfNull(settings);
            Enabled = settings.Enabled;
            Level = settings.Level;
            TimerMode = settings.TimerMode;
            SetPlayerSide(settings.PlayerSide);
        }

        /// <summary>Chooses one of <see cref="OpeningStyles"/>; other names are ignored.</summary>
        public void SelectOpening(string name)
        {
            if (name != null && OpeningStyles.Contains(name)) OpeningStyle = name;
        }

        /// <summary>
        /// Restricts named repertoires to the computer's side. A random player side lists every opening;
        /// when the drawn side does not fit the chosen opening, the book has no move and the engine plays normally.
        /// </summary>
        public void SetPlayerSide(AiPlayerSide side)
        {
            if (!Enum.IsDefined(side)) throw new ArgumentOutOfRangeException(nameof(side));
            PlayerSide = side;
            if (_book == null && Enabled && GameSession.CanStartVsComputer(_kind))
                _book = new AiOpeningBook(_load());
            if (_book == null) return;
            OpeningStyles = side switch
            {
                AiPlayerSide.First => _book.StylesFor(PlayerNumber.Player2),
                AiPlayerSide.Second => _book.StylesFor(PlayerNumber.Player1),
                _ => _book.StylesFor(PlayerNumber.Player1)
                    .Union(_book.StylesFor(PlayerNumber.Player2), StringComparer.Ordinal).OrderBy(name => name, StringComparer.Ordinal).ToArray(),
            };
            if (!OpeningStyles.Contains(OpeningStyle)) OpeningStyle = OpeningStyles.FirstOrDefault();
            if (OpeningStyles.Count == 0) AssignOpening = false;
        }

        public void SetAssignOpening(bool assign) => AssignOpening = assign && OpeningStyles.Count > 0;

        /// <summary>Captures the choices and begins the game; invalid selections leave the form open.</summary>
        public bool Start()
        {
            if (!_prepared) return false;
            if (!Enabled)
            {
                if (_kind == GameKind.ThreeKingdoms && _session.IsTeamSplitInvalid) return false;
                _session.StartNew(_kind);
                return true;
            }
            if (!GameSession.CanStartVsComputer(_kind)) return false;
            if (_book == null || !Enum.IsDefined(Level) || (AssignOpening && !OpeningStyles.Contains(OpeningStyle)))
                return false;
            _session.StartVsComputer(_kind, new AiGameOptions(Level, PlayerSide, AssignOpening ? OpeningStyle : null, UseConfiguredTimer: true, TimerMode: TimerMode), _book);
            return true;
        }
    }
}

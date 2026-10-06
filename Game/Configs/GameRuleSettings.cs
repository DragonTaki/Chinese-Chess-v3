/* ----- ----- ----- ----- */
// GameRuleSettings.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/05
// Update Date: 2026/10/05
// Version: v1.0
/* ----- ----- ----- ----- */

using System;
using System.Collections.Generic;
using System.Linq;

using Chinese_Chess_v3.Game.Core;
using Chinese_Chess_v3.Game.Core.Families.ThreeKingdoms;

using Engine.Configs;

namespace Chinese_Chess_v3.Game.Configs
{
    /// <summary>
    /// The settings area of the local games' rules (<c>[rules.*]</c>, one section per
    /// <see cref="GameKind"/>; a network game does not use them). <see cref="Apply"/> raises
    /// <see cref="Applied"/>, on which the composition root copies the rules onto the rules new
    /// games start with (<see cref="ApplyTo"/>); a new game manager takes them when it is created
    /// (<see cref="CreateRuleSets"/>).
    /// </summary>
    public sealed class GameRuleSettings : ISettingsArea
    {
        // One rule set per game kind.
        private readonly Dictionary<GameKind, RuleSettings> _rules =
            Enum.GetValues<GameKind>().ToDictionary(kind => kind, _ => new RuleSettings());

        /// <summary>
        /// The rule and clock choices for new games of <paramref name="kind"/> (the stored
        /// object, edited in place). Default: every kind with the <see cref="RuleSettings"/> defaults.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="kind"/> is not a <see cref="GameKind"/>.</exception>
        public RuleSettings RulesFor(GameKind kind) =>
            _rules.TryGetValue(kind, out var rules)
                ? rules
                : throw new ArgumentOutOfRangeException(nameof(kind), kind, "Unknown game kind");

        /// <summary>Raised at the end of <see cref="Apply"/> (the rules changed or were read).</summary>
        public event Action Applied;

        /// <summary>Raises <see cref="Applied"/>: the rules themselves are copied where they are used by its handler.</summary>
        public void Apply() => Applied?.Invoke();

        /// <summary>
        /// The rule sets a new <see cref="GameManager"/> starts games with: per kind, the
        /// default <see cref="Rules"/> with that kind's choices applied.
        /// </summary>
        public GameRuleSets CreateRuleSets()
        {
            var sets = new GameRuleSets();
            ApplyTo(sets);
            return sets;
        }

        /// <summary>
        /// Applies every kind's choices to that kind's rules in <paramref name="sets"/>
        /// (<see cref="RuleSettings.ApplyTo"/>, the objects updated in place): the rules new games
        /// start with (<c>GameManager.DefaultRuleSets</c>); a game already started keeps its own copy.
        /// </summary>
        /// <exception cref="ArgumentNullException"><paramref name="sets"/> is null.</exception>
        public void ApplyTo(GameRuleSets sets)
        {
            ArgumentNullException.ThrowIfNull(sets);
            foreach (var (kind, rules) in _rules)
                rules.ApplyTo(sets[kind]);
        }

        #region Keys

        private IReadOnlyList<SettingsKey> _keys;

        /// <inheritdoc/>
        public IReadOnlyList<SettingsKey> Keys => _keys ??= Enum.GetValues<GameKind>().SelectMany(RuleKeys).ToArray();

        /// <summary>
        /// The section of <paramref name="kind"/>'s rules: <c>rules.traditional</c>,
        /// <c>rules.flip</c>, <c>rules.dark_half</c>, <c>rules.open_half</c>, <c>rules.three_kingdoms</c>.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="kind"/> is not a <see cref="GameKind"/>.</exception>
        public static string SectionFor(GameKind kind) => kind switch
        {
            GameKind.Traditional => "rules.traditional",
            GameKind.Flip => "rules.flip",
            GameKind.DarkHalf => "rules.dark_half",
            GameKind.OpenHalf => "rules.open_half",
            GameKind.ThreeKingdoms => "rules.three_kingdoms",
            _ => throw new ArgumentOutOfRangeException(nameof(kind), kind, "Unknown game kind"),
        };

        private static readonly int[] TeamNumbers = { 1, 2, 3 };

        /// <summary>The kind's name in the file's comments (the new-game menu's name).</summary>
        private static string KindName(GameKind kind) => kind switch
        {
            GameKind.Traditional => "傳統大盤（也用於殘局、開局練習與讀取的存檔）",
            GameKind.Flip => "揭棋大盤",
            GameKind.DarkHalf => "暗棋半盤",
            GameKind.OpenHalf => "明棋半盤",
            GameKind.ThreeKingdoms => "三國半盤",
            _ => throw new ArgumentOutOfRangeException(nameof(kind), kind, "Unknown game kind"),
        };

        /// <summary>
        /// The keys of <paramref name="kind"/>'s section: the clock settings, then the kind's
        /// on/off options (<see cref="RuleSettings.OptionsFor"/>), then 三國's choices.
        /// </summary>
        private IEnumerable<SettingsKey> RuleKeys(GameKind kind)
        {
            string section = SectionFor(kind);
            var r = RulesFor(kind);
            var d = new RuleSettings();

            yield return SettingsKey.Int(section, "total_minutes", () => r.TotalTimeMinutes, v => r.TotalTimeMinutes = v, d.TotalTimeMinutes,
                RuleSettings.TotalTimeMinutesMin, RuleSettings.TotalTimeMinutesMax,
                $"── {KindName(kind)}的計時與規則（只用於單機對局，連線對局不適用；下一局開始生效）──",
                $"每方的局時（分鐘，{RuleSettings.TotalTimeMinutesMin}～{RuleSettings.TotalTimeMinutesMax}）；只用於倒數計時，正數計時（mode = CountUp）沒有時限，不使用。");
            yield return SettingsKey.Int(section, "step_seconds", () => r.StepTimeSeconds, v => r.StepTimeSeconds = v, d.StepTimeSeconds,
                RuleSettings.StepTimeSecondsMin, RuleSettings.StepTimeSecondsMax,
                $"每步的步時（秒，{RuleSettings.StepTimeSecondsMin}～{RuleSettings.StepTimeSecondsMax}）；step_timer = false 或正數計時時不使用。");
            yield return SettingsKey.Int(section, "increment_seconds", () => r.IncrementSeconds, v => r.IncrementSeconds = v, d.IncrementSeconds,
                RuleSettings.IncrementSecondsMin, RuleSettings.IncrementSecondsMax,
                $"每走一步加回局時的秒數（{RuleSettings.IncrementSecondsMin}～{RuleSettings.IncrementSecondsMax}，0 表示不加秒）；僅倒數計時會加秒，正數計時不加。");
            yield return SettingsKey.Bool(section, "step_timer", () => r.StepTimerEnabled, v => r.StepTimerEnabled = v, d.StepTimerEnabled,
                "是否限制步時（true／false）；只用於倒數計時，正數計時一律只計步時、不限制。");
            yield return SettingsKey.Enum(section, "mode", () => r.TimerMode, v => r.TimerMode = v, d.TimerMode,
                "計時方式：CountDown（倒數到時限）或 CountUp（從 0 正數，只計時：沒有時限、不會超時判負、不加秒）。");
            yield return SettingsKey.Bool(section, "lose_on_time_up", () => r.EndGameWhenTimesUp, v => r.EndGameWhenTimesUp = v, d.EndGameWhenTimesUp,
                "時間用完的一方是否判負（true／false）；false 時只停掉那一方的時鐘，棋局繼續。只用於倒數計時，正數計時不會超時。");

            foreach (var option in RuleSettings.OptionsFor(kind))
            {
                var (name, comment) = OptionKey(option);
                yield return SettingsKey.Bool(section, name, () => r.Get(option), v => r.Set(option, v), d.Get(option), comment);
            }

            if (kind == GameKind.ThreeKingdoms)
            {
                bool first = true;
                foreach (var color in ThreeKingdomsTeamSplit.Colors)
                {
                    foreach (var type in ThreeKingdomsTeamSplit.Types)
                    {
                        var (c, t) = (color, type);
                        string comment = first
                            ? "自訂分隊：每種顏色與兵種各屬第幾隊（1／2／3）；每隊至少一顆子（否則無法開局），門檻＝該隊的棋子數。預設：第 1 隊 帥將兵卒（12 子）、第 2 隊 仕相俥傌炮（10 子）、第 3 隊 士象車馬包（10 子）。"
                            : null;
                        first = false;
                        var key = SettingsKey.IntOneOf(section, $"team_{c.ToString().ToLowerInvariant()}_{t.ToString().ToLowerInvariant()}",
                            () => r.HalfCrossTeams.TeamOf(c, t), v => r.HalfCrossTeams = r.HalfCrossTeams.With(c, t, v),
                            d.HalfCrossTeams.TeamOf(c, t), TeamNumbers, comment == null ? Array.Empty<string>() : new[] { comment });
                        yield return key;
                    }
                }
                yield return SettingsKey.Enum(section, "win_condition", () => r.HalfCrossWinCondition, v => r.HalfCrossWinCondition = v, d.HalfCrossWinCondition,
                    "勝負方式：Points（計分，預設：車／將／帥 2 分、其他 1 分，名次比超分＝得分－該隊起始棋子數，同分先達成者在前）、Annihilation（全滅：存活者第一，其餘依出局先後）、ScoreBalance（得失分：殘存棋子分數＋吃子分數，維基分值）或 FirstTo200（先得 200 分，維基分值）。");
            }
        }

        /// <summary>The key name and comment of an on/off rule option.</summary>
        private static (string Name, string Comment) OptionKey(RuleOption option) => option switch
        {
            RuleOption.GeneralCanSeeGeneral => ("general_can_see_general", "是否允許王見王（兩將在同一直線、中間無子）。"),
            RuleOption.GeneralCanLeavePalace => ("general_can_leave_palace", "將帥是否可以出九宮。"),
            RuleOption.AdvisorCanLeavePalace => ("advisor_can_leave_palace", "士是否可以出九宮。"),
            RuleOption.ElephantEyeBlocks => ("elephant_eye_blocks", "是否有塞象眼（象田字中心有子就不能走）。"),
            RuleOption.HorseLegBlocks => ("horse_leg_blocks", "是否有蹩馬腳（馬腳有子就不能往那邊走）。"),
            RuleOption.CanCaptureHiddenPiece => ("can_capture_hidden_piece", "是否允許暗吃（直接吃蓋著的棋子）。"),
            RuleOption.CaptureHiddenStrongerSuicide => ("capture_hidden_stronger_suicide",
                "暗吃吃到比自己大的子時：true = 吃的一方被吃掉、對方保持翻開；false = 吃的一方回到原位，對方翻開。"),
            RuleOption.AllowChainCapture => ("allow_chain_capture", "是否允許連吃（一步連續吃多次）。"),
            RuleOption.ChariotRushHorseDiagonal => ("chariot_rush_horse_diagonal",
                "是否採用車衝馬斜（同一個變體）：車可以沿直線一次走多格，走超過一格吃子不看大小、吃相鄰的子仍照大小；馬改成斜走一格，斜走吃子不看大小。"),
            RuleOption.CannonMustJump => ("cannon_must_jump", "包／炮吃子是否一定要跳過一個子。"),
            RuleOption.CaptureOwnPiece => ("can_capture_own_piece",
                "是否可以吃己方的棋子。大盤不能吃自己的將帥；半盤吃己方的子跟吃對方一樣照大小。"),
            RuleOption.Suicide => ("can_suicide",
                "半盤是否可以去撞已翻開、比自己大的對方棋子（走一格的吃法）：撞的一方陣亡，對方留在原位。"),
            _ => throw new ArgumentOutOfRangeException(nameof(option), option, "Unknown rule option"),
        };

        #endregion
    }
}

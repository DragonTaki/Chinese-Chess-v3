/* ----- ----- ----- ----- */
// PlayerSettingsFile.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/01
// Update Date: 2026/10/04
// Version: v1.3
/* ----- ----- ----- ----- */

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

using Chinese_Chess_v3.Game.Core;

using Engine.Configs;
using Engine.Logging;

namespace Chinese_Chess_v3.Game.Configs
{
    /// <summary>
    /// Reads and writes <see cref="PlayerSettings"/> as the player's plain-text
    /// <c>settings.ini</c> (format: <see cref="IniDocument"/>; keys: docs/SETTINGS.md).
    /// <para>
    /// Load rule - the file wins over the code defaults:
    /// missing file: created from the defaults;
    /// malformed (unparsable) file: copied to <c>settings.ini.bak</c> (older backups rotate:
    /// .bak -> .bak2 -> .bak3 ..., so .bak is always the newest), then recreated from the defaults;
    /// a key missing or with an invalid value: that key uses its default, a warning is
    /// logged, and the file is written back with the key added / the value reset - every
    /// other line (the player's valid values, comments, unknown keys) is kept as it was.
    /// </para>
    /// </summary>
    public static class PlayerSettingsFile
    {
        /// <summary>One key of the file: where it lives, its comment, and how it maps to <see cref="PlayerSettings"/>.</summary>
        private sealed class KeyDef
        {
            public string Section;
            public string Name;
            public string[] Comment;
            public Func<PlayerSettings, string> Format;
            /// <summary>Parses and validates the text; on success sets the property and returns true.</summary>
            public Func<string, PlayerSettings, bool> TryApply;
        }

        private static readonly string[] Header =
        {
            "Chinese Chess v3 玩家設定檔",
            "格式：每行一個「key = value」；# 或 ; 開頭的行是註解；[名稱] 是區段。",
            "遊戲啟動時讀取。缺少或打錯的項目會用預設值，並在這個檔案補上／改回預設值；",
            "整個檔案無法解析時，會先備份成 settings.ini.bak 再用預設值重建。",
            "每個項目的說明：docs/SETTINGS.md",
        };

        private static readonly KeyDef[] Keys = BuildKeys().ToArray();

        /// <summary>Every key of the file, in file order: [player], one [rules.*] section per game kind, then the rest.</summary>
        private static IEnumerable<KeyDef> BuildKeys()
        {
            yield return Str("player", "name", s => s.PlayerName, (s, v) => s.PlayerName = v, PlayerSettings.PlayerNameMaxLength,
                $"玩家名稱，用在紀錄區的問候語（可留空，最多 {PlayerSettings.PlayerNameMaxLength} 字）。");
            yield return Str("player", "player1_name", s => s.Player1Name, (s, v) => s.Player1Name = v, PlayerSettings.PlayerNameMaxLength,
                $"單機對局玩家一（先手）的名稱，顯示在資訊看板、結果視窗與存檔（可留空＝「玩家一」，最多 {PlayerSettings.PlayerNameMaxLength} 字）。");
            yield return Str("player", "player2_name", s => s.Player2Name, (s, v) => s.Player2Name = v, PlayerSettings.PlayerNameMaxLength,
                $"單機對局玩家二（後手）的名稱（可留空＝「玩家二」，最多 {PlayerSettings.PlayerNameMaxLength} 字）。");
            yield return Str("player", "player3_name", s => s.Player3Name, (s, v) => s.Player3Name = v, PlayerSettings.PlayerNameMaxLength,
                $"單機對局玩家三（三國）的名稱（可留空＝「玩家三」，最多 {PlayerSettings.PlayerNameMaxLength} 字）。");

            foreach (var kind in System.Enum.GetValues<GameKind>())
                foreach (var key in RuleKeys(kind))
                    yield return key;

            yield return IntOneOf("display", "fps", s => s.Fps, (s, v) => s.Fps = v, PlayerSettings.FpsOptions,
                $"畫面更新率（每秒幾格動畫）：{string.Join("／", PlayerSettings.FpsOptions)}。開著垂直同步時最多到螢幕更新率；WinForms 版的計時器實際上大約只到 64。");

            yield return Bool("hints", "legal_moves", s => s.ShowLegalMoveHints, (s, v) => s.ShowLegalMoveHints = v,
                "選子時是否用圓圈標出可以走的位置。");
            yield return Bool("hints", "hanging_pieces", s => s.ShowHangingPieceHints, (s, v) => s.ShowHangingPieceHints = v,
                "是否用圓圈標出無根、可被吃的棋子。");

            yield return Float("input", "wheel_scroll_step", s => s.WheelScrollStep, (s, v) => s.WheelScrollStep = v,
                PlayerSettings.WheelScrollStepMin, PlayerSettings.WheelScrollStepMax,
                $"滑鼠滾輪每一格捲動的距離（介面設計單位，{PlayerSettings.WheelScrollStepMin}～{PlayerSettings.WheelScrollStepMax}）。");

            yield return Bool("debug", "verbose_log", s => s.VerboseLog, (s, v) => s.VerboseLog = v,
                "── 除錯功能（設定畫面的 DEBUG 分頁；改了立即生效）──",
                "紀錄：是否寫出 DEBUG 等級的紀錄（較詳細）。");
            yield return Bool("debug", "console_trace", s => s.ConsoleTrace, (s, v) => s.ConsoleTrace = v,
                "紀錄：是否在主控台印出開發用的追蹤訊息（介面元件初始化、選單選擇、連線狀態；錯誤訊息不受影響，一律會印）。");
            yield return Bool("debug", "label_backgrounds", s => s.LabelBackgrounds, (s, v) => s.LabelBackgrounds = v,
                "視覺除錯：文字標籤後面畫半透明紅色背景（看出標籤的範圍）。");
            yield return Bool("debug", "layout_outlines", s => s.LayoutOutlines, (s, v) => s.LayoutOutlines = v,
                "視覺除錯：選單（虛線）與文字框（實線）畫灰色外框（看出排版範圍）。");
            yield return Bool("debug", "star_effect_frames", s => s.StarEffectFrames, (s, v) => s.StarEffectFrames = v,
                "視覺除錯：星空背景每個特效開始時，用特效的除錯顏色框出它的範圍。");
            yield return Bool("debug", "show_fps", s => s.ShowFps, (s, v) => s.ShowFps = v,
                "效能與連線：在視窗左上角顯示實際量到的畫面更新率（FPS）。");
            yield return Bool("debug", "show_network_latency", s => s.ShowNetworkLatency, (s, v) => s.ShowNetworkLatency = v,
                "效能與連線：在 FPS 下面顯示網路延遲（目前還沒有連線功能，只會顯示「—」）。");

            yield return Path("endgame", "user_folder", s => s.EndgameUserFolder, (s, v) => s.EndgameUserFolder = v,
                "自己的殘局題目資料夾；留空 = 預設位置（這個設定檔旁邊的 Endgames 資料夾）。",
                "相對路徑以這個設定檔所在的資料夾為準；可以用 %環境變數%。");

            yield return Path("opening", "user_folder", s => s.OpeningUserFolder, (s, v) => s.OpeningUserFolder = v,
                "自己的開局練習資料夾；留空 = 預設位置（這個設定檔旁邊的 Openings 資料夾）。",
                "相對路徑以這個設定檔所在的資料夾為準；可以用 %環境變數%。");
        }

        /// <summary>
        /// The section of <paramref name="kind"/>'s rules: <c>rules.traditional</c>,
        /// <c>rules.flip</c>, <c>rules.dark_half</c>, <c>rules.open_half</c>, <c>rules.three_kingdoms</c>.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="kind"/> is not a <see cref="GameKind"/>.</exception>
        public static string RulesSection(GameKind kind) => kind switch
        {
            GameKind.Traditional => "rules.traditional",
            GameKind.Flip => "rules.flip",
            GameKind.DarkHalf => "rules.dark_half",
            GameKind.OpenHalf => "rules.open_half",
            GameKind.ThreeKingdoms => "rules.three_kingdoms",
            _ => throw new ArgumentOutOfRangeException(nameof(kind), kind, "Unknown game kind"),
        };

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
        private static IEnumerable<KeyDef> RuleKeys(GameKind kind)
        {
            string section = RulesSection(kind);
            RuleSettings R(PlayerSettings s) => s.RulesFor(kind);

            yield return Int(section, "total_minutes", s => R(s).TotalTimeMinutes, (s, v) => R(s).TotalTimeMinutes = v,
                RuleSettings.TotalTimeMinutesMin, RuleSettings.TotalTimeMinutesMax,
                $"── {KindName(kind)}的計時與規則（只用於單機對局，連線對局不適用；下一局開始生效）──",
                $"每方的局時（分鐘，{RuleSettings.TotalTimeMinutesMin}～{RuleSettings.TotalTimeMinutesMax}）；只用於倒數計時，正數計時（mode = CountUp）沒有時限，不使用。");
            yield return Int(section, "step_seconds", s => R(s).StepTimeSeconds, (s, v) => R(s).StepTimeSeconds = v,
                RuleSettings.StepTimeSecondsMin, RuleSettings.StepTimeSecondsMax,
                $"每步的步時（秒，{RuleSettings.StepTimeSecondsMin}～{RuleSettings.StepTimeSecondsMax}）；step_timer = false 或正數計時時不使用。");
            yield return Int(section, "increment_seconds", s => R(s).IncrementSeconds, (s, v) => R(s).IncrementSeconds = v,
                RuleSettings.IncrementSecondsMin, RuleSettings.IncrementSecondsMax,
                $"每走一步加回局時的秒數（{RuleSettings.IncrementSecondsMin}～{RuleSettings.IncrementSecondsMax}，0 表示不加秒）；僅倒數計時會加秒，正數計時不加。");
            yield return Bool(section, "step_timer", s => R(s).StepTimerEnabled, (s, v) => R(s).StepTimerEnabled = v,
                "是否限制步時（true／false）；只用於倒數計時，正數計時一律只計步時、不限制。");
            yield return Enum(section, "mode", s => R(s).TimerMode, (s, v) => R(s).TimerMode = v,
                "計時方式：CountDown（倒數到時限）或 CountUp（從 0 正數，只計時：沒有時限、不會超時判負、不加秒）。");
            yield return Bool(section, "lose_on_time_up", s => R(s).EndGameWhenTimesUp, (s, v) => R(s).EndGameWhenTimesUp = v,
                "時間用完的一方是否判負（true／false）；false 時只停掉那一方的時鐘，棋局繼續。只用於倒數計時，正數計時不會超時。");

            foreach (var option in RuleSettings.OptionsFor(kind))
            {
                var (name, comment) = OptionKey(option);
                yield return Bool(section, name, s => R(s).Get(option), (s, v) => R(s).Set(option, v), comment);
            }

            if (kind == GameKind.ThreeKingdoms)
            {
                yield return Enum(section, "team_setup", s => R(s).HalfCrossTeamVariant, (s, v) => R(s).HalfCrossTeamVariant = v,
                    "分隊（未實作，目前只記錄）：Standard（第一種：帥將兵卒／仕相俥傌炮／士象車馬包）或 Handicap（第二種，讓子用：兵卒／帥仕相將士象／俥傌炮車馬包）。");
                yield return Enum(section, "win_condition", s => R(s).HalfCrossWinCondition, (s, v) => R(s).HalfCrossWinCondition = v,
                    "勝負方式（未實作，目前只記錄）：Points（計分，預設：車／將／帥 2 分、其他 1 分，將帥隊吃到 12 分、其他兩隊 10 分獲勝）、Annihilation（全滅）、Recall（收軍）、ScoreBalance（得失分）或 FirstTo200（先得 200 分）。");
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

        /// <summary>Every key of the file as (section, key), in file order.</summary>
        public static IReadOnlyList<(string Section, string Key)> KeyNames =>
            Keys.Select(k => (k.Section, k.Name)).ToList();

        #region Load / save

        /// <summary>Loads <see cref="SystemSettings.PlayerSettingsFilePath"/> by the load rule (see the class summary).</summary>
        public static PlayerSettings Load() => Load(SystemSettings.PlayerSettingsFilePath);

        /// <summary>
        /// Loads <paramref name="path"/> by the load rule (see the class summary), repairing
        /// or creating the file as needed. Never throws for file problems: anything that
        /// cannot be read or written is logged and the defaults are used for it. Each
        /// warning is logged (WARN) and also added to <paramref name="warnings"/> if given.
        /// </summary>
        public static PlayerSettings Load(string path, ICollection<string> warnings = null)
        {
            void Warn(string message)
            {
                AppLogger.Log($"(Settings) {message}", LogLevel.WARN);
                warnings?.Add(message);
            }

            var settings = PlayerSettings.Defaults;

            if (!File.Exists(path))
            {
                Warn($"{path} not found: created it with the default settings");
                TryWrite(path, CreateDocument(settings), Warn);
                return settings;
            }

            string text;
            try
            {
                text = File.ReadAllText(path);
            }
            catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException)
            {
                Warn($"cannot read {path} ({ex.Message}): using the default settings, file left as is");
                return settings;
            }

            if (!IniDocument.TryParse(text, out var doc, out string error))
            {
                string backup = TryBackup(path, Warn);
                if (backup == null)
                {
                    Warn($"{path} is malformed ({error}) and could not be backed up: using the default settings, file left as is");
                    return settings;
                }
                Warn($"{path} is malformed ({error}): backed up to {backup}, recreated with the default settings");
                TryWrite(path, CreateDocument(settings), Warn);
                return settings;
            }

            bool changed = Apply(doc, settings, Warn);
            if (changed)
                TryWrite(path, doc, Warn);
            return settings;
        }

        /// <summary>
        /// Applies the parsed <paramref name="doc"/> to <paramref name="settings"/> (which
        /// should hold the defaults): valid values are taken; a missing key is added to the
        /// document with its comment and default, an invalid value is reset to the default
        /// with a comment recording the old value. Returns whether the document changed
        /// (and so should be written back).
        /// </summary>
        private static bool Apply(IniDocument doc, PlayerSettings settings, Action<string> warn)
        {
            var defaults = PlayerSettings.Defaults;
            bool changed = false;

            foreach (var key in Keys)
            {
                if (!doc.TryGetValue(key.Section, key.Name, out string value))
                {
                    warn($"[{key.Section}] {key.Name} missing: using the default ({key.Format(defaults)}), added to the file");
                    doc.AddEntry(key.Section, key.Name, key.Format(defaults), CommentFor(key, defaults));
                    changed = true;
                    continue;
                }

                if (doc.IsRepeated(key.Section, key.Name))
                    warn($"[{key.Section}] {key.Name} appears more than once: the last one ({value}) is used");

                if (!key.TryApply(value, settings))
                {
                    string def = key.Format(defaults);
                    warn($"[{key.Section}] {key.Name} = \"{value}\" is invalid: using the default ({def}), reset in the file");
                    doc.InsertCommentBefore(key.Section, key.Name, $"原本的值「{value}」無效，已改回預設值 {def}。");
                    doc.SetValue(key.Section, key.Name, def);
                    changed = true;
                }
            }

            foreach (var (section, name, _) in doc.Entries)
            {
                if (!Keys.Any(k => string.Equals(k.Section, section, StringComparison.OrdinalIgnoreCase) &&
                                   string.Equals(k.Name, name, StringComparison.OrdinalIgnoreCase)))
                    warn($"unknown key [{section}] {name}: ignored (kept in the file)");
            }

            return changed;
        }

        /// <summary>
        /// Writes <paramref name="settings"/> to <paramref name="path"/> (for the settings
        /// screen). A missing file is created fresh. A parsable file keeps every line it has
        /// (comments, layout, unknown keys) with only the values updated and any missing key
        /// added to its section. A malformed file is first backed up (<c>.bak</c> rotation,
        /// as in <see cref="Load(string, ICollection{string})"/>) and then replaced by a
        /// fresh one. The write goes through a temp file, so a crash never leaves half a
        /// file. Returns false (logged) when the file cannot be read, backed up or written;
        /// the existing file is then left as it was.
        /// </summary>
        public static bool Save(PlayerSettings settings, string path)
        {
            void Warn(string message) => AppLogger.Log($"(Settings) {message}", LogLevel.WARN);

            IniDocument doc = null;
            if (File.Exists(path))
            {
                string text;
                try
                {
                    text = File.ReadAllText(path);
                }
                catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException)
                {
                    Warn($"cannot read {path} ({ex.Message}): not saved, file left as is");
                    return false;
                }

                if (IniDocument.TryParse(text, out var existing, out string error))
                {
                    doc = existing;
                }
                else
                {
                    string backup = TryBackup(path, Warn);
                    if (backup == null)
                    {
                        Warn($"{path} is malformed ({error}) and could not be backed up: not saved, file left as is");
                        return false;
                    }
                    Warn($"{path} is malformed ({error}): backed up to {backup}, replaced with the saved settings");
                }
            }

            if (doc == null)
            {
                doc = CreateDocument(settings);
            }
            else
            {
                foreach (var key in Keys)
                {
                    if (doc.TryGetValue(key.Section, key.Name, out _))
                        doc.SetValue(key.Section, key.Name, key.Format(settings));
                    else
                        doc.AddEntry(key.Section, key.Name, key.Format(settings), CommentFor(key, PlayerSettings.Defaults));
                }
            }

            return TryWrite(path, doc, Warn);
        }

        /// <summary>
        /// Copies every setting of <paramref name="from"/> into <paramref name="to"/> (through
        /// the key table, so a newly added key is covered automatically). A value that would
        /// not be valid in the file (e.g. a name over its length limit) is left unchanged in
        /// <paramref name="to"/>.
        /// </summary>
        public static void Copy(PlayerSettings from, PlayerSettings to)
        {
            ArgumentNullException.ThrowIfNull(from);
            ArgumentNullException.ThrowIfNull(to);
            foreach (var key in Keys)
                key.TryApply(key.Format(from), to);
        }

        /// <summary>A new <see cref="PlayerSettings"/> with the same values as <paramref name="settings"/> (see <see cref="Copy"/>).</summary>
        public static PlayerSettings Clone(PlayerSettings settings)
        {
            var clone = PlayerSettings.Defaults;
            Copy(settings, clone);
            return clone;
        }

        /// <summary>Whether two settings hold the same value for every key (as they would be written to the file).</summary>
        public static bool AreEqual(PlayerSettings a, PlayerSettings b) =>
            Keys.All(key => key.Format(a) == key.Format(b));

        /// <summary>The full text of a fresh settings file holding <paramref name="settings"/>, every key commented.</summary>
        public static string Generate(PlayerSettings settings) => CreateDocument(settings).ToString();

        private static IniDocument CreateDocument(PlayerSettings settings)
        {
            var defaults = PlayerSettings.Defaults;
            var doc = new IniDocument();
            foreach (string line in Header)
                doc.AppendComment(line);
            foreach (var key in Keys)
                doc.AddEntry(key.Section, key.Name, key.Format(settings), CommentFor(key, defaults));
            return doc;
        }

        private static IEnumerable<string> CommentFor(KeyDef key, PlayerSettings defaults)
        {
            string def = key.Format(defaults);
            return key.Comment.Append($"預設值：{(def.Length == 0 ? "（空白）" : def)}");
        }

        private static bool TryWrite(string path, IniDocument doc, Action<string> warn)
        {
            string temp = path + ".tmp";
            try
            {
                string dir = System.IO.Path.GetDirectoryName(System.IO.Path.GetFullPath(path));
                if (!string.IsNullOrEmpty(dir))
                    Directory.CreateDirectory(dir);
                // Written next to the target, then moved over it: a crash mid-write
                // never leaves a half-written settings file.
                File.WriteAllText(temp, doc.ToString());
                File.Move(temp, path, overwrite: true);
                return true;
            }
            catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException || ex is NotSupportedException)
            {
                warn($"cannot write {path}: {ex.Message}");
                try { if (File.Exists(temp)) File.Delete(temp); } catch (Exception) { }
                return false;
            }
        }

        /// <summary>
        /// Copies <paramref name="path"/> to <c>&lt;path&gt;.bak</c>, which is always the newest
        /// backup: existing ones rotate first (logrotate style) - .bakN -> .bak(N+1) from the
        /// highest down, then .bak -> .bak2 - so no backup is ever lost or overwritten.
        /// Returns the backup path, or null when rotating or copying failed.
        /// </summary>
        private static string TryBackup(string path, Action<string> warn)
        {
            string backup = path + ".bak";
            try
            {
                string Numbered(int n) => n == 1 ? backup : backup + n;

                int highest = 0;
                while (File.Exists(Numbered(highest + 1)))
                    highest++;

                for (int n = highest; n >= 1; n--)
                    File.Move(Numbered(n), Numbered(n + 1));

                File.Copy(path, backup, overwrite: false);
                return backup;
            }
            catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException)
            {
                warn($"cannot back up {path} to {backup}: {ex.Message}");
                return null;
            }
        }

        #endregion

        #region Key definitions

        private static readonly string[] TrueWords = { "true", "yes", "on", "1" };
        private static readonly string[] FalseWords = { "false", "no", "off", "0" };

        private static KeyDef Bool(string section, string name, Func<PlayerSettings, bool> get, Action<PlayerSettings, bool> set, params string[] comment) =>
            new KeyDef
            {
                Section = section, Name = name, Comment = comment,
                Format = s => get(s) ? "true" : "false",
                TryApply = (text, s) =>
                {
                    if (TrueWords.Contains(text, StringComparer.OrdinalIgnoreCase)) { set(s, true); return true; }
                    if (FalseWords.Contains(text, StringComparer.OrdinalIgnoreCase)) { set(s, false); return true; }
                    return false;
                },
            };

        private static KeyDef Int(string section, string name, Func<PlayerSettings, int> get, Action<PlayerSettings, int> set, int min, int max, params string[] comment) =>
            new KeyDef
            {
                Section = section, Name = name, Comment = comment,
                Format = s => get(s).ToString(CultureInfo.InvariantCulture),
                TryApply = (text, s) =>
                {
                    if (!int.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out int v) || v < min || v > max)
                        return false;
                    set(s, v);
                    return true;
                },
            };

        /// <summary>An integer key that must be one of <paramref name="allowed"/>.</summary>
        private static KeyDef IntOneOf(string section, string name, Func<PlayerSettings, int> get, Action<PlayerSettings, int> set, IReadOnlyList<int> allowed, params string[] comment) =>
            new KeyDef
            {
                Section = section, Name = name, Comment = comment,
                Format = s => get(s).ToString(CultureInfo.InvariantCulture),
                TryApply = (text, s) =>
                {
                    if (!int.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out int v) || !allowed.Contains(v))
                        return false;
                    set(s, v);
                    return true;
                },
            };

        private static KeyDef Float(string section, string name, Func<PlayerSettings, float> get, Action<PlayerSettings, float> set, float min, float max, params string[] comment) =>
            new KeyDef
            {
                Section = section, Name = name, Comment = comment,
                Format = s => get(s).ToString("0.###", CultureInfo.InvariantCulture),
                TryApply = (text, s) =>
                {
                    if (!float.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out float v) ||
                        float.IsNaN(v) || v < min || v > max)
                        return false;
                    set(s, v);
                    return true;
                },
            };

        private static KeyDef Enum<T>(string section, string name, Func<PlayerSettings, T> get, Action<PlayerSettings, T> set, params string[] comment)
            where T : struct, System.Enum =>
            new KeyDef
            {
                Section = section, Name = name, Comment = comment,
                Format = s => get(s).ToString(),
                TryApply = (text, s) =>
                {
                    // Names only: Enum.TryParse would also take any number.
                    string match = System.Enum.GetNames(typeof(T)).FirstOrDefault(n => string.Equals(n, text, StringComparison.OrdinalIgnoreCase));
                    if (match == null)
                        return false;
                    set(s, System.Enum.Parse<T>(match));
                    return true;
                },
            };

        private static KeyDef Str(string section, string name, Func<PlayerSettings, string> get, Action<PlayerSettings, string> set, int maxLength, params string[] comment) =>
            new KeyDef
            {
                Section = section, Name = name, Comment = comment,
                Format = s => get(s) ?? string.Empty,
                TryApply = (text, s) =>
                {
                    if (text.Length > maxLength)
                        return false;
                    set(s, text);
                    return true;
                },
            };

        private static KeyDef Path(string section, string name, Func<PlayerSettings, string> get, Action<PlayerSettings, string> set, params string[] comment) =>
            new KeyDef
            {
                Section = section, Name = name, Comment = comment,
                Format = s => get(s) ?? string.Empty,
                TryApply = (text, s) =>
                {
                    if (text.Length > 0)
                    {
                        if (text.IndexOfAny(System.IO.Path.GetInvalidPathChars()) >= 0)
                            return false;
                        try
                        {
                            System.IO.Path.GetFullPath(Environment.ExpandEnvironmentVariables(text), SystemSettings.UserDataFolder);
                        }
                        catch (Exception ex) when (ex is ArgumentException || ex is NotSupportedException || ex is PathTooLongException)
                        {
                            return false;
                        }
                    }
                    set(s, text);
                    return true;
                },
            };

        #endregion
    }
}

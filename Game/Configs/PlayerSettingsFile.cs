/* ----- ----- ----- ----- */
// PlayerSettingsFile.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/01
// Update Date: 2026/10/01
// Version: v1.0
/* ----- ----- ----- ----- */

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

using Chinese_Chess_v3.Game.Core.Players;

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

        private static readonly KeyDef[] Keys =
        {
            Str("player", "name", s => s.PlayerName, (s, v) => s.PlayerName = v, maxLength: 32,
                "玩家名稱，用在紀錄區的問候語（可留空，最多 32 字）。"),

            Int("timer", "total_minutes", s => s.TotalTimeMinutes, (s, v) => s.TotalTimeMinutes = v, 1, 600,
                "每方的局時（分鐘，1～600）。"),
            Int("timer", "step_seconds", s => s.StepTimeSeconds, (s, v) => s.StepTimeSeconds = v, 1, 3600,
                "每步的步時（秒，1～3600）；step_timer = false 時不使用。"),
            Int("timer", "increment_seconds", s => s.IncrementSeconds, (s, v) => s.IncrementSeconds = v, 0, 600,
                "每走一步加回局時的秒數（0～600，0 表示不加秒）；僅倒數計時會加秒，正數計時不加。"),
            Bool("timer", "step_timer", s => s.StepTimerEnabled, (s, v) => s.StepTimerEnabled = v,
                "是否限制步時（true／false）。"),
            Enum("timer", "mode", s => s.TimerMode, (s, v) => s.TimerMode = v,
                "計時方式：CountDown（倒數到時限）或 CountUp（從 0 正數）。"),
            Bool("timer", "lose_on_time_up", s => s.EndGameWhenTimesUp, (s, v) => s.EndGameWhenTimesUp = v,
                "時間用完的一方是否判負（true／false）；false 時只停掉那一方的時鐘，棋局繼續。"),

            Bool("rules", "general_can_see_general", s => s.CanGeneralSeeGeneral, (s, v) => s.CanGeneralSeeGeneral = v,
                "是否允許王見王（兩將在同一直線、中間無子）。"),
            Bool("rules", "general_can_leave_palace", s => s.CanGeneralLeavePalace, (s, v) => s.CanGeneralLeavePalace = v,
                "將帥是否可以出九宮。"),
            Bool("rules", "advisor_can_leave_palace", s => s.CanAdvisorLeavePalace, (s, v) => s.CanAdvisorLeavePalace = v,
                "士是否可以出九宮。"),
            Bool("rules", "elephant_eye_blocks", s => s.ElephantEyeCanBeBlocked, (s, v) => s.ElephantEyeCanBeBlocked = v,
                "是否有塞象眼（象田字中心有子就不能走）。"),
            Bool("rules", "horse_leg_blocks", s => s.HorseLegCanBeHobbled, (s, v) => s.HorseLegCanBeHobbled = v,
                "是否有蹩馬腳（馬腳有子就不能往那邊走）。"),

            Bool("hints", "legal_moves", s => s.ShowLegalMoveHints, (s, v) => s.ShowLegalMoveHints = v,
                "選子時是否用圓圈標出可以走的位置。"),
            Bool("hints", "hanging_pieces", s => s.ShowHangingPieceHints, (s, v) => s.ShowHangingPieceHints = v,
                "是否用圓圈標出無根、可被吃的棋子。"),

            Float("input", "wheel_scroll_step", s => s.WheelScrollStep, (s, v) => s.WheelScrollStep = v, 1f, 500f,
                "滑鼠滾輪每一格捲動的距離（介面設計單位，1～500）。"),

            Bool("log", "debug", s => s.ShowDebugLog, (s, v) => s.ShowDebugLog = v,
                "是否顯示 DEBUG 等級的紀錄（較詳細）。"),

            Path("endgame", "user_folder", s => s.EndgameUserFolder, (s, v) => s.EndgameUserFolder = v,
                "自己的殘局題目資料夾；留空 = 預設位置（這個設定檔旁邊的 Endgames 資料夾）。",
                "相對路徑以這個設定檔所在的資料夾為準；可以用 %環境變數%。"),

            Path("opening", "user_folder", s => s.OpeningUserFolder, (s, v) => s.OpeningUserFolder = v,
                "自己的開局練習資料夾；留空 = 預設位置（這個設定檔旁邊的 Openings 資料夾）。",
                "相對路徑以這個設定檔所在的資料夾為準；可以用 %環境變數%。"),
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
        /// Writes <paramref name="settings"/> to <paramref name="path"/> (for the future
        /// settings screen): an existing parsable file keeps its comments and layout with
        /// only the values updated (missing keys added); otherwise a fresh file is written.
        /// Returns false (logged) when the file cannot be written.
        /// </summary>
        public static bool Save(PlayerSettings settings, string path)
        {
            IniDocument doc = null;
            try
            {
                if (File.Exists(path) && IniDocument.TryParse(File.ReadAllText(path), out var existing, out _))
                    doc = existing;
            }
            catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException) { }

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

            return TryWrite(path, doc, m => AppLogger.Log($"(Settings) {m}", LogLevel.WARN));
        }

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

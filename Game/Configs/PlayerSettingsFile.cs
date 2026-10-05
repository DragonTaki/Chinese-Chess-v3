/* ----- ----- ----- ----- */
// PlayerSettingsFile.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/01
// Update Date: 2026/10/05
// Version: v2.0
/* ----- ----- ----- ----- */

using System;
using System.Collections.Generic;
using System.Linq;

using Engine.Configs;

namespace Chinese_Chess_v3.Game.Configs
{
    /// <summary>
    /// The player's <c>settings.ini</c>: its header and the settings file of a
    /// <see cref="PlayerSettings"/> (<see cref="Open"/>: the engine's <see cref="SettingsFile"/>
    /// with the set's areas registered in file order). The reading, repairing and writing rules
    /// are the settings file's. Also copies and compares settings sets through their keys.
    /// </summary>
    public static class PlayerSettingsFile
    {
        /// <summary>The comment lines at the top of a fresh file.</summary>
        public static readonly IReadOnlyList<string> Header = new[]
        {
            "Chinese Chess v3 玩家設定檔",
            "格式：每行一個「key = value」；# 或 ; 開頭的行是註解；[名稱] 是區段。",
            "遊戲啟動時讀取。缺少或打錯的項目會用預設值，並在這個檔案補上／改回預設值；",
            "整個檔案無法解析時，會先備份成 settings.ini.bak 再用預設值重建。",
        };

        /// <summary>The settings file at <paramref name="path"/> (null: <see cref="SystemSettings.PlayerSettingsFilePath"/>) with <paramref name="settings"/>' areas registered.</summary>
        public static SettingsFile Open(PlayerSettings settings, string path = null)
        {
            ArgumentNullException.ThrowIfNull(settings);
            var file = new SettingsFile(path ?? SystemSettings.PlayerSettingsFilePath, Header);
            foreach (var area in settings.Areas)
                file.Register(area);
            return file;
        }

        /// <summary>A new set read from <paramref name="path"/> by the settings file's load rule (<see cref="SettingsFile.Read"/>, nothing applied); warnings are added to <paramref name="warnings"/> if given.</summary>
        public static PlayerSettings Load(string path, ICollection<string> warnings = null)
        {
            var settings = PlayerSettings.Defaults;
            Open(settings, path).Read(warnings);
            return settings;
        }

        /// <summary>Writes <paramref name="settings"/> to <paramref name="path"/> (see <see cref="SettingsFile.Save"/>).</summary>
        public static bool Save(PlayerSettings settings, string path) => Open(settings, path).Save();

        /// <summary>The full text of a fresh settings file holding <paramref name="settings"/>, every key commented.</summary>
        public static string Generate(PlayerSettings settings) => Open(settings, SystemSettings.PlayerSettingsFilePath).Generate();

        /// <summary>Every key of the file as (section, key), in file order.</summary>
        public static IReadOnlyList<(string Section, string Key)> KeyNames => Open(PlayerSettings.Defaults).KeyNames;

        /// <summary>The section of <paramref name="kind"/>'s rules (<see cref="GameRuleSettings.SectionFor"/>).</summary>
        public static string RulesSection(Core.GameKind kind) => GameRuleSettings.SectionFor(kind);

        /// <summary>
        /// Copies every setting of <paramref name="from"/> into <paramref name="to"/> (through
        /// the keys, so a newly added key is covered automatically). A value that would not be
        /// valid in the file (e.g. a name over its length limit) is left unchanged in <paramref name="to"/>.
        /// </summary>
        public static void Copy(PlayerSettings from, PlayerSettings to)
        {
            ArgumentNullException.ThrowIfNull(from);
            ArgumentNullException.ThrowIfNull(to);
            foreach (var (source, target) in KeysOf(from).Zip(KeysOf(to)))
                target.TryApply(source.Format());
        }

        /// <summary>A new set with the same values as <paramref name="settings"/> (see <see cref="Copy"/>).</summary>
        public static PlayerSettings Clone(PlayerSettings settings)
        {
            var clone = PlayerSettings.Defaults;
            Copy(settings, clone);
            return clone;
        }

        /// <summary>Whether two sets hold the same value for every key (as they would be written to the file).</summary>
        public static bool AreEqual(PlayerSettings a, PlayerSettings b) =>
            KeysOf(a).Zip(KeysOf(b)).All(pair => pair.First.Format() == pair.Second.Format());

        private static IEnumerable<SettingsKey> KeysOf(PlayerSettings settings) => settings.Areas.SelectMany(area => area.Keys);
    }
}

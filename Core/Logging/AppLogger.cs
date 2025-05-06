/* ----- ----- ----- ----- */
// AppLogger.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/06
// Update Date: 2025/05/06
// Version: v1.0
/* ----- ----- ----- ----- */

using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

using Chinese_Chess_v3.Configs;

namespace Chinese_Chess_v3.Core.Logging
{
    public enum LogLevel
    {
        INIT,
        DEBUG,
        INFO,
        WARN,
        ERROR
    }

    public class LogLevelMeta
    {
        public string Label { get; set; }
        public string Color { get; set; }

        public static Dictionary<LogLevel, LogLevelMeta> MetaMap = new()
        {
            { LogLevel.INIT,  new LogLevelMeta { Label = "INIT",  Color = null } },
            { LogLevel.DEBUG, new LogLevelMeta { Label = "DEBUG", Color = "gray" } },
            { LogLevel.INFO,  new LogLevelMeta { Label = "INFO",  Color = "white" } },
            { LogLevel.WARN,  new LogLevelMeta { Label = "WARN",  Color = "yellow" } },
            { LogLevel.ERROR, new LogLevelMeta { Label = "ERROR", Color = "red" } }
        };
    }

    public class LogRecord
    {
        public string Message { get; }
        public LogLevel Level { get; }
        public string Timestamp { get; }

        public LogRecord(string message, LogLevel level)
        {
            Message = message;
            Level = level;
            Timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }

        public string ToText()
        {
            var meta = LogLevelMeta.MetaMap[Level];
            return $"{Timestamp} [{meta.Label}] {Message}";
        }

        public string ToJson()
        {
            var meta = LogLevelMeta.MetaMap[Level];
            var payload = new
            {
                text = ToText(),
                color = meta.Color ?? "white",
                tag = $"tag_{meta.Label.ToLower()}"
            };
            return JsonSerializer.Serialize(payload);
        }
    }

    public static class AppLogger
    {
        private static Action<string>? externalLogger = null;
        private static readonly bool enableDebug = Configs.Settings.EnableDebugMode;
        private static readonly string currentUser = Configs.Settings.CurrentUser;

        public static void SetExternalLogger(Action<string> callback)
        {
            externalLogger = callback;
        }

        public static void Log(string message, LogLevel level = LogLevel.INFO)
        {
            if (level == LogLevel.DEBUG && !enableDebug)
                return;

            var record = new LogRecord(message, level);
            Console.WriteLine(record.ToText());

            if (externalLogger != null)
            {
                try
                {
                    externalLogger(record.ToJson());
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[LOGGER ERROR] Failed to send to external logger: {ex.Message}");
                }
            }
        }

        public static void LogWelcomeMessage()
        {
            if (externalLogger == null)
                return;

            var rainbowColors = new[]
            {
                "#FFB3BA", "#FFDFBA", "#FFFFBA", "#BAFFC9",
                "#BAE1FF", "#D5BAFF", "#FFBAED"
            };

            string greeting = string.IsNullOrEmpty(currentUser)
                ? "Hello!"
                : $"Hello, {currentUser}!";

            string welcome = "Welcome to use Griffin Empire Attendance Bot!";
            string author = "Author: DragonTaki";

            try
            {
                // Greeting
                externalLogger(JsonSerializer.Serialize(new[]
                {
                    new {
                        text = greeting + "\n",
                        color = "lightgreen",
                        bold = true,
                        tag = "greeting_tag"
                    }
                }));

                // Rainbow welcome
                var rainbowLine = new List<object>();
                for (int i = 0; i < welcome.Length; i++)
                {
                    rainbowLine.Add(new
                    {
                        text = welcome[i].ToString(),
                        color = rainbowColors[i % rainbowColors.Length],
                        bold = true,
                        tag = $"rainbow_{i}"
                    });
                }
                externalLogger(JsonSerializer.Serialize(rainbowLine));

                // Author
                externalLogger(JsonSerializer.Serialize(new[]
                {
                    new {
                        text = "\n" + author + "\n",
                        color = "cyan",
                        italic = true,
                        tag = "author_tag"
                    }
                }));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[WELCOME ERROR] Failed to send welcome message: {ex.Message}");
            }
        }
    }
}
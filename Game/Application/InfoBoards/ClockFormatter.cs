/* ----- ----- ----- ----- */
// ClockFormatter.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2026/10/05
// Update Date: 2026/10/05
// Version: v1.0
/* ----- ----- ----- ----- */

using System;
using System.Globalization;
using System.Text.RegularExpressions;

using Chinese_Chess_v3.Game.Core.Players;

namespace Chinese_Chess_v3.Game.Application.InfoBoards
{
    /// <summary>
    /// The clock texts shown on the info board: a <see cref="PlayerTimer"/>'s total and step
    /// time (remaining for a countdown clock, elapsed for a count-up one) through a display
    /// template. The timer only measures time; how it is written is decided here.
    /// </summary>
    public class ClockFormatter
    {
        // --- Custom display templates ---
        public string StoppedSymbol { get; set; } = "--:--";
        public string UnlimitedSymbol { get; set; } = "∞:∞";

        // Time display template; placeholders: {hour}, {minute} (total minutes, or the minutes within the hour when {hour} is used too), {second}, {second.N} (N decimals), {totalSecond}. Default "{minute}:{second.2}"
        public string TimeFormat { get; set; } = "{minute}:{second.2}";

        /// <summary>The step time of <paramref name="timer"/>: remaining (countdown) or elapsed (count-up).</summary>
        public string GetStepTimeString(PlayerTimer timer)
        {
            ArgumentNullException.ThrowIfNull(timer);
            TimeSpan display = timer.Mode == TimerMode.CountDown
                ? timer.StepTimeLimit - timer.CurrentStepTime
                : timer.CurrentStepTime;

            return FormatClock(display, timer.ContinueAfterTimeUp);
        }

        /// <summary>The total time of <paramref name="timer"/>: remaining (countdown) or elapsed (count-up).</summary>
        public string GetTotalTimeString(PlayerTimer timer)
        {
            ArgumentNullException.ThrowIfNull(timer);
            TimeSpan display = timer.Mode == TimerMode.CountDown
                ? timer.TotalTimeLimit - timer.CurrentTotalTime
                : timer.CurrentTotalTime;

            return FormatClock(display, timer.ContinueAfterTimeUp);
        }

        /// <summary>
        /// A countdown value for display: time past a limit is shown as a negative time (e.g.
        /// <c>-00:12.30</c>) when the clock runs on into overtime
        /// (<see cref="PlayerTimer.ContinueAfterTimeUp"/>), otherwise it stops at zero.
        /// </summary>
        private string FormatClock(TimeSpan display, bool continueAfterTimeUp)
        {
            if (display >= TimeSpan.Zero)
                return FormatTimeSpan(display);
            return continueAfterTimeUp ? "-" + FormatTimeSpan(display.Negate()) : FormatTimeSpan(TimeSpan.Zero);
        }

        /// <summary>
        /// Writes <paramref name="time"/> (not negative) with <see cref="TimeFormat"/> (the
        /// default template when it is blank), culture-independent.
        /// </summary>
        public string FormatTimeSpan(TimeSpan time)
        {
            const string defaultTemplate = "{minute}:{second.2}";  // default

            string template = TimeFormat;
            if (string.IsNullOrWhiteSpace(template))
                template = defaultTemplate;

            try
            {
                double totalSeconds = time.TotalSeconds;
                int hours = (int)time.TotalHours;
                // {minute} is the minutes within the hour when the template also shows {hour},
                // otherwise the total minutes (e.g. 65 for 1:05:00).
                int minutes = template.Contains("{hour}") ? time.Minutes : (int)time.TotalMinutes;
                int seconds = time.Seconds;

                // Invariant culture throughout: the decimal point is always '.', whatever the
                // system's regional settings.
                // --- Supports fractional-second formats {second.2}, {second.3} ---
                string result = template;

                // Parse {second.X}
                result = Regex.Replace(result, @"\{second\.(\d+)\}", m =>
                {
                    int digits = int.Parse(m.Groups[1].Value, CultureInfo.InvariantCulture);
                    return TruncatedSecondsInMinute(time, digits).ToString($"00.{new string('0', digits)}", CultureInfo.InvariantCulture);
                });

                // Standard fields
                result = result
                    .Replace("{hour}", hours.ToString("00", CultureInfo.InvariantCulture))
                    .Replace("{minute}", minutes.ToString("00", CultureInfo.InvariantCulture))
                    .Replace("{second}", seconds.ToString("00", CultureInfo.InvariantCulture))
                    .Replace("{totalSecond}", totalSeconds.ToString("0.##", CultureInfo.InvariantCulture));

                return result;
            }
            catch
            {
                // fallback when error
                int minutes = (int)time.TotalMinutes;
                double secondsInMinute = TruncatedSecondsInMinute(time, 2);

                return string.Create(CultureInfo.InvariantCulture, $"{minutes:00}:{secondsInMinute:00.00}");
            }
        }

        /// <summary>
        /// Seconds within the current minute, truncated (not rounded) to
        /// <paramref name="digits"/> decimals. Rounding made e.g. 4:59.996 print as
        /// "04:60.00" — a countdown shows that right after every step starts — so the
        /// fraction is cut from the integer tick count instead.
        /// </summary>
        private static double TruncatedSecondsInMinute(TimeSpan time, int digits)
        {
            digits = Math.Clamp(digits, 0, 7);  // TimeSpan resolution is 10^-7 s
            long scale = 1;
            for (int i = 0; i < digits; i++)
                scale *= 10;

            long ticksInMinute = time.Ticks % TimeSpan.TicksPerMinute;
            long units = ticksInMinute * scale / TimeSpan.TicksPerSecond;
            return (double)units / scale;
        }
    }
}

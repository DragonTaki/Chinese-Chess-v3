/* ----- ----- ----- ----- */
// PlayerTimer.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/10
// Update Date: 2025/05/10
// Version: v1.0
/* ----- ----- ----- ----- */

using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace Chinese_Chess_v3.Timing
{
    /// <summary>
    /// Manages countdown timer and total time tracking for a player.
    /// </summary>
    public class PlayerTimer
    {
        private readonly Timer timer;
        private readonly Stopwatch stopwatch;

        public TimeSpan TotalTime { get; private set; }
        public TimeSpan TimeLeft { get; private set; }
        public bool IsRunning => timer.Enabled;

        /// <summary>
        /// Raised when TimeLeft is updated.
        /// </summary>
        public event Action<TimeSpan>? OnTimeUpdated;

        public PlayerTimer(TimeSpan initialTime, int intervalMilliseconds = 1000)
        {
            TimeLeft = initialTime;
            TotalTime = initialTime;
            stopwatch = new Stopwatch();
            timer = new Timer { Interval = intervalMilliseconds };
            timer.Tick += Timer_Tick;
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            TimeLeft = TimeLeft.Subtract(TimeSpan.FromMilliseconds(timer.Interval));

            if (TimeLeft <= TimeSpan.Zero)
            {
                TimeLeft = TimeSpan.Zero;
                Stop();
            }

            OnTimeUpdated?.Invoke(TimeLeft);
        }

        public void Start()
        {
            if (!IsRunning)
            {
                stopwatch.Start();
                timer.Start();
            }
        }

        public void Stop()
        {
            if (IsRunning)
            {
                stopwatch.Stop();
                timer.Stop();
            }
        }

        public void Reset()
        {
            Stop();
            TimeLeft = TotalTime;
            stopwatch.Reset();
            OnTimeUpdated?.Invoke(TimeLeft);
        }

        public void Set(TimeSpan time)
        {
            TotalTime = time;
            TimeLeft = time;
            OnTimeUpdated?.Invoke(TimeLeft);
        }
    }
}

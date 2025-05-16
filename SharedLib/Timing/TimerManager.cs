/* ----- ----- ----- ----- */
// TimerManager.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/07
// Update Date: 2025/05/10
// Version: v2.0
/* ----- ----- ----- ----- */

using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace SharedLib.Timing
{
    /// <summary>
    /// Timer manager that uses a Stopwatch and a Windows Forms Timer
    /// to provide a fixed interval animation timer with delta time calculation.
    /// </summary>
    public class TimerManager : ITimerProvider
    {
        private Stopwatch animationStopwatch;
        private long lastAnimationTimestamp;

        /// <summary>
        /// Gets the elapsed time in seconds since the last animation frame.
        /// </summary>
        public float DeltaTimeInSeconds { get; private set; }

        /// <summary>
        /// Gets the total elapsed time in seconds since the timer started.
        /// </summary>
        public float ElapsedTimeInSeconds => animationStopwatch.ElapsedMilliseconds / 1000f;

        private Timer animationTimer;

        /// <summary>
        /// Event invoked on every animation frame tick.
        /// </summary>
        public event Action OnAnimationFrame;

        /// <summary>
        /// Initializes a new instance of the <see cref="TimerManager"/> class.
        /// </summary>
        public TimerManager()
        {
            animationStopwatch = new Stopwatch();
            lastAnimationTimestamp = 0;

            animationTimer = new Timer { Interval = TimerSettings.GameAnimationInterval };
            animationTimer.Tick += (s, e) =>
            {
                if (!animationStopwatch.IsRunning) return;

                long current = animationStopwatch.ElapsedMilliseconds;
                DeltaTimeInSeconds = (current - lastAnimationTimestamp) / 1000f;
                lastAnimationTimestamp = current;

                OnAnimationFrame?.Invoke();
            };
        }

        /// <summary>
        /// Starts the animation stopwatch and timer.
        /// </summary>
        public void Start() => StartTimers();

        /// <summary>
        /// Stops the animation timer.
        /// </summary>
        public void Stop() => StopTimers();

        /// <summary>
        /// Starts or restarts the stopwatch and timer, resetting elapsed time.
        /// </summary>
        public void StartTimers()
        {
            animationStopwatch.Restart();
            lastAnimationTimestamp = 0;
            animationTimer.Start();
        }

        /// <summary>
        /// Stops the animation timer.
        /// </summary>
        public void StopTimers()
        {
            animationTimer.Stop();
        }
    }
}

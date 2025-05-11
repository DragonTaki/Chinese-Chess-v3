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
    public class TimerManager : ITimerProvider
    {
        private Stopwatch animationStopwatch;
        private long lastAnimationTimestamp;
        public float DeltaTimeInSeconds { get; private set; }
        public float ElapsedTimeInSeconds => animationStopwatch.ElapsedMilliseconds / 1000f;
        private Timer animationTimer;
        public event Action OnAnimationFrame;
        
        public void Start() => StartTimers();
        public void Stop() => StopTimers();

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

        public void StartTimers()
        {
            animationStopwatch.Restart();
            lastAnimationTimestamp = 0;
            animationTimer.Start();
        }

        public void StopTimers()
        {
            animationTimer.Stop();
        }
    }
}

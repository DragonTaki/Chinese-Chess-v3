/* ----- ----- ----- ----- */
// TimerManager.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/07
// Update Date: 2025/05/07
// Version: v1.0
/* ----- ----- ----- ----- */

using System;
using System.Windows.Forms;

using Chinese_Chess_v3.Configs;
using Chinese_Chess_v3.Configs.Board;
using Chinese_Chess_v3.Core;

namespace Chinese_Chess_v3.Utils
{
    public class TimerManager
    {
        private Timer blackPlayerTimer;
        private Timer redPlayerTimer;
        private TimeSpan blackTime;
        private TimeSpan redTime;

        private Timer animationTimer;
        public event Action OnAnimationFrame;
        public event Action<TimeSpan, TimeSpan> OnTimersUpdated;  // Update GUI when timer updates

        public TimerManager()
        {
            redTime = TimeSpan.FromMinutes(5);
            blackTime = TimeSpan.FromMinutes(5);

            redPlayerTimer = new Timer { Interval = TimerSettings.RedPlayerTimerInterval };
            blackPlayerTimer = new Timer { Interval = TimerSettings.BlackPlayerTimerInterval };

            redPlayerTimer.Tick += RedPlayerTimer_Tick;
            blackPlayerTimer.Tick += BlackPlayerTimer_Tick;
            animationTimer = new Timer { Interval = TimerSettings.GameAnimationInterval };
            animationTimer.Tick += (s, e) => OnAnimationFrame?.Invoke();
        }

        public void StartTimers()
        {
            blackPlayerTimer.Start();
            redPlayerTimer.Start();
            animationTimer.Start();
        }

        public void StopTimers()
        {
            blackPlayerTimer.Stop();
            redPlayerTimer.Stop();
            animationTimer.Stop();
        }

        public void SwitchTurn()
        {
            if (GameManager.Instance.CurrentTurn == PlayerSide.Red)
            {
                redPlayerTimer.Start();
                blackPlayerTimer.Stop();
            }
            else
            {
                blackPlayerTimer.Start();
                redPlayerTimer.Stop();
            }
        }

        // Timer update
        private void RedPlayerTimer_Tick(object sender, EventArgs e)
        {
            if (redTime > TimeSpan.Zero)
            {
                redTime = redTime.Add(TimeSpan.FromSeconds(-1));
                OnTimersUpdated?.Invoke(blackTime, redTime);
            }
            else
            {
                StopTimers();
            }
        }
        private void BlackPlayerTimer_Tick(object sender, EventArgs e)
        {
            if (blackTime > TimeSpan.Zero)
            {
                blackTime = blackTime.Add(TimeSpan.FromSeconds(-1));
                OnTimersUpdated?.Invoke(blackTime, redTime);
            }
            else
            {
                StopTimers();
            }
        }

        public (TimeSpan blackTime, TimeSpan redTime) GetCurrentTimes()
        {
            return (blackTime, redTime);
        }
    }
}

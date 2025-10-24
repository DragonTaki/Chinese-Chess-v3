/* ----- ----- ----- ----- */
// LoggerBox.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/06
// Update Date: 2025/10/22
// Version: v2.0
/* ----- ----- ----- ----- */

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text.Json;
using System.Text.Json.Serialization;

using Engine.UI.Core.Base;
using Engine.UI.Core.Interfaces;

namespace Chinese_Chess_v3.Game.UI.Screens.Games.Sidebars.LoggerBoxes
{
    public class LoggerBox
        : InitializableOnceElement<(IUiFactory factory, LoggerBoxHandler handler, LoggerBoxRenderer renderer)>
        , IScreen
    {
        private LoggerBoxHandler _handler;
        private LoggerBoxRenderer _renderer;
        private readonly List<string> _messages = new();
        public IReadOnlyList<string> Messages => _messages;
        public LoggerBox() {}
        protected override void OnInit((IUiFactory factory, LoggerBoxHandler handler, LoggerBoxRenderer renderer) arg)
        {
            _handler = arg.handler;
            _renderer = arg.renderer;
        }

        public void AddMessage(string msg)
        {
            _messages.Add(msg);
            OnMessageAdded?.Invoke(msg);
        }

        public event Action<string> OnMessageAdded;

        public void OnEnter()
        {
            _handler.OnEnter();
        }

        public void OnExit()
        {
            _handler.OnExit();
        }
        protected override void OnDraw(Graphics g)
        {
            _renderer.Draw(g);
        }
    }
}
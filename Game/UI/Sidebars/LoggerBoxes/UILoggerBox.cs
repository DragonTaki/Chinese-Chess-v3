/* ----- ----- ----- ----- */
// UILoggerBox.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/06
// Update Date: 2026/10/13
// Version: v2.6
/* ----- ----- ----- ----- */

using System.Collections.Concurrent;
using System.Collections.Generic;

using Chinese_Chess_v3.Game.UI.Constants;

using Engine.Logging;
using Engine.UI.Core.Elements;
using Engine.UI.Core.Interfaces;
using Engine.UI.Utils;

namespace Chinese_Chess_v3.Game.UI.Sidebars.LoggerBoxes
{
    public class UILoggerBox : UITextBox<UILoggerBox, UILoggerBoxHandler, UILoggerBoxRenderer>, IResettable
    {
        private readonly ConcurrentQueue<LogRecord> _pendingRecords = new();
        private readonly SortedList<int, UILabel> _remoteMoves = new();

        /// <summary>Inserts a move before any later server ply without rebuilding existing log labels.</summary>
        public void AddRemoteMove(int ply, string message)
        {
            if (_remoteMoves.ContainsKey(ply))
                return;
            AppendLine(message);
            var label = (UILabel)ScrollContainer.Children[ScrollContainer.Children.Count - 1];
            _remoteMoves.Add(ply, label);
            int index = _remoteMoves.IndexOfKey(ply);
            if (index + 1 < _remoteMoves.Count)
            {
                var next = _remoteMoves.Values[index + 1];
                int childIndex = ScrollContainer.Children.IndexOf(next);
                float y = next.Layout.Y;
                float offset = label.Layout.Height + ParagraphSpacing;
                ScrollContainer.Children.Remove(label);
                for (int i = childIndex; i < ScrollContainer.Children.Count; i++)
                {
                    var child = ScrollContainer.Children[i];
                    child.Layout = new Engine.Geometry.LayoutF(child.Layout.X, child.Layout.Y + offset,
                        child.Layout.Width, child.Layout.Height);
                    child.LocalPosition.Current.Y = child.Layout.Y;
                }
                label.Layout = new Engine.Geometry.LayoutF(label.Layout.X, y, label.Layout.Width, label.Layout.Height);
                label.LocalPosition.Current.Y = y;
                ScrollContainer.Children.Insert(childIndex, label);
                ScrollContainer.NotifyChildOrderChanged();
                ScrollContainer.RefreshContentSize(forceAlignment: true);
            }
        }

        public UILoggerBox() { }

        private void QueueRecord(LogRecord record) => _pendingRecords.Enqueue(record);

        internal void AppendPendingRecords()
        {
            while (_pendingRecords.TryDequeue(out var record))
            {
                var colorName = LogLevelMeta.MetaMap[record.Level].Color;
                var color = colorName == null ? TextColor : System.Drawing.Color.FromName(colorName);
                AppendLine(record.ToText(), color,
                    isDebugContent: LogLineBackground.IsDebug(record.Level));
            }
        }

        protected override void DisposeUI()
        {
            AppLogger.RecordLogged -= QueueRecord;
            base.DisposeUI();
        }

        protected override void OnInit(IUiFactory factory)
        {
            Layout = UILayoutConstants.Sidebar.LoggerBox.Layout;
            ScrollContainer.Layout = UILayoutConstants.Sidebar.LoggerBox.ScrollContainer.Layout;

            // Flex item of the sidebar column; the scroll area follows the box, inset by the
            // logger margin (see UILayoutSheet.GameScreen.LoggerBox*).
            LayoutRules.Apply(UILayoutSheet.GameScreen.LoggerBox);
            ScrollContainer.LayoutRules.Apply(UILayoutSheet.GameScreen.LoggerBoxScrollContainer);

            // The logger's own style settings (defined but never applied before).
            BackgroundColor = UILoggerBoxSettings.BackgroundColor;
            TextColor = UILoggerBoxSettings.TextColor;
            Font = UILoggerBoxSettings.Font;

            // Typed records retain their level; drain them on the rendering thread.
            AppLogger.RecordLogged += QueueRecord;
        }

        protected override void OnReset()
        {
            while (_pendingRecords.TryDequeue(out _)) { }
            _remoteMoves.Clear();
            ClearLogs();
        }
    }
}

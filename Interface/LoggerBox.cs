/* ----- ----- ----- ----- */
// LoggerBox.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/06
// Update Date: 2025/05/06
// Version: v1.0
/* ----- ----- ----- ----- */

using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows.Forms;

using Chinese_Chess_v3.Configs;

namespace Chinese_Chess_v3.Interface
{
    public class LoggerBox : RichTextBox
    {
        public LoggerBox()
        {
            this.ReadOnly = true;
            this.BorderStyle = BorderStyle.FixedSingle;
            this.Font = LoggerSettings.Font;
            this.BackColor = LoggerSettings.BackgroundColor;
            this.ForeColor = LoggerSettings.TextColor;
            this.Location = new System.Drawing.Point(SidebarSettings.StartX + SidebarSettings.Margin, SidebarSettings.StartY + SidebarSettings.Margin);
            this.Size = new System.Drawing.Size(SidebarSettings.LoggerWidth, SidebarSettings.LoggerHeight);
            this.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            //this.ContextMenuStrip = new ContextMenuStrip();
            //this.MouseClick += LoggerBox_MouseClick;
        }
        public void AppendLog(string json)
        {
            try
            {
                var logEntry = JsonSerializer.Deserialize<LogEntry>(json);
                if (logEntry != null)
                {
                    this.AppendText(logEntry.Text + "\n");
                }
            }
            catch
            {
                this.AppendText("[Log Parse Error] " + json + "\n");
            }
        }

        private class LogEntry
        {
            [JsonPropertyName("text")]
            public string Text { get; set; } = "";

            [JsonPropertyName("color")]
            public string Color { get; set; } = "white";

            [JsonPropertyName("tag")]
            public string Tag { get; set; } = "";
        }
    }
}
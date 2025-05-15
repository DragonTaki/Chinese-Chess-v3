/* ----- ----- ----- ----- */
// LoggerBox.cs
// Do not distribute or modify
// Author: DragonTaki (https://github.com/DragonTaki)
// Create Date: 2025/05/06
// Update Date: 2025/05/06
// Version: v1.0
/* ----- ----- ----- ----- */

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows.Forms;

using Chinese_Chess_v3.Configs.Sidebar;
using Chinese_Chess_v3.Interface.UI.Constants;

namespace Chinese_Chess_v3.Interface.Sidebar
{
    public class LoggerBox : RichTextBox
    {
        public LoggerBox()
        {
            this.Location = UILayoutConstants.Sidebar.Logger.Position.ToPoint();
            this.Size = UILayoutConstants.Sidebar.Logger.Size.ToSize();
            this.ReadOnly = true;
            this.BorderStyle = BorderStyle.FixedSingle;
            this.Font = LoggerSettings.Font;
            this.BackColor = LoggerSettings.BackgroundColor;
            this.ForeColor = LoggerSettings.TextColor;
            this.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            //this.ContextMenuStrip = new ContextMenuStrip();
            //this.MouseClick += LoggerBox_MouseClick;
        }

        public void AppendLog(string json)
        {
            try
            {
                if (json.TrimStart().StartsWith('['))
                {
                    var fragments = JsonSerializer.Deserialize<List<LogFragment>>(json);
                    if (fragments != null)
                        AppendFragments(fragments);
                }
                else
                {
                    var fragment = JsonSerializer.Deserialize<LogFragment>(json);
                    if (fragment != null)
                        AppendFragments(new List<LogFragment> { fragment });
                }
            }
            catch (Exception ex)
            {
                this.AppendText($"[Logger Error] Failed to parse JSON: {ex.Message}\n");
            }
        }

        private void AppendFragments(List<LogFragment> fragments)
        {
            foreach (var frag in fragments)
            {
                string[] lines = frag.Text.Split('\n');

                for (int i = 0; i < lines.Length; i++)
                {
                    AppendFormattedText(lines[i], frag.Color, frag.Bold, frag.Italic);

                    if (i < lines.Length - 1)
                        AppendFormattedText(Environment.NewLine, frag.Color, frag.Bold, frag.Italic);
                }
            }

            this.AppendText(Environment.NewLine);
            this.ScrollToCaret();
        }

        private void AppendFormattedText(string text, string? colorHex, bool bold = false, bool italic = false)
        {
            Color foreColor = Color.White;
            try
            {
                if (!string.IsNullOrEmpty(colorHex))
                    foreColor = ColorTranslator.FromHtml(colorHex);
            }
            catch { }

            FontStyle style = FontStyle.Regular;
            if (bold) style |= FontStyle.Bold;
            if (italic) style |= FontStyle.Italic;

            this.SelectionStart = this.TextLength;
            this.SelectionLength = 0;
            this.SelectionColor = foreColor;
            this.SelectionFont = new Font(this.Font, style);
            this.AppendText(text);
        }

        private class LogEntry
        {
            [JsonPropertyName("text")]
            public string Text { get; set; } = "";

            [JsonPropertyName("color")]
            public string Color { get; set; } = "white";

            [JsonPropertyName("bold")]
            public bool Bold { get; set; }

            [JsonPropertyName("italic")]
            public bool Italic { get; set; }

            [JsonPropertyName("tag")]
            public string Tag { get; set; } = "";
        }

        private class LogFragment
        {
            [JsonPropertyName("text")]
            public string Text { get; set; } = "";

            [JsonPropertyName("color")]
            public string Color { get; set; } = "white";

            [JsonPropertyName("bold")]
            public bool Bold { get; set; }

            [JsonPropertyName("italic")]
            public bool Italic { get; set; }

            [JsonPropertyName("tag")]
            public string Tag { get; set; } = "";
        }
    }
}
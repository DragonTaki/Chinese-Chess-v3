using System;
using System.Drawing;
using System.Windows.Forms;
using Chinese_Chess_v3.Core;
using Chinese_Chess_v3.Constants;

namespace Chinese_Chess_v3.Interface
{
    public class MainForm : Form
    {
        private GameManager game;

        public MainForm()
        {
            this.Text = "Chinese Chess v3";
            this.ClientSize = new Size(Settings.BoardWidth * Settings.CellSize, Settings.BoardHeight * Settings.CellSize);
            this.DoubleBuffered = true;

            game = new GameManager();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            DrawBoard(e.Graphics);
        }

        private void DrawBoard(Graphics g)
        {
            for (int i = 0; i <= Settings.BoardWidth; i++)
                g.DrawLine(Pens.Black, i * Settings.CellSize, 0, i * Settings.CellSize, Settings.CellSize * Settings.BoardHeight);

            for (int j = 0; j <= Settings.BoardHeight; j++)
                g.DrawLine(Pens.Black, 0, j * Settings.CellSize, Settings.CellSize * Settings.BoardWidth, j * Settings.CellSize);
        }
    }
}
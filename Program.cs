using System;
using System.Windows.Forms;
using Chinese_Chess_v3.Interface;

namespace Chinese_Chess_v3
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}
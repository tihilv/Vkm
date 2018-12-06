using System;
using System.Windows.Forms;
using HarmonyHub;

namespace Vkm.Console
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());

            Client c;
            c.SendCommandAsync()
        }
   }
}

using System;
using System.Windows.Forms;

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
            Test();
            Application.Run(new Form1());
        }

        public static void Test()
        {

        }
    }
}

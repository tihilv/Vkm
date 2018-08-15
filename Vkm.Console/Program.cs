using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Vkm.Common;
using Vkm.Core;
using Vkm.Device.StreamDeck;
using Vkm.Library;
using Vkm.Library.Numpad;

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
            StreamDeckDeviceFactory factory = new StreamDeckDeviceFactory();
            var device = factory.GetDevices().FirstOrDefault();


        }
    }
}

﻿using System;
using System.Windows.Forms;

namespace Vkm.Manager
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new VkmApplicationContext());
        }
    }
}

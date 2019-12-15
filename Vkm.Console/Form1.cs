﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Vkm.Api.Basic;
using Vkm.Api.Identification;
using Vkm.Api.Layout;
using Vkm.Intercom.Service;
using Vkm.Intercom.Service.Api;

namespace Vkm.Console
{
    public partial class Form1 : Form, IVkmRemoteCallback
    {
        public Form1()
        {
            InitializeComponent();
            var x = ShowInTaskbar;
        }

        private VkmIntercomClient _client;
        
        private void button1_Click(object sender, EventArgs e)
        {
            _client = VkmIntercomClient.Create(this);
            var layoutId = _client.GetLayout("MyLayout");
            var devices = _client.GetDevices();
            _client.SwitchToLayout(devices[0].Identifier, layoutId);
        }

        public void ButtonPressed(Identifier layoutId, Location location, ButtonEvent buttonEvent)
        {
            MessageBox.Show($"Pressed: {layoutId} {location} {buttonEvent}");
        }
    }
}

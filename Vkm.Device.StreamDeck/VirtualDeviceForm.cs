using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Vkm.Api.Basic;
using Vkm.Api.Device;

namespace Vkm.Device.StreamDeck
{
    public partial class VirtualDeviceForm : Form
    {
        public event EventHandler<ButtonEventArgs> ButtonEvent;

        public VirtualDeviceForm()
        {
            InitializeComponent();
        }

        internal void Init(DeviceSize deviceSize, IconSize iconSize)
        {
            Controls.Clear();

            for (byte i = 0; i < deviceSize.Width; i++)
            for (byte j = 0; j < deviceSize.Height; j++)
            {
                PictureBox box = new PictureBox();
                box.Size = new Size(iconSize.Width, iconSize.Height);
                box.Tag = new Location(i, j);
                box.MouseDown += BoxOnMouseDown;
                box.MouseUp += BoxOnMouseUp;

                Controls.Add(box);
                box.Parent = this;
                box.Left = i * iconSize.Width;
                box.Top = j * iconSize.Height;
            }

            Size = new Size(deviceSize.Width * iconSize.Width, deviceSize.Height * iconSize.Height);
        }

        internal void SetImage(Location location, Bitmap bitmap)
        {
            var picBox = Controls.OfType<PictureBox>().FirstOrDefault(p => (Location)p.Tag == location);
            if (picBox != null)
            {
                if (picBox.Image != null)
                    picBox.Image.Dispose();
                picBox.Image = (Image)bitmap.Clone();
            }
        }

        private void BoxOnMouseDown(object sender, MouseEventArgs e)
        {
            var location = (Location)((PictureBox)sender).Tag;
            ButtonEvent?.Invoke(this, new ButtonEventArgs(location, true));
        }

        private void BoxOnMouseUp(object sender, MouseEventArgs e)
        {
            var location = (Location)((PictureBox)sender).Tag;
            ButtonEvent?.Invoke(this, new ButtonEventArgs(location, false));
        }

        public void ClearImages()
        {
            foreach (var pictureBox in Controls.OfType<PictureBox>())
            {
                pictureBox.Image?.Dispose();
                pictureBox.Image = null;
            }
        }
    }
}

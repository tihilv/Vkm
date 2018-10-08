using Vkm.Api.Identification;
using Vkm.Common.Win32.Win32;
using Vkm.Library.Interfaces.Service;

namespace Vkm.Library.Service
{
    public class KeyboardService: IKeyboardService
    {
        public Identifier Id => new Identifier("Vkm.KeyboardService");
        public string Name => "Keyboard Service";

        public void SendKeys(string keys)
        {
            System.Windows.Forms.SendKeys.SendWait(keys);
        }

        public void SendKeys(byte keys)
        {
            Win32.ButtonPress(keys);
        }

        public byte PreviousTrack => 0xB1;
        public byte NextTrack => 0xB0;
        public byte PlayPause => 0xB3;
    }
}

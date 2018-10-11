using Vkm.Api.Identification;
using Vkm.Common.Win32.Win32;
using Vkm.Library.Interfaces.Service;

namespace Vkm.Library.Service
{
    class PowerService: IPowerService
    {
        public Identifier Id => new Identifier("Vkm.PowerService");
        public string Name => "Power Service";

        public void DoPowerAction(PowerAction action)
        {
            Win32.DoExitWin((int)action);
        }
    }
}

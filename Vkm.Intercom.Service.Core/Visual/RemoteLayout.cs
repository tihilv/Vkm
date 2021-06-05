using Vkm.Api.Basic;
using Vkm.Api.Data;
using Vkm.Api.Drawable;
using Vkm.Api.Identification;
using Vkm.Api.Layout;
using Vkm.Intercom.Service.Api;

namespace Vkm.Intercom.Service.Visual
{
    internal class RemoteLayout : LayoutBase
    {
        readonly IVkmRemoteCallback _callbackClient;

        public RemoteLayout(Identifier identifier, IVkmRemoteCallback callbackClient) : base(identifier)
        {
            _callbackClient = callbackClient;
        }

        public void SetBitmap(Location location, byte[] bitmapBytes)
        {
            foreach (var elementPlacement in Elements)
            {
                if (elementPlacement.Location == location)
                {
                    ((RemoteElement) elementPlacement.Element).SetBitmap(bitmapBytes);
                    return;
                }
            }

            RemoteElement newElement = GlobalContext.InitializeEntity(new RemoteElement(new Identifier()));
            AddElement(location, newElement);
            newElement.SetBitmap(bitmapBytes);
        }
        
        public void RemoveBitmap(Location location)
        {
            RemoveElement(location);
        }

        public override void ButtonPressed(Location location, ButtonEvent buttonEvent, LayoutContext layoutContext)
        {
            base.ButtonPressed(location, buttonEvent, layoutContext);

            _callbackClient.ButtonPressed(Id, location, buttonEvent);
        }
    }
}
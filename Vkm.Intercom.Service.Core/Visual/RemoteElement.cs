using Vkm.Api.Basic;
using Vkm.Api.Common;
using Vkm.Api.Data;
using Vkm.Api.Element;
using Vkm.Api.Identification;
using Vkm.Api.Layout;

namespace Vkm.Intercom.Service.Visual
{
    internal class RemoteElement : ElementBase
    {
        private byte[] _bitmapBytes;
        private BitmapRepresentation _bitmapRepresentation;

        public RemoteElement(Identifier identifier) : base(identifier)
        {
        }

        public override DeviceSize ButtonCount => new DeviceSize(1, 1);

        public override void EnterLayout(LayoutContext layoutContext, ILayout previousLayout)
        {
            base.EnterLayout(layoutContext, previousLayout);

            Draw();
        }

        private void Draw()
        {
            if (_bitmapBytes != null && LayoutContext != null)
            {
                if (_bitmapRepresentation == null)
                    _bitmapRepresentation = GetBitmapRepresentation(_bitmapBytes);

                DrawInvoke(new[] {new LayoutDrawElement(new Location(0, 0), _bitmapRepresentation.Clone())});
            }
        }

        private BitmapRepresentation GetBitmapRepresentation(byte[] bitmapBytes)
        {
            using (var bitmapRepresentation = new BitmapRepresentation(bitmapBytes))
            using (var bmpEx = bitmapRepresentation.CreateBitmap())
            using (var result = LayoutContext.CreateBitmap())
            {
                BitmapHelpers.ResizeBitmap(bmpEx, result);
                return new BitmapRepresentation(result);
            }
        }

        public void SetBitmap(byte[] bitmapBytes)
        {
            _bitmapBytes = bitmapBytes;
            _bitmapRepresentation = null;

            Draw();
        }

    }
}
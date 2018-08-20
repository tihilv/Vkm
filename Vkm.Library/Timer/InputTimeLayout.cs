using System.Drawing;
using Vkm.Api.Basic;
using Vkm.Api.Data;
using Vkm.Api.Element;
using Vkm.Api.Identification;
using Vkm.Api.Layout;
using Vkm.Library.Common;

namespace Vkm.Library.Timer
{
    class InputTimeLayout: LayoutBase
    {
        private readonly byte[] _values;
        private byte _currentIndex;

        public byte[] Values => _values;

        public InputTimeLayout(Identifier identifier) : base(identifier)
        {
            _values = new byte[4];
            _currentIndex = 0;
        }

        public override void Init()
        {
            base.Init();

            AddElement(new Location(0,0), GlobalContext.InitializeEntity(new InputButtonElement(this, 7)));
            AddElement(new Location(1,0), GlobalContext.InitializeEntity(new InputButtonElement(this, 8)));
            AddElement(new Location(2,0), GlobalContext.InitializeEntity(new InputButtonElement(this, 9)));

            AddElement(new Location(0,1), GlobalContext.InitializeEntity(new InputButtonElement(this, 4)));
            AddElement(new Location(1,1), GlobalContext.InitializeEntity(new InputButtonElement(this, 5)));
            AddElement(new Location(2,1), GlobalContext.InitializeEntity(new InputButtonElement(this, 6)));

            AddElement(new Location(0,2), GlobalContext.InitializeEntity(new InputButtonElement(this, 1)));
            AddElement(new Location(1,2), GlobalContext.InitializeEntity(new InputButtonElement(this, 2)));
            AddElement(new Location(2,2), GlobalContext.InitializeEntity(new InputButtonElement(this, 3)));

            AddElement(new Location(3,2), GlobalContext.InitializeEntity(new InputButtonElement(this, 0)));
        }

        public override void EnterLayout(LayoutContext layoutContext, ILayout previousLayout)
        {
            base.EnterLayout(layoutContext, previousLayout);

            _currentIndex = 0;
        }

        private void SetValue(byte value)
        {
            if (_currentIndex == 2 && value > 5)
                return;

            _values[_currentIndex] = value;

            var bmp = LayoutContext.CreateBitmap();
            DefaultDrawingAlgs.DrawText(bmp, GlobalContext.Options.Theme.FontFamily, value.ToString(), "8", GlobalContext.Options.Theme.ForegroundColor);
            DrawInvoke(new[] {new LayoutDrawElement(new Location((byte) (3 + _currentIndex % 2), (byte) (_currentIndex / 2)), bmp)});

            _currentIndex++;
            if (_currentIndex == _values.Length)
                LayoutContext.SetPreviousLayout();
        }

        class InputButtonElement : ElementBase
        {
            public override DeviceSize ButtonCount => new DeviceSize(1, 1);

            private readonly InputTimeLayout _inputTimeLayout;
            private readonly byte _value;

            public InputButtonElement(InputTimeLayout inputTimeLayout, byte value) : base(new Identifier($"ButtonValue.V{value}"))
            {
                _inputTimeLayout = inputTimeLayout;
                _value = value;
            }

            public override void EnterLayout(LayoutContext layoutContext, ILayout previousLayout)
            {
                base.EnterLayout(layoutContext, previousLayout);

                DrawInvoke(new [] {new LayoutDrawElement(new Location(0, 0), DrawKey())});
            }

            private BitmapEx DrawKey()
            {
                var bitmap = LayoutContext.CreateBitmap();

                var fontFamily = GlobalContext.Options.Theme.FontFamily;

                DefaultDrawingAlgs.DrawText(bitmap, fontFamily, _value.ToString(), "8", GlobalContext.Options.Theme.ForegroundColor);

                return bitmap;
            }

            public override bool ButtonPressed(Location location, bool isDown)
            {
                if (isDown && location.X == 0 && location.Y == 0)
                {
                    _inputTimeLayout.SetValue(_value);
                    return true;
                }

                return false;
            }

        }
    }
}

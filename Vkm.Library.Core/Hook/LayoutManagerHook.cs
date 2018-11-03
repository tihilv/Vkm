using Vkm.Api.Basic;
using Vkm.Api.Hook;
using Vkm.Api.Identification;
using Vkm.Api.Layout;

namespace Vkm.Library.Hook
{
    public class LayoutManagerHook: IDeviceHook
    {
        public static Identifier Identifier = new Identifier("Vkm.Hook.LayoutManager");

        public Identifier Id => Identifier;

        public string Name => "Layout Manager Hook";

        private bool _longPress;
        public bool OnKeyEventHook(Location location, ButtonEvent buttonEvent, ILayout layout)
        {
            if (location == new Location(4, 2))
            {
                if (buttonEvent == ButtonEvent.Down)
                {
                    _longPress = false;
                    return true;
                }

                if (buttonEvent == ButtonEvent.Up)
                {
                    if (!_longPress)
                    {
                        layout.ButtonPressed(location, ButtonEvent.Down);
                        layout.ButtonPressed(location, ButtonEvent.Up);
                    }

                    return true;
                }

                if (buttonEvent == ButtonEvent.LongPress)
                {
                    _longPress = true;
                }
            }

            return false;
        }
    }
}

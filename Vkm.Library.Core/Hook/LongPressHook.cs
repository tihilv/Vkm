using Vkm.Api.Basic;
using Vkm.Api.Data;
using Vkm.Api.Hook;
using Vkm.Api.Identification;
using Vkm.Api.Layout;
using Vkm.Api.Options;

namespace Vkm.Library.Hook
{
    public class LongPressHook: IDeviceHook, IInitializable, IOptionsProvider
    {
        public static Identifier Identifier = new Identifier("Vkm.Hook.LayoutManager");

        public Identifier Id => Identifier;

        public string Name => "Layout Manager Hook";

        private LongPressHookOptions _options;
        
        private bool _longPress;

        public bool OnKeyEventHook(Location location, ButtonEvent buttonEvent, ILayout layout, LayoutContext layoutContext)
        {
            if (location == _options.Location)
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
                    layoutContext.SetLayout(_options.LayoutIdentifier);
                }
            }

            return false;
        }

        public void InitContext(GlobalContext context)
        {
            
        }

        public void Init()
        {
            
        }

        public IOptions GetDefaultOptions()
        {
            return new LongPressHookOptions();
        }

        public void InitOptions(IOptions options)
        {
            _options = (LongPressHookOptions) options;
        }
    }
}

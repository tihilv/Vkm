using System;
using Vkm.Api.Options;

namespace Vkm.Core.Options
{
    [Serializable]
    class CoreOptions: IOptions
    {
        private readonly GlobalOptions _globalOptions;
        private readonly LayoutLoadOptions _layoutLoadOptions;
        private readonly TransitionLoadOptions _transitionLoadOptions;

        public CoreOptions()
        {
            _globalOptions = new GlobalOptions();
            _layoutLoadOptions = new LayoutLoadOptions();
            _transitionLoadOptions = new TransitionLoadOptions();
        }

        public LayoutLoadOptions LayoutLoadOptions => _layoutLoadOptions;

        public TransitionLoadOptions TransitionLoadOptions => _transitionLoadOptions;

        public GlobalOptions GlobalOptions => _globalOptions;
    }
}

using Vkm.Api.Identification;
using Vkm.Api.Module;
using Vkm.Api.Options;
using Vkm.Common;
using Vkm.Core.Options;

namespace Vkm.Core
{
    public partial class VkmCore
    {
        private CoreOptions _coreOptions;

        public Identifier Id => new Identifier("CoreContext.Options");

        public IOptions GetDefaultOptions()
        {
            var result = new CoreOptions();

            result.LayoutLoadOptions.InitializationInfos.Add(new ModuleInitializationInfo(Identifiers.DefaultNumpadFactory, Identifiers.DefaultNumpadLayout));
            result.LayoutLoadOptions.InitializationInfos.Add(new ModuleInitializationInfo(Identifiers.DefaultScreenSaverFactory, Identifiers.DefaultScreenSaverLayout));
            result.LayoutLoadOptions.InitializationInfos.Add(new ModuleInitializationInfo(Identifiers.DefaultCompositeLayoutFactory, Identifiers.DefaultCompositeLayout));

            result.TransitionLoadOptions.InitializationInfos.Add(new ModuleInitializationInfo(Identifiers.DefaultStartupTransitionFactory, Identifiers.DefaultStartupTransition));
            result.TransitionLoadOptions.InitializationInfos.Add(new ModuleInitializationInfo(Identifiers.DefaultIdleTransitionFactory, Identifiers.DefaultIdleTransition));

            result.TransitionLoadOptions.InitializationInfos.Add(new ModuleInitializationInfo(Identifiers.DefaultApplicationChangeTransitionFactory, Identifiers.DefaultApplicationChangeTransitionCalc));
            result.TransitionLoadOptions.InitializationInfos.Add(new ModuleInitializationInfo(Identifiers.DefaultApplicationChangeTransitionFactory, Identifiers.DefaultApplicationChangeTransitionExcel));
            result.TransitionLoadOptions.InitializationInfos.Add(new ModuleInitializationInfo(Identifiers.DefaultApplicationChangeTransitionFactory, Identifiers.DefaultApplicationChangeTransitionTotalCmd));

            

            return result;
        }

        public void InitOptions(IOptions options)
        {
            _coreOptions = (CoreOptions) options;
        }
    }
}

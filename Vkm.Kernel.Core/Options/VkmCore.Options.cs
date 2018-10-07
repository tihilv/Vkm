using Vkm.Api.Identification;
using Vkm.Api.Options;

namespace Vkm.Kernel
{
    public partial class VkmKernel
    {
        private GlobalOptions _coreOptions;

        public Identifier Id => new Identifier("CoreContext.Options");

        public IOptions GetDefaultOptions()
        {
            return new GlobalOptions();
        }

        public void InitOptions(IOptions options)
        {
            _coreOptions = (GlobalOptions) options;
        }
    }
}

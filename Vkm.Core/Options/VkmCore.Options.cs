using Vkm.Api.Identification;
using Vkm.Api.Options;

namespace Vkm.Core
{
    public partial class VkmCore
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

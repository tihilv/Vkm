using System.Collections.Generic;
using Vkm.Api.Configurator;
using Vkm.Api.Identification;
using Vkm.Api.Options;

namespace Vkm.TestProject.Entities
{
    internal class TestOptionsService: IOptionsService
    {
        public void InitOptions(IEnumerable<IConfigurator> configurators)
        {
            
        }

        public void InitEntity(IOptionsProvider optionsProvider)
        {
            optionsProvider.InitOptions(optionsProvider.GetDefaultOptions());
        }

        public void SaveOptions()
        {
            
        }

        public void SetDefaultOptions(Identifier identifier, IOptions options)
        {
            
        }
    }
}
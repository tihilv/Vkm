using System.Collections.Generic;
using Vkm.Api.Configurator;
using Vkm.Api.Identification;

namespace Vkm.Api.Options
{
    public interface IOptionsService
    {
        void InitOptions(IEnumerable<IConfigurator> configurators);
        void InitEntity(IOptionsProvider optionsProvider);
        void SaveOptions();

        void SetDefaultOptions(Identifier identifier, IOptions options);
    }
}
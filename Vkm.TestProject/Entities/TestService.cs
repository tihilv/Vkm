using Vkm.Api.Data;
using Vkm.Api.Identification;
using Vkm.Api.Options;
using Vkm.Api.Service;

namespace Vkm.TestProject.Entities
{
    internal interface ITestService : IService
    {
        int GetValue(int input);
        
        int InitContextId { get; set; }
        int InitId { get; set; }
        int InitOptionsId { get; set; }
        
        IOptions CreatedOptions { get; set; }
        IOptions AssignedOptions { get; set; }
    }
    
    internal class TestService: IInitializable, IOptionsProvider, ITestService
    {
        public Identifier Id { get; }
        public string Name { get; }

        private int _initIndex;
        
        public int InitOptionsId { get; set; }
        public IOptions CreatedOptions { get; set; }
        public IOptions AssignedOptions { get; set; }
        public int InitContextId { get; set; }
        public int InitId { get; set; }
        
        public int GetValue(int input)
        {
            return input + 1;
        }

        public void InitContext(GlobalContext context)
        {
            InitContextId = _initIndex++;
        }

        public void Init()
        {
            InitId = _initIndex++;
        }

        public IOptions GetDefaultOptions()
        {
            CreatedOptions = new TestServiceOptions();
            return CreatedOptions;
        }

        public void InitOptions(IOptions options)
        {
            InitOptionsId = _initIndex++;
            AssignedOptions = options;
        }
    }

    internal class TestServiceOptions: IOptions
    {
        
    }
}
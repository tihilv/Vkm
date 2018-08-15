using System;
using Vkm.Api.Options;

namespace Vkm.Library.Run
{
    [Serializable]
    public class RunOptions: IOptions
    {
        private string _executable;
        private string _symbol;

        public string Executable
        {
            get => _executable;
            set => _executable = value;
        }

        public string Symbol
        {
            get => _symbol;
            set => _symbol = value;
        }
    }
}
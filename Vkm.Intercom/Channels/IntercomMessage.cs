using System;

namespace Vkm.Intercom.Channels
{
    [Serializable]
    public class IntercomMessage
    {
        private readonly string _methodName;

        private readonly object[] _arguments;

        public string MethodName => _methodName;

        public object[] Arguments => _arguments;

        public IntercomMessage(string methodName, object[] arguments)
        {
            _methodName = methodName;
            _arguments = arguments;
        }
    }
}
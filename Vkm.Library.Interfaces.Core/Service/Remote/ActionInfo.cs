namespace Vkm.Library.Interfaces.Service.Remote
{
    public class ActionInfo
    {
        private readonly string _id;
        
        private readonly string _device;
        private readonly string _command;
        
        private readonly string _text;
        private readonly bool _active;
        
        public string Id => _id;

        public string Text => _text;

        public bool Active => _active;

        public string Device => _device;

        public string Command => _command;


        public ActionInfo(string id, string text, bool active)
        {
            _id = id;
            _text = text;
            _active = active;
        }

        public ActionInfo(string device, string command, string text)
        {
            _device = device;
            _command = command;
            _text = text;
        }
    }
}
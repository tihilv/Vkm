using System;

namespace Vkm.Library.Interfaces.Service.Remote
{
    public class ActionEventArgs : EventArgs
    {
        private readonly string _actionId;

        public string ActionId => _actionId;

        public ActionEventArgs(string actionId)
        {
            _actionId = actionId;
        }
    }
}
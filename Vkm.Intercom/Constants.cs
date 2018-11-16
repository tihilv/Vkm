using Vkm.Intercom.Dispatchers;

namespace Vkm.Intercom
{
    internal static class Constants
    {
        internal const string Result = "+result";
        internal const string Exception = "+exception";
        internal const string OneWay = "+oneway";
        
        internal const string Dispatcher = "_dispatcher";
        internal const string Callback = "_callback";
        
        internal const string DispatchMethod = nameof(IntercomMasterDispatcher<IRemoteService,IRemoteService>.Dispatch);
    }
}
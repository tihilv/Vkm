using System;

namespace Vkm.Api.Basic
{
    public static class DisposeHelper
    {
        public static void DisposeAndNull<T>(ref T disposable) where T : IDisposable
        {
            var d = disposable;
            disposable = default(T);
            d?.Dispose();
        }
    }
}

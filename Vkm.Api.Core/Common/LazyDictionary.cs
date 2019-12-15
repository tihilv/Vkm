using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Vkm.Api.Common
{
    public class LazyDictionary<TKey, TValue>
    {
        private readonly ConcurrentDictionary<TKey, Lazy<TValue>> _inner;

        public LazyDictionary()
        {
            _inner = new ConcurrentDictionary<TKey, Lazy<TValue>>();
        }

        public TValue GetOrAdd(TKey key, Func<TKey, TValue> func)
        {
            return _inner.GetOrAdd(key, k => new Lazy<TValue>(() => func(k))).Value;
        }

        public TValue AddOrUpdate(TKey key, Func<TKey, TValue> addFunc, Func<TKey, TValue, TValue> updateFunc)
        {
            if (updateFunc != null)
                return _inner.AddOrUpdate(key, k => new Lazy<TValue>(() => addFunc(k)), (k, lv) => new Lazy<TValue>(() => updateFunc(k, lv.Value))).Value;
            else
                return _inner.AddOrUpdate(key, k => new Lazy<TValue>(() => addFunc(k)), (k, lv) => lv).Value;
        }

        public bool TryRemove(TKey key, out Lazy<TValue> lazyValue)
        {
            return _inner.TryRemove(key, out lazyValue);
        }

        public ICollection<TKey> Keys => _inner.Keys;

        public IEnumerable<TValue> Values => _inner.Values.Select(v=>v.Value);
        
        public int Count => _inner.Count;

        public void Clear()
        {
            _inner.Clear();
        }
    }
}
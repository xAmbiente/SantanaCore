using System.Collections;
using System;
using System.Collections.Concurrent;
namespace SantanaLib.Collections.Concurrent
{
    public static class ConcurrentDictionaryExtensions
    {
        public static bool Remove<TKey, TValue>(this ConcurrentDictionary<TKey, TValue> @this, TKey key)
        {
            TValue item;
            return @this.TryRemove(key, out item);
        }

        public static TValue GetValueOrDefault<TKey, TValue>(this ConcurrentDictionary<TKey, TValue> @this, TKey key)
        {
            TValue value;
            return @this.TryGetValue(key, out value) ? value : default(TValue);
        }
    }
}

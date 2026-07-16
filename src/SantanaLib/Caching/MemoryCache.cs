using SantanaLib.Collections.Concurrent;
﻿using System;
using System.Collections.Concurrent;

namespace SantanaLib.Caching
{
    public sealed class MemoryCache : ICache
    {
        private readonly ConcurrentDictionary<string, CacheEntry> _cache = new ConcurrentDictionary<string, CacheEntry>();

        public void Set(string key, object value)
        {
            Set(key, value, TimeSpan.Zero);
        }

        public void Set(string key, object value, TimeSpan ttl)
        {
            var expireTime = ttl != TimeSpan.Zero
                ? DateTimeOffset.UtcNow.ToUnixTimeSeconds() + (long)ttl.TotalSeconds
                : 0;
            var entry = new CacheEntry(expireTime, value);
            _cache.AddOrUpdate(key, entry, (k, o) => entry);
        }

        public object Get(string key)
        {
            CacheEntry entry;
            if (!_cache.TryGetValue(key, out entry))
                return null;

            if (entry.ExpireTime == 0)
                return entry.Item;

            var isExpired = entry.ExpireTime <= DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            if (!isExpired)
                return entry.Item;

            Remove(key);
            return null;
        }

        public T Get<T>(string key)
        {
            var item = Get(key);
            return item == null ? default(T) : DynamicCast<T>.From(item);
        }

        public bool Remove(string key)
        {
            return _cache.Remove(key);
        }

        public void Clear()
        {
            _cache.Clear();
        }

        public void Dispose()
        {
            Clear();
        }

        private struct CacheEntry
        {
            public long ExpireTime { get; }
            public object Item { get; }

            public CacheEntry(long expireTime, object item)
                : this()
            {
                ExpireTime = expireTime;
                Item = item;
            }
        }
    }
}

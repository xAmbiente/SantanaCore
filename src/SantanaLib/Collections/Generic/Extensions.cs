using System.Collections;
using System;
using System.Collections.Generic;
﻿using System.Linq;

namespace SantanaLib.Collections.Generic
{
    public static class EnumerableExtensions
    {
        public static T FirstOfType<T>(this IEnumerable<object> @this)
        {
            return @this.OfType<T>().First();
        }

        public static T FirstOfTypeOrDefault<T>(this IEnumerable<object> @this)
        {
            return @this.OfType<T>().FirstOrDefault();
        }
    }

    public static class DictionaryExtensions
    {
        public static bool TryAdd<TKey, TValue>(this IDictionary<TKey, TValue> @this, TKey key, TValue value)
        {
            if (@this.ContainsKey(key))
                return false;

            @this.Add(key, value);
            return true;
        }

        public static bool TryRemove<TKey, TValue>(this IDictionary<TKey, TValue> @this, TKey key, out TValue value)
        {
            return @this.TryGetValue(key, out value) && @this.Remove(key);
        }

        public static TValue AddOrUpdate<TKey, TValue>(this IDictionary<TKey, TValue> @this, TKey key, Func<TKey, TValue> addValueFactory, Func<TKey, TValue, TValue> updateValueFactory)
        {
            TValue oldValue;
            if (@this.TryGetValue(key, out oldValue))
            {
                var newValue = updateValueFactory(key, oldValue);
                @this[key] = newValue;
                return newValue;
            }

            var value = addValueFactory(key);
            @this.Add(key, value);
            return value;
        }

        public static TValue AddOrUpdate<TKey, TValue>(this IDictionary<TKey, TValue> @this, TKey key, TValue addValue, Func<TKey, TValue, TValue> updateValueFactory)
        {
            TValue oldValue;
            if (@this.TryGetValue(key, out oldValue))
            {
                var newValue = updateValueFactory(key, oldValue);
                @this[key] = newValue;
                return newValue;
            }

            @this.Add(key, addValue);
            return addValue;
        }

        public static TValue GetValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> @this, TKey key)
        {
            TValue value;
            return @this.TryGetValue(key, out value) ? value : default(TValue);
        }

        public static TValue GetValueOrDefault<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> @this, TKey key)
        {
            TValue value;
            return @this.TryGetValue(key, out value) ? value : default(TValue);
        }

        public static TValue GetValueOrDefault<TKey, TValue>(this Dictionary<TKey, TValue> @this, TKey key)
        {
            TValue value;
            return @this.TryGetValue(key, out value) ? value : default(TValue);
        }
    }
}

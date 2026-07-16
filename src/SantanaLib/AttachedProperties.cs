using SantanaLib.Collections.Concurrent;
﻿using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Dynamic;
using System.Runtime.CompilerServices;

namespace SantanaLib
{
    public static class AttachedPropertiesExtensions
    {
        private static readonly ConditionalWeakTable<object, AttachedProperties> s_properties = new ConditionalWeakTable<object, AttachedProperties>();

        public static AttachedProperties GetAttachedProperties(this object obj)
        {
            return s_properties.GetValue(obj, _ => new AttachedProperties());
        }

        public static object GetProperty(this object obj, string key)
        {
            var attachedProperties = GetAttachedProperties(obj);
            return attachedProperties[key];
        }

        public static T GetProperty<T>(this object obj, string key)
        {
            var attachedProperties = GetAttachedProperties(obj);
            return DynamicCast<T>.From(attachedProperties[key]);
        }

        public static void SetProperty(this object obj, string key, object value)
        {
            var attachedProperties = GetAttachedProperties(obj);
            attachedProperties[key] = value;
        }
    }

    public class AttachedProperties : DynamicObject, IDictionary<string, object>
    {
        private readonly ConcurrentDictionary<string, object> _properties = new ConcurrentDictionary<string, object>();

        #region DynamicObject

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            return _properties.TryGetValue(binder.Name, out result);
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            _properties[binder.Name] = value;
            return true;
        }

        #endregion

        #region IDictionary<string, object>

        public int Count => _properties.Count;
        bool ICollection<KeyValuePair<string, object>>.IsReadOnly => false;
        public ICollection<string> Keys => _properties.Keys;
        public ICollection<object> Values => _properties.Values;

        public object this[string key]
        {
            get { return _properties[key]; }
            set { _properties[key] = value; }
        }

        public void Add(KeyValuePair<string, object> item)
        {
            IDictionary<string, object> dict = _properties;
            dict.Add(item);
        }

        public void Clear()
        {
            _properties.Clear();
        }

        public bool Contains(KeyValuePair<string, object> item)
        {
            ICollection<KeyValuePair<string, object>> dict = _properties;
            return dict.Contains(item);
        }

        public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
        {
            ICollection<KeyValuePair<string, object>> dict = _properties;
            dict.CopyTo(array, arrayIndex);
        }

        public bool Remove(KeyValuePair<string, object> item)
        {
            ICollection<KeyValuePair<string, object>> dict = _properties;
            return dict.Remove(item);
        }

        public bool ContainsKey(string key)
        {
            return _properties.ContainsKey(key);
        }

        public void Add(string key, object value)
        {
            IDictionary<string, object> dict = _properties;
            dict.Add(key, value);
        }

        public bool Remove(string key)
        {
            return _properties.Remove(key);
        }

        public bool TryGetValue(string key, out object value)
        {
            return _properties.TryGetValue(key, out value);
        }

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            return _properties.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }
}

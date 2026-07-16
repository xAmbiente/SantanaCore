using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using SantanaLib.IO;
namespace SantanaLib.Configuration
{
    public class IniFile : DynamicObject, IReadOnlyDictionary<string, IniSection>
    {
        private static readonly Regex s_sectionRegex = new Regex(@"^\[([a-zA-Z0-9_-]+)\]");
        private static readonly Regex s_valueRegex = new Regex(@"^([a-zA-Z0-9.,_-]+)=(.*)");
        private readonly ConcurrentDictionary<string, IniSection> _dictionary = new ConcurrentDictionary<string, IniSection>();
        private string _filePath;
        public int Count => _dictionary.Count;
        public IEnumerable<string> Keys => _dictionary.Keys;
        public IEnumerable<IniSection> Values => _dictionary.Values;
        public IniSection this[string key] => GetSection(key);
        #region Load
        public static IniFile Load(string fileName)
        {
            IniFile config;
            if (File.Exists(fileName))
            {
                using (var fs = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
                    config = Load(fs);
            }
            else
            {
                config = new IniFile();
            }
            config._filePath = fileName;
            return config;
        }
        public static IniFile Load(Stream stream)
        {
            return Load(stream, Encoding.Default);
        }
        public static IniFile Load(Stream stream, Encoding encoding)
        {
            var config = new IniFile();
            using (var r = new StreamReader(new NonClosingStream(stream), encoding))
            {
                string line;
                string lastSection = null;
                while ((line = r.ReadLine()) != null)
                {
                    if (string.IsNullOrWhiteSpace(line)) continue;
                    if (s_sectionRegex.IsMatch(line))
                    {
                        var match = s_sectionRegex.Match(line);
                        var name = match.Groups[1].Value;
                        lastSection = name;
                        config.GetSection(name);
                    }
                    else if (s_valueRegex.IsMatch(line) && lastSection != null)
                    {
                        var match = s_valueRegex.Match(line);
                        config[lastSection][match.Groups[1].Value] = match.Groups[2].Value;
                    }
                }
            }
            return config;
        }
        #endregion
        #region Save
        public void Save(Stream stream, Encoding encoding = null, bool sort = false)
        {
            var sb = new StringBuilder();
            if (sort)
            {
                var sections = Keys.ToList();
                sections.Sort();
                foreach (var section in sections)
                {
                    sb.AppendLine("[" + section + "]");
                    var keys = this[section].Keys.ToList();
                    keys.Sort();
                    foreach (var key in keys)
                        sb.AppendLine(key + "=" + this[section][key]);
                    sb.AppendLine();
                }
            }
            else
            {
                foreach (var pair in this)
                {
                    sb.AppendLine("[" + pair.Key + "]");
                    foreach (var value in pair.Value)
                        sb.AppendLine(value.Key + "=" + value.Value);
                    sb.AppendLine("");
                }
            }
            using (var w = new StreamWriter(stream, encoding ?? Encoding.Default, 1024, true))
                w.Write(sb.ToString());
        }
        public void Save(string fileName, Encoding encoding = null, bool sort = false)
        {
            using (var fs = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None))
                Save(fs, encoding, sort);
        }
        public void Save(Encoding encoding = null, bool sort = false)
        {
            if (!string.IsNullOrWhiteSpace(_filePath))
                Save(_filePath, encoding, sort);
        }
        #endregion
        public IniSection GetSection(string key)
        {
            IniSection section;
            if (!_dictionary.TryGetValue(key, out section))
            {
                section = new IniSection(key);
                _dictionary.TryAdd(key, section);
            }
            return section;
        }
        public bool ContainsKey(string key)
        {
            return _dictionary.ContainsKey(key);
        }
        public bool TryGetValue(string key, out IniSection value)
        {
            return _dictionary.TryGetValue(key, out value);
        }
        #region DynamicObject
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            result = GetSection(binder.Name);
            return true;
        }
        #endregion
        #region IEnumerator
        public IEnumerator<KeyValuePair<string, IniSection>> GetEnumerator()
        {
            return _dictionary.GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        #endregion
    }
    public sealed class IniSection : DynamicObject, IReadOnlyDictionary<string, IniValue>
    {
        private readonly ConcurrentDictionary<string, IniValue> _dictionary = new ConcurrentDictionary<string, IniValue>();
        public string Name { get; }
        public int Count => _dictionary.Count;
        public IEnumerable<string> Keys => _dictionary.Keys;
        public IEnumerable<IniValue> Values => _dictionary.Values;
        public IniValue this[string key]
        {
            get { return GetValue(key); }
            set { SetValue(key, value); }
        }
        public IniSection(string name)
        {
            Name = name;
        }
        public IniValue GetValue(string key)
        {
            IniValue configValue;
            if (!_dictionary.TryGetValue(key, out configValue))
            {
                configValue = new IniValue();
                _dictionary.TryAdd(key, configValue);
            }
            return configValue;
        }
        public void SetValue(string key, IniValue value)
        {
            _dictionary.AddOrUpdate(key, value, (k, o) => value);
        }
        public bool ContainsKey(string key)
        {
            return _dictionary.ContainsKey(key);
        }
        public bool TryGetValue(string key, out IniValue value)
        {
            return _dictionary.TryGetValue(key, out value);
        }
        #region DynamicObject
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            result = GetValue(binder.Name);
            return true;
        }
        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            var configValue = value as IniValue;
            SetValue(binder.Name, configValue ?? new IniValue(value.ToString()));
            return true;
        }
        #endregion
        #region IEnumerator
        public IEnumerator<KeyValuePair<string, IniValue>> GetEnumerator()
        {
            return _dictionary.GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        #endregion
        public override string ToString()
        {
            return Name;
        }
    }
    public sealed class IniValue : IComparable, IConvertible, IFormattable
    {
        public string Value { get; set; }
        public CultureInfo CultureInfo { get; set; }
        public IniValue()
            : this("")
        { }
        public IniValue(string value)
        {
            Value = value;
            CultureInfo = CultureInfo.InvariantCulture;
        }
        #region implicit operators ConfigValue -> x
        public static implicit operator string(IniValue value)
        {
            return value.Value;
        }
        public static implicit operator char(IniValue value)
        {
            return value.ToChar(value.CultureInfo);
        }
        public static implicit operator byte(IniValue value)
        {
            return value.ToByte(value.CultureInfo);
        }
        public static implicit operator bool(IniValue value)
        {
            return value.ToBoolean(value.CultureInfo);
        }
        public static implicit operator short(IniValue value)
        {
            return value.ToInt16(value.CultureInfo);
        }
        public static implicit operator int(IniValue value)
        {
            return value.ToInt32(value.CultureInfo);
        }
        public static implicit operator long(IniValue value)
        {
            return value.ToInt64(value.CultureInfo);
        }
        public static implicit operator ushort(IniValue value)
        {
            return value.ToUInt16(value.CultureInfo);
        }
        public static implicit operator uint(IniValue value)
        {
            return value.ToUInt32(value.CultureInfo);
        }
        public static implicit operator ulong(IniValue value)
        {
            return value.ToUInt64(value.CultureInfo);
        }
        public static implicit operator float(IniValue value)
        {
            return value.ToSingle(value.CultureInfo);
        }
        public static implicit operator double(IniValue value)
        {
            return value.ToDouble(value.CultureInfo);
        }
        #endregion
        #region implicit operators x -> ConfigValue
        public static implicit operator IniValue(string value)
        {
            return new IniValue(value);
        }
        public static implicit operator IniValue(char value)
        {
            return new IniValue(value.ToString());
        }
        public static implicit operator IniValue(byte value)
        {
            return new IniValue(value.ToString(CultureInfo.InvariantCulture));
        }
        public static implicit operator IniValue(bool value)
        {
            return new IniValue(value.ToString(CultureInfo.InvariantCulture));
        }
        public static implicit operator IniValue(short value)
        {
            return new IniValue(value.ToString(CultureInfo.InvariantCulture));
        }
        public static implicit operator IniValue(int value)
        {
            return new IniValue(value.ToString(CultureInfo.InvariantCulture));
        }
        public static implicit operator IniValue(long value)
        {
            return new IniValue(value.ToString(CultureInfo.InvariantCulture));
        }
        public static implicit operator IniValue(ushort value)
        {
            return new IniValue(value.ToString(CultureInfo.InvariantCulture));
        }
        public static implicit operator IniValue(uint value)
        {
            return new IniValue(value.ToString(CultureInfo.InvariantCulture));
        }
        public static implicit operator IniValue(ulong value)
        {
            return new IniValue(value.ToString(CultureInfo.InvariantCulture));
        }
        public static implicit operator IniValue(float value)
        {
            return new IniValue(value.ToString(CultureInfo.InvariantCulture));
        }
        public static implicit operator IniValue(double value)
        {
            return new IniValue(value.ToString(CultureInfo.InvariantCulture));
        }
        #endregion
        public override string ToString()
        {
            return Value;
        }
        public string ToString(string format, IFormatProvider formatProvider)
        {
            return Value;
        }
        public int CompareTo(object obj)
        {
            var val = Convert.ChangeType(Value, obj.GetType());
            return Comparer.DefaultInvariant.Compare(val, obj);
        }
        #region IConvertible
        public TypeCode GetTypeCode()
        {
            return TypeCode.Object;
        }
        public bool ToBoolean(IFormatProvider provider)
        {
            bool val;
            if (bool.TryParse(Value, out val))
                return val;
            if (Value.Equals("y", StringComparison.InvariantCultureIgnoreCase) ||
                Value.Equals("yes", StringComparison.InvariantCultureIgnoreCase))
                return true;
            if (Value.Equals("n", StringComparison.InvariantCultureIgnoreCase) ||
                Value.Equals("no", StringComparison.InvariantCultureIgnoreCase))
                return false;
            return ToInt32(provider) > 0;
        }
        public char ToChar(IFormatProvider provider)
        {
            char val;
            return char.TryParse(Value, out val) ? val : ' ';
        }
        public sbyte ToSByte(IFormatProvider provider)
        {
            sbyte val;
            return (sbyte)(sbyte.TryParse(Value, out val) ? val : 0);
        }
        public byte ToByte(IFormatProvider provider)
        {
            byte val;
            return (byte)(byte.TryParse(Value, out val) ? val : 0);
        }
        public short ToInt16(IFormatProvider provider)
        {
            short val;
            return (short)(short.TryParse(Value, out val) ? val : 0);
        }
        public ushort ToUInt16(IFormatProvider provider)
        {
            ushort val;
            return (ushort)(ushort.TryParse(Value, out val) ? val : 0);
        }
        public int ToInt32(IFormatProvider provider)
        {
            int val;
            return int.TryParse(Value, out val) ? val : 0;
        }
        public uint ToUInt32(IFormatProvider provider)
        {
            uint val;
            return uint.TryParse(Value, out val) ? val : 0;
        }
        public long ToInt64(IFormatProvider provider)
        {
            long val;
            return long.TryParse(Value, out val) ? val : 0;
        }
        public ulong ToUInt64(IFormatProvider provider)
        {
            ulong val;
            return ulong.TryParse(Value, out val) ? val : 0;
        }
        public float ToSingle(IFormatProvider provider)
        {
            float val;
            return float.TryParse(Value, NumberStyles.Float, provider, out val) ? val : 0;
        }
        public double ToDouble(IFormatProvider provider)
        {
            double val;
            return double.TryParse(Value, NumberStyles.Float, provider, out val) ? val : 0;
        }
        public decimal ToDecimal(IFormatProvider provider)
        {
            decimal val;
            return decimal.TryParse(Value, NumberStyles.Float, provider, out val) ? val : 0;
        }
        public DateTime ToDateTime(IFormatProvider provider)
        {
            throw new NotSupportedException();
        }
        public string ToString(IFormatProvider provider)
        {
            return Value;
        }
        public object ToType(Type conversionType, IFormatProvider provider)
        {
            return Convert.ChangeType(Value, conversionType, provider);
        }
        #endregion
    }
}

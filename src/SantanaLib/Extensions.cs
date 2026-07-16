using SantanaLib.IO;
﻿using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using SantanaLib;
using System.Reflection;
using System.Runtime.ExceptionServices;

namespace System
{
    public static class IntExtensions
    {
        public static string ToFormattedSize(this int @this)
        {
            return Utilities.ToFormattedSize(@this);
        }

        public static string ToFormattedSize(this uint @this)
        {
            return Utilities.ToFormattedSize(@this);
        }
    }

    public static class LongExtensions
    {
        public static string ToFormattedSize(this long @this)
        {
            return Utilities.ToFormattedSize(@this);
        }

        public static string ToFormattedSize(this ulong @this)
        {
            return Utilities.ToFormattedSize(@this);
        }
    }

    public static class ByteArrayExtensions
    {
        public static BinaryReader ToBinaryReader(this byte[] @this)
        {
            return new BinaryReader(new MemoryStream(@this));
        }

        public static BinaryWriter ToBinaryWriter(this byte[] @this)
        {
            return new BinaryWriter(new MemoryStream(@this));
        }

        public static string ToHexString(this IEnumerable<byte> @this)
        {
            return ToHexString(@this, " ");
        }

        public static string ToHexString(this IEnumerable<byte> @this, string separator)
        {
            var sb = new StringBuilder();
            foreach (var @byte in @this)
            {
                sb.Append(@byte.ToString("X2"));
                sb.Append(separator);
            }
            return sb.ToString();
        }

        public static string ToFormattedSize(this byte[] @this)
        {
            return Utilities.ToFormattedSize(@this.Length);
        }

        public static byte[] CompressGZip(this byte[] @this)
        {
            using (var ms = new MemoryStream())
            using (var stream = new GZipStream(ms, CompressionMode.Compress))
            {
                stream.Write(@this, 0, @this.Length);
                stream.Flush();
                return ms.ToArray();
            }
        }

        public static void CompressGZip(this byte[] @this, Stream output)
        {
            using (var stream = new GZipStream(output, CompressionMode.Compress, true))
            {
                stream.Write(@this, 0, @this.Length);
                stream.Flush();
            }
        }

        public static byte[] CompressDeflate(this byte[] @this)
        {
            using (var ms = new MemoryStream())
            using (var stream = new DeflateStream(ms, CompressionMode.Compress))
            {
                stream.Write(@this, 0, @this.Length);
                stream.Flush();
                return ms.ToArray();
            }
        }

        public static void CompressDeflate(this byte[] @this, Stream output)
        {
            using (var stream = new DeflateStream(output, CompressionMode.Compress, true))
            {
                stream.Write(@this, 0, @this.Length);
                stream.Flush();
            }
        }

        public static byte[] DecompressGZip(this byte[] @this)
        {
            using (var ms = new MemoryStream(@this))
            using (var stream = new GZipStream(ms, CompressionMode.Decompress))
                return stream.ReadToEnd();
        }

        public static void DecompressGZip(this byte[] @this, Stream output)
        {
            using (var ms = new MemoryStream(@this))
            using (var stream = new GZipStream(ms, CompressionMode.Decompress))
                stream.CopyTo(output);
        }

        public static byte[] DecompressDeflate(this byte[] @this)
        {
            using (var ms = new MemoryStream(@this))
            using (var stream = new DeflateStream(ms, CompressionMode.Decompress))
                return stream.ReadToEnd();
        }

        public static void DecompressDeflate(this byte[] @this, Stream output)
        {
            using (var ms = new MemoryStream(@this))
            using (var stream = new DeflateStream(ms, CompressionMode.Decompress))
                stream.CopyTo(output);
        }

        public static byte[] FastClone(this byte[] @this)
        {
            var outBuffer = new byte[@this.Length];
            Array.Copy(@this, outBuffer, @this.Length);
            return outBuffer;
        }
    }

    public static class StringExtensions
    {
        public static string[] GetArgs(this string @this)
        {
            var args = new List<string>();
            using (var r = new StringReader(@this))
            {
                while (r.Peek() != -1)
                {
                    if (r.Peek() == ' ')
                        r.Read();

                    var tmp = new StringBuilder();
                    if (r.Peek() == '\"')
                    {
                        r.Read();
                        while (r.Peek() != '\"' && r.Peek() != -1)
                            tmp.Append((char)r.Read());

                        r.Read();
                    }
                    else
                    {
                        while (r.Peek() != -1 && r.Peek() != ' ')
                            tmp.Append((char)r.Read());
                        r.Read();
                    }
                    args.Add(tmp.ToString());
                }
            }

            return args.ToArray();
        }

        public static bool Contains(this string @this, string value, StringComparison comparisonType)
        {
            return @this.IndexOf(value, comparisonType) != -1;
        }

        public static byte[] HexToArray(this string @this)
        {
            return Enumerable.Range(0, @this.Length / 2)
                .Select(x => Convert.ToByte(@this.Substring(x * 2, 2), 16))
                .ToArray();
        }
    }

    public static class ServiceProviderExtensions
    {
        public static T GetService<T>(this IServiceProvider @this)
        {
            return (T)@this.GetService(typeof(T));
        }
    }

    public static class TypeExtensions
    {
        public static TypeCode GetTypeCode(this Type @this)
        {
            if (@this == null)
                return TypeCode.Empty;
            if (@this == typeof(bool))
                return TypeCode.Boolean;
            if (@this == typeof(char))
                return TypeCode.Char;
            if (@this == typeof(sbyte))
                return TypeCode.SByte;
            if (@this == typeof(byte))
                return TypeCode.Byte;
            if (@this == typeof(short))
                return TypeCode.Int16;
            if (@this == typeof(ushort))
                return TypeCode.UInt16;
            if (@this == typeof(int))
                return TypeCode.Int32;
            if (@this == typeof(uint))
                return TypeCode.UInt32;
            if (@this == typeof(long))
                return TypeCode.Int64;
            if (@this == typeof(ulong))
                return TypeCode.UInt64;
            if (@this == typeof(float))
                return TypeCode.Single;
            if (@this == typeof(double))
                return TypeCode.Double;
            if (@this == typeof(decimal))
                return TypeCode.Decimal;
            if (@this == typeof(DateTime))
                return TypeCode.DateTime;
            if (@this == typeof(string))
                return TypeCode.String;
            if (@this.GetTypeInfo().IsEnum)
                return GetTypeCode(Enum.GetUnderlyingType(@this));

            return TypeCode.Object;
        }
    }

    public static class ExceptionExtensions
    {
        public static Exception Rethrow(this Exception @this)
        {
            ExceptionDispatchInfo.Capture(@this).Throw();
            return null;
        }
    }
}

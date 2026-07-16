using System;
using System.IO;
﻿using System.Collections.Generic;
using System.IO.Compression;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using SantanaLib;
using SantanaLib.Buffers;
using SantanaLib.IO;
namespace SantanaLib.IO
{
    public static class BinaryReaderExtensions
    {
        private const string BufferKey = "BinaryReaderExtensions_Buffer";
        private const string CharBufferKey = "BinaryReaderExtensions_CharBuffer";
        private const string StringBuilderKey = "BinaryReaderExtensions_StringBuilder";
        private const string DecoderKey = "BinaryReaderExtensions_Decoder";
        private const int BufferLength = 128;
        public static byte[] ReadToEnd(this BinaryReader @this)
        {
            return @this.BaseStream.ReadToEnd();
        }
        public static Task<byte[]> ReadToEndAsync(this BinaryReader @this)
        {
            return @this.BaseStream.ReadToEndAsync();
        }
        public static string[] ReadStrings(this BinaryReader @this, int count)
        {
            var array = new string[count];
            for (var i = 0; i < count; i++)
                array[i] = @this.ReadString();
            return array;
        }
        public static T[] ReadArray<T>(this BinaryReader @this, int count)
            where T : struct, IComparable, IConvertible
        {
            var type = typeof(T);
            var array = FastActivator<T>.CreateArray(count);
            byte[] data;
            switch (type.GetTypeCode())
            {
                case TypeCode.Boolean:
                    data = @this.ReadBytes(sizeof(bool) * count);
                    break;
                case TypeCode.Char:
                    data = @this.ReadBytes(sizeof(char) * count);
                    break;
                case TypeCode.Byte:
                    data = @this.ReadBytes(sizeof(byte) * count);
                    break;
                case TypeCode.SByte:
                    data = @this.ReadBytes(sizeof(sbyte) * count);
                    break;
                case TypeCode.Int16:
                    data = @this.ReadBytes(sizeof(short) * count);
                    break;
                case TypeCode.Int32:
                    data = @this.ReadBytes(sizeof(int) * count);
                    break;
                case TypeCode.Int64:
                    data = @this.ReadBytes(sizeof(long) * count);
                    break;
                case TypeCode.UInt16:
                    data = @this.ReadBytes(sizeof(ushort) * count);
                    break;
                case TypeCode.UInt32:
                    data = @this.ReadBytes(sizeof(uint) * count);
                    break;
                case TypeCode.UInt64:
                    data = @this.ReadBytes(sizeof(ulong) * count);
                    break;
                case TypeCode.Single:
                    data = @this.ReadBytes(sizeof(float) * count);
                    break;
                case TypeCode.Double:
                    data = @this.ReadBytes(sizeof(double) * count);
                    break;
                case TypeCode.Decimal:
                    data = @this.ReadBytes(sizeof(decimal) * count);
                    break;
                default:
                    throw new NotSupportedException("Type is not supported");
            }
            System.Buffer.BlockCopy(data, 0, array, 0, data.Length);
            return array;
        }
        public static T ReadEnum<T>(this BinaryReader @this)
            where T : struct, IComparable, IConvertible
        {
            var type = typeof(T);
            if (!type.GetTypeInfo().IsEnum)
                throw new ArgumentException("T is not an enum");
            var derivedType = Enum.GetUnderlyingType(type);
            switch (derivedType.GetTypeCode())
            {
                case TypeCode.Byte:
                    return DynamicCast<T>.From(@this.ReadByte());
                case TypeCode.SByte:
                    return DynamicCast<T>.From(@this.ReadSByte());
                case TypeCode.Int16:
                    return DynamicCast<T>.From(@this.ReadInt16());
                case TypeCode.Int32:
                    return DynamicCast<T>.From(@this.ReadInt32());
                case TypeCode.Int64:
                    return DynamicCast<T>.From(@this.ReadInt64());
                case TypeCode.UInt16:
                    return DynamicCast<T>.From(@this.ReadUInt16());
                case TypeCode.UInt32:
                    return DynamicCast<T>.From(@this.ReadUInt32());
                case TypeCode.UInt64:
                    return DynamicCast<T>.From(@this.ReadUInt64());
                default:
                    throw new NotSupportedException("Type is not supported");
            }
        }
        public static T[] ReadEnums<T>(this BinaryReader @this, int count)
            where T : struct, IComparable, IConvertible
        {
            var type = typeof(T);
            if (!type.GetTypeInfo().IsEnum)
                throw new ArgumentException("T is not an enum");
            var array = FastActivator<T>.CreateArray(count);
            for (var i = 0; i < count; i++)
                array[i] = @this.ReadEnum<T>();
            return array;
        }
        public static T Deserialize<T>(this BinaryReader @this) where T : IManualSerializer, new()
        {
            var instance = FastActivator<T>.Create();
            instance.Deserialize(@this.BaseStream);
            return instance;
        }
        public static T[] DeserializeArray<T>(this BinaryReader @this, int count)
            where T : IManualSerializer, new()
        {
            var array = FastActivator<T>.CreateArray(count);
            for (var i = 0; i < count; i++)
                array[i] = @this.Deserialize<T>();
            return array;
        }
        public static IPEndPoint ReadIPEndPoint(this BinaryReader @this)
        {
            var ip = new IPAddress(@this.ReadBytes(4));
            return new IPEndPoint(ip, @this.ReadUInt16());
        }
        public static string ReadCString(this BinaryReader @this)
        {
            var attachedProperties = @this.GetAttachedProperties();
            var buffer = GetBuffer(attachedProperties);
            var charBuffer = GetCharBuffer(attachedProperties, @this);
            var sb = GetStringBuilder(attachedProperties);
            var decoder = GetDecoder(attachedProperties, @this);
            int charsRead;
            var i = 0;
            byte b;
            while ((b = @this.ReadByte()) != 0)
            {
                buffer[i++] = b;
                if (i == buffer.Length)
                {
                    charsRead = decoder.GetChars(buffer, 0, i, charBuffer, 0);
                    sb.Append(charBuffer, 0, charsRead);
                    i = 0;
                }
            }
            if (i == 0)
                return sb.ToString();
            charsRead = decoder.GetChars(buffer, 0, i, charBuffer, 0);
            sb.Append(charBuffer, 0, charsRead);
            return sb.ToString();
        }
        public static string ReadCString(this BinaryReader @this, int length)
        {
            if (length < 1)
                throw new ArgumentOutOfRangeException(nameof(length));
            var attachedProperties = @this.GetAttachedProperties();
            var buffer = GetBuffer(attachedProperties);
            var charBuffer = GetCharBuffer(attachedProperties, @this);
            var sb = GetStringBuilder(attachedProperties);
            var decoder = GetDecoder(attachedProperties, @this);
            int charsRead;
            var totalRead = 0;
            var i = 0;
            byte b;
            while ((b = @this.ReadByte()) != 0)
            {
                ++totalRead;
                buffer[i++] = b;
                if (totalRead >= length)
                    break;
                if (i == buffer.Length)
                {
                    charsRead = decoder.GetChars(buffer, 0, i, charBuffer, 0);
                    sb.Append(charBuffer, 0, charsRead);
                    i = 0;
                }
            }
            if (totalRead < length)
                @this.BaseStream.Seek(length - totalRead - 1, SeekOrigin.Current);
            if (i == 0)
                return sb.ToString();
            charsRead = decoder.GetChars(buffer, 0, i, charBuffer, 0);
            sb.Append(charBuffer, 0, charsRead);
            return sb.ToString();
        }
        public static bool IsEOF(this BinaryReader @this)
        {
            return @this.BaseStream.IsEOF();
        }
        private static byte[] GetBuffer(AttachedProperties properties)
        {
            object obj;
            byte[] buffer;
            if (properties.TryGetValue(BufferKey, out obj))
            {
                buffer = (byte[])obj;
            }
            else
            {
                buffer = new byte[BufferLength];
                properties.SetProperty(BufferKey, buffer);
            }
            return buffer;
        }
        private static char[] GetCharBuffer(AttachedProperties properties, BinaryReader r)
        {
            const string fieldName = "m_maxCharsSize";
            object obj;
            char[] buffer;
            if (properties.TryGetValue(CharBufferKey, out obj))
            {
                buffer = (char[])obj;
            }
            else
            {
                var typeinfo = typeof(BinaryReader).GetTypeInfo();
                var fieldInfo = typeinfo.GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
                if (fieldInfo == null)
                    throw new Exception($"Type {typeinfo.FullName} has no field {fieldName}");
                var maxCharsSize = (int)fieldInfo.GetValue(r);
                buffer = new char[maxCharsSize];
                properties.SetProperty(CharBufferKey, buffer);
            }
            return buffer;
        }
        private static StringBuilder GetStringBuilder(AttachedProperties properties)
        {
            object obj;
            StringBuilder sb;
            if (properties.TryGetValue(StringBuilderKey, out obj))
            {
                sb = (StringBuilder)obj;
                sb.Clear();
            }
            else
            {
                sb = new StringBuilder();
                properties.SetProperty(StringBuilderKey, sb);
            }
            return sb;
        }
        private static Decoder GetDecoder(AttachedProperties properties, BinaryReader r)
        {
            const string fieldName = "m_decoder";
            object obj;
            Decoder decoder;
            if (properties.TryGetValue(DecoderKey, out obj))
            {
                decoder = (Decoder)obj;
            }
            else
            {
                var typeinfo = typeof(BinaryReader).GetTypeInfo();
                var fieldInfo = typeinfo.GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
                if (fieldInfo == null)
                    throw new Exception($"Type {typeinfo.FullName} has no field {fieldName}");
                decoder = (Decoder)fieldInfo.GetValue(r);
                properties.SetProperty(DecoderKey, decoder);
            }
            return decoder;
        }
    }
    public static class BinaryWriterExtensions
    {
        private const string BufferKey = "BinaryWriterExtensions_Buffer";
        private const string StringBuilderKey = "BinaryWriterExtensions_StringBuilder";
        private const string EncodingKey = "BinaryWriterExtensions_Encoding";
        private const string EncoderKey = "BinaryWriterExtensions_Encoder";
        private const int BufferLength = 256;
        #region Write Arrays
        public static void Write(this BinaryWriter @this, IEnumerable<byte> values)
        {
            foreach (var value in values)
                @this.Write(value);
        }
        public static void Write(this BinaryWriter @this, IEnumerable<bool> values)
        {
            foreach (var value in values)
                @this.Write(value);
        }
        public static void Write(this BinaryWriter @this, IEnumerable<char> values)
        {
            foreach (var value in values)
                @this.Write(value);
        }
        public static void Write(this BinaryWriter @this, IEnumerable<sbyte> values)
        {
            foreach (var value in values)
                @this.Write(value);
        }
        public static void Write(this BinaryWriter @this, IEnumerable<short> values)
        {
            foreach (var value in values)
                @this.Write(value);
        }
        public static void Write(this BinaryWriter @this, IEnumerable<ushort> values)
        {
            foreach (var value in values)
                @this.Write(value);
        }
        public static void Write(this BinaryWriter @this, IEnumerable<int> values)
        {
            foreach (var value in values)
                @this.Write(value);
        }
        public static void Write(this BinaryWriter @this, IEnumerable<uint> values)
        {
            foreach (var value in values)
                @this.Write(value);
        }
        public static void Write(this BinaryWriter @this, IEnumerable<long> values)
        {
            foreach (var value in values)
                @this.Write(value);
        }
        public static void Write(this BinaryWriter @this, IEnumerable<ulong> values)
        {
            foreach (var value in values)
                @this.Write(value);
        }
        public static void Write(this BinaryWriter @this, IEnumerable<float> values)
        {
            foreach (var value in values)
                @this.Write(value);
        }
        public static void Write(this BinaryWriter @this, IEnumerable<double> values)
        {
            foreach (var value in values)
                @this.Write(value);
        }
        public static void Write(this BinaryWriter @this, IEnumerable<decimal> values)
        {
            foreach (var value in values)
                @this.Write(value);
        }
        public static void Write(this BinaryWriter @this, IEnumerable<string> values)
        {
            foreach (var value in values)
                @this.Write(value);
        }
        #endregion
        public static void WriteEnum<T>(this BinaryWriter @this, T value)
            where T : struct, IComparable, IConvertible
        {
            var type = value.GetType();
            if (!type.GetTypeInfo().IsEnum)
                throw new ArgumentException("T is not an enum");
            var derivedType = Enum.GetUnderlyingType(type);
            switch (derivedType.GetTypeCode())
            {
                case TypeCode.Byte:
                    @this.Write(DynamicCast<byte>.From(value));
                    break;
                case TypeCode.SByte:
                    @this.Write(DynamicCast<sbyte>.From(value));
                    break;
                case TypeCode.Int16:
                    @this.Write(DynamicCast<short>.From(value));
                    break;
                case TypeCode.UInt16:
                    @this.Write(DynamicCast<ushort>.From(value));
                    break;
                case TypeCode.Int32:
                    @this.Write(DynamicCast<int>.From(value));
                    break;
                case TypeCode.UInt32:
                    @this.Write(DynamicCast<uint>.From(value));
                    break;
                case TypeCode.Int64:
                    @this.Write(DynamicCast<long>.From(value));
                    break;
                case TypeCode.UInt64:
                    @this.Write(DynamicCast<ulong>.From(value));
                    break;
                case TypeCode.Single:
                    @this.Write(DynamicCast<float>.From(value));
                    break;
                case TypeCode.Double:
                    @this.Write(DynamicCast<double>.From(value));
                    break;
                case TypeCode.Decimal:
                    @this.Write(DynamicCast<decimal>.From(value));
                    break;
                default:
                    throw new NotSupportedException("Type is not supported");
            }
        }
        public static void Serialize(this BinaryWriter @this, IManualSerializer value)
        {
            value.Serialize(@this.BaseStream);
        }
        public static void Serialize<T>(this BinaryWriter @this, IEnumerable<T> values)
            where T : IManualSerializer
        {
            foreach (var value in values)
                @this.Serialize(value);
        }
        public static void Write(this BinaryWriter @this, IPEndPoint value)
        {
            @this.Write(value.Address.GetAddressBytes());
            @this.Write((ushort)value.Port);
        }
        public static void Write(this BinaryWriter @this, ArraySegment<byte> segment)
        {
            @this.Write(segment.Array, segment.Offset, segment.Count);
        }
        public static void WriteCString(this BinaryWriter @this, string value)
        {
            value = value ?? "";
            var attachedProperties = @this.GetAttachedProperties();
            var buffer = GetBuffer(attachedProperties);
            var sb = GetStringBuilder(attachedProperties);
            var encoding = GetEncoding(attachedProperties, @this);
            var encoder = GetEncoder(attachedProperties, @this);
            sb.Append(value);
            sb.Append("\0");
            value = sb.ToString();
            sb.Clear();
            var byteCount = encoding.GetByteCount(value);
            if (byteCount <= buffer.Length)
            {
                encoding.GetBytes(value, 0, value.Length, buffer, 0);
                @this.Write(buffer, 0, byteCount);
            }
            else
            {
                var charStart = 0;
                var numLeft = value.Length;
                var maxChars = buffer.Length / encoding.GetMaxByteCount(1);
                while (numLeft > 0)
                {
                    var charCount = numLeft > maxChars ? maxChars : numLeft;
                    int byteLen;
                    unsafe
                    {
                        fixed (char* pChars = value)
                        fixed (byte* pBytes = buffer)
                            byteLen = encoder.GetBytes(pChars + charStart, charCount, pBytes, buffer.Length, charCount == numLeft);
                    }
                    @this.Write(buffer, 0, byteLen);
                    charStart += charCount;
                    numLeft -= charCount;
                }
            }
        }
        public static void WriteCString(this BinaryWriter @this, string value, int maxLength)
        {
            value = value ?? "";
            var attachedProperties = @this.GetAttachedProperties();
            var buffer = GetBuffer(attachedProperties);
            var sb = GetStringBuilder(attachedProperties);
            var encoding = GetEncoding(attachedProperties, @this);
            var encoder = GetEncoder(attachedProperties, @this);
            sb.Append(value);
            sb.Append("\0");
            value = sb.ToString();
            sb.Clear();
            var byteCount = encoding.GetByteCount(value);
            if (byteCount > maxLength)
                throw new ArgumentOutOfRangeException($"{nameof(value)} is longer than {nameof(maxLength)}", nameof(value));
            if (byteCount <= buffer.Length)
            {
                encoding.GetBytes(value, 0, value.Length, buffer, 0);
                @this.Write(buffer, 0, byteCount);
            }
            else
            {
                var charStart = 0;
                var numLeft = value.Length;
                var maxChars = buffer.Length / encoding.GetMaxByteCount(1);
                while (numLeft > 0)
                {
                    var charCount = numLeft > maxChars ? maxChars : numLeft;
                    int byteLen;
                    unsafe
                    {
                        fixed (char* pChars = value)
                        fixed (byte* pBytes = buffer)
                            byteLen = encoder.GetBytes(pChars + charStart, charCount, pBytes, buffer.Length, charCount == numLeft);
                    }
                    @this.Write(buffer, 0, byteLen);
                    charStart += charCount;
                    numLeft -= charCount;
                }
            }
            @this.Fill(maxLength - byteCount);
        }
        public static void Fill(this BinaryWriter @this, int count)
        {
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count));
            if (count == 0)
                return;
            byte[] buffer;
            var attachedProperties = @this.GetAttachedProperties();
            if (!attachedProperties.ContainsKey(BufferKey))
            {
                buffer = new byte[BufferLength];
                @this.SetProperty(BufferKey, buffer);
            }
            else
            {
                buffer = @this.GetProperty<byte[]>(BufferKey);
            }
            var bytesLeft = count;
            while (bytesLeft > 0)
            {
                var bytesToWrite = bytesLeft > buffer.Length ? buffer.Length : bytesLeft;
                @this.Write(buffer, 0, bytesToWrite);
                bytesLeft -= bytesToWrite;
            }
        }
        public static bool IsEOF(this BinaryWriter @this)
        {
            return @this.BaseStream.IsEOF();
        }
        public static byte[] ToArray(this BinaryWriter @this)
        {
            var memoryStream = @this.BaseStream as MemoryStream;
            if (memoryStream == null)
            {
                var bufferStream = @this.BaseStream as BufferStream;
                if (bufferStream == null)
                    throw new InvalidOperationException("BaseStream must be a MemoryStream or BufferStream");
                return bufferStream.ToArray();
            }
            return memoryStream.ToArray();
        }
        private static byte[] GetBuffer(AttachedProperties properties)
        {
            object obj;
            byte[] buffer;
            if (properties.TryGetValue(BufferKey, out obj))
            {
                buffer = (byte[])obj;
            }
            else
            {
                buffer = new byte[BufferLength];
                properties.SetProperty(BufferKey, buffer);
            }
            return buffer;
        }
        private static StringBuilder GetStringBuilder(AttachedProperties properties)
        {
            object obj;
            StringBuilder sb;
            if (properties.TryGetValue(StringBuilderKey, out obj))
            {
                sb = (StringBuilder)obj;
                sb.Clear();
            }
            else
            {
                sb = new StringBuilder();
                properties.SetProperty(StringBuilderKey, sb);
            }
            return sb;
        }
        private static Encoding GetEncoding(AttachedProperties properties, BinaryWriter w)
        {
            const string fieldName = "_encoding";
            object obj;
            Encoding encoding;
            if (properties.TryGetValue(EncodingKey, out obj))
            {
                encoding = (Encoding)obj;
            }
            else
            {
                var typeinfo = w.GetType().GetTypeInfo();
                var fieldInfo = typeinfo.GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
                if (fieldInfo == null)
                    throw new Exception($"Type {typeinfo.FullName} has no field {fieldName}");
                encoding = (Encoding)fieldInfo.GetValue(w);
                properties.SetProperty(EncodingKey, encoding);
            }
            return encoding;
        }
        private static Encoder GetEncoder(AttachedProperties properties, BinaryWriter w)
        {
            const string fieldName = "_encoder";
            object obj;
            Encoder encoder;
            if (properties.TryGetValue(EncoderKey, out obj))
            {
                encoder = (Encoder)obj;
            }
            else
            {
                var typeinfo = w.GetType().GetTypeInfo();
                var fieldInfo = typeinfo.GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
                if (fieldInfo == null)
                    throw new Exception($"Type {typeinfo.FullName} has no field {fieldName}");
                encoder = (Encoder)fieldInfo.GetValue(w);
                properties.SetProperty(EncoderKey, encoder);
            }
            return encoder;
        }
    }
    public static class StreamExtensions
    {
        public static BinaryReader ToBinaryReader(this Stream @this, Encoding encoding, bool leaveOpen)
        {
            return new BinaryReader(@this, encoding, leaveOpen);
        }
        public static BinaryReader ToBinaryReader(this Stream @this, bool leaveOpen)
        {
            return new BinaryReader(@this, Encoding.UTF8, leaveOpen);
        }
        public static BinaryWriter ToBinaryWriter(this Stream @this, Encoding encoding, bool leaveOpen)
        {
            return new BinaryWriter(@this, encoding, leaveOpen);
        }
        public static BinaryWriter ToBinaryWriter(this Stream @this, bool leaveOpen)
        {
            return new BinaryWriter(@this, Encoding.UTF8, leaveOpen);
        }
        public static byte[] ReadToEnd(this Stream @this)
        {
            using (var ms = new MemoryStream())
            {
                @this.CopyTo(ms);
                return ms.ToArray();
            }
        }
        public static async Task<byte[]> ReadToEndAsync(this Stream @this)
        {
            using (var ms = new MemoryStream())
            {
                await @this.CopyToAsync(ms).ConfigureAwait(false);
                return ms.ToArray();
            }
        }
        public static void Serialize(this Stream @this, IManualSerializer value)
        {
            value.Serialize(@this);
        }
        public static void Serialize<T>(this Stream @this, IEnumerable<T> values)
            where T : IManualSerializer
        {
            foreach (var value in values)
                @this.Serialize(value);
        }
        public static T Deserialize<T>(this Stream @this)
            where T : IManualSerializer, new()
        {
            var instance = new T();
            instance.Deserialize(@this);
            return instance;
        }
        public static T[] DeserializeArray<T>(this Stream @this, int count)
            where T : IManualSerializer, new()
        {
            return @this.DeserializeArray<T>((uint)count);
        }
        public static T[] DeserializeArray<T>(this Stream @this, uint count)
            where T : IManualSerializer, new()
        {
            var buffer = new T[count];
            for (var i = 0; i < count; i++)
                buffer[i] = @this.Deserialize<T>();
            return buffer;
        }
        public static bool IsEOF(this Stream @this)
        {
            return @this.Position == @this.Length;
        }
        public static byte[] CompressGZip(this Stream @this)
        {
            using(var ms = new MemoryStream())
            using (var stream = new GZipStream(@this, CompressionMode.Compress, true))
            {
                stream.CopyTo(ms);
                stream.Flush();
                return ms.ToArray();
            }
        }
        public static void CompressGZip(this Stream @this, Stream output)
        {
            using (var stream = new GZipStream(@this, CompressionMode.Compress, true))
            {
                stream.CopyTo(output);
                stream.Flush();
            }
        }
        public static byte[] CompressDeflate(this Stream @this)
        {
            using (var ms = new MemoryStream())
            using (var stream = new DeflateStream(@this, CompressionMode.Compress, true))
            {
                stream.CopyTo(ms);
                stream.Flush();
                return ms.ToArray();
            }
        }
        public static void CompressDeflate(this Stream @this, Stream output)
        {
            using (var stream = new DeflateStream(@this, CompressionMode.Compress, true))
            {
                stream.CopyTo(output);
                stream.Flush();
            }
        }
        public static byte[] DecompressGZip(this Stream @this)
        {
            using (var stream = new GZipStream(@this, CompressionMode.Decompress, true))
                return stream.ReadToEnd();
        }
        public static void DecompressGZip(this Stream @this, Stream output)
        {
            using (var stream = new GZipStream(@this, CompressionMode.Decompress, true))
                stream.CopyTo(output);
        }
        public static byte[] DecompressDeflate(this Stream @this)
        {
            using (var stream = new DeflateStream(@this, CompressionMode.Decompress, true))
                return stream.ReadToEnd();
        }
        public static void DecompressDeflate(this Stream @this, Stream output)
        {
            using (var stream = new DeflateStream(@this, CompressionMode.Decompress, true))
                stream.CopyTo(output);
        }
    }
}

using System;
using System.IO;
using System.Text;
using System.Security.Cryptography;

namespace SantanaLib.Security.Cryptography
{
    public static class Hash
    {
        public static byte[] GetBytes<T>(Stream inputStream) where T : HashAlgorithm, new()
        {
            using (var hashAlgorithm = FastActivator<T>.Create())
            {
                hashAlgorithm.Initialize();
                return hashAlgorithm.ComputeHash(inputStream);
            }
        }

        public static byte[] GetBytes<T>(byte[] data, int offset, int count) where T : HashAlgorithm, new()
        {
            using (var hashAlgorithm = FastActivator<T>.Create())
            {
                hashAlgorithm.Initialize();
                return hashAlgorithm.ComputeHash(data, offset, count);
            }
        }

        public static byte[] GetBytes<T>(byte[] data) where T : HashAlgorithm, new()
        {
            return GetBytes<T>(data, 0, data.Length);
        }

        public static byte[] GetBytes<T>(string data, Encoding encoding = null) where T : HashAlgorithm, new()
        {
            encoding = encoding ?? Encoding.Default;
            return GetBytes<T>(encoding.GetBytes(data));
        }

        public static string GetString<T>(Stream inputStream) where T : HashAlgorithm, new()
        {
            using (var hashAlgorithm = FastActivator<T>.Create())
            {
                hashAlgorithm.Initialize();
                var tmp = hashAlgorithm.ComputeHash(inputStream);
                return BitConverter.ToString(tmp).Replace("-", "").ToLowerInvariant();
            }
        }

        public static string GetString<T>(byte[] data, int offset, int count) where T : HashAlgorithm, new()
        {
            using (var hashAlgorithm = FastActivator<T>.Create())
            {
                hashAlgorithm.Initialize();
                var tmp = hashAlgorithm.ComputeHash(data, offset, count);
                return BitConverter.ToString(tmp).Replace("-", "").ToLowerInvariant();
            }
        }

        public static string GetString<T>(byte[] data) where T : HashAlgorithm, new()
        {
            return GetString<T>(data, 0, data.Length);
        }

        public static string GetString<T>(string data, Encoding encoding = null) where T : HashAlgorithm, new()
        {
            encoding = encoding ?? Encoding.Default;
            return GetString<T>(encoding.GetBytes(data));
        }

        public static ushort GetUInt16<T>(Stream inputStream) where T : HashAlgorithm, new()
        {
            using (var hashAlgorithm = FastActivator<T>.Create())
            {
                hashAlgorithm.Initialize();
                var tmp = hashAlgorithm.ComputeHash(inputStream);
                return BitConverter.ToUInt16(tmp, 0);
            }
        }

        public static ushort GetUInt16<T>(byte[] data, int offset, int count) where T : HashAlgorithm, new()
        {
            using (var hashAlgorithm = FastActivator<T>.Create())
            {
                hashAlgorithm.Initialize();
                var tmp = hashAlgorithm.ComputeHash(data, offset, count);
                return BitConverter.ToUInt16(tmp, 0);
            }
        }

        public static ushort GetUInt16<T>(byte[] data) where T : HashAlgorithm, new()
        {
            return GetUInt16<T>(data, 0, data.Length);
        }

        public static ushort GetUInt16<T>(string data, Encoding encoding = null) where T : HashAlgorithm, new()
        {
            encoding = encoding ?? Encoding.Default;
            return GetUInt16<T>(encoding.GetBytes(data));
        }

        public static uint GetUInt32<T>(Stream inputStream) where T : HashAlgorithm, new()
        {
            using (var hashAlgorithm = FastActivator<T>.Create())
            {
                hashAlgorithm.Initialize();
                var tmp = hashAlgorithm.ComputeHash(inputStream);
                return BitConverter.ToUInt32(tmp, 0);
            }
        }

        public static uint GetUInt32<T>(byte[] data, int offset, int count) where T : HashAlgorithm, new()
        {
            using (var hashAlgorithm = FastActivator<T>.Create())
            {
                hashAlgorithm.Initialize();
                var tmp = hashAlgorithm.ComputeHash(data, offset, count);
                return BitConverter.ToUInt32(tmp, 0);
            }
        }

        public static uint GetUInt32<T>(byte[] data) where T : HashAlgorithm, new()
        {
            return GetUInt32<T>(data, 0, data.Length);
        }

        public static uint GetUInt32<T>(string data, Encoding encoding = null) where T : HashAlgorithm, new()
        {
            encoding = encoding ?? Encoding.Default;
            return GetUInt32<T>(encoding.GetBytes(data));
        }

        public static ulong GetUInt64<T>(Stream inputStream) where T : HashAlgorithm, new()
        {
            using (var hashAlgorithm = FastActivator<T>.Create())
            {
                hashAlgorithm.Initialize();
                var tmp = hashAlgorithm.ComputeHash(inputStream);
                return BitConverter.ToUInt64(tmp, 0);
            }
        }

        public static ulong GetUInt64<T>(byte[] data, int offset, int count) where T : HashAlgorithm, new()
        {
            using (var hashAlgorithm = FastActivator<T>.Create())
            {
                hashAlgorithm.Initialize();
                var tmp = hashAlgorithm.ComputeHash(data, offset, count);
                return BitConverter.ToUInt64(tmp, 0);
            }
        }

        public static ulong GetUInt64<T>(byte[] data) where T : HashAlgorithm, new()
        {
            return GetUInt64<T>(data, 0, data.Length);
        }

        public static ulong GetUInt64<T>(string data, Encoding encoding = null) where T : HashAlgorithm, new()
        {
            encoding = encoding ?? Encoding.Default;
            return GetUInt64<T>(encoding.GetBytes(data));
        }
    }
}

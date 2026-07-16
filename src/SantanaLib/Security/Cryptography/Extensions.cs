using SantanaLib.IO;
using System.Security;
using System;
using System.Security.Cryptography;
﻿using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace SantanaLib.Security.Cryptography
{
    public static class SymmetricAlgorithmExtensions
    {
        public static byte[] Encrypt(this SymmetricAlgorithm @this, byte[] buffer)
        {
            using (var encryptor = @this.CreateEncryptor())
            using (var ms = new MemoryStream())
            using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
            {
                cs.Write(buffer, 0, buffer.Length);
                cs.Flush();
                return ms.ToArray();
            }
        }

        public static byte[] Encrypt(this SymmetricAlgorithm @this, Stream stream)
        {
            using (var encryptor = @this.CreateEncryptor())
            using (var ms = new MemoryStream())
            using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
            {
                stream.CopyTo(cs);
                cs.Flush();
                return ms.ToArray();
            }
        }

        public static async Task<byte[]> EncryptAsync(this SymmetricAlgorithm @this, byte[] buffer)
        {
            using (var encryptor = @this.CreateEncryptor())
            using (var ms = new MemoryStream())
            using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
            {
                await cs.WriteAsync(buffer, 0, buffer.Length).ConfigureAwait(false);
                await cs.FlushAsync().ConfigureAwait(false);
                return ms.ToArray();
            }
        }

        public static async Task<byte[]> EncryptAsync(this SymmetricAlgorithm @this, Stream stream)
        {
            using (var encryptor = @this.CreateEncryptor())
            using (var ms = new MemoryStream())
            using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
            {
                await stream.CopyToAsync(cs).ConfigureAwait(false);
                await cs.FlushAsync().ConfigureAwait(false);
                return ms.ToArray();
            }
        }

        public static byte[] Decrypt(this SymmetricAlgorithm @this, byte[] buffer)
        {
            using (var decryptor = @this.CreateDecryptor())
            using (var ms = new MemoryStream(buffer))
            using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                return cs.ReadToEnd();
        }

        public static byte[] Decrypt(this SymmetricAlgorithm @this, Stream stream)
        {
            using (var decryptor = @this.CreateDecryptor())
            using (var cs = new CryptoStream(stream, decryptor, CryptoStreamMode.Read))
                return cs.ReadToEnd();
        }

        public static async Task<byte[]> DecryptAsync(this SymmetricAlgorithm @this, byte[] buffer)
        {
            using (var decryptor = @this.CreateDecryptor())
            using (var ms = new MemoryStream(buffer))
            using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                return await cs.ReadToEndAsync().ConfigureAwait(false);
        }

        public static async Task<byte[]> DecryptAsync(this SymmetricAlgorithm @this, Stream stream)
        {
            using (var decryptor = @this.CreateDecryptor())
            using (var cs = new CryptoStream(stream, decryptor, CryptoStreamMode.Read))
                return await cs.ReadToEndAsync().ConfigureAwait(false);
        }
    }

    public static class HashAlgorithmExtensions
    {
        public static byte[] GetBytes(this HashAlgorithm @this, Stream inputStream)
        {
            return @this.ComputeHash(inputStream);
        }

        public static byte[] GetBytes(this HashAlgorithm @this, byte[] data, int offset, int count)
        {
            return @this.ComputeHash(data, offset, count);
        }

        public static byte[] GetBytes(this HashAlgorithm @this, byte[] data)
        {
            return GetBytes(@this, data, 0, data.Length);
        }

        public static byte[] GetBytes(this HashAlgorithm @this, string data, Encoding encoding = null)
        {
            encoding = encoding ?? Encoding.Default;
            return GetBytes(@this, encoding.GetBytes(data));
        }

        public static string GetString(this HashAlgorithm @this, Stream inputStream)
        {
            var tmp = @this.ComputeHash(inputStream);
            return BitConverter.ToString(tmp).Replace("-", "").ToLowerInvariant();
        }

        public static string GetString(this HashAlgorithm @this, byte[] data, int offset, int count)
        {
            var tmp = @this.ComputeHash(data, offset, count);
            return BitConverter.ToString(tmp).Replace("-", "").ToLowerInvariant();
        }

        public static string GetString(this HashAlgorithm @this, byte[] data)
        {
            return GetString(@this, data, 0, data.Length);
        }

        public static string GetString(this HashAlgorithm @this, string data, Encoding encoding = null)
        {
            encoding = encoding ?? Encoding.Default;
            return GetString(@this, encoding.GetBytes(data));
        }

        public static ushort GetUInt16(this HashAlgorithm @this, Stream inputStream)
        {
            var tmp = @this.ComputeHash(inputStream);
            return BitConverter.ToUInt16(tmp, 0);
        }

        public static ushort GetUInt16(this HashAlgorithm @this, byte[] data, int offset, int count)
        {
            var tmp = @this.ComputeHash(data, offset, count);
            return BitConverter.ToUInt16(tmp, 0);
        }

        public static ushort GetUInt16(this HashAlgorithm @this, byte[] data)
        {
            return GetUInt16(@this, data, 0, data.Length);
        }

        public static ushort GetUInt16(this HashAlgorithm @this, string data, Encoding encoding = null)
        {
            encoding = encoding ?? Encoding.Default;
            return GetUInt16(@this, encoding.GetBytes(data));
        }

        public static uint GetUInt32(this HashAlgorithm @this, Stream inputStream)
        {
            var tmp = @this.ComputeHash(inputStream);
            return BitConverter.ToUInt32(tmp, 0);
        }

        public static uint GetUInt32(this HashAlgorithm @this, byte[] data, int offset, int count)
        {
            var tmp = @this.ComputeHash(data, offset, count);
            return BitConverter.ToUInt32(tmp, 0);
        }

        public static uint GetUInt32(this HashAlgorithm @this, byte[] data)
        {
            return GetUInt32(@this, data, 0, data.Length);
        }

        public static uint GetUInt32(this HashAlgorithm @this, string data, Encoding encoding = null)
        {
            encoding = encoding ?? Encoding.Default;
            return GetUInt32(@this, encoding.GetBytes(data));
        }

        public static ulong GetUInt64(this HashAlgorithm @this, Stream inputStream)
        {
            var tmp = @this.ComputeHash(inputStream);
            return BitConverter.ToUInt64(tmp, 0);
        }

        public static ulong GetUInt64(this HashAlgorithm @this, byte[] data, int offset, int count)
        {
            var tmp = @this.ComputeHash(data, offset, count);
            return BitConverter.ToUInt64(tmp, 0);
        }

        public static ulong GetUInt64(this HashAlgorithm @this, byte[] data)
        {
            return GetUInt64(@this, data, 0, data.Length);
        }

        public static ulong GetUInt64(this HashAlgorithm @this, string data, Encoding encoding = null)
        {
            encoding = encoding ?? Encoding.Default;
            return GetUInt64(@this, encoding.GetBytes(data));
        }
    }
}

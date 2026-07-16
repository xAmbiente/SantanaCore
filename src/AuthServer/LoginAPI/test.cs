using SantanaLib.IO;
using Ionic.Zlib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Santana.LoginAPI
{
    public static class test
    {
        public static byte[] DecompressZLib(this byte[] @this)
        {
            using (var inflate = new ZlibStream(new MemoryStream(@this), CompressionMode.Decompress, CompressionLevel.Level9))
            {
                return inflate.ReadToEnd();
            }
        }

        public static byte[] CompressZLib(this byte[] @this)
        {
            using (var buffer = new MemoryStream())
            using (var deflate = new ZlibStream(buffer, CompressionMode.Compress, CompressionLevel.Level9))
            {
                deflate.Write(@this, 0, @this.Length);
                deflate.Close();
                return buffer.ToArray();
            }
        }
    }
}

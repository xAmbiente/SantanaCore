using System;
using System.Security.Cryptography;

namespace SantanaLib.Security.Cryptography
{
    public sealed class FNV1a32 : HashAlgorithm
    {
        private const uint OffsetBasis = 2166136261;
        private const uint Prime = 16777619;
        private uint _hash = OffsetBasis;

        public override void Initialize()
        {
            _hash = OffsetBasis;
        }

        protected override void HashCore(byte[] array, int ibStart, int cbSize)
        {
            foreach (var @byte in array)
            {
                _hash ^= @byte;
                _hash *= Prime;
            }
        }

        protected override byte[] HashFinal()
        {
            return BitConverter.GetBytes(_hash);
        }
    }

    public sealed class FNV1a64 : HashAlgorithm
    {
        private const ulong OffsetBasis = 14695981039346656037;
        private const ulong Prime = 1099511628211;
        private ulong _hash = OffsetBasis;

        public override void Initialize()
        {
            _hash = OffsetBasis;
        }

        protected override void HashCore(byte[] array, int ibStart, int cbSize)
        {
            foreach (var @byte in array)
            {
                _hash ^= @byte;
                _hash *= Prime;
            }
        }

        protected override byte[] HashFinal()
        {
            return BitConverter.GetBytes(_hash);
        }
    }
}

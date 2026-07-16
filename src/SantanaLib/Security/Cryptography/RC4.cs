
using System;
using System.Security.Cryptography;

namespace SantanaLib.Security.Cryptography
{
    public sealed class RC4 : SymmetricAlgorithm
    {
#if DNXCORE50
        private RandomNumberGenerator _rng = RandomNumberGenerator.Create();
#else
        private RNGCryptoServiceProvider _rng = new RNGCryptoServiceProvider();
#endif

        public override int BlockSize
        {
            get { return 8; }
            set { throw new NotSupportedException(); }
        }
#if !DNXCORE50
        public override int FeedbackSize
        {
            get { return 0; }
            set { throw new NotSupportedException(); }
        }
#endif
        public override byte[] IV
        {
            get { return Array.Empty<byte>(); }
            set { throw new NotSupportedException(); }
        }
        public override KeySizes[] LegalBlockSizes { get; }
        public override KeySizes[] LegalKeySizes { get; }
        public override CipherMode Mode
        {
            get
            {
                return CipherMode.ECB;
            }
            set
            {
                if (value != CipherMode.ECB)
                    throw new NotSupportedException("RC4 only supports OFB");
            }
        }
        public override PaddingMode Padding
        {
            get { return PaddingMode.None; }
            set { throw new NotSupportedException(); }
        }

        public RC4()
        {
#if DNXCORE50
            KeySize = 128;
#else
            KeySizeValue = 128;
#endif
            LegalBlockSizes = new[] { new KeySizes(8, 8, 0) };
            LegalKeySizes = new[] { new KeySizes(8, 2048, 8) };
        }

        public override void GenerateIV()
        { }

        public override void GenerateKey()
        {
            if (_rng == null)
                throw new ObjectDisposedException(GetType().FullName);

            var key = new byte[KeySize / 8];
            _rng.GetBytes(key);
            Key = key;
        }

        public override ICryptoTransform CreateDecryptor(byte[] rgbKey, byte[] rgbIV)
        {
            if (rgbKey == null)
                throw new ArgumentNullException(nameof(rgbKey));

            if (rgbKey.Length == 0 || rgbKey.Length > 256)
                throw new CryptographicException("Invalid Key");

            if (rgbIV != null && rgbIV.Length > 1)
                throw new CryptographicException("Invalid Initialization Vector");

            return new RC4ManagedTransform(rgbKey);
        }

        public override ICryptoTransform CreateEncryptor(byte[] rgbKey, byte[] rgbIV)
        {
            return CreateDecryptor(rgbKey, rgbIV);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_rng != null)
                {
                    _rng.Dispose();
                    _rng = null;
                }
            }
            base.Dispose(disposing);
        }

        private class RC4ManagedTransform : ICryptoTransform
        {
            private readonly byte[] _key;
            private readonly int _keyLen;
            private readonly byte[] _permutation;
            private byte _index1;
            private byte _index2;
            private bool _disposed;

            public bool CanReuseTransform => true;
            public bool CanTransformMultipleBlocks => true;
            public int InputBlockSize => 1;
            public int OutputBlockSize => 1;

            public RC4ManagedTransform(byte[] key)
            {
                _key = key.FastClone();
                _keyLen = key.Length;
                _permutation = new byte[256];
                _disposed = false;
                Init();
            }

            public int TransformBlock(byte[] inputBuffer, int inputOffset, int inputCount, byte[] outputBuffer, int outputOffset)
            {
                if (_disposed)
                    throw new ObjectDisposedException(GetType().FullName);

                if (inputBuffer == null || outputBuffer == null)
                    throw new ArgumentNullException();

                if (inputOffset < 0 || outputOffset < 0 || inputOffset + inputCount > inputBuffer.Length || outputOffset + inputCount > outputBuffer.Length)
                    throw new ArgumentOutOfRangeException();

                var length = inputOffset + inputCount;
                for (; inputOffset < length; inputOffset++, outputOffset++)
                {
                    _index1 = (byte)((_index1 + 1) % 256);
                    _index2 = (byte)((_index2 + _permutation[_index1]) % 256);
                    var temp = _permutation[_index1];
                    _permutation[_index1] = _permutation[_index2];
                    _permutation[_index2] = temp;
                    var j = (byte)((_permutation[_index1] + _permutation[_index2]) % 256);
                    outputBuffer[outputOffset] = (byte)(inputBuffer[inputOffset] ^ _permutation[j]);
                }
                return inputCount;
            }

            public byte[] TransformFinalBlock(byte[] inputBuffer, int inputOffset, int inputCount)
            {
                if (_disposed)
                    throw new ObjectDisposedException(GetType().FullName);

                var ret = new byte[inputCount];
                TransformBlock(inputBuffer, inputOffset, inputCount, ret, 0);
                Init();
                return ret;
            }

            public void Dispose()
            {
                _disposed = true;
            }

            private void Init()
            {
                byte temp;
                for (int i = 0; i < 256; i++)
                {
                    _permutation[i] = (byte)i;
                }
                _index1 = 0;
                _index2 = 0;
                for (int j = 0, i = 0; i < 256; i++)
                {
                    j = (j + _permutation[i] + _key[i % _keyLen]) % 256;
                    temp = _permutation[i];
                    _permutation[i] = _permutation[j];
                    _permutation[j] = temp;
                }
            }
        }
    }
}

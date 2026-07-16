using System;
using System.IO;

namespace SantanaLib.IO
{
    public class BitStreamReader
    {
        private readonly byte[] _byteArray;
        private uint _bufferLengthInBits;
        private int _byteArrayIndex;
        private byte _partialByte;
        private int _cbitsInPartialByte;

        public bool EndOfStream => 0 == _bufferLengthInBits;
        public int CurrentIndex => _byteArrayIndex - 1;

        public BitStreamReader(byte[] buffer)
        {
            _byteArray = buffer;
            _bufferLengthInBits = (uint)buffer.Length * 8;
        }

        public BitStreamReader(byte[] buffer, int startIndex)
        {
            if (startIndex < 0 || startIndex >= buffer.Length)
                throw new ArgumentOutOfRangeException(nameof(startIndex));

            _byteArray = buffer;
            _byteArrayIndex = startIndex;
            _bufferLengthInBits = (uint)(buffer.Length - startIndex) * 8;
        }

        public BitStreamReader(byte[] buffer, uint bufferLengthInBits)
            : this(buffer)
        {
            if (bufferLengthInBits > (buffer.Length * 8))
                throw new ArgumentOutOfRangeException(nameof(bufferLengthInBits));

            _bufferLengthInBits = bufferLengthInBits;
        }

        public long ReadUInt64(int countOfBits)
        {
            if (countOfBits > 64 || countOfBits <= 0)
                throw new ArgumentOutOfRangeException(nameof(countOfBits));

            long retVal = 0;
            while (countOfBits > 0)
            {
                var countToRead = 8;
                if (countOfBits < 8)
                {
                    countToRead = countOfBits;
                }
                retVal <<= countToRead;
                var b = ReadByte(countToRead);
                retVal |= b;
                countOfBits -= countToRead;
            }
            return retVal;
        }

        public ushort ReadUInt16(int countOfBits)
        {
            if (countOfBits > 16 || countOfBits <= 0)
                throw new ArgumentOutOfRangeException(nameof(countOfBits));

            ushort retVal = 0;
            while (countOfBits > 0)
            {
                var countToRead = 8;
                if (countOfBits < 8)
                {
                    countToRead = countOfBits;
                }
                retVal <<= countToRead;
                var b = ReadByte(countToRead);
                retVal |= b;
                countOfBits -= countToRead;
            }
            return retVal;
        }
        
        public uint ReadUInt16Reverse(int countOfBits)
        {
            if (countOfBits > 16 || countOfBits <= 0)
                throw new ArgumentOutOfRangeException(nameof(countOfBits));

            ushort retVal = 0;
            var fullBytesRead = 0;
            while (countOfBits > 0)
            {
                var countToRead = 8;
                if (countOfBits < 8)
                {
                    countToRead = countOfBits;
                }
                ushort b = ReadByte(countToRead);
                b <<= (fullBytesRead * 8);
                retVal |= b;
                fullBytesRead++;
                countOfBits -= countToRead;
            }
            return retVal;
        }

        public uint ReadUInt32(int countOfBits)
        {
            if (countOfBits > 32 || countOfBits <= 0)
                throw new ArgumentOutOfRangeException(nameof(countOfBits));

            uint retVal = 0;
            while (countOfBits > 0)
            {
                var countToRead = 8;
                if (countOfBits < 8)
                {
                    countToRead = countOfBits;
                }
                retVal <<= countToRead;
                var b = ReadByte(countToRead);
                retVal |= b;
                countOfBits -= countToRead;
            }
            return retVal;
        }

        public uint ReadUInt32Reverse(int countOfBits)
        {
            if (countOfBits > 32 || countOfBits <= 0)
                throw new ArgumentOutOfRangeException(nameof(countOfBits));

            uint retVal = 0;
            var fullBytesRead = 0;
            while (countOfBits > 0)
            {
                var countToRead = 8;
                if (countOfBits < 8)
                {
                    countToRead = countOfBits;
                }
                uint b = ReadByte(countToRead);
                b <<= (fullBytesRead * 8);
                retVal |= b;
                fullBytesRead++;
                countOfBits -= countToRead;
            }
            return retVal;
        }

        public bool ReadBit()
        {
            var b = ReadByte(1);
            return ((b & 1) == 1);
        }

        public byte ReadByte(int countOfBits)
        {
            if (EndOfStream)
                throw new EndOfStreamException();

            if (countOfBits > 8 || countOfBits <= 0)
                throw new ArgumentOutOfRangeException(nameof(countOfBits));

            if (countOfBits > _bufferLengthInBits)
                throw new ArgumentOutOfRangeException(nameof(countOfBits));

            _bufferLengthInBits -= (uint)countOfBits;

            byte returnByte;

            if (_cbitsInPartialByte >= countOfBits)
            {
                var rightShiftPartialByteBy = 8 - countOfBits;
                returnByte = (byte)(_partialByte >> rightShiftPartialByteBy);

                unchecked
                {
                    _partialByte <<= countOfBits;
                }
                _cbitsInPartialByte -= countOfBits;
            }
            else
            {
                var nextByte = _byteArray[_byteArrayIndex];
                _byteArrayIndex++;

                var rightShiftPartialByteBy = 8 - countOfBits;
                returnByte = (byte)(_partialByte >> rightShiftPartialByteBy);

                var rightShiftNextByteBy = Math.Abs((countOfBits - _cbitsInPartialByte) - 8);
                returnByte |= (byte)(nextByte >> rightShiftNextByteBy);

                unchecked
                {
                    _partialByte = (byte)(nextByte << (countOfBits - _cbitsInPartialByte));
                }

                _cbitsInPartialByte = 8 - (countOfBits - _cbitsInPartialByte);
            }
            return returnByte;
        }
    }
}

using System;
using System.Collections.Generic;

namespace SantanaLib.IO
{
    public class BitStreamWriter
    {
        private readonly IList<byte> _targetBuffer;
        private int _remaining;

        public BitStreamWriter(IList<byte> bufferToWriteTo)
        {
            if (bufferToWriteTo == null)
                throw new ArgumentNullException(nameof(bufferToWriteTo));

            _targetBuffer = bufferToWriteTo;
        }

        public void Write(uint bits, int countOfBits)
        {
            if (countOfBits <= 0 || countOfBits > 32)
                throw new ArgumentOutOfRangeException(nameof(countOfBits));

            var fullBytes = countOfBits / 8;

            var bitsToWrite = countOfBits % 8;

            for (; fullBytes >= 0; fullBytes--)
            {
                var byteOfData = (byte)(bits >> (fullBytes * 8));
                if (bitsToWrite > 0)
                    Write(byteOfData, bitsToWrite);
                if (fullBytes > 0)
                    bitsToWrite = 8;
            }
        }

        public void WriteReverse(uint bits, int countOfBits)
        {
            if (countOfBits <= 0 || countOfBits > 32)
                throw new ArgumentOutOfRangeException(nameof(countOfBits));

            var fullBytes = countOfBits / 8;

            var bitsToWrite = countOfBits % 8;
            if (bitsToWrite > 0)
            {
                fullBytes++;
            }
            for (var x = 0; x < fullBytes; x++)
            {
                var byteOfData = (byte)(bits >> (x * 8));
                Write(byteOfData, 8);
            }
        }

        public void Write(byte bits, int countOfBits)
        {
            if (countOfBits <= 0 || countOfBits > 8)
                throw new ArgumentOutOfRangeException(nameof(countOfBits));

            byte buffer;
            if (_remaining > 0)
            {
                buffer = _targetBuffer[_targetBuffer.Count - 1];
                if (countOfBits > _remaining)
                {
                    buffer |= (byte)((bits & (0xFF >> (8 - countOfBits))) >> (countOfBits - _remaining));
                }
                else
                {
                    buffer |= (byte)((bits & (0xFF >> (8 - countOfBits))) << (_remaining - countOfBits));
                }
                _targetBuffer[_targetBuffer.Count - 1] = buffer;
            }

            if (countOfBits > _remaining)
            {
                _remaining = 8 - (countOfBits - _remaining);
                unchecked
                {
                    buffer = (byte)(bits << _remaining);
                }
                _targetBuffer.Add(buffer);
            }
            else
            {
                _remaining -= countOfBits;
            }
        }
    }
}

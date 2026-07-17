using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Santana.LoginAPI
{
    public class SecureRandom : RandomNumberGenerator
    {
        private readonly RandomNumberGenerator source = RandomNumberGenerator.Create();

        public int Next()
        {
            var intBytes = new byte[sizeof(int)];
            source.GetBytes(intBytes);
            return BitConverter.ToInt32(intBytes, 0) & (int.MaxValue - 1);
        }

        public int Next(int maxValue)
        {
            return Next(0, maxValue);
        }

        public int Next(int minValue, int maxValue)
        {
            if (minValue > maxValue)
            {
                throw new ArgumentOutOfRangeException();
            }
            return (int)Math.Floor((minValue + ((double)maxValue - minValue) * NextDouble()));
        }

        public double NextDouble()
        {
            var uintBytes = new byte[sizeof(uint)];
            source.GetBytes(uintBytes);
            var sample = BitConverter.ToUInt32(uintBytes, 0);
            return sample / (uint.MaxValue + 1.0);
        }

        public override void GetBytes(byte[] data)
        {
            source.GetBytes(data);
        }

        public override void GetNonZeroBytes(byte[] data)
        {
            source.GetNonZeroBytes(data);
        }
    }
}

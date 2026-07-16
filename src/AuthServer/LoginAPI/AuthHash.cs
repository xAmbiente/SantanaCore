using System.Security.Cryptography;
using System.Text;

namespace Santana.LoginAPI
{
    internal class AuthHash
    {
        public static string GetHash256(string data)
        {
            var output = string.Empty;

            if (data != null)
                using (var sha = SHA256.Create())
                {
                    var raw = Encoding.UTF8.GetBytes(data);
                    var digest = sha.ComputeHash(raw);
                    output = GetHashString(digest);
                }
            return output;
        }

        public static string GetHash512(string data)
        {
            var output = string.Empty;

            if (data != null)
                using (var sha = SHA512.Create())
                {
                    var raw = Encoding.UTF8.GetBytes(data);
                    var digest = sha.ComputeHash(raw);
                    output = GetHashString(digest);
                }
            return output;
        }

        private static string GetHashString(byte[] dataBufferHashed)
        {
            var hex = new StringBuilder();
            foreach (var octet in dataBufferHashed)
                hex.Append(octet.ToString("X2"));
            return hex.ToString();
        }
    }
}

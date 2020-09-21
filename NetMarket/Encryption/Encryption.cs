using System;
using System.Security.Cryptography;
using System.Text;

namespace NetMarket.Encryption
{
    public static class Encryption
    {
        public static string GetHash(string password)
        {
            var sha = new SHA1Managed();
            byte[] hash = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hash);
        }
    }
}
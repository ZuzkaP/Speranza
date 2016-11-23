using System;
using System.Security.Cryptography;
using System.Text;

namespace Speranza.Services
{
    public class Hasher : IHasher
    {
        private byte[] hash;
        private string result;

        public Hasher()
        {
        }

        public string HashPassword(string password)
        {
            if(password == null)
            {
                throw new ArgumentNullException();
            }
            result = "";
            var data = Encoding.UTF8.GetBytes(password);

            using (SHA512 shaM = new SHA512Managed())
            {
                hash = shaM.ComputeHash(data);
            }
            result = Convert.ToBase64String(hash);

            return result;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLibrary
{
    public class PasswordEncryption
    {
        public static string GetSalt
        {
            get
            {
                RNGCryptoServiceProvider rngCryptoServiceProvider = new RNGCryptoServiceProvider();
                byte[] buffer = new byte[32];
                rngCryptoServiceProvider.GetBytes(buffer);
                return Convert.ToBase64String(buffer);
            }
        }

        public static string GenerateCombinedHashPassword(string salt, string password)
        {
            byte[] binary = Encoding.UTF8.GetBytes(String.Concat(password, salt));
            SHA256Managed sha256ManagedString = new SHA256Managed();
            byte[] hash = sha256ManagedString.ComputeHash(binary);

            return String.Concat(salt, ":", Convert.ToBase64String(hash));
        }

        public static (string, string) SplitHashedPassword(string combinedHashPassword)
        {
            string[] splittedSaltHashedPassword = combinedHashPassword.Split(':');

            return (splittedSaltHashedPassword[0], splittedSaltHashedPassword[1]);
        }

        public static bool ValidateHashedPassword(string salt, string password, string hashedPassword)
        {
            return GenerateCombinedHashPassword(salt, password).Split(':')[1].Equals(hashedPassword);
        }
    }
}

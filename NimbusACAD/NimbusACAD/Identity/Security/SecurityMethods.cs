using System;
using System.Security.Cryptography;
using System.Text;

namespace NimbusACAD.Identity.Security
{
    public class SecurityMethods
    {
        //Generate salt and return as string to storage in DB
        public static string GenerateSalt()
        {
            const int SALT_LENGTH = 32;
            using (var rng = new RNGCryptoServiceProvider())
            {
                var random = new byte[SALT_LENGTH];
                rng.GetBytes(random);

                return Convert.ToBase64String(random);
            }
        }

        //Generate temporary token access and return as string to send email
        public static string GenerateTempTokenAccess()
        {
            using (var rng = new RNGCryptoServiceProvider())
            {
                const int TOKEN_LENGTH = 7;
                var random = new byte[TOKEN_LENGTH];
                rng.GetBytes(random);

                return Convert.ToBase64String(random);
            }
        }

        //Hash the password with PBKDF-2
        public static string HashPasswordPBKDF2(string passTyped, string saltFromDB)
        {
            const int ROUNDS = 1000;

            byte[] toBeHashed = Encoding.UTF8.GetBytes(passTyped);
            byte[] salt = Encoding.UTF8.GetBytes(saltFromDB);

            using (var cript = new Rfc2898DeriveBytes(toBeHashed, salt, ROUNDS))
            {
                return Convert.ToBase64String(cript.GetBytes(32));
            }
        }
    }
}
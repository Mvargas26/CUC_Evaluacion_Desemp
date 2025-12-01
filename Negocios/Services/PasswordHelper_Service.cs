using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Negocios.Services
{
    public class PasswordHelper_Service
    {
       
            private const int SaltSize = 16;
            private const int HashSize = 20;
            private const int Iterations = 10000;

            public static string EncriptarPassword(string password)
            {
                if (string.IsNullOrWhiteSpace(password))
                    throw new ArgumentException("La contraseña no puede estar vacía");

                byte[] salt = new byte[SaltSize];
                using (var rng = RandomNumberGenerator.Create())
                {
                    rng.GetBytes(salt);
                }

                byte[] hash = GenerarHash(password, salt);

                byte[] hashBytes = new byte[SaltSize + HashSize];
                Array.Copy(salt, 0, hashBytes, 0, SaltSize);
                Array.Copy(hash, 0, hashBytes, SaltSize, HashSize);

                return Convert.ToBase64String(hashBytes);
            }

            public static bool VerificarPassword(string password, string hashedPassword)
            {
                if (string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(hashedPassword))
                    return false;

                try
                {
                    byte[] hashBytes = Convert.FromBase64String(hashedPassword);

                    byte[] salt = new byte[SaltSize];
                    Array.Copy(hashBytes, 0, salt, 0, SaltSize);

                    byte[] hash = GenerarHash(password, salt);

                    for (int i = 0; i < HashSize; i++)
                    {
                        if (hashBytes[i + SaltSize] != hash[i])
                            return false;
                    }

                    return true;
                }
                catch
                {
                    return false;
                }
            }

            private static byte[] GenerarHash(string password, byte[] salt)
            {
                using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256))
                {
                    return pbkdf2.GetBytes(HashSize);
                }
            }
        
    }//fin class

}//fin space

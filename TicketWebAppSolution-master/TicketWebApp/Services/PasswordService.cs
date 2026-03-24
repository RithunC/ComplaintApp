using System.Security.Cryptography;
using TicketWebApp.Interfaces;

namespace TicketWebApp.Services
{
    public class PasswordService : IPasswordService
    {

        public string HashPassword(string password, byte[]? salt, out byte[] key)
        {
            // PBKDF2 (you can upgrade to Argon2 later)
            salt ??= RandomNumberGenerator.GetBytes(16);
            key = salt;
            using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 100_000, HashAlgorithmName.SHA256);
            var hash = pbkdf2.GetBytes(32);
            return Convert.ToBase64String(hash);
        }

        public bool VerifyPassword(string password, string hashed, byte[] key)
        {
            using var pbkdf2 = new Rfc2898DeriveBytes(password, key, 100_000, HashAlgorithmName.SHA256);
            var hash = pbkdf2.GetBytes(32);
            return Convert.ToBase64String(hash) == hashed;
        }

    }
}

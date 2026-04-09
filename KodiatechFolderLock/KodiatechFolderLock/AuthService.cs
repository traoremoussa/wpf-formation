using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace KodiatechFolderLock
{
    // Simple auth service that stores a SHA-256 hash of the password in AppData
    public class AuthService
    {
        private readonly string _storePath;

        public AuthService()
        {
            var dir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "KodiatechFolderLock");
            Directory.CreateDirectory(dir);
            _storePath = Path.Combine(dir, "password.hash");
        }

        public string Hash(string input)
        {
            using (var sha = SHA256.Create())
            {
                return Convert.ToBase64String(sha.ComputeHash(Encoding.UTF8.GetBytes(input ?? string.Empty)));
            }
        }

        public void SaveHash(string hash)
        {
            File.WriteAllText(_storePath, hash ?? string.Empty);
        }

        public string? LoadHash()
        {
            if (!File.Exists(_storePath)) return null;
            try
            {
                return File.ReadAllText(_storePath);
            }
            catch
            {
                return null;
            }
        }

        public bool Verify(string password, string storedHash)
        {
            if (password == null) return false;
            var h = Hash(password);
            return string.Equals(h, storedHash, StringComparison.Ordinal);
        }
    }
}

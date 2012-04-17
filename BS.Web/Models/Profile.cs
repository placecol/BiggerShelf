using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.Text;

namespace Kaiser.BiggerShelf.Web.Models
{
    public class Profile
    {
        [Key]
        public string Id { get; set; }

        public string AspNetUserGuid { get; set; }

        public string Email { get; set; }

        private string Hash { get; set; }

        private string salt;
        private string Salt
        {
            get { return salt ?? (salt = Guid.NewGuid().ToString("N")); }
            set { salt = value; }
        }

        public string Name { get; set; }

        public List<FriendProfile> Friends { get; set; }

        public List<SelectedBook> ReadingList { get; set; }

        public List<Category> InterestedCategories { get; set; }

        public void SetPassword(string password)
        {
            Hash = CalculateHash(password);
        }

        private string CalculateHash(string password)
        {
            using (var sha = SHA256.Create())
            {
                var computedHash = sha.ComputeHash(Encoding.Unicode.GetBytes(Salt + password));
                return Convert.ToBase64String(computedHash);
            }
        }

        public bool TryPassword(string passwordTry)
        {
            return Hash == null || Hash == CalculateHash(passwordTry);
        }
    }
}
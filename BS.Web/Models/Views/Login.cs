using System.ComponentModel.DataAnnotations;

namespace Kaiser.BiggerShelf.Web.Models.Views
{
    public class Login
    {
        [Required]
        public string Email { get; set; }

        [Required, DataType(DataType.Password)]
        public string Password { get; set; }

        public bool Persistent { get; set; }
    }
}
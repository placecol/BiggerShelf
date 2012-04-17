using System.ComponentModel.DataAnnotations;

namespace Kaiser.BiggerShelf.Web.Models
{
    public class FriendProfile
    {
        [Key]
        public string Id { get; set; }

        public string Name { get; set; }
    }
}
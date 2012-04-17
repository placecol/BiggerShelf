using System.ComponentModel.DataAnnotations;

namespace Kaiser.BiggerShelf.Web.Models
{
    public class SelectedBook
    {
        [Key]
        public string Id { get; set; }

        public int Rating { get; set; }

        public string Title { get; set; }
    }
}

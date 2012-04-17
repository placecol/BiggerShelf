using System.ComponentModel.DataAnnotations;

namespace Kaiser.BiggerShelf.Web.Models.Views
{
    public class BookView
    {
        [Key]
        public string Id { get; set; }

        public string Title { get; set; }

        public string Author { get; set; }

        public string ASIN { get; set; }

        public string ProfileId { get; set; }
    }
}
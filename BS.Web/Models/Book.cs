using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Kaiser.BiggerShelf.Web.Models
{
    public class Book
    {
        [Key]
        public string Id { get; set; }

        public DateTime AddedDate { get; set; }

        public DateTime PublishDate { get; set; }

        public string ASIN { get; set; }

        public string Author { get; set; }

        public List<Category> Categories { get; set; }

        public string Description { get; set; }

        public string Title { get; set; }

        public int Rating { get; set; }
    }
}
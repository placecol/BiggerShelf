using System.Collections.Generic;
using System.Linq;
using Kaiser.BiggerShelf.Web.Controllers.Api;
using Kaiser.BiggerShelf.Web.Models;
using Xunit;

namespace Kaiser.BiggerShelf.Test.Web.Controllers.Api
{
    public class BooksControllerTest
    {
        [Fact]
        public void GetAllBooks()
        {
            Exec.Action<BooksController>(
                dbObjects: new object[] {new Book {Title = "A Book"}},
                action: controller =>
                            {
                                var books = controller.Get();

                                Assert.Equal(1, books.Count());
                                Assert.Equal("A Book", books.Single().Title);
                            }
                );
        }

        [Fact]
        public void MaximumOf128BooksReturned()
        {
            var existingBooks = new List<Book>();
            for (var i = 0; i < 129; i++)
            {
                existingBooks.Add(new Book {Title = "Book #" + i});
            }

            Exec.Action<BooksController>(
                dbObjects: existingBooks.ToArray(),
                action: controller =>
                            {
                                var books = controller.Get();

                                Assert.Equal(128, books.ToList().Count());
                            }
                );
        }

        [Fact]
        public void GetSingleBook()
        {
            Exec.Action<BooksController>(
                dbObjects: new object[] {new Book {Title = "A Book"}},
                action: controller =>
                            {
                                var book = controller.Get(1);

                                Assert.Equal("A Book", book.Title);
                            }
                );
        }
    }
}

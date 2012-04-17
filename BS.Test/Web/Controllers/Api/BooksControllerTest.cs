using System.Linq;
using Kaiser.BiggerShelf.Web.Controllers.Api;
using Kaiser.BiggerShelf.Web.Models;
using Raven.Client.Embedded;
using Xunit;

namespace Kaiser.BiggerShelf.Test.Web.Controllers.Api
{
    public class BooksControllerTest
    {
        [Fact]
        public void ReturnsAllBooksOnGet()
        {
            using (var store = new EmbeddableDocumentStore{RunInMemory = true})
            {
                store.Initialize();
                using (var session = store.OpenSession())
                {
                    session.Store(new Book {Title = "A Book"});
                    session.SaveChanges();
                }


                using (var session = store.OpenSession())
                {
                    var controller = new BooksController {Docs = session};
                    var books = controller.Get();

                    Assert.Equal(1, books.Count());
                    Assert.Equal("A Book", books.Single().Title);
                }
            }
            
        }
    }
}

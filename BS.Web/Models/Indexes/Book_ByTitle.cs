using System.Linq;
using Kaiser.BiggerShelf.Web.Models.Views;
using Raven.Abstractions.Indexing;
using Raven.Client.Indexes;

namespace Kaiser.BiggerShelf.Web.Models.Indexes
{
    public class Book_ByTitle : AbstractIndexCreationTask<Book, BookView>
    {
        public Book_ByTitle()
        {
            Map = books => from book in books
                           select new BookView {Title = book.Title, Id = book.Id};

            Reduce = results => from result in results
                                group result by result.Id
                                into g
                                from book in g
                                select new BookView { Title = book.Title, Id = book.Id };

            Indexes.Add(x => x.Title, FieldIndexing.Analyzed);
        }
    }
}

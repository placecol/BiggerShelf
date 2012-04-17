using System.Linq;
using Kaiser.BiggerShelf.Web.Models.Views;
using Raven.Abstractions.Indexing;
using Raven.Client.Indexes;

namespace Kaiser.BiggerShelf.Web.Models.Indexes
{
    public class Profile_ByBookId : AbstractIndexCreationTask<Profile, BookView>
    {
        public Profile_ByBookId()
        {
            Map = profiles => from profile in profiles
                              from book in profile.ReadingList
                              select new BookView
                                         {
                                             Id = book.Id,
                                             ProfileId = profile.Id,
                                             Title = book.Title
                                         };

            Reduce = results => from result in results
                                group result by new {result.Id, result.ProfileId}
                                into g
                                from book in g.Where(b => b.Id != null).DefaultIfEmpty()
                                where book != null
                                select new BookView
                                           {
                                               Id = g.Key.Id,
                                               ProfileId = g.Key.ProfileId,
                                               Title = book.Title
                                           };

            TransformResults = (database, results) => from result in results
                                                      group result by result.Id
                                                      into g
                                                      let book = database.Load<Book>(g.Key)
                                                      select new BookView
                                                                 {
                                                                     Id = book.Id,
                                                                     ASIN = book.ASIN,
                                                                     Author = book.Author,
                                                                     Title = book.Title
                                                                 };

            Index(b => b.Title, FieldIndexing.Analyzed);
        }
    }
}
using System.Linq;
using Kaiser.BiggerShelf.Web.Models.Views;
using Raven.Abstractions.Indexing;
using Raven.Client.Indexes;

namespace Kaiser.BiggerShelf.Web.Models.Indexes
{
    public class Profile_ByBookTitle : AbstractMultiMapIndexCreationTask<BookView>
    {
        public Profile_ByBookTitle()
        {
            AddMap<Profile>(profiles => from profile in profiles
                                        from book in profile.ReadingList
                                        select
                                            new BookView
                                                {
                                                    Id = book.Id,
                                                    ProfileId = profile.Id,
                                                    Title = string.Empty,
                                                    ASIN = string.Empty,
                                                    Author = string.Empty,
                                                });

            AddMap<Book>(books => from book in books
                                  select
                                      new BookView
                                          {
                                              Id = book.Id,
                                              ProfileId = string.Empty,
                                              Title = book.Title,
                                              ASIN = book.ASIN,
                                              Author = book.Author,
                                          });

            Reduce = results => from result in results
                                group result by result.Id
                                into g
                                from profile in g.Where(p => p.ProfileId != string.Empty).DefaultIfEmpty()
                                from book in g.Where(p => p.ProfileId == string.Empty).DefaultIfEmpty()
                                select
                                    new BookView
                                        {
                                            ProfileId = profile.ProfileId,
                                            Id = g.Key,
                                            Title = book.Title,
                                            ASIN = book.ASIN,
                                            Author = book.Author
                                        };

            Index(b => b.Title, FieldIndexing.Analyzed);
        }
    }
}
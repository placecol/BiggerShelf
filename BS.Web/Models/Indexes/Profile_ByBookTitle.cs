using System;
using System.Linq;
using Raven.Abstractions.Indexing;
using Raven.Client.Indexes;

namespace Kaiser.BiggerShelf.Web.Models.Indexes
{
    public class Profile_ByBookTitle : AbstractMultiMapIndexCreationTask<Profile_ByBookTitle.Result>
    {
        public class Result
        {
            public string Id { get; set; }
            public string ProfileIds { get; set; }
            public string Query { get; set; }
            public bool IsBook { get; set; }
            public string Title { get; set; }
            public int Rating { get; set; }
            public DateTime? PublishDate { get; set; }
        }

        public Profile_ByBookTitle()
        {
            AddMap<Profile>(profiles => from profile in profiles
                                        from book in profile.ReadingList.DefaultIfEmpty()
                                        where book != null
                                        select
                                            new
                                                {
                                                    Id = book.Id,
                                                    ProfileIds = new [] {profile.Id},
                                                    Query = string.Empty,
                                                    IsBook = false,
                                                    Title = string.Empty,
                                                    Rating = 0,
                                                    PublishDate = string.Empty
                                                });

            AddMap<Book>(books => from book in books
                                  select
                                      new
                                          {
                                              Id = book.Id,
                                              ProfileIds = new [] {string.Empty},
                                              Query = new[] { book.Title, book.ASIN, book.Author },
                                              IsBook = true,
                                              Title = book.Title,
                                              Rating = book.Rating,
                                              PublishDate = book.PublishDate
                                          });

            Reduce = results => from result in results
                                group result by result.Id
                                into g
                                select
                                    new
                                        {
                                            Id = g.Key,
                                            ProfileIds = g.SelectMany(r => r.ProfileIds).ToArray(),
                                            Query = g.Where(b => b.IsBook).Select(b => b.Query),
                                            IsBook = true,
                                            Title = g.Where(b => b.IsBook).Select(b => b.Title),
                                            Rating = g.Where(b => b.IsBook).Select(b => b.Rating),
                                            PublishDate = g.Where(b => b.IsBook).Select(b => b.PublishDate)
                                        };

            TransformResults = (database, results) =>
                               from result in results
                               let book = database.Load<Book>(result.Id)
                               select book;

            Index(r => r.ProfileIds, FieldIndexing.Analyzed);
            Index(r => r.Query, FieldIndexing.Analyzed);
        }
    }
}
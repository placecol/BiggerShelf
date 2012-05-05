using System;
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
        public void CanQueryBooksUnfiltered()
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
        public void CanFilterByMultipleProfileIds()
        {
            // Three books, two profiles, books/1 and books/3 selected across profiles,
            // should return books/1 and books/3 when filtering by both profiles

            Exec.Action<BooksController>(
                dbObjects: new object[]
                               {
                                   new Book {Id = "books/1", Title = "Book 1"},
                                   new Book {Id = "books/2", Title = "Book 2"},
                                   new Book {Id = "books/3", Title = "Book 3"},
                                   new Profile
                                       {
                                           Id = "profiles/1",
                                           ReadingList = new List<SelectedBook> {new SelectedBook {Id = "books/1"}}
                                       },
                                   new Profile
                                       {
                                           Id = "profiles/2",
                                           ReadingList = new List<SelectedBook>
                                                             {
                                                                 new SelectedBook {Id = "books/1"},
                                                                 new SelectedBook{Id = "books/3"}
                                                             }
                                       }
                               },
                action: controller =>
                            {
                                var filteredBooks = controller.Get("profiles/1, profiles/2").ToList();

                                Assert.Equal(2, filteredBooks.Count());
                                Assert.True(filteredBooks.Any(b => b.Id == "books/1"));
                                Assert.True(filteredBooks.Any(b => b.Id == "books/3"));
                            }
                );
        }

        [Fact]
        public void CanFilterByTitle()
        {
            Exec.Action<BooksController>(
                dbObjects: new object[]
                               {
                                   new Book {Id = "books/1", Title = "Another book"},
                                   new Book {Id = "books/2", Title = "This is a book"},
                                   new Book {Id = "books/3", Title = "Spelled buk wrong"}
                               },
                action: controller =>
                            {
                                var filteredBooks = controller.Get(search: "book").ToList();

                                Assert.Equal(2, filteredBooks.Count());
                                Assert.True(filteredBooks.Any(b => b.Id == "books/1"));
                                Assert.True(filteredBooks.Any(b => b.Id == "books/2"));
                            }
                );
        }

        [Fact]
        public void CanFilterByASIN()
        {
            Exec.Action<BooksController>(
                dbObjects: new object[]
                               {
                                   new Book {Id = "books/1", ASIN = "12345"},
                                   new Book {Id = "books/2", ASIN = "54321"},
                                   new Book {Id = "books/3", ASIN = "09876"}
                               },
                action: controller =>
                            {
                                var filteredBooks = controller.Get(search: "12345").ToList();

                                Assert.Equal(1, filteredBooks.Count());
                                Assert.True(filteredBooks.Any(b => b.Id == "books/1"));
                            }
                );
        }

        [Fact]
        public void CanFilterByAuthor()
        {
            Exec.Action<BooksController>(
                dbObjects: new object[]
                               {
                                   new Book {Id = "books/1", Author = "Jane Austin"},
                                   new Book {Id = "books/2", Author = "George RR Martin"},
                               },
                action: controller =>
                            {
                                var filteredBooks = controller.Get(search: "Jane Austin").ToList();

                                Assert.Equal(1, filteredBooks.Count());
                                Assert.True(filteredBooks.Any(b => b.Id == "books/1"));
                            }
                );
        }

        [Fact]
        public void CanCombineFilters()
        {
            Exec.Action<BooksController>(
                dbObjects: new object[]
                               {
                                   new Book {Id = "books/1", Author = "Jane Austin", ASIN = "12345", Title = "Some book"},
                                   new Book {Id = "books/2", Author = "George RR Martin", ASIN = "54321", Title = "Another book"},
                                   new Book {Id = "books/3", Author = "Random Author", ASIN = "09876", Title = "Best Seller"},
                               },
                action: controller =>
                            {
                                var filteredBooks = controller.Get(search: "Jane 09876 another").ToList();

                                Assert.Equal(3, filteredBooks.Count());
                                Assert.True(filteredBooks.Any(b => b.Id == "books/1"));
                                Assert.True(filteredBooks.Any(b => b.Id == "books/2"));
                                Assert.True(filteredBooks.Any(b => b.Id == "books/3"));
                            }
                );
        }

        [Fact]
        public void CanCombineSearchAndProfileFilters()
        {
            Exec.Action<BooksController>(
                dbObjects: new object[]
                               {
                                   new Book {Id = "books/1", Author = "Jane Austin"},
                                   new Book {Id = "books/2", Author = "George RR Martin"},
                                   new Book {Id = "books/3", Author = "Random Author"},
                                   new Profile
                                       {
                                           Id = "profiles/1",
                                           ReadingList = new List<SelectedBook> {new SelectedBook {Id = "books/1"}}
                                       },
                                   new Profile
                                       {
                                           Id = "profiles/2",
                                           ReadingList = new List<SelectedBook>
                                                             {
                                                                 new SelectedBook {Id = "books/1"},
                                                                 new SelectedBook{Id = "books/3"}
                                                             }
                                       }
                               },
                action: controller =>
                            {
                                var filteredBooks =
                                    controller.Get(profileIds: "profiles/1, profiles/2",
                                                   search: "Jane").ToList();

                                Assert.Equal(1, filteredBooks.Count());
                                Assert.True(filteredBooks.Any(b => b.Id == "books/1"));
                            }
            );
        }

        [Fact]
        public void DefaultOrderIsRatingThenPublishDate()
        {
            Exec.Action<BooksController>(
                dbObjects: new object[]
                               {
                                   new Book {Id = "books/1", Rating = 4, PublishDate = new DateTime(2010, 11, 3)},
                                   new Book {Id = "books/2", Rating = 4, PublishDate = new DateTime(2011, 6, 5)},
                                   new Book {Id = "books/3", Rating = 3, PublishDate = new DateTime(2010, 4, 20)},
                               },
                action: controller =>
                            {
                                var filteredBooks = controller.Get().ToList();

                                Assert.Equal("books/2", filteredBooks[0].Id);
                                Assert.Equal("books/1", filteredBooks[1].Id);
                                Assert.Equal("books/3", filteredBooks[2].Id);
                            }
                );
        }
    }
}

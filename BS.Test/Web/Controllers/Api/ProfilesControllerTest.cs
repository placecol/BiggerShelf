using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using Kaiser.BiggerShelf.Web.Controllers.Api;
using Kaiser.BiggerShelf.Web.Infrastructure.Raven;
using Kaiser.BiggerShelf.Web.Models;
using Xunit;

namespace Kaiser.BiggerShelf.Test.Web.Controllers.Api
{
    public class ProfilesControllerTest
    {
        [Fact]
        public void GetProfileOtherThanYourOwnIsForbidden()
        {
            Exec.Action<ProfilesController>(
                dbObjects: new object[]
                               {
                                   new Profile {Id = "profiles/1", AspNetUserGuid = "me"},
                                   new Profile {Id = "profiles/2", AspNetUserGuid = "them"}
                               },
                loggedInUserName: "me",
                action: controller =>
                            {
                                var response = controller.Get(new RavenId<Profile>("profiles", "2"));

                                Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
                            });
        }

        [Fact]
        public void GetProfileForCurrentUser()
        {
            Exec.Action<ProfilesController>(
                dbObjects: new object[]
                               {
                                   new Profile {Id = "profiles/1", AspNetUserGuid = "me"},
                                   new Profile {Id = "profiles/2", AspNetUserGuid = "them"}
                               },
                loggedInUserName: "me",
                action: controller =>
                {
                    var response = controller.Get(new RavenId<Profile>("profiles", "1"));

                    Assert.Equal("profiles/1", response.Content.ReadOrDefaultAsync().Result.Id);
                });
        }

        [Fact]
        public void GetProfilesReadingList()
        {
            Exec.Action<ProfilesController>(
                dbObjects: new object[]
                               {
                                   new Profile
                                       {
                                           Id = "profiles/1",
                                           AspNetUserGuid = "me",
                                           ReadingList =
                                               new List<SelectedBook>(new[]
                                                                          {
                                                                              new SelectedBook
                                                                                  {
                                                                                      Id = "books/1",
                                                                                      Rating = 4,
                                                                                      Title = "First Book"
                                                                                  },

                                                                              new SelectedBook
                                                                                  {
                                                                                      Id = "books/2",
                                                                                      Rating = 4,
                                                                                      Title = "Second Book"
                                                                                  }
                                                                          })
                                       }
                               },
                loggedInUserName: "me",
                action: controller =>
                            {
                                var readingList = controller.GetBooks(new RavenId<Profile>("profiles", "1"));

                                Assert.Equal(2, readingList.Count());
                            });
        }

        [Fact]
        public void UpdatingBookNotOnReadingListAddsBookToReadingList()
        {
            Exec.Action<ProfilesController>(
                dbObjects: new object[]
                               {
                                   new Profile
                                       {
                                           Id = "profiles/1",
                                           AspNetUserGuid = "me",
                                           ReadingList =
                                               new List<SelectedBook>(new[]
                                                                          {
                                                                              new SelectedBook
                                                                                  {
                                                                                      Id = "books/1",
                                                                                      Rating = 4
                                                                                  }
                                                                          })
                                       },
                                   new Book
                                       {
                                           Id = "books/1",
                                           Title = "A Book"
                                       },
                                   new Book
                                       {
                                           Id = "books/2",
                                           Title = "Another Book"
                                       }
                               },
                loggedInUserName: "me",
                action: controller =>
                            {
                                var response =
                                    controller.UpdateBook(
                                        new RavenId<Profile>("profiles", "1"),
                                        new RavenId<Book>("books", "2"),
                                        3);

                                var profile = controller.Docs.Load<Profile>("profiles/1");
                                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                                Assert.True(
                                    profile.ReadingList.Any(b => b.Id == "books/2"
                                                                 && b.Rating == 3 &&
                                                                 b.Title == "Another Book"));
                                var selectedBook = response.Content.ReadOrDefaultAsync().Result;
                                Assert.True(selectedBook.Id == "books/2" &&
                                            selectedBook.Rating == 3 &&
                                            selectedBook.Title == "Another Book");
                            });
        }

        [Fact]
        public void UpdateBookAlreadyOnReadingList()
        {
            Exec.Action<ProfilesController>(
                dbObjects: new object[]
                               {
                                   new Profile
                                       {
                                           Id = "profiles/1",
                                           AspNetUserGuid = "me",
                                           ReadingList =
                                               new List<SelectedBook>(new[]
                                                                          {
                                                                              new SelectedBook
                                                                                  {
                                                                                      Id = "books/1",
                                                                                      Rating = 4
                                                                                  }
                                                                          })
                                       },
                                    new Book
                                        {
                                            Id = "books/1",
                                            Title = "A Book"
                                        },
                                    new Book
                                        {
                                            Id = "books/2",
                                            Title = "Another Book"
                                        }
                               },
                loggedInUserName: "me",
                action: controller =>
                {
                    // was a rating of 4, changing it to 3
                    var response = controller.UpdateBook(new RavenId<Profile>("profiles", "1"),
                                                         new RavenId<Book>("books", "1"),
                                                         3);

                    var profile = controller.Docs.Load<Profile>("profiles/1");
                    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                    Assert.Equal(1, profile.ReadingList.Count(b => b.Id == "books/1"));
                    Assert.True(profile.ReadingList.Any(b => b.Id == "books/1" && b.Rating == 3));
                    var selectedBook = response.Content.ReadOrDefaultAsync().Result;
                    Assert.True(selectedBook.Id == "books/1" &&
                                selectedBook.Rating == 3);
                });
        }

        [Fact]
        public void AttemptToUpdateNonExistantProfileResultsIn404()
        {
            Exec.Action<ProfilesController>(
                dbObjects: new object[]
                               {
                                    new Book
                                        {
                                            Id = "books/1",
                                            Title = "A Book"
                                        }
                               },
                loggedInUserName: "me",
                action: controller =>
                {
                    // profiles/1 doesn't exist
                    var response = controller.UpdateBook(new RavenId<Profile>("profiles", "1"),
                                                         new RavenId<Book>("books", "1"),
                                                         3);

                    Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
                });
        }

        [Fact]
        public void AttemptToUpdateNonExistantBookOnExistingProfileResultsIn404()
        {
            Exec.Action<ProfilesController>(
                dbObjects: new object[]
                               {
                                   new Profile
                                       {
                                           Id = "profiles/1",
                                           AspNetUserGuid = "me",
                                           ReadingList =
                                               new List<SelectedBook>(new[]
                                                                          {
                                                                              new SelectedBook
                                                                                  {
                                                                                      Id = "books/1",
                                                                                      Rating = 4,
                                                                                      Title = "First Book"
                                                                                  }
                                                                          })
                                       },
                                    new Book
                                        {
                                            Id = "books/1",
                                            Title = "A Book"
                                        }
                               },
                loggedInUserName: "me",
                action: controller =>
                {
                    // books/2 doesn't exist
                    var response = controller.UpdateBook(new RavenId<Profile>("profiles", "1"),
                                                         new RavenId<Book>("books", "2"), 3);

                    Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
                });
        }

        [Fact]
        public void AdjustRatingToNoLessThanZeroAndNoGreaterThanFive()
        {
            Exec.Action<ProfilesController>(
                dbObjects: new object[]
                               {
                                   new Profile
                                       {
                                           Id = "profiles/1",
                                           AspNetUserGuid = "me",
                                           ReadingList =
                                               new List<SelectedBook>(new[]
                                                                          {
                                                                              new SelectedBook
                                                                                  {
                                                                                      Id = "books/1",
                                                                                      Rating = 4,
                                                                                      Title = "First Book"
                                                                                  }
                                                                          })
                                       },
                                    new Book
                                        {
                                            Id = "books/1",
                                            Title = "A Book"
                                        }
                               },
                loggedInUserName: "me",
                action: controller =>
                {
                    // attempting to set rating to -1
                    var response = controller.UpdateBook(new RavenId<Profile>("profiles", "1"),
                                                         new RavenId<Book>("books", "1"), -1);

                    var profile = controller.Docs.Load<Profile>("profiles/1");
                    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                    Assert.Equal(1, profile.ReadingList.Count(b => b.Id == "books/1" && b.Rating == 0));

                    // attempting to set rating to 6
                    response = controller.UpdateBook(new RavenId<Profile>("profiles", "1"),
                                                         new RavenId<Book>("books", "1"), 6);

                    profile = controller.Docs.Load<Profile>("profiles/1");
                    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                    Assert.Equal(1, profile.ReadingList.Count(b => b.Id == "books/1" && b.Rating == 5));
                });
        }
    }
}

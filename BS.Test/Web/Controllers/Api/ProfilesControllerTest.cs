using System.Collections.Generic;
using System.Linq;
using System.Net;
using Kaiser.BiggerShelf.Web.Controllers.Api;
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
                                var response = controller.Get(2);

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
                    var response = controller.Get(1);

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
                                var readingList = controller.GetReadingList(1);

                                Assert.Equal(2, readingList.Count());
                            });
        }
    }
}

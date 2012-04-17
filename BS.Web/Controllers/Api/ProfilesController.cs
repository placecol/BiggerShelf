using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Principal;
using System.Web.Http.Hosting;
using Kaiser.BiggerShelf.Web.Models;

namespace Kaiser.BiggerShelf.Web.Controllers.Api
{
    public class ProfilesController : RavenApiController
    {
        public HttpResponseMessage<Profile> Get(int id)
        {
            var fullId = string.Format("profiles/{0}", id);
            return fullId != GetUser().Id
                       ? new HttpResponseMessage<Profile>(HttpStatusCode.Forbidden)
                       : new HttpResponseMessage<Profile>(Docs.Load<Profile>(fullId));
        }

        private Profile GetUser()
        {
            var userName = ((IPrincipal) Request.Properties[HttpPropertyKeys.UserPrincipalKey]).Identity.Name;
            return Docs.Query<Profile>().Single(p => p.AspNetUserGuid == userName);
        }

        //public IQueryable<FriendProfile> GetFriends(string id)
        //{
        //    var profile = Docs.Load<Profile>(id);
        //    return profile.Friends.AsQueryable();
        //}

        //public FriendProfile GetFriend(string id, string friendId)
        //{
        //    var profile = Docs.Load<Profile>(id);
        //    return profile.Friends.SingleOrDefault(f => f.Id == friendId);
        //}

        //public void PutFriend(string id, string friendId, FriendProfile friend)
        //{
        //    var profile = Docs.Load<Profile>(id);
        //    if (profile == null) return;

        //    var currentFriend = profile.Friends.SingleOrDefault(f => f.Id == friendId);
        //    if (currentFriend == null)
        //    {
        //        friend.Id = friendId;
        //        profile.Friends.Add(friend);
        //    }
        //    else
        //    {
        //        currentFriend.Name = friend.Name;
        //    }
        //}

        //public void PostFriend(string id, string friendId, FriendProfile friend)
        //{
        //    var profile = Docs.Load<Profile>(id);
        //    if (profile == null) return;

        //    var currentFriend = profile.Friends.SingleOrDefault(f => f.Id == friendId);
        //    if (currentFriend == null)
        //    {
        //        friend.Id = friendId;
        //        profile.Friends.Add(friend);
        //    }
        //}

        //public void DeleteFriend(string id, string friendId)
        //{
        //    var profile = Docs.Load<Profile>(id);
        //    if (profile == null) return;

        //    var currentFriend = profile.Friends.SingleOrDefault(f => f.Id == friendId);
        //    if (currentFriend != null)
        //    {
        //        profile.Friends.Remove(currentFriend);
        //    }
        //}


        public IQueryable<SelectedBook> GetReadingList(int id)
        {
            var profile = Docs.Load<Profile>("profiles" + id);
            return profile.ReadingList.AsQueryable();
        }

        public HttpResponseMessage<SelectedBook> GetBookFromReadingList(int id, int bookId)
        {
            var profile = Docs.Load<Profile>("profiles" + id);
            if (profile == null) return new HttpResponseMessage<SelectedBook>(HttpStatusCode.NotFound);

            return new HttpResponseMessage<SelectedBook>(profile.ReadingList.SingleOrDefault(b => b.Id == "books" + bookId));
        }

        public HttpResponseMessage UpdateBookRating(int id, int bookId, int rating)
        {
            // make sure rating is from 0 - 5
            rating = rating < 0 ? 0 : rating > 5 ? 5 : rating;

            var profile = Docs.Load<Profile>("profiles/" + id);
            if (profile == null) return new HttpResponseMessage(HttpStatusCode.NotFound);

            var selectedBook = profile.ReadingList.SingleOrDefault(f => f.Id == "books/" + bookId);

            if (selectedBook == null)
                return new HttpResponseMessage(HttpStatusCode.NotFound);

            selectedBook.Rating = rating;
            return new HttpResponseMessage(HttpStatusCode.OK);
        }

        public HttpResponseMessage<SelectedBook> AddBookToReadingList(int id, SelectedBook bookToAdd)
        {
            var profile = Docs.Load<Profile>("profiles/" + id);
            if (profile == null) return new HttpResponseMessage<SelectedBook>(HttpStatusCode.NotFound);

            var book = Docs.Load<Book>(bookToAdd.Id);
            if (book == null) return new HttpResponseMessage<SelectedBook>(HttpStatusCode.NotFound);

            var selectedBook = profile.ReadingList.SingleOrDefault(b => b.Id == bookToAdd.Id);
            if (selectedBook == null)
            {
                selectedBook = new SelectedBook
                                   {
                                       Id = book.Id,
                                       Title = book.Title,
                                       Rating = bookToAdd.Rating
                                   };
                profile.ReadingList.Add(selectedBook);
            }

            return new HttpResponseMessage<SelectedBook>(selectedBook);
        }

        public HttpResponseMessage RemoveBookFromReadingList(int id, int bookId)
        {
            var profile = Docs.Load<Profile>("profiles/" + id);
            if (profile == null) return new HttpResponseMessage(HttpStatusCode.NotFound);

            var selectedBook = profile.ReadingList.SingleOrDefault(b => b.Id == "books/" + bookId);
            if (selectedBook != null)
            {
                profile.ReadingList.Remove(selectedBook);
            }

            return new HttpResponseMessage(HttpStatusCode.OK);
        }
    }
}

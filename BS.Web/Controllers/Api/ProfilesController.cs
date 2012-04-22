using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Principal;
using System.Web.Http.Hosting;
using Kaiser.BiggerShelf.Web.Infrastructure.Raven;
using Kaiser.BiggerShelf.Web.Models;

namespace Kaiser.BiggerShelf.Web.Controllers.Api
{
    public class ProfilesController : RavenApiController
    {
        public HttpResponseMessage<Profile> Get(RavenId<Profile> id)
        {
            return id.ToString() != GetUser().Id
                       ? new HttpResponseMessage<Profile>(HttpStatusCode.Forbidden)
                       : new HttpResponseMessage<Profile>(Docs.Load<Profile>(id.ToString()));
        }

        private Profile GetUser()
        {
            var userName = ((IPrincipal) Request.Properties[HttpPropertyKeys.UserPrincipalKey]).Identity.Name;
            return Docs.Query<Profile>().Single(p => p.AspNetUserGuid == userName);
        }

        public IQueryable<SelectedBook> GetBooks(RavenId<Profile> id)
        {
            var profile = Docs.Load<Profile>(id.ToString());
            return profile.ReadingList.AsQueryable();
        }

        public HttpResponseMessage<SelectedBook> GetBook(RavenId<Profile> id, RavenId<Book> bookId)
        {
            var profile = Docs.Load<Profile>(id.ToString());
            if (profile == null) return new HttpResponseMessage<SelectedBook>(HttpStatusCode.NotFound);

            return new HttpResponseMessage<SelectedBook>(profile.ReadingList.SingleOrDefault(b => b.Id == bookId.ToString()));
        }

        public HttpResponseMessage RemoveBook(RavenId<Profile> id, RavenId<Book> bookId)
        {
            var profile = Docs.Load<Profile>(id.ToString());
            if (profile == null) return new HttpResponseMessage(HttpStatusCode.NotFound);

            var selectedBook = profile.ReadingList.SingleOrDefault(b => b.Id == bookId.ToString());
            if (selectedBook != null)
            {
                profile.ReadingList.Remove(selectedBook);
            }

            return new HttpResponseMessage(HttpStatusCode.OK);
        }

        public HttpResponseMessage<SelectedBook> UpdateBook(RavenId<Profile> id, RavenId<Book> bookId, int rating = 0)
        {
            var profile = Docs.Load<Profile>(id.ToString());
            if (profile == null) return new HttpResponseMessage<SelectedBook>(HttpStatusCode.NotFound);

            var book = Docs.Load<Book>(bookId.ToString());
            if (book == null) return new HttpResponseMessage<SelectedBook>(HttpStatusCode.NotFound);

            var selectedBook = profile.ReadingList.SingleOrDefault(b => b.Id == bookId.ToString());

            if (selectedBook == null)
            {
                selectedBook = new SelectedBook
                               {
                                   Id = book.Id,
                                   Title = book.Title
                               };
                profile.ReadingList.Add(selectedBook);
            }
            
            selectedBook.Rating = NormalizeRating(rating);

            return new HttpResponseMessage<SelectedBook>(selectedBook, HttpStatusCode.OK);
        }

        private int NormalizeRating(int rating)
        {
            return rating < 0 ? 0 : rating > 5 ? 5 : rating;
        }
    }
}

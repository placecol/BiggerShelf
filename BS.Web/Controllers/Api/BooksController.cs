using System.Linq;
using Kaiser.BiggerShelf.Web.Infrastructure.Raven;
using Kaiser.BiggerShelf.Web.Models;

namespace Kaiser.BiggerShelf.Web.Controllers.Api
{
    public class BooksController : RavenApiController
    {
        public IQueryable<Book> Get()
        {
            return Docs.Query<Book>();
        }

        //public Book Get(RavenId<Book> id)
        //{
        //    return Docs.Load<Book>(id.ToString());
        //}
    }
}

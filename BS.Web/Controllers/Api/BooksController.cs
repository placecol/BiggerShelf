using System.Linq;
using Kaiser.BiggerShelf.Web.Models;

namespace Kaiser.BiggerShelf.Web.Controllers.Api
{
    public class BooksController : RavenApiController
    {
        public IQueryable<Book> Get()
        {
            return Docs.Query<Book>();
        }

        //public Book Get(int id)
        //{
        //    return Docs.Load<Book>("books/" + id);
        //}
    }
}

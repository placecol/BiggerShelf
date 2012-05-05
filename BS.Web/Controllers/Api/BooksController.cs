using System.Linq;
using Kaiser.BiggerShelf.Web.Infrastructure.Raven;
using Kaiser.BiggerShelf.Web.Models;
using Kaiser.BiggerShelf.Web.Models.Indexes;
using Raven.Client.Linq;

namespace Kaiser.BiggerShelf.Web.Controllers.Api
{
    public class BooksController : RavenApiController
    {
        public enum BookOrder
        {
            Default,
            PublishDate,
            Title,
            Rating
        }

        public IQueryable<Book> Get(string profileIds = null, string search = null, BookOrder orderBy = BookOrder.Default, int skip = 0, int take = 128)
        {
            var query = Docs
                .Query<Profile_ByBookTitle.Result, Profile_ByBookTitle>();

            if (!string.IsNullOrWhiteSpace(profileIds))
                query = query.Search(r => r.ProfileIds, string.Join(" ", profileIds.Split(',')), options:SearchOptions.And);

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Search(r => r.Query, search, options:SearchOptions.And);


            query = (IRavenQueryable<Profile_ByBookTitle.Result>)
                    query
                        .Skip(skip)
                        .Take(take);

            switch (orderBy)
            {
                case BookOrder.Default:
                    query = (IRavenQueryable<Profile_ByBookTitle.Result>)
                            query.OrderByDescending(b => b.Rating)
                                .ThenByDescending(b => b.PublishDate);
                    break;
                case BookOrder.Title:
                    query = query.OrderBy(b => b.Title);
                    break;
                case BookOrder.PublishDate:
                    query = query.OrderByDescending(b => b.PublishDate);
                    break;
                case BookOrder.Rating:
                    query = query.OrderByDescending(b => b.Rating);
                    break;
            }

            return query.AsProjection<Book>();
        }
    }
}

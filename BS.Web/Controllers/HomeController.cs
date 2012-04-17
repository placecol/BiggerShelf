using System.Linq;
using System.Web.Mvc;
using Kaiser.BiggerShelf.Web.Models;

namespace Kaiser.BiggerShelf.Web.Controllers
{
    [Authorize]
    public class HomeController : RavenController
    {
        public ActionResult Index()
        {
            var user = GetUser();
            return View(user);
        }

        private Profile GetUser()
        {
            var userName = User.Identity.Name;
            return Docs.Query<Profile>().Single(p => p.AspNetUserGuid == userName);
        }
    }
}
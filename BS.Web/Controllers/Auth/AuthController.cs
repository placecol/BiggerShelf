using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using Kaiser.BiggerShelf.Web.Infrastructure.Raven;
using Kaiser.BiggerShelf.Web.Models;
using Kaiser.BiggerShelf.Web.Models.Views;
using Raven.Client;

namespace Kaiser.BiggerShelf.Web.Controllers.Auth
{
    public class AuthController : Controller
    {
        private readonly IDocumentSession db = DocumentStoreHolder.Store.OpenSession();

        public ActionResult LogIn()
        {
            return View();
        }

        [HttpPost]
        public ActionResult LogIn(Login model, string returnUrl)
        {
            var helper = new HtmlHelper<object>(new ViewContext(), new ViewPage());

            Models.Profile profile;
            if (!AreCredentialsValid(model.Email, model.Password, out profile))
            {
                ModelState.AddModelError(string.Empty, "Incorrect username or password");
                return View();
            }

            FormsAuthentication.SetAuthCookie(profile.AspNetUserGuid, model.Persistent);
            return Redirect(returnUrl ?? "~/");
        }

        public ActionResult LogOut()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }

        private bool AreCredentialsValid(string email, string password, out Profile profile)
        {
            profile = db.Query<Profile>().FirstOrDefault(p => p.Email == (email).Trim());

            if (profile == null) return false;

            return profile.TryPassword(password);
        }
    }
}

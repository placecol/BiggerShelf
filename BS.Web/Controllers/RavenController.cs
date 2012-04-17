using System.Web.Mvc;
using Raven.Client;

namespace Kaiser.BiggerShelf.Web.Controllers
{
    public class RavenController : Controller
    {
        protected IDocumentSession Docs { get; set; }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            Docs = (IDocumentSession)HttpContext.Items["CurrentRequestRavenSession"];
            base.OnActionExecuting(filterContext);
        }
    }
}
using System.Web;
using System.Web.Http;
using Raven.Client;

namespace Kaiser.BiggerShelf.Web.Controllers.Api
{
    public class RavenApiController : ApiController
    {
        public IDocumentSession Docs { get; set; }

        public override System.Threading.Tasks.Task<System.Net.Http.HttpResponseMessage> ExecuteAsync(System.Web.Http.Controllers.HttpControllerContext controllerContext, System.Threading.CancellationToken cancellationToken)
        {
            Docs = (IDocumentSession) HttpContext.Current.Items["CurrentRequestRavenSession"];
            return base.ExecuteAsync(controllerContext, cancellationToken);
        }
    }
}
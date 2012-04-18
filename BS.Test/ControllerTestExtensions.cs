using System;
using System.Net.Http;
using System.Security.Principal;
using System.Web.Http.Hosting;
using Kaiser.BiggerShelf.Web.Controllers.Api;

namespace Kaiser.BiggerShelf.Test
{
    public static class ControllerTestExtensions
    {
        public static void ExecuteAction<TController>(this IControllerTest test, object [] dbObjects = null, string loggedInUserName = "me", Action<TController> action = null) where TController : RavenApiController
        {
            test.ExecuteInStore(
                dbObjects: dbObjects,
                action: store =>
                            {
                                using (var session = store.OpenSession())
                                {
                                    var controller = Activator.CreateInstance<TController>();
                                    var request = new HttpRequestMessage();
                                    request.Properties[HttpPropertyKeys.UserPrincipalKey] =
                                        new GenericPrincipal(new GenericIdentity(loggedInUserName), null);
                                    controller.Request = request;
                                    controller.Docs = session;
                                    action(controller);
                                }
                            });
        }
    }
}

using System;
using System.Net.Http;
using System.Security.Principal;
using System.Web.Http.Hosting;
using Kaiser.BiggerShelf.Web.Controllers.Api;
using Raven.Client;
using Raven.Client.Embedded;

namespace Kaiser.BiggerShelf.Test
{
    public static class Exec
    {
        public static void InStore(object[] dbObjects = null, Action<IDocumentStore> action = null)
        {
            using (var store = new EmbeddableDocumentStore())
            {
                store.Configuration.RunInMemory = true;
                store.Configuration.RunInUnreliableYetFastModeThatIsNotSuitableForProduction = true;
                store.Initialize();

                using (var session = store.OpenSession())
                {
                    foreach (var obj in dbObjects)
                    {
                        session.Store(obj);
                    }
                    session.SaveChanges();
                }

                action(store);
            }
        }

        public static void Action<TController>(object[] dbObjects = null, string loggedInUserName = "me", Action<TController> action = null) where TController : RavenApiController
        {
            InStore(
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

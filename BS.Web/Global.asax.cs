using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Kaiser.BiggerShelf.Web.Infrastructure.Api;
using Kaiser.BiggerShelf.Web.Infrastructure.Raven;
using Raven.Client;

namespace Kaiser.BiggerShelf.Web
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : HttpApplication
    {
        public MvcApplication()
        {
            BeginRequest +=
                (sender, args) =>
                    {
                        HttpContext.Current.Items["CurrentRequestRavenSession"] =
                            DocumentStoreHolder.Store.OpenSession();
                    };
            EndRequest += (sender, args) =>
                              {
                                  using (
                                      var session =
                                          (IDocumentSession) HttpContext.Current.Items["CurrentRequestRavenSession"])
                                  {
                                      if (session == null) return;
                                      if (Server.GetLastError() != null) return;
                                      session.SaveChanges();
                                  }
                              };
        }

        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            //routes.MapHttpRoute(
            //    name: "DefaultApi",
            //    routeTemplate: "api/{controller}/{id}",
            //    defaults: new { id = RouteParameter.Optional }
            //);

            routes.MapHttpRoute("GetBooks", "api/books",
                                new {controller = "Books", action = "Get"});
            routes.MapHttpRoute("GetProfile", "api/profiles/{id}",
                                new { controller = "Profiles", action = "Get" });
            routes.MapHttpRoute("GetBooksForProfile", "api/profiles/{id}/books",
                                new {controller = "Profiles", action = "GetBooks"});
            routes.MapHttpRoute("GetBookForProfile", "api/profiles/{id}/books/{bookId}",
                                new {controller = "Profiles", action = "GetBook"},
                                new {httpMethod = new HttpMethodConstraint(new[] {"GET"})});
            routes.MapHttpRoute("UpdateBookForProfile", "api/profiles/{id}/books/{bookId}",
                                new {controller = "Profiles", action = "UpdateBook"},
                                new {httpMethod = new HttpMethodConstraint(new[] {"PUT"})});
            routes.MapHttpRoute("RemoveBookFromProfile", "api/profiles/{id}/books/{bookId}",
                                new {controller = "Profiles", action = "RemoveBook"},
                                new {httpMethod = new HttpMethodConstraint(new[] {"DELETE"})});

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);

            BundleTable.Bundles.RegisterTemplateBundles();

            RegisterBinders();
        }

        private void RegisterBinders()
        {
            IEnumerable<object> modelBinderProviderServices = GlobalConfiguration.Configuration.ServiceResolver.GetServices(typeof(ModelBinderProvider));
            List<Object> services = new List<object>();
            services.Add(new RavenIdBinderProvider());
            services.AddRange(modelBinderProviderServices);
            GlobalConfiguration.Configuration.ServiceResolver.SetServices(typeof(ModelBinderProvider), services.ToArray());
        }
    }
}
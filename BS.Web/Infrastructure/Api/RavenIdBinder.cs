using System;
using System.Linq;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using System.Web.Http.ModelBinding;
using Kaiser.BiggerShelf.Web.Infrastructure.Raven;

namespace Kaiser.BiggerShelf.Web.Infrastructure.Api
{
    public class RavenIdBinder : IModelBinder
    {
        public bool BindModel(HttpActionContext actionContext, ModelBindingContext bindingContext)
        {
            var type = bindingContext.ModelType.GetGenericArguments().FirstOrDefault();
            if (type == null) return false;

            var hilo = bindingContext.
                ValueProvider.
                GetValue(bindingContext.ValidationNode.ModelStateKey).
                AttemptedValue;
            var prefix = DocumentStoreHolder.Store.Conventions.FindTypeTagName(type).ToLower();

            bindingContext.Model = Activator.CreateInstance(bindingContext.ModelType, prefix, hilo);

            return true;
        }
    }
}
using System.Web.Http.Controllers;
using System.Web.Http.ModelBinding;
using Kaiser.BiggerShelf.Web.Infrastructure.Raven;

namespace Kaiser.BiggerShelf.Web.Infrastructure.Api
{
    public class RavenIdBinderProvider : ModelBinderProvider
    {
        private readonly RavenIdBinder binder = new RavenIdBinder();

        public override IModelBinder GetBinder(HttpActionContext actionContext, ModelBindingContext bindingContext)
        {
            if (bindingContext.ModelType.BaseType != typeof(RavenId) ||
                !bindingContext.ModelType.IsGenericType)
                return null;

            return binder;
        }
    }
}
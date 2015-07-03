using Umbraco.Core;
using Umbraco.Core.Models.PublishedContent;

namespace Umbraco.CodeGen.Umbraco
{
    public class Bootstrap : ApplicationEventHandler
    {
        protected override void ApplicationStarting(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            base.ApplicationStarting(umbracoApplication, applicationContext);

            PublishedContentModelFactoryResolver.Current.SetFactory(new PublishedContentModelFactory(new[]{ typeof(PublishedContentModel) }));


        }

    }
}

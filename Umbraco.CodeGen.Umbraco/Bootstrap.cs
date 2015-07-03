using System;
using Lucene.Net.Search.Function;
using Umbraco.Core;
using Umbraco.Core.Events;
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Core.Services;

namespace Umbraco.CodeGen.Umbraco
{
    public class Bootstrap : ApplicationEventHandler
    {
        protected override void ApplicationStarting(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            base.ApplicationStarting(umbracoApplication, applicationContext);

            PublishedContentModelFactoryResolver.Current.SetFactory(new PublishedContentModelFactory(new[]{ typeof(PublishedContentModel) }));

            ContentTypeService.SavedContentType += GenerateContentType;
        }

        private void GenerateContentType(IContentTypeService sender, SaveEventArgs<IContentType> e)
        {
            
        }
    }
}

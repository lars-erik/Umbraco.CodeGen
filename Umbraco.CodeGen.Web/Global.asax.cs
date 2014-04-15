using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Reflection;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;
using System.Web.Mvc;
using System.Web.Routing;
using Lucene.Net.Documents;
using Umbraco.CodeGen.Web.App_Start;
using Umbraco.Core;
using Umbraco.Core.Persistence;
using Umbraco.Core.PropertyEditors;
using Umbraco.Core.Services;
using Umbraco.Web;
using Umbraco.Web.Models.ContentEditing;
using Umbraco.Web.PropertyEditors;

namespace Umbraco.CodeGen.Web
{
    public class Global : System.Web.HttpApplication
    {
        public Global()
        {
            BeginRequest += OnBeginRequest;
        }

        private void OnBeginRequest(object sender, EventArgs e)
        {
            UmbracoContext.EnsureContext(
                new HttpContextWrapper(HttpContext.Current),
                ApplicationContext.EnsureContext(
                    new DatabaseContext(new FakeDatabaseFactory()),
                    new ServiceContext(null, null, null, null, null, null, null, null, null, null, null, null, null),
                    new CacheHelper(),
                    true
                    )
                );
        }

        protected void Application_Start(object sender, EventArgs e)
        {
            RouteConfig.ApplyRoutes(RouteTable.Routes);

            GlobalConfiguration.Configuration.Services.Replace(
                typeof(IHttpControllerSelector),
                new ControllerSelectorWrapper((IHttpControllerSelector)GlobalConfiguration.Configuration.Services.GetService(typeof(IHttpControllerSelector)))
                );

            Integration.Configuration.Load();

            PropertyEditorResolver.Current = new PropertyEditorResolver(
                () => new List<Type>
                {
                    typeof(CheckBoxListPropertyEditor),
                    typeof(RichTextPropertyEditor),
                    typeof(IntegerPropertyEditor)
                }
            );

            AutoMapper.Mapper.CreateMap<PropertyEditor, PropertyEditorBasic>();

            Type.GetType("Umbraco.Core.ObjectResolution.Resolution, Umbraco.Core")
                .GetMethod("Freeze", BindingFlags.Static | BindingFlags.Public)
                .Invoke(null, new object[0]);
        }

    }

    internal class FakeDatabaseFactory : IDatabaseFactory
    {
        public void Dispose()
        {
        }

        public UmbracoDatabase CreateDatabase()
        {
            return null;
        }
    }

    public class ControllerSelectorWrapper : IHttpControllerSelector
    {
        private readonly IHttpControllerSelector inner;

        public ControllerSelectorWrapper(IHttpControllerSelector inner)
        {
            this.inner = inner;
        }

        public HttpControllerDescriptor SelectController(HttpRequestMessage request)
        {
            var httpControllerDescriptor = inner.SelectController(request);
            return httpControllerDescriptor;
        }

        public IDictionary<string, HttpControllerDescriptor> GetControllerMapping()
        {
            var httpControllerDescriptors = inner.GetControllerMapping();
            return httpControllerDescriptors;
        }
    }
}
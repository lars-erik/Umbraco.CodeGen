using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Umbraco.CodeGen.Configuration;
using Umbraco.CodeGen.Generators;
using Umbraco.Core;
using Umbraco.Core.Events;
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Core.Services;

namespace Umbraco.CodeGen.Umbraco
{
    public class Bootstrap : ApplicationEventHandler
    {
        private ModelGenerator generator;
        
        private CodeGeneratorConfiguration configuration;
        private IEnumerable<Type> types;

        protected override void ApplicationStarting(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            Initialize();
            ListenForContentTypeSaves();
            SetModelFactory();
        }

        private void Initialize()
        {
            LoadConfiguration();
            InitializeGenerator();
            FindModelTypes();
        }

        private void ListenForContentTypeSaves()
        {
            ContentTypeService.SavedContentType += ContentTypeSaved;
        }

        public void ContentTypeSaved(IContentTypeService service, SaveEventArgs<IContentType> args)
        {
            foreach(var contentType in args.SavedEntities)
                generator.GenerateModelAndDependants(service, contentType);
        }

        private void SetModelFactory()
        {
            PublishedContentModelFactoryResolver.Current.SetFactory(new PublishedContentModelFactory(types));
        }

        private void FindModelTypes()
        {
            var namespaces = new[] {configuration.DocumentTypes.Namespace, configuration.MediaTypes.Namespace}.Distinct();
            types = AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes().Where(t => namespaces.Contains(t.Namespace) && !t.IsInterface));
        }

        private void InitializeGenerator()
        {
            var generatorFactory = CodeGeneratorFactory.CreateFactory<CodeGeneratorFactory>(configuration.GeneratorFactory);
            var interfaceGeneratorFactory = CodeGeneratorFactory.CreateFactory<CodeGeneratorFactory>(configuration.InterfaceFactory);
            var dataTypeProvider = new UmbracoDataTypesProvider();
            var paths = new Dictionary<string, string>
            {
                {"DocumentType", HttpContext.Current.Server.MapPath(configuration.DocumentTypes.ModelPath)},
                {"MediaType", HttpContext.Current.Server.MapPath(configuration.MediaTypes.ModelPath)}
            };

            generator = new ModelGenerator(
                configuration,
                generatorFactory,
                interfaceGeneratorFactory,
                dataTypeProvider,
                paths
                );
        }

        private void LoadConfiguration()
        {
            var configurationProvider = new CodeGeneratorConfigurationFileProvider(HttpContext.Current.Server.MapPath("~/config/CodeGen.config"));
            configuration = configurationProvider.GetConfiguration();
        }
    }
}

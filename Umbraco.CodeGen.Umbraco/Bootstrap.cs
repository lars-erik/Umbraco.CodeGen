using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Umbraco.CodeGen.Configuration;
using Umbraco.CodeGen.Factories;
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
        
        private GeneratorConfig configuration;
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
            {
                // TODO: Find content type from ModelsBuilder
                //generator.GenerateModelAndDependents(null);
            }
        }

        private void SetModelFactory()
        {
            PublishedContentModelFactoryResolver.Current.SetFactory(new PublishedContentModelFactory(types));
        }

        private void FindModelTypes()
        {
            var namespaces = new[] {configuration.Namespace, configuration.Namespace}.Distinct();
            types = AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes().Where(t => namespaces.Contains(t.Namespace) && !t.IsInterface));
        }

        private void InitializeGenerator()
        {
            var generatorFactory = CodeGeneratorFactory.CreateFactory(configuration.GeneratorFactory);
            var interfaceGeneratorFactory = CodeGeneratorFactory.CreateFactory(configuration.InterfaceFactory);

            generator = new ModelGenerator(
                configuration,
                generatorFactory,
                interfaceGeneratorFactory
                );
        }

        private void LoadConfiguration()
        {
            configuration = GeneratorConfig.FromModelsBuilder();
        }
    }
}

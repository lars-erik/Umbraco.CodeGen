using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Umbraco.CodeGen.Configuration;
using Umbraco.CodeGen.Definitions;
using Umbraco.CodeGen.Generators;
using Umbraco.Core;
using Umbraco.Core.Logging;
using Umbraco.Core.Models;
using Umbraco.Core.Services;
using ContentType = Umbraco.CodeGen.Definitions.ContentType;

namespace Umbraco.CodeGen.Umbraco
{
    public class ModelGenerator
    {
        private readonly CodeGeneratorConfiguration configuration;
        private readonly CodeGeneratorFactory generatorFactory;
        private readonly CodeGeneratorFactory interfaceGeneratorFactory;
        private readonly UmbracoDataTypesProvider dataTypeProvider;
        private readonly IDictionary<string, string> paths;

        public ModelGenerator(
            CodeGeneratorConfiguration configuration, 
            CodeGeneratorFactory generatorFactory, 
            CodeGeneratorFactory interfaceGeneratorFactory,
            UmbracoDataTypesProvider dataTypeProvider, 
            IDictionary<string, string> paths)
        {
            this.configuration = configuration;
            this.generatorFactory = generatorFactory;
            this.interfaceGeneratorFactory = interfaceGeneratorFactory;
            this.dataTypeProvider = dataTypeProvider;
            this.paths = paths;
        }

        public void GenerateModelAndDependants(IContentTypeService service, IContentTypeComposition umbracoContentType)
        {
            var contentType = ContentTypeMapping.Map(umbracoContentType);
            var allContentTypes = service.GetAllContentTypes().ToList();

            GenerateDependencies(umbracoContentType, allContentTypes);

            var composedOfThis = GetDependants(umbracoContentType, allContentTypes);

            var isMixin = composedOfThis.Any();
            
            if (isMixin)
            {
                contentType.IsMixin = true;
            }

            GenerateClass(contentType);

            if (isMixin)
            {
                GenerateInterface(contentType);
                GenerateDependants(composedOfThis);
            }
        }

        private void GenerateDependants(IEnumerable<IContentType> composedOfThis)
        {
            foreach (var umbracoCompositeType in composedOfThis)
            {
                var compositeType = ContentTypeMapping.Map(umbracoCompositeType);
                GenerateClass(compositeType);
            }
        }

        private static List<IContentType> GetDependants(IContentTypeComposition umbracoContentType, IEnumerable<IContentType> allContentTypes)
        {
            var id = umbracoContentType.Id;
            var composedOfThis = allContentTypes
                .Where(ct => ct.CompositionIds().Contains(id) && ct.ParentId != id)
                .ToList();
            return composedOfThis;
        }

        private void GenerateDependencies(IContentTypeComposition umbracoContentType, IEnumerable<IContentType> allContentTypes)
        {
            var compositionIds = umbracoContentType.CompositionIds();
            var thisIsComposedOf = allContentTypes
                .Where(ct => compositionIds.Contains(ct.Id) && umbracoContentType.ParentId != ct.Id)
                .ToList();

            foreach (var composition in thisIsComposedOf)
            {
                var compositionType = ContentTypeMapping.Map(composition);
                compositionType.IsMixin = true;
                GenerateClass(compositionType);
                GenerateInterface(compositionType);
            }
        }

        private void GenerateInterface(ContentType contentType)
        {
            GenerateModel(contentType, interfaceGeneratorFactory, c => "I" + c.Alias.PascalCase());
        }

        private void GenerateClass(ContentType contentType)
        {
            GenerateModel(contentType, generatorFactory, c => c.Alias.PascalCase());
        }

        private void GenerateModel(ContentType contentType, CodeGeneratorFactory specificGeneratorFactory, Func<ContentType, string> fileNameGetter)
        {
            var typeConfig = contentType is DocumentType
                ? configuration.DocumentTypes
                : configuration.MediaTypes;

            if (!typeConfig.GenerateClasses)
                return;

            var itemStart = DateTime.Now;

            LogHelper.Debug<CodeGenerator>(
                () => String.Format("Content type {0} saved, generating typed model", contentType.Name));
            LogHelper.Info<CodeGenerator>(() => String.Format("Generating typed model for {0}", contentType.Alias));

            var modelPath = EnsureModelPath(typeConfig);
            var path = GetPath(modelPath, fileNameGetter(contentType));
            RemoveReadonly(path);

            var classGenerator = new CodeGenerator(typeConfig.Namespace, specificGeneratorFactory);
            using (var stream = System.IO.File.CreateText(path))
            { 
                WriteGeneratedClass(contentType, classGenerator, stream);

            }

            LogHelper.Debug<CodeGenerator>(
                () => String.Format("Typed model for {0} generated. Took {1}", contentType.Alias, DateTime.Now - itemStart));
        }

        private static void WriteGeneratedClass(ContentType contentType, CodeGenerator classGenerator, StreamWriter stream)
        {
            try
            {
                classGenerator.Generate(contentType, stream);
            }
            catch (Exception ex)
            {
                stream.WriteLine();
                stream.WriteLine("#warning " + ex.Message);
            }
        }

        private static string GetPath(string modelPath, string fileName)
        {
            return Path.Combine(modelPath, fileName + ".cs");
        }

        private void RemoveReadonly(string path)
        {
            if (configuration.OverwriteReadOnly && System.IO.File.Exists(path))
                System.IO.File.SetAttributes(path, System.IO.File.GetAttributes(path) & ~FileAttributes.ReadOnly);
        }

        private string EnsureModelPath(ContentTypeConfiguration typeConfig)
        {
            var modelPath = paths[typeConfig.ContentTypeName];
            if (!Directory.Exists(modelPath))
                Directory.CreateDirectory(modelPath);
            return modelPath;
        }
    }
}

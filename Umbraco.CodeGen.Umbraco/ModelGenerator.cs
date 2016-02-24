using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Umbraco.CodeGen.Configuration;
using Umbraco.CodeGen.Definitions;
using Umbraco.CodeGen.Factories;
using Umbraco.CodeGen.Generators;
using Umbraco.Core;
using Umbraco.Core.Logging;
using Umbraco.Core.Models;
using Umbraco.Core.Services;
using Umbraco.ModelsBuilder.Building;

namespace Umbraco.CodeGen.Umbraco
{
    public class ModelGenerator
    {
        private readonly GeneratorConfig configuration;
        private readonly CodeGeneratorFactory generatorFactory;
        private readonly CodeGeneratorFactory interfaceGeneratorFactory;

        public ModelGenerator(
            GeneratorConfig configuration,
            CodeGeneratorFactory generatorFactory,
            CodeGeneratorFactory interfaceGeneratorFactory)
        {
            this.configuration = configuration;
            this.generatorFactory = generatorFactory;
            this.interfaceGeneratorFactory = interfaceGeneratorFactory;
        }

        public void GenerateModelAndDependents(TypeModel model)
        {
            var composedOfThis = GetDependents(model);

            GenerateClass(model);

            if (model.IsMixin)
            {
                GenerateInterface(model);
            }

            if (model.IsMixin || model.IsRenamed)
                GenerateDependants(composedOfThis);
        }

        private void GenerateDependants(IEnumerable<TypeModel> composedOfThis)
        {
            foreach (var composite in composedOfThis)
            {
                GenerateClass(composite);
            }
        }

        private static List<TypeModel> GetDependents(TypeModel model)
        {
            return new List<TypeModel>();
            // TODO: Find dependents in TypeModel list
        }

        private void GenerateInterface(TypeModel contentType)
        {
            GenerateModel(contentType, interfaceGeneratorFactory, c => "I" + c.Alias.PascalCase());
        }

        private void GenerateClass(TypeModel contentType)
        {
            GenerateModel(contentType, generatorFactory, c => c.Alias.PascalCase());
        }

        private void GenerateModel(TypeModel contentType, CodeGeneratorFactory specificGeneratorFactory, Func<TypeModel, string> fileNameGetter)
        {
            var itemStart = DateTime.Now;

            LogHelper.Debug<CodeGenerator>(
                () => String.Format("Content type {0} saved, generating typed model", contentType.Name));
            LogHelper.Info<CodeGenerator>(() => String.Format("Generating typed model for {0}", contentType.Alias));

            var modelPath = EnsureModelPath(configuration.ModelsPath);
            var path = GetPath(modelPath, fileNameGetter(contentType));

            var classGenerator = new CodeGenerator(configuration, specificGeneratorFactory);
            using (var stream = System.IO.File.CreateText(path))
            {
                WriteGeneratedClass(contentType, classGenerator, stream);

            }

            LogHelper.Debug<CodeGenerator>(
                () => String.Format("Typed model for {0} generated. Took {1}", contentType.Alias, DateTime.Now - itemStart));
        }

        private static void WriteGeneratedClass(TypeModel contentType, CodeGenerator classGenerator, StreamWriter stream)
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

        private string EnsureModelPath(string modelPath)
        {
            if (!Directory.Exists(modelPath))
                Directory.CreateDirectory(modelPath);
            return modelPath;
        }
    }
}

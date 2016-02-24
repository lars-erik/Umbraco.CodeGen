using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.CodeGen.Configuration;
using Umbraco.CodeGen.Factories;
using Umbraco.CodeGen.Generators;
using Umbraco.Core.Configuration;
using Umbraco.ModelsBuilder.Building;
using Umbraco.ModelsBuilder.Configuration;

namespace Umbraco.CodeGen
{
    public class CodeDomTextBuilder : TextBuilderBase
    {
        private const string ConfigPrefix = "Umbraco.ModelsBuilder.Contrib.";
        private static readonly GeneratorConfig Config = GeneratorConfig.FromModelsBuilder();
        private readonly CodeGeneratorFactory classFactory;
        private readonly CodeGeneratorFactory interfaceFactory;

        public CodeDomTextBuilder(IList<TypeModel> typeModels, ParseResult parseResult) : 
            this(typeModels, parseResult, Config.Namespace)
        {
        }

        public CodeDomTextBuilder(IList<TypeModel> typeModels, ParseResult parseResult, string modelsNamespace) : base(typeModels, parseResult, modelsNamespace)
        {
            Config.Namespace = modelsNamespace;

            // TODO: Bother to put factory name on config?
            Config.GeneratorFactory = ConfigurationManager.AppSettings[ConfigPrefix + "GeneratorFactory"];
            if (String.IsNullOrWhiteSpace(Config.GeneratorFactory))
            {
                Config.GeneratorFactory = typeof (SimpleModelGeneratorFactory).FullName;
            }

            classFactory = CodeGeneratorFactory.CreateFactory(Config.GeneratorFactory);
            // TODO: Decide interfacefactory
        }

        public override void Generate(StringBuilder sb, TypeModel typeModel)
        {
            var codeGenerator = new CodeGenerator(Config, classFactory);
            codeGenerator.Generate(typeModel, new StringWriter(sb));
        }
    }
}

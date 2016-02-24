using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.CodeGen.Configuration;
using Umbraco.CodeGen.Generators;
using Umbraco.Core.Configuration;
using Umbraco.ModelsBuilder.Building;
using Umbraco.ModelsBuilder.Configuration;

namespace Umbraco.CodeGen
{
    public class CodeDomTextBuilder : TextBuilderBase
    {
        private readonly GeneratorConfig config = GeneratorConfig.FromModelsBuilder();

        public CodeDomTextBuilder(IList<TypeModel> typeModels, ParseResult parseResult) : base(typeModels, parseResult)
        {
        }

        public CodeDomTextBuilder(IList<TypeModel> typeModels, ParseResult parseResult, string modelsNamespace) : base(typeModels, parseResult, modelsNamespace)
        {
            config.Namespace = modelsNamespace;
        }

        public override void Generate(StringBuilder sb, TypeModel typeModel)
        {
            var classFactory = new SimpleModelGeneratorFactory();
            var codeGenerator = new CodeGenerator(config, classFactory);
            codeGenerator.Generate(typeModel, new StringWriter(sb));
        }
    }
}

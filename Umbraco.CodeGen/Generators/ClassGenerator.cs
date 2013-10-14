using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Umbraco.CodeGen.Configuration;
using Umbraco.CodeGen.Definitions;

namespace Umbraco.CodeGen.Generators
{
    public class ClassGenerator : CodeGeneratorBase
    {
        private readonly CodeGeneratorBase entityDescriptionGenerator;
        private readonly CodeGeneratorBase[] memberGenerators;

        public ClassGenerator(
            ContentTypeConfiguration config, 
            CodeGeneratorBase entityDescriptionGenerator,
            params CodeGeneratorBase[] memberGenerators
            )
            : base(config)
        {
            this.entityDescriptionGenerator = entityDescriptionGenerator;
            this.memberGenerators = memberGenerators;
        }

        public override void Generate(CodeObject codeObject, Entity entity)
        {
            var contentType = (ContentType) entity;
            var info = contentType.Info;

            var ns = (CodeNamespace)codeObject;
            var type = new CodeTypeDeclaration();
            ns.Types.Add(type);

            entityDescriptionGenerator.Generate(type, info);
            SetBaseClass(type, info);

            if (memberGenerators != null)
                foreach(var generator in memberGenerators)
                    generator.Generate(type, contentType);
        }

        protected void SetBaseClass(CodeTypeDeclaration type, Info info)
        {
            var baseReference = String.IsNullOrWhiteSpace(info.Master)
                                    ? new CodeTypeReference(Config.BaseClass)
                                    : new CodeTypeReference(info.Master.PascalCase());
            type.BaseTypes.Add(baseReference);
        }
    }
}

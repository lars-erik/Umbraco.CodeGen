using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Umbraco.CodeGen.Configuration;
using Umbraco.CodeGen.Definitions;

namespace Umbraco.CodeGen.Generators
{
    public class ClassGenerator : CompositeCodeGenerator
    {
        public ClassGenerator(
            ContentTypeConfiguration config, 
            params CodeGeneratorBase[] memberGenerators
            )
            : base(config, memberGenerators)
        {
        }

        public override void Generate(object codeObject, Entity entity)
        {
            var contentType = (ContentType) entity;
            var info = contentType.Info;

            var ns = (CodeNamespace)codeObject;
            var type = new CodeTypeDeclaration();
            ns.Types.Add(type);

            SetPartial(type);
            SetBaseClass(type, info);
            AddInterfaces(type, contentType);

            base.Generate(type, entity);
        }

        private void SetPartial(CodeTypeDeclaration type)
        {
            type.IsPartial = true;
        }

        protected void SetBaseClass(CodeTypeDeclaration type, Info info)
        {
            var baseReference = String.IsNullOrWhiteSpace(info.Master)
                                    ? new CodeTypeReference(Config.BaseClass)
                                    : new CodeTypeReference(info.Master.PascalCase());
            type.BaseTypes.Add(baseReference);
        }

        private void AddInterfaces(CodeTypeDeclaration type, ContentType contentType)
        {
            if (contentType.IsMixin)
                type.BaseTypes.Add((new CodeTypeReference("I" + contentType.Alias.PascalCase())));

            foreach (var composition in contentType.Composition)
                type.BaseTypes.Add((new CodeTypeReference("I" + composition.Alias.PascalCase())));
        }
    }
}

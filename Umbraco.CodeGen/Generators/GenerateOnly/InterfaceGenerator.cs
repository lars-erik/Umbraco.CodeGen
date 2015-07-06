using System;
using System.CodeDom;
using Umbraco.CodeGen.Configuration;
using Umbraco.CodeGen.Definitions;

namespace Umbraco.CodeGen.Generators.GenerateOnly
{
    public class InterfaceGenerator : CompositeCodeGenerator
    {
        public InterfaceGenerator(ContentTypeConfiguration config, params CodeGeneratorBase[] generators) : base(config, generators)
        {
        }

        public override void Generate(object codeObject, Entity entity)
        {
            var ns = (CodeNamespace)codeObject;
            var type = new CodeTypeDeclaration();
            ns.Types.Add(type);

            type.IsInterface = true;
            type.IsPartial = true;
            type.BaseTypes.Add(new CodeTypeReference("IPublishedContent"));

            base.Generate(type, entity);
        }
    }
}

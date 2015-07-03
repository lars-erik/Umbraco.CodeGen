using System.CodeDom;
using Umbraco.CodeGen.Configuration;
using Umbraco.CodeGen.Definitions;

namespace Umbraco.CodeGen.Generators.BaseSupportedAnnotated
{
    public class ImportsGenerator : CodeGeneratorBase
    {
        public ImportsGenerator(ContentTypeConfiguration config)
            : base(config)
        {
        }

        public override void Generate(object codeObject, Entity entity)
        {
            var ns = (CodeNamespace)codeObject;
            AddImports(ns);
        }

        private static void AddImports(CodeNamespace ns)
        {
            ns.Imports.Add(new CodeNamespaceImport("global::System"));
            ns.Imports.Add(new CodeNamespaceImport("global::Umbraco.CodeGen.Annotations"));
        }
    }
}

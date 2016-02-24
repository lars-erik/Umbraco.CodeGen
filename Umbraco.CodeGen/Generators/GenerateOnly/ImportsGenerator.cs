using System.CodeDom;
using Umbraco.CodeGen.Configuration;
using Umbraco.CodeGen.Definitions;

namespace Umbraco.CodeGen.Generators.GenerateOnly
{
    public class ImportsGenerator : CodeGeneratorBase
    {
        public ImportsGenerator(Configuration.GeneratorConfig config) : base(config)
        {
        }

        public override void Generate(object codeObject, object typeOrPropertyModel)
        {
            var ns = (CodeNamespace) codeObject;
            AddImports(ns);
        }

        private static void AddImports(CodeNamespace ns)
        {
            ns.Imports.Add(new CodeNamespaceImport("global::System"));
            ns.Imports.Add(new CodeNamespaceImport("global::Umbraco.Core.Models"));
            ns.Imports.Add(new CodeNamespaceImport("global::Umbraco.Web"));
        }
    }
}

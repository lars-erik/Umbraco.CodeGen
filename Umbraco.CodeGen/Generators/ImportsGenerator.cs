using System.CodeDom;
using Umbraco.CodeGen.Definitions;

namespace Umbraco.CodeGen.Generators
{
    public class ImportsGenerator : CodeGeneratorBase
    {
        public ImportsGenerator(ContentTypeConfiguration config) : base(config)
        {
        }

        public override void Generate(CodeObject codeObject, Entity entity)
        {
            var ns = (CodeNamespace) codeObject;
            AddImports(ns);
        }

        private static void AddImports(CodeNamespace ns)
        {
            ns.Imports.Add(new CodeNamespaceImport("System"));
            ns.Imports.Add(new CodeNamespaceImport("System.ComponentModel"));
            ns.Imports.Add(new CodeNamespaceImport("System.ComponentModel.DataAnnotations"));
            ns.Imports.Add(new CodeNamespaceImport("Umbraco.Core.Models"));
            ns.Imports.Add(new CodeNamespaceImport("Umbraco.Web"));
        }
    }
}

using System;
using System.CodeDom;
using Umbraco.CodeGen.Configuration;
using Umbraco.CodeGen.Definitions;

namespace Umbraco.CodeGen.Generators
{
    public class NamespaceGenerator : CodeGeneratorBase
    {
        private readonly string configuredNamespace;
        private readonly CodeGeneratorBase[] memberGenerators;

        public NamespaceGenerator(
            string configuredNamespace,
            params CodeGeneratorBase[] memberGenerators
            ) : base(null)
        {
            this.configuredNamespace = configuredNamespace;
            this.memberGenerators = memberGenerators;
        }

        public override void Generate(object codeObject, object typeOrPropertyModel)
        {
            CodeNamespace ns;
            if (codeObject is CodeCompileUnit)
                ns = AddToCompileUnit(codeObject);
            else
            { 
                ns = (CodeNamespace) codeObject;
                ns.Name = configuredNamespace;
            }

            foreach(var generator in memberGenerators)
                generator.Generate(ns, typeOrPropertyModel);
        }

        private CodeNamespace AddToCompileUnit(object codeObject)
        {
            var compileUnit = (CodeCompileUnit) codeObject;

            if (String.IsNullOrWhiteSpace(configuredNamespace))
                throw new Exception("Namespace not configured.");

            var ns = new CodeNamespace(configuredNamespace);
            compileUnit.Namespaces.Add(ns);
            return ns;
        }
    }
}

using System;
using System.CodeDom;
using Umbraco.CodeGen.Configuration;
using Umbraco.CodeGen.Definitions;

namespace Umbraco.CodeGen.Generators
{
    public class NamespaceGenerator : CodeGeneratorBase
    {
        private readonly Configuration.GeneratorConfig configuration;
        private readonly CodeGeneratorBase[] memberGenerators;

        public NamespaceGenerator(
            Configuration.GeneratorConfig configuration,
            params CodeGeneratorBase[] memberGenerators
            ) : base(null)
        {
            this.configuration = configuration;
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
                ns.Name = configuration.Namespace;
            }

            foreach(var generator in memberGenerators)
                generator.Generate(ns, typeOrPropertyModel);
        }

        private CodeNamespace AddToCompileUnit(object codeObject)
        {
            var compileUnit = (CodeCompileUnit) codeObject;

            if (String.IsNullOrWhiteSpace(configuration.Namespace))
                throw new Exception("Namespace not configured.");

            var ns = new CodeNamespace(configuration.Namespace);
            compileUnit.Namespaces.Add(ns);
            return ns;
        }
    }
}

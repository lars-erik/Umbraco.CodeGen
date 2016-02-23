using System;
using System.CodeDom;
using Umbraco.CodeGen.Configuration;
using Umbraco.CodeGen.Definitions;

namespace Umbraco.CodeGen.Generators
{
    public class NamespaceGenerator : CodeGeneratorBase
    {
        private readonly CodeGeneratorBase[] memberGenerators;

        public NamespaceGenerator(
            ContentTypeConfiguration config,
            params CodeGeneratorBase[] memberGenerators
            ) : base(config)
        {
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
                ns.Name = Config.Namespace;
            }

            foreach(var generator in memberGenerators)
                generator.Generate(ns, typeOrPropertyModel);
        }

        private CodeNamespace AddToCompileUnit(object codeObject)
        {
            var compileUnit = (CodeCompileUnit) codeObject;

            if (String.IsNullOrWhiteSpace(Config.Namespace))
                throw new Exception("ContentType namespace not configured.");

            var ns = new CodeNamespace(Config.Namespace);
            compileUnit.Namespaces.Add(ns);
            return ns;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.CodeGen.Configuration;
using Umbraco.CodeGen.Generators.GenerateOnly;

namespace Umbraco.CodeGen.Generators
{
    public class InterfaceGeneratorFactory : CodeGeneratorFactory
    {
        public override CodeGeneratorBase Create(string configuredNamespace)
        {
            return CreateInterfaceGenerator(configuredNamespace);
        }

        public CodeGeneratorBase CreateInterfaceGenerator(string configuredNamespace)
        {
            return new NamespaceGenerator(
                configuredNamespace,
                new ImportsGenerator(null),
                new InterfaceGenerator(null,
                    new CompositeCodeGenerator(
                        null,
                        new InterfaceNameGenerator(null)
                        ),
                    new PropertiesGenerator(
                        null,
                        new InterfacePropertyDeclarationGenerator(
                            null,
                            null,
                            new EntityNameGenerator(null),
                            new InterfacePropertyBodyGenerator(null)
                            )
                        )
                    )
                );
        }
    }
}

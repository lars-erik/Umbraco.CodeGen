using System.Collections.Generic;
using System.Linq;
using Umbraco.CodeGen.Configuration;
using Umbraco.CodeGen.Generators.GenerateOnly;

namespace Umbraco.CodeGen.Generators
{
    public class GenerateOnlyGeneratorFactory : CodeGeneratorFactory
    {
        public override CodeGeneratorBase Create(string configuredNamespaces)
        {
            return CreateGenerators(configuredNamespaces);
        }

        private static CodeGeneratorBase CreateGenerators(
            string configuredNamespace
            )
        {
            return new NamespaceGenerator(
                configuredNamespace,
                new ImportsGenerator(null),
                new ClassGenerator(null,
                    new CompositeCodeGenerator(
                        null,
                        new EntityNameGenerator(null)
                        ),
                    new CtorGenerator(null),
                    new PropertiesGenerator(
                        null,
                        new PublicPropertyDeclarationGenerator(
                            null,
                            null,
                            new EntityNameGenerator(null),
                            new PropertyBodyGenerator(null)
                            )
                        )
                    )
                );
        }
    }
}

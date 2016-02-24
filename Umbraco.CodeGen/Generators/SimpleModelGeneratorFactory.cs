using System.Collections.Generic;
using System.Linq;
using Umbraco.CodeGen.Configuration;
using Umbraco.CodeGen.Generators.GenerateOnly;

namespace Umbraco.CodeGen.Generators
{
    public class SimpleModelGeneratorFactory : CodeGeneratorFactory
    {
        public override CodeGeneratorBase Create(Configuration.GeneratorConfig configuration)
        {
            return CreateGenerators(configuration);
        }


        private static CodeGeneratorBase CreateGenerators(
            Configuration.GeneratorConfig configuration
            )
        {
            return new NamespaceGenerator(
                configuration,
                new ImportsGenerator(configuration),
                new ClassGenerator(configuration,
                    new CompositeCodeGenerator(
                        configuration,
                        new EntityNameGenerator(configuration)
                        ),
                    new CtorGenerator(configuration),
                    new PropertiesGenerator(
                        configuration,
                        new PublicPropertyDeclarationGenerator(
                            configuration,
                            new EntityNameGenerator(configuration),
                            new PropertyBodyGenerator(configuration)
                            )
                        )
                    )
                );
        }
    }
}

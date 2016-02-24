using Umbraco.CodeGen.Generators;
using Umbraco.CodeGen.Generators.Compositions;

namespace Umbraco.CodeGen.Factories
{
    public class SimpleModelGeneratorFactory : CodeGeneratorFactory
    {
        public override CodeGeneratorBase Create(Configuration.GeneratorConfig config)
        {
            return new NamespaceGenerator(config,
                new ImportsGenerator(config),
                new SimpleClassGenerator(config,
                    new PropertiesGenerator(config,
                        new SimplePropertyGenerator(config)
                    )
                )
            );
        }
    }
}

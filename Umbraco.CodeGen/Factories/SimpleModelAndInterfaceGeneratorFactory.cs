using Umbraco.CodeGen.Configuration;
using Umbraco.CodeGen.Generators;
using Umbraco.CodeGen.Generators.Compositions;

namespace Umbraco.CodeGen.Factories
{
    public class SimpleModelAndInterfaceGeneratorFactory : CodeGeneratorFactory
    {
        public override CodeGeneratorBase Create(GeneratorConfig config)
        {
            return new NamespaceGenerator(config,
                
                new ImportsGenerator(config),
                
                new SimpleInterfaceGenerator(config,
                    new PropertiesGenerator(config,
                        new InterfacePropertyGenerator(config)
                    )
                ),
                
                new SimpleClassGenerator(config,
                    new PropertiesGenerator(config,
                        new SimplePropertyGenerator(config)
                    )
                )
            );

        }
    }
}

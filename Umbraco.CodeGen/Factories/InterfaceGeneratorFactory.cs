using Umbraco.CodeGen.Generators;
using Umbraco.CodeGen.Generators.Compositions;

namespace Umbraco.CodeGen.Factories
{
    public class InterfaceGeneratorFactory : CodeGeneratorFactory
    {
        public override CodeGeneratorBase Create(Configuration.GeneratorConfig config)
        {
            return CreateInterfaceGenerator(config);
        }

        public CodeGeneratorBase CreateInterfaceGenerator(Configuration.GeneratorConfig configuration)
        {
            return new NamespaceGenerator(
                configuration,
                new ImportsGenerator(configuration),
                new SimpleInterfaceGenerator(configuration,
                    new PropertiesGenerator(
                        configuration,
                        new InterfacePropertyGenerator(configuration)
                    )
                )
            );
        }
    }
}

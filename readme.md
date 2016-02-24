# Umbraco ModelsBuilder Contrib

CodeDom based generation of types with ModelsBuilder.  
Completely extendable and modifyable on all levels.

Generated classes are clean, partial, all virtual for painless extension.

For now ignores all ModelsBuilder logic for parsing existing partials.

For now not possible to swap factory, but configuration imminent.

Example generator configuration:

    public class SimpleModelGeneratorFactory : CodeGeneratorFactory
    {
        public override CodeGeneratorBase Create(Configuration.GeneratorConfig configuration)
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
using System.Collections.Generic;
using System.Linq;
using Umbraco.CodeGen.Configuration;
using Umbraco.CodeGen.Generators.GenerateOnly;

namespace Umbraco.CodeGen.Generators
{
    public class GenerateOnlyGeneratorFactory : CodeGeneratorFactory
    {
        public override CodeGeneratorBase Create(ContentTypeConfiguration configuration, IEnumerable<DataTypeDefinition> dataTypes)
        {
            if (configuration.ContentTypeName == "DocumentType")
                return CreateDocTypeGenerator(configuration, dataTypes);
            return CreateMediaTypeGenerator(configuration, dataTypes);
        }

        public CodeGeneratorBase CreateDocTypeGenerator(ContentTypeConfiguration configuration, IEnumerable<DataTypeDefinition> dataTypes)
        {
            return CreateGenerators(
                configuration,
                dataTypes
                );
        }

        public CodeGeneratorBase CreateMediaTypeGenerator(ContentTypeConfiguration configuration, IEnumerable<DataTypeDefinition> dataTypes)
        {
            return CreateGenerators(
                configuration,
                dataTypes
                );
        }

        private static CodeGeneratorBase CreateGenerators(
            ContentTypeConfiguration configuration,
            IEnumerable<DataTypeDefinition> dataTypes
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
                            dataTypes.ToList(),
                            new EntityNameGenerator(configuration),
                            new PropertyBodyGenerator(configuration)
                            )
                        )
                    )
                );
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Umbraco.CodeGen.Configuration;

namespace Umbraco.CodeGen.Generators
{
    public class DefaultCodeGeneratorFactory : CodeGeneratorFactory
    {
        public override CodeGeneratorBase Create(ContentTypeConfiguration configuration, IEnumerable<DataTypeDefinition> dataTypes)
        {
            if (configuration.ContentTypeName == "DocumentType")
                return CreateDocumentTypeGenerator(configuration, dataTypes);
            return CreateMediaTypeGenerator(configuration, dataTypes);
        }

        private CodeGeneratorBase CreateDocumentTypeGenerator(ContentTypeConfiguration configuration, IEnumerable<DataTypeDefinition> dataTypes)
        {
            return CreateGenerators(configuration, new DocumentTypeInfoGenerator(configuration), dataTypes);
        }

        private CodeGeneratorBase CreateMediaTypeGenerator(ContentTypeConfiguration configuration, IEnumerable<DataTypeDefinition> dataTypes)
        {
            return CreateGenerators(configuration, new CommonInfoGenerator(configuration), dataTypes);
        }

        private CodeGeneratorBase CreateGenerators(ContentTypeConfiguration c, CodeGeneratorBase infoGenerator, IEnumerable<DataTypeDefinition> dataTypes)
        {
            return new NamespaceGenerator(c,
                new ImportsGenerator(c),
                new ClassGenerator(c,
                    new EntityDescriptionGenerator(c),
                    new CtorGenerator(c),
                    infoGenerator,
                    new StructureGenerator(c),
                    new PropertiesGenerator(c,
                        new PropertyDeclarationGenerator(c, dataTypes.ToList(), new EntityDescriptionGenerator(c)),
                        new PropertyBodyGenerator(c)
                        )
                    )
                );
        }
    }
}

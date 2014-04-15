using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Umbraco.CodeGen.Configuration;
using Umbraco.CodeGen.Generators.Bcl;
using Umbraco.CodeGen.Parsers;

namespace Umbraco.CodeGen.Generators
{
    [Parser(typeof(BclParserFactory))]
    [Description("Models only depend on the BCL and uses attributes from amongst others, the ComponentModel namespace.")]
    public class BclCodeGeneratorFactory : CodeGeneratorFactory
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
                        new PropertyInfoGenerator(c, dataTypes.ToList(), new EntityDescriptionGenerator(c)),
                        new PropertyBodyGenerator(c)
                        )
                    )
                );
        }
    }
}

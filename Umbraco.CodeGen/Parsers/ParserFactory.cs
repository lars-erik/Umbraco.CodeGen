using System.Collections.Generic;
using System.Linq;

namespace Umbraco.CodeGen.Parsers
{
    public class ParserFactory
    {
        private ContentTypeConfiguration config;
        private IList<DataTypeDefinition> types;

        public ContentTypeCodeParser Create(
            ContentTypeConfiguration configuration,
            IEnumerable<DataTypeDefinition> dataTypes 
        )
        {
            config = configuration;
            types = dataTypes as List<DataTypeDefinition> ?? dataTypes.ToList();
            var typedParser = configuration.ContentTypeName == "DocumentType" 
                ? CreateDocumentTypeParser() 
                : CreateMediaTypeParser();
            return typedParser;
        }

        private ContentTypeCodeParser CreateMediaTypeParser()
        {
            var parsers = CreateParsers(new CommonInfoParser(config));
            return new MediaTypeCodeParser(config, parsers.ToArray());
        }

        private ContentTypeCodeParser CreateDocumentTypeParser()
        {
            var parsers = CreateParsers(new DocumentTypeInfoParser(config));
            return new DocumentTypeCodeParser(config, parsers.ToArray());
        }

        private List<ContentTypeCodeParserBase> CreateParsers(CommonInfoParser infoParser)
        {
            var parsers = CreateParsers();
            parsers.Insert(0, infoParser);
            return parsers;
        }

        private List<ContentTypeCodeParserBase> CreateParsers()
        {
            return new List<ContentTypeCodeParserBase>
            {
                new StructureParser(config),
                new PropertiesParser(config,
                    new PropertyParser(config, types)
                ),
                new TabsParser(config)
            };
        }
    }
}

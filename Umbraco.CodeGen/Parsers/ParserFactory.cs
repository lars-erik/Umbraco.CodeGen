using System.Collections.Generic;

namespace Umbraco.CodeGen.Parsers
{
    public class ParserFactory
    {
        public ContentTypeCodeParser Create(ContentTypeConfiguration configuration)
        {
            ContentTypeCodeParser typedParser;
            if (configuration.ContentTypeName == "DocumentType")
                typedParser = CreateDocumentTypeParser(configuration);
            else
                typedParser = CreateMediaTypeParser(configuration);
            return typedParser;
        }

        private ContentTypeCodeParser CreateMediaTypeParser(ContentTypeConfiguration configuration)
        {
            var parsers = CreateParsers(configuration);
            parsers.Insert(0, new CommonInfoParser(configuration));
            return new MediaTypeCodeParser(
                configuration,
                parsers.ToArray()
            );
        }

        private ContentTypeCodeParser CreateDocumentTypeParser(ContentTypeConfiguration configuration)
        {
            var parsers = CreateParsers(configuration);
            parsers.Insert(0, new DocumentTypeInfoParser(configuration));
            return new DocumentTypeCodeParser(
                configuration,
                parsers.ToArray()
            );
        }

        private List<ContentTypeCodeParserBase> CreateParsers(ContentTypeConfiguration configuration)
        {
            return new List<ContentTypeCodeParserBase>
            {
                new StructureParser(configuration)
            };
        }
    }
}

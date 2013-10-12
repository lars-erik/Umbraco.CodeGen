using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
            return new MediaTypeCodeParser(
                configuration,
                new CommonInfoParser(configuration)
                );
        }

        private ContentTypeCodeParser CreateDocumentTypeParser(ContentTypeConfiguration configuration)
        {
            return new DocumentTypeCodeParser(
                configuration,
                new DocumentTypeInfoParser(configuration)
                );
        }
    }
}

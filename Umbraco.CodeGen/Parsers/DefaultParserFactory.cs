using System.Collections.Generic;
using Umbraco.CodeGen.Configuration;
using Umbraco.CodeGen.Parsers.Bcl;

namespace Umbraco.CodeGen.Parsers
{
    public class DefaultParserFactory : ParserFactory
    {
        private readonly ParserFactory inner = new AnnotatedParserFactory();

        public override ContentTypeCodeParser Create(ContentTypeConfiguration configuration, IEnumerable<DataTypeDefinition> dataTypes)
        {
            return inner.Create(configuration, dataTypes);
        }

        public override ContentTypeCodeParser CreateDocumentTypeParser()
        {
            return inner.CreateDocumentTypeParser();
        }

        public override ContentTypeCodeParser CreateMediaTypeParser()
        {
            return inner.CreateMediaTypeParser();
        }
    }
}

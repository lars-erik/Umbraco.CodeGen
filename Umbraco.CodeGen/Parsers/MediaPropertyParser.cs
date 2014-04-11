using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ICSharpCode.NRefactory.CSharp;
using Umbraco.CodeGen.Configuration;
using Umbraco.CodeGen.Definitions;

namespace Umbraco.CodeGen.Parsers
{
    public class MediaPropertyParser : ContentTypeCodeParserBase
    {
        private readonly ContentTypeCodeParserBase propertyParser;
        private readonly IEnumerable<DataTypeDefinition> dataTypes;

        public MediaPropertyParser(ContentTypeCodeParserBase propertyParser, ContentTypeConfiguration configuration, IEnumerable<DataTypeDefinition> dataTypes) : 
            base(configuration)
        {
            this.propertyParser = propertyParser;
            this.dataTypes = dataTypes;
        }

        public override void Parse(AstNode node, ContentType contentType)
        {
            propertyParser.Parse(node, contentType);

            var property = contentType.GenericProperties.Last();
            var dataTypeDefinition = dataTypes.Single(dt => String.Equals(dt.DataTypeId, property.Type, StringComparison.InvariantCultureIgnoreCase));
            property.Type = dataTypeDefinition.DataTypeGuid;
        }
    }
}

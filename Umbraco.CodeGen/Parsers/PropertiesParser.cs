using ICSharpCode.NRefactory.CSharp;
using Umbraco.CodeGen.Configuration;
using Umbraco.CodeGen.Definitions;

namespace Umbraco.CodeGen.Parsers
{
    public class PropertiesParser : ContentTypeCodeParserBase
    {
        private readonly ContentTypeCodeParserBase propertyParser;

        public PropertiesParser(
            ContentTypeConfiguration configuration,
            ContentTypeCodeParserBase propertyParser
            ) : base(configuration)
        {
            this.propertyParser = propertyParser;
        }

        public override void Parse(AstNode node, ContentType contentType)
        {
            var type = (TypeDeclaration) node;
            var props = FindProperties(type);
            foreach(var prop in props)
                propertyParser.Parse(prop, contentType);
        }
    }
}
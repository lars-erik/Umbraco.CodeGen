using ICSharpCode.NRefactory.CSharp;
using Umbraco.CodeGen.Definitions;

namespace Umbraco.CodeGen.Parsers
{
    public class PropertyParser : ContentTypeCodeParserBase
    {
        public PropertyParser(ContentTypeConfiguration configuration) : base(configuration)
        {
        }

        public override void Parse(AstNode node, ContentType contentType)
        {
            var propNode = (PropertyDeclaration) node;
            var property = new GenericProperty
            {
                Alias = propNode.Name.CamelCase(),
                Name = AttributeValue(propNode, "DisplayName", propNode.Name.SplitPascalCase())
            };
            contentType.GenericProperties.Add(property);
        }
    }
}
using System.Linq;
using ICSharpCode.NRefactory.CSharp;
using Umbraco.CodeGen.Configuration;
using Umbraco.CodeGen.Definitions;

namespace Umbraco.CodeGen.Parsers
{
    public class StructureParser : ContentTypeCodeParserBase
    {
        public StructureParser(ContentTypeConfiguration configuration) : base(configuration)
        {
        }

        public override void Parse(AstNode node, ContentType contentType)
        {
            var type = (TypeDeclaration) node;

            contentType.Structure = TypeArrayValue(type, "Structure")
                .Select(val => val.CamelCase())
                .ToList();
        }
    }
}

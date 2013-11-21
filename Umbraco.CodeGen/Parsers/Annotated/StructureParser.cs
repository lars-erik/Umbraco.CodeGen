using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSharpCode.NRefactory.CSharp;
using Umbraco.CodeGen.Configuration;
using Umbraco.CodeGen.Definitions;
using Attribute = ICSharpCode.NRefactory.CSharp.Attribute;

namespace Umbraco.CodeGen.Parsers.Annotated
{
    public class StructureParser : ContentTypeCodeParserBase
    {
        public StructureParser(ContentTypeConfiguration configuration) : base(configuration)
        {
        }

        public override void Parse(AstNode node, ContentType contentType)
        {
            var type = (TypeDeclaration) node;

            var attribute = FindContentTypeAttribute(type, contentType);
            
            contentType.Structure = TypeArrayValue(attribute, "Structure")
                .Select(val => val.CamelCase())
                .ToList();
        }

        // TODO: Dry up
        protected static Attribute FindContentTypeAttribute(TypeDeclaration type, ContentType definition)
        {
            string attributeName;
            if (definition is MediaType)
                attributeName = "MediaType";
            else
                attributeName = "DocumentType";
            var attribute = FindAttribute(type.Attributes, attributeName);
            return attribute;
        }
    }
}

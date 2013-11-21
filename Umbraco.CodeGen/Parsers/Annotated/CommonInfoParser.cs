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
    public class CommonInfoParser : InfoParserBase
    {
        public CommonInfoParser(ContentTypeConfiguration configuration) : base(configuration)
        {
        }

        protected override void OnParseInfo(TypeDeclaration type, ContentType definition)
        {
            // TODO: Dry this
            var info = definition.Info;
            info.Alias = type.Name.PascalCase();
            info.Master = FindMaster(type, Configuration).PascalCase();

            var attribute = FindContentTypeAttribute(type, definition);

            info.Name = AttributeArgumentValue(attribute, "DisplayName", type.Name.SplitPascalCase());
            info.Description = AttributeArgumentValue<string>(attribute, "Description", null);
            info.AllowAtRoot = AttributeArgumentValue(attribute, "AllowAtRoot", false);
            info.Icon = AttributeArgumentValue(attribute, "Icon", "folder.gif");
            info.Thumbnail = AttributeArgumentValue(attribute, "Thumbnail", "folder.png");
        }

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

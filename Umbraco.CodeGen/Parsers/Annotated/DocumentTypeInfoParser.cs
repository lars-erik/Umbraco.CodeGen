using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSharpCode.NRefactory.CSharp;
using Umbraco.CodeGen.Configuration;
using Umbraco.CodeGen.Definitions;

namespace Umbraco.CodeGen.Parsers.Annotated
{
    public class DocumentTypeInfoParser : CommonInfoParser
    {
        public DocumentTypeInfoParser(ContentTypeConfiguration configuration) : base(configuration)
        {
        }

        protected override void OnParseInfo(TypeDeclaration type, ContentType definition)
        {
            base.OnParseInfo(type, definition);

            var docType = (DocumentType)definition;
            var info = (DocumentTypeInfo)docType.Info;
            var attribute = FindAttribute(type.Attributes, "DocumentType");

            info.DefaultTemplate = AttributeArgumentValue<string>(attribute, "DefaultTemplate", null);
            info.AllowedTemplates = StringArrayValue(attribute, "AllowedTemplates").ToList();
        }
    }
}

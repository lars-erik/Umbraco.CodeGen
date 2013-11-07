using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Umbraco.CodeGen.Configuration;
using Umbraco.CodeGen.Definitions;

namespace Umbraco.CodeGen.Generators.Annotated
{
    public class DocumentTypeInfoGenerator : CodeGeneratorBase
    {
        public DocumentTypeInfoGenerator(ContentTypeConfiguration config) : base(config)
        {
        }

        public override void Generate(object codeObject, Entity entity)
        {
            var attribute = codeObject as CodeAttributeDeclaration;
            if (attribute == null)
                throw new Exception("Common info generator must be used on an attribute declaration");

            var contentType = (ContentType)entity;
            var info = (DocumentTypeInfo)contentType.Info;

            AddAttributeArgumentIfValue(attribute, "DefaultTemplate", info.DefaultTemplate);

            var allowedTemplates = 
                info.AllowedTemplates
                    .NonNullOrWhiteSpace()
                    .AsPrimitiveExpressions();
            var arrayCreateExpression = new CodeArrayCreateExpression("String", allowedTemplates);
            if (allowedTemplates.Any())
                AddAttributeArgument(attribute, "AllowedTemplates", arrayCreateExpression);
        }
    }
}

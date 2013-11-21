using System.CodeDom;
using System.Linq;
using Umbraco.CodeGen.Configuration;
using Umbraco.CodeGen.Definitions;

namespace Umbraco.CodeGen.Generators.Annotated
{
    public class DocumentTypeInfoGenerator : CommonInfoGenerator
    {
        public DocumentTypeInfoGenerator(ContentTypeConfiguration config) : base(config)
        {
        }

        public override void Generate(object codeObject, Entity entity)
        {
            base.Generate(codeObject, entity);

            var attribute = (CodeAttributeDeclaration)codeObject;
            var docType = (DocumentType)entity;
            var info = (DocumentTypeInfo)docType.Info;

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

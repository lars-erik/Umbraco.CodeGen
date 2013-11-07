using System;
using System.CodeDom;
using System.Linq;
using Umbraco.CodeGen.Configuration;
using Umbraco.CodeGen.Definitions;

namespace Umbraco.CodeGen.Generators.Bcl
{
    public class DocumentTypeInfoGenerator : CommonInfoGenerator
    {
        public DocumentTypeInfoGenerator(ContentTypeConfiguration config) : base(config)
        {
        }

        public override void Generate(object codeObject, Entity entity)
        {
            var contentType = (ContentType) entity;
            var info = (DocumentTypeInfo) contentType.Info;
            var type = (CodeTypeDeclaration) codeObject;

            base.Generate(type, contentType);

            AddFieldIfNotEmpty(type, "defaultTemplate", info.DefaultTemplate);
            AddAllowedTemplates(type, info);
        }

        private static void AddAllowedTemplates(CodeTypeDeclaration type, DocumentTypeInfo info)
        {
            if (info.AllowedTemplates.All(String.IsNullOrWhiteSpace))
                return;
            var field = new CodeMemberField(
                typeof (string[]),
                "allowedTemplates"
                );
            var expressions = 
                info.AllowedTemplates
                    .NonNullOrWhiteSpace()
                    .AsPrimitiveExpressions();
            field.InitExpression = new CodeArrayCreateExpression(
                typeof(string[]),
                expressions
                );
            type.Members.Add(field);
        }
    }
}

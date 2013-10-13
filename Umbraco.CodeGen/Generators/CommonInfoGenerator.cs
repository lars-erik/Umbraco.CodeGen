using System;
using System.CodeDom;
using Umbraco.CodeGen.Definitions;

namespace Umbraco.CodeGen.Generators
{
    public class CommonInfoGenerator
    {
        private const StringComparison IgnoreCase = StringComparison.OrdinalIgnoreCase;

        public CommonInfoGenerator(ContentTypeConfiguration config)
        {
            
        }

        public void Generate(CodeObject codeObject, ContentType contentType)
        {
            var type = (CodeTypeDeclaration) codeObject;
            var info = contentType.Info;

            // TODO: Consider moving these to base for both info & props
            ValidateAlias(info);
            SetAlias(type, info);
            AddDisplayNameIfDifferent(type, info);
            AddDescription(type, info);

        }

        private static void SetAlias(CodeTypeMember type, EntityDescription info)
        {
            type.Name = info.Alias.PascalCase();
        }

        private static void ValidateAlias(Info info)
        {
            if (String.IsNullOrWhiteSpace(info.Alias))
                throw new Exception("Cannot generate class with alias null or empty");
        }

        private static void AddDisplayNameIfDifferent(CodeTypeMember type, EntityDescription info)
        {
            if (String.Compare(info.Name, info.Alias, IgnoreCase) == 0 &&
                String.Compare(info.Name, info.Alias.SplitPascalCase(), IgnoreCase) == 0)
                return;
            type.CustomAttributes.Add(
                new CodeAttributeDeclaration(
                    "DisplayName",
                    new CodeAttributeArgument(
                        new CodePrimitiveExpression(info.Name)
                        )
                    )
                );
        }

        private void AddDescription(CodeTypeDeclaration type, Info info)
        {
            if (String.IsNullOrWhiteSpace(info.Description))
                return;


        }
    }
}

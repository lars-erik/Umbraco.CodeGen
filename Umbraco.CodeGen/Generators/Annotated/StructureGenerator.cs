using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Umbraco.CodeGen.Configuration;
using Umbraco.CodeGen.Definitions;

namespace Umbraco.CodeGen.Generators.Annotated
{
    public class StructureGenerator : CodeGeneratorBase
    {
        public StructureGenerator(ContentTypeConfiguration config) : base(config)
        {
        }

        public override void Generate(object codeObject, Entity entity)
        {
            var contentType = (ContentType) entity;
            var attribute = (CodeAttributeDeclaration) codeObject;

            var structure = contentType.Structure;
            if (structure.All(String.IsNullOrWhiteSpace))
                return;

            var typeofExpressions = 
                structure
                    .Where(allowedType => !String.IsNullOrWhiteSpace(allowedType))
                    .Select(allowedType => new CodeTypeOfExpression(allowedType.PascalCase()))
                    .Cast<CodeExpression>()
                    .ToArray();
            var expression = new CodeArrayCreateExpression(
                typeof(Type[]),
                typeofExpressions
                );

            AddAttributeArgument(attribute, "Structure", expression);
        }
    }
}

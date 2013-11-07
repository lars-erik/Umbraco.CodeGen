using System.CodeDom;
using System.Linq;

namespace Umbraco.CodeGen.Tests.Generators.Annotated
{
    public class AnnotationCodeGeneratorTestBase : CodeGeneratorTestBase
    {
        protected static object FindAttributeArgumentValue(CodeAttributeDeclaration attributeDeclaration, string attributeName)
        {
            var argument = FindAttributeArgument(attributeDeclaration, attributeName);
            var argValue = ((CodePrimitiveExpression)argument.Value).Value;
            return argValue;
        }

        protected static CodeAttributeArgument FindAttributeArgument(CodeAttributeDeclaration attributeDeclaration,
            string attributeName)
        {
            var argument = attributeDeclaration.Arguments
                .Cast<CodeAttributeArgument>()
                .SingleOrDefault(arg => arg.Name == attributeName);
            return argument;
        }
    }
}
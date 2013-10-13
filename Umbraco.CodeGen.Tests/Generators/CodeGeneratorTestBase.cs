using System.CodeDom;
using System.Linq;
using Umbraco.CodeGen.Generators;

namespace Umbraco.CodeGen.Tests.Generators
{
    public abstract class CodeGeneratorTestBase
    {
        protected CodeGeneratorBase Generator;
        protected CodeTypeMember Candidate;
        protected ContentTypeConfiguration Configuration;

        protected object FindAttributeValue(string attributeName)
        {
            var attribute = FindAttribute(attributeName);
            var value = ((CodePrimitiveExpression) attribute.Arguments[0].Value).Value;
            return value;
        }

        protected CodeAttributeDeclaration FindAttribute(string attributeName)
        {
            var attribute = Candidate.CustomAttributes.Cast<CodeAttributeDeclaration>().SingleOrDefault(att => att.Name == attributeName);
            return attribute;
        }
    }
}
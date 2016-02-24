using System.CodeDom;
using Umbraco.CodeGen.Configuration;

namespace Umbraco.CodeGen.Generators
{
    public class PublicPropertyDeclarationGenerator : PropertyDeclarationGenerator
    {
        public PublicPropertyDeclarationGenerator(Configuration.GeneratorConfig config, params CodeGeneratorBase[] memberGenerators) : base(config, memberGenerators)
        {
        }

        protected override void SetAttributes(CodeTypeMember propNode)
        {
            propNode.Attributes = MemberAttributes.Public;
        }
    }
}
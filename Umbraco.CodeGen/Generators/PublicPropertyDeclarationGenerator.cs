using System.CodeDom;
using Umbraco.CodeGen.Configuration;

namespace Umbraco.CodeGen.Generators
{
    public class PublicPropertyDeclarationGenerator : PropertyDeclarationGenerator
    {
        public PublicPropertyDeclarationGenerator(Configuration.GeneratorConfig config, params CodeGeneratorBase[] childGenerators) : base(config, childGenerators)
        {
        }

        protected override void SetAttributes(CodeTypeMember propNode)
        {
            propNode.Attributes = propNode.Attributes | MemberAttributes.Public;
        }
    }
}
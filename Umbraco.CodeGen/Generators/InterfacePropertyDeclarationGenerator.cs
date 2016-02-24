using System.CodeDom;

namespace Umbraco.CodeGen.Generators
{
    public class InterfacePropertyDeclarationGenerator : PropertyDeclarationGenerator
    {
        public InterfacePropertyDeclarationGenerator(Configuration.GeneratorConfig config, params CodeGeneratorBase[] generators) : base(config, generators)
        {
        }

        protected override void SetAttributes(CodeTypeMember propNode)
        {
            propNode.Attributes = MemberAttributes.Abstract;
        }
    }
}

using System.CodeDom;

namespace Umbraco.CodeGen.Generators
{
    public class InterfacePropertyBodyGenerator : CodeGeneratorBase
    {
        public InterfacePropertyBodyGenerator(Configuration.GeneratorConfig config) : base(config)
        {
        }

        public override void Generate(object codeObject, object typeOrPropertyModel)
        {
            var property = (CodeMemberProperty) codeObject;
            property.HasGet = true;
        }
    }
}

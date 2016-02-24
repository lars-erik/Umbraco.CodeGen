using System.CodeDom;
using Umbraco.CodeGen.Definitions;

namespace Umbraco.CodeGen.Generators
{
    public class InterfaceNameGenerator : NameGenerator
    {
        public InterfaceNameGenerator(Configuration.GeneratorConfig config) : base(config)
        {
        }

        protected override void SetName(CodeTypeMember type, IEntityDescription description)
        {
            type.Name = "I" + description.Alias.PascalCase();
        }
    }
}

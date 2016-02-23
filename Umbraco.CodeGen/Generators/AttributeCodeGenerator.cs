using System.CodeDom;
using Umbraco.CodeGen.Configuration;
using Umbraco.CodeGen.Definitions;

namespace Umbraco.CodeGen.Generators
{
    public class AttributeCodeGenerator : CodeGeneratorBase
    {
        private readonly string attributeName;
        private readonly CodeGeneratorBase[] memberGenerators;

        public AttributeCodeGenerator(
            string attributeName,
            ContentTypeConfiguration config,
            params CodeGeneratorBase[] memberGenerators
            ) : base(config)
        {
            this.attributeName = attributeName;
            this.memberGenerators = memberGenerators;
        }

        public override void Generate(object codeObject, object typeOrPropertyModel)
        {
            var type = (CodeTypeMember)codeObject;

            var attribute = AddAttribute(type, attributeName);

            if (memberGenerators != null)
                foreach(var generator in memberGenerators)
                    generator.Generate(attribute, typeOrPropertyModel);
        }
    }
}

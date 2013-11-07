using System;
using System.CodeDom;
using Umbraco.CodeGen.Configuration;
using Umbraco.CodeGen.Definitions;

namespace Umbraco.CodeGen.Generators.Annotated
{
    public class EntityDescriptionGenerator : CodeGeneratorBase
    {
        public EntityDescriptionGenerator(ContentTypeConfiguration config) : base(config)
        {
        }

        public override void Generate(object codeObject, Entity entity)
        {
            var description = (EntityDescription)entity;
            var attribute = (CodeAttributeDeclaration)codeObject;

            AddDisplayNameIfDifferent(attribute, description);
            AddAttributeArgumentIfValue(attribute, "Description", description.Description);
        }

        protected static void AddDisplayNameIfDifferent(CodeAttributeDeclaration attribute, EntityDescription description)
        {
            var name = description.Name;
            if (String.Compare(name, description.Alias, IgnoreCase) == 0 ||
                String.Compare(name, description.Alias.SplitPascalCase(), IgnoreCase) == 0)
                return;
            AddAttributePrimitiveArgument(attribute, "DisplayName", name);
        }
    }
}

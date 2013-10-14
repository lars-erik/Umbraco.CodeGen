using System;
using System.CodeDom;
using Umbraco.CodeGen.Configuration;
using Umbraco.CodeGen.Definitions;

namespace Umbraco.CodeGen.Generators
{
    public class EntityDescriptionGenerator : CodeGeneratorBase
    {
        public EntityDescriptionGenerator(ContentTypeConfiguration config) : base(config)
        {
        }

        public override void Generate(CodeObject codeObject, Entity entity)
        {
            var description = (EntityDescription) entity;
            var type = (CodeTypeMember)codeObject;

            ValidateAlias(description);
            SetName(type, description);
            AddDisplayNameIfDifferent(type, description);
            AddDescription(type, description);
        }

        protected static void ValidateAlias(EntityDescription description)
        {
            if (String.IsNullOrWhiteSpace(description.Alias))
                throw new Exception("Cannot generate entity with alias null or empty");
        }

        protected static void SetName(CodeTypeMember type, EntityDescription description)
        {
            type.Name = description.Alias.PascalCase();
        }

        protected static void AddDisplayNameIfDifferent(CodeTypeMember type, EntityDescription description)
        {
            var name = description.Name;
            if (String.Compare(name, description.Alias, IgnoreCase) == 0 ||
                String.Compare(name, description.Alias.SplitPascalCase(), IgnoreCase) == 0)
                return;
            AddAttribute(type, "DisplayName", name);
        }

        protected void AddDescription(CodeTypeMember type, EntityDescription description)
        {
            if (String.IsNullOrWhiteSpace(description.Description))
                return;

            AddAttribute(type, "Description", description.Description);
        }
    }
}
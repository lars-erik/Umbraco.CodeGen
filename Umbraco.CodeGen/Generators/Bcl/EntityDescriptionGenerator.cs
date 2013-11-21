using System;
using System.CodeDom;
using Umbraco.CodeGen.Configuration;
using Umbraco.CodeGen.Definitions;

namespace Umbraco.CodeGen.Generators.Bcl
{
    public class EntityDescriptionGenerator : EntityNameGenerator
    {
        public EntityDescriptionGenerator(ContentTypeConfiguration config) : base(config)
        {
        }

        public override void Generate(object codeObject, Entity entity)
        {
            base.Generate(codeObject, entity);

            var description = (IEntityDescription) entity;
            var type = (CodeTypeMember)codeObject;

            AddDisplayNameIfDifferent(type, description);
            AddDescription(type, description);
        }

        protected static void AddDisplayNameIfDifferent(CodeTypeMember type, IEntityDescription description)
        {
            var name = description.Name;
            if (String.Compare(name, description.Alias, IgnoreCase) == 0 ||
                String.Compare(name, description.Alias.SplitPascalCase(), IgnoreCase) == 0)
                return;
            AddAttribute(type, "DisplayName", name);
        }

        protected void AddDescription(CodeTypeMember type, IEntityDescription description)
        {
            if (String.IsNullOrWhiteSpace(description.Description))
                return;

            AddAttribute(type, "Description", description.Description);
        }
    }
}
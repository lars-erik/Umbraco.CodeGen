using System;
using System.CodeDom;
using Umbraco.CodeGen.Configuration;
using Umbraco.CodeGen.Definitions;

namespace Umbraco.CodeGen.Generators
{
    public class EntityNameGenerator : CodeGeneratorBase
    {
        public EntityNameGenerator(ContentTypeConfiguration config) : base(config)
        {
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

        public override void Generate(object codeObject, Entity entity)
        {
            var description = (EntityDescription)entity;
            var type = (CodeTypeMember)codeObject;

            ValidateAlias(description);
            SetName(type, description);
        }
    }
}
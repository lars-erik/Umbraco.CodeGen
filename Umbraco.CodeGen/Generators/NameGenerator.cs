using System;
using System.CodeDom;
using Umbraco.CodeGen.Configuration;
using Umbraco.CodeGen.Definitions;

namespace Umbraco.CodeGen.Generators
{
    public abstract class NameGenerator : CodeGeneratorBase
    {
        protected NameGenerator(ContentTypeConfiguration config) : base(config)
        {
        }

        protected static void ValidateAlias(IEntityDescription description)
        {
            if (String.IsNullOrWhiteSpace(description.Alias))
                throw new Exception("Cannot generate entity with alias null or empty");
        }

        protected abstract void SetName(CodeTypeMember type, IEntityDescription description);

        public override void Generate(object codeObject, Entity entity)
        {
            var description = (IEntityDescription)entity;
            var type = (CodeTypeMember)codeObject;

            ValidateAlias(description);
            SetName(type, description);
        }
    }
}
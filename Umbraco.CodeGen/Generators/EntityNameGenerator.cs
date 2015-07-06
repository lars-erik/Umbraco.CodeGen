using System.CodeDom;
using Umbraco.CodeGen.Configuration;
using Umbraco.CodeGen.Definitions;

namespace Umbraco.CodeGen.Generators
{
    public class EntityNameGenerator : NameGenerator
    {
        public EntityNameGenerator(ContentTypeConfiguration config) : base(config)
        {
        }

        protected override void SetName(CodeTypeMember type, IEntityDescription description)
        {
            type.Name = description.Alias.PascalCase();
        }
    }
}
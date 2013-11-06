using System.CodeDom;
using Umbraco.CodeGen.Configuration;
using Umbraco.CodeGen.Definitions;

namespace Umbraco.CodeGen.Generators.Bcl
{
    public class CommonInfoGenerator : CodeGeneratorBase
    {
        public CommonInfoGenerator(ContentTypeConfiguration config) : base(config)
        {
        }

        public override void Generate(CodeObject codeObject, Entity entity)
        {
            var type = (CodeTypeDeclaration)codeObject;
            var contentType = (ContentType) entity;
            var info = contentType.Info;

            AddFieldIfNotEmpty(type, "icon", info.Icon);
            AddFieldIfNotEmpty(type, "thumbnail", info.Thumbnail);
            AddFieldIfTrue(type, "allowAtRoot", info.AllowAtRoot);
        }
    }
}

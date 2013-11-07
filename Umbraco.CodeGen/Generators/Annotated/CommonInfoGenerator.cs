using System;
using System.CodeDom;
using Umbraco.CodeGen.Configuration;
using Umbraco.CodeGen.Definitions;

namespace Umbraco.CodeGen.Generators.Annotated
{
    public class CommonInfoGenerator : CodeGeneratorBase
    {
        public CommonInfoGenerator(ContentTypeConfiguration configuration)
            : base(configuration)
        {
            
        }

        public override void Generate(object codeObject, Entity entity)
        {
            var attribute = codeObject as CodeAttributeDeclaration;
            if (attribute == null)
                throw new Exception("Common info generator must be used on an attribute declaration");

            var contentType = (ContentType)entity;
            var info = contentType.Info;

            AddAttributeArgumentIfValue(attribute, "Icon", info.Icon);
            AddAttributeArgumentIfValue(attribute, "Thumbnail", info.Thumbnail);
            if (info.AllowAtRoot)
                AddAttributePrimitiveArgument(attribute, "AllowAtRoot", true);
        }
    }
}

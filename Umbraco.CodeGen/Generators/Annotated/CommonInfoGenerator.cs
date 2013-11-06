using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        public override void Generate(CodeObject codeObject, Entity entity)
        {
            var type = codeObject as CodeAttributeDeclaration;
            if (type == null)
                throw new Exception("Common info generator must be used on an attribute declaration");

            var contentType = (ContentType)entity;
            var info = contentType.Info;

            var attribute = type.CustomAttributes.Cast<CodeAttributeDeclaration>().Single();

            if (!String.IsNullOrWhiteSpace(info.Icon))
                attribute.Arguments.Add(new CodeAttributeArgument("Icon", new CodePrimitiveExpression(info.Icon)));
        }
    }
}

using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Umbraco.CodeGen.Definitions;

namespace Umbraco.CodeGen.Generators
{
    public class CtorGenerator : CodeGeneratorBase
    {
        public CtorGenerator(ContentTypeConfiguration config) : base(config)
        {
        }

        public override void Generate(CodeObject codeObject, Entity entity)
        {
            var type = (CodeTypeDeclaration) codeObject;
            var ctor = new CodeConstructor
            {
                Attributes = MemberAttributes.Public
            };
            ctor.Parameters.Add(
                new CodeParameterDeclarationExpression(
                    "IPublishedContent",
                    "content"
                    )
                );
            ctor.BaseConstructorArgs.Add(
                new CodeVariableReferenceExpression("content")
                );
            type.Members.Add(ctor);
        }
    }
}

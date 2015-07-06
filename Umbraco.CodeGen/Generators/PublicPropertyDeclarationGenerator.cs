using System.CodeDom;
using System.Collections.Generic;
using Umbraco.CodeGen.Configuration;

namespace Umbraco.CodeGen.Generators
{
    public class PublicPropertyDeclarationGenerator : PropertyDeclarationGenerator
    {
        public PublicPropertyDeclarationGenerator(ContentTypeConfiguration config, IList<DataTypeDefinition> dataTypes, params CodeGeneratorBase[] memberGenerators) : base(config, dataTypes, memberGenerators)
        {
        }

        protected override void SetAttributes(CodeTypeMember propNode)
        {
            propNode.Attributes = MemberAttributes.Public;
        }
    }
}
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.CodeGen.Configuration;
using Umbraco.CodeGen.Definitions;

namespace Umbraco.CodeGen.Generators.GenerateOnly
{
    public class InterfacePropertyDeclarationGenerator : PropertyDeclarationGenerator
    {
        public InterfacePropertyDeclarationGenerator(ContentTypeConfiguration config, IList<DataTypeDefinition> dataTypes, params CodeGeneratorBase[] generators) : base(config, dataTypes, generators)
        {
        }

        protected override void SetAttributes(CodeTypeMember propNode)
        {
            propNode.Attributes = MemberAttributes.Abstract;
        }
    }
}

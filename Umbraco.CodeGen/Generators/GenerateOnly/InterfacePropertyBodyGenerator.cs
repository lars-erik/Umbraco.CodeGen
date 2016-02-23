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
    public class InterfacePropertyBodyGenerator : CodeGeneratorBase
    {
        public InterfacePropertyBodyGenerator(ContentTypeConfiguration config) : base(config)
        {
        }

        public override void Generate(object codeObject, object typeOrPropertyModel)
        {
            var property = (CodeMemberProperty) codeObject;
            property.HasGet = true;
        }
    }
}

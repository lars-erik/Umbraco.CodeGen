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
    public class InterfaceNameGenerator : NameGenerator
    {
        public InterfaceNameGenerator(ContentTypeConfiguration config) : base(config)
        {
        }

        protected override void SetName(CodeTypeMember type, IEntityDescription description)
        {
            type.Name = "I" + description.Alias.PascalCase();
        }
    }
}

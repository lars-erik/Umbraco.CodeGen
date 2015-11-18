using System.Collections.Generic;
using System.ComponentModel;
using Umbraco.CodeGen.Configuration;

namespace Umbraco.CodeGen.Generators
{
    [Description("Use whichever generator the dev deems appropriate for this version")]
    public class DefaultCodeGeneratorFactory : CodeGeneratorFactory
    {
        private readonly CodeGeneratorFactory inner = new GenerateOnlyGeneratorFactory();

        public override CodeGeneratorBase Create(ContentTypeConfiguration configuration, IEnumerable<DataTypeDefinition> dataTypes)
        {
            return inner.Create(configuration, dataTypes);
        }
    }
}

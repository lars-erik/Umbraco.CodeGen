using System.Collections.Generic;
using Umbraco.CodeGen.Configuration;

namespace Umbraco.CodeGen.Generators
{
    public class DefaultCodeGeneratorFactory : CodeGeneratorFactory
    {
        private readonly CodeGeneratorFactory inner = new BaseSupportedAnnotatedCodeGeneratorFactory();

        public override CodeGeneratorBase Create(ContentTypeConfiguration configuration, IEnumerable<DataTypeDefinition> dataTypes)
        {
            return inner.Create(configuration, dataTypes);
        }
    }
}

using System.Collections.Generic;
using Umbraco.CodeGen.Configuration;

namespace Umbraco.CodeGen.Generators
{
    public abstract class CodeGeneratorFactory
    {
        public abstract CodeGeneratorBase Create(ContentTypeConfiguration configuration, IEnumerable<DataTypeDefinition> dataTypes);
    }
}
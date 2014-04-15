using System.Collections.Generic;
using System.ComponentModel;
using Umbraco.CodeGen.Configuration;
using Umbraco.CodeGen.Parsers;

namespace Umbraco.CodeGen.Generators
{
    [Parser(typeof(DefaultParserFactory))]
    [Description("Use whichever generator and parser the dev deems appropriate for this version")]
    public class DefaultCodeGeneratorFactory : CodeGeneratorFactory
    {
        private readonly CodeGeneratorFactory inner = new BaseSupportedAnnotatedCodeGeneratorFactory();

        public override CodeGeneratorBase Create(ContentTypeConfiguration configuration, IEnumerable<DataTypeDefinition> dataTypes)
        {
            return inner.Create(configuration, dataTypes);
        }
    }
}

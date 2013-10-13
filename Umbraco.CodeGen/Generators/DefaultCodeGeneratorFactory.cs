using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Umbraco.CodeGen.Generators
{
    public class DefaultCodeGeneratorFactory
    {
        public CodeGeneratorBase Create(ContentTypeConfiguration configuration)
        {
            if (configuration.ContentTypeName == "DocumentType")
                return CreateDocumentTypeGenerator(configuration);
            return CreateMediaTypeGenerator(configuration);
        }

        private CodeGeneratorBase CreateDocumentTypeGenerator(ContentTypeConfiguration configuration)
        {
            return null;
        }

        private CodeGeneratorBase CreateMediaTypeGenerator(ContentTypeConfiguration configuration)
        {
            return null;
        }
    }
}

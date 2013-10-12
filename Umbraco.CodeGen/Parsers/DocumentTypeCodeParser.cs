using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Umbraco.CodeGen.Definitions;

namespace Umbraco.CodeGen.Parsers
{
    public class DocumentTypeCodeParser : ContentTypeCodeParser
    {
        public DocumentTypeCodeParser(
            ContentTypeConfiguration configuration, 
            InfoCodeParser infoParser
            ) : base(configuration, infoParser)
        {
        }

        protected override ContentType CreateDefinition()
        {
            return new DocumentType();
        }
    }
}

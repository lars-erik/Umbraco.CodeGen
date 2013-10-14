using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Umbraco.CodeGen.Configuration;
using Umbraco.CodeGen.Definitions;

namespace Umbraco.CodeGen.Parsers
{
    public class MediaTypeCodeParser : ContentTypeCodeParser
    {
        public MediaTypeCodeParser(
            ContentTypeConfiguration configuration, 
            params ContentTypeCodeParserBase[] memberParsers
            )
            : base(
                configuration,
                memberParsers
            )
        {
        }

        protected override ContentType CreateDefinition()
        {
            return new MediaType();
        }
    }
}

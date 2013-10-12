using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Umbraco.CodeGen.Parsers
{
    public abstract class ContentTypeCodeParserBase : CodeParserBase
    {
        protected ContentTypeConfiguration Configuration;

        protected ContentTypeCodeParserBase(ContentTypeConfiguration configuration)
        {
            Configuration = configuration;
        }
    }
}

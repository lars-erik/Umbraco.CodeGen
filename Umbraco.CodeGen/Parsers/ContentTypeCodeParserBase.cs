using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSharpCode.NRefactory.CSharp;
using Umbraco.CodeGen.Definitions;

namespace Umbraco.CodeGen.Parsers
{
    public abstract class ContentTypeCodeParserBase : CodeParserBase
    {
        protected ContentTypeConfiguration Configuration;

        protected ContentTypeCodeParserBase(ContentTypeConfiguration configuration)
        {
            Configuration = configuration;
        }

        public abstract void Parse(AstNode node, ContentType contentType);
    }
}

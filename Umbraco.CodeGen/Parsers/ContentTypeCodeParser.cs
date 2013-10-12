using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSharpCode.NRefactory.CSharp;
using Umbraco.CodeGen.Definitions;

namespace Umbraco.CodeGen.Parsers
{
    public abstract class ContentTypeCodeParser : ContentTypeCodeParserBase
    {
        private readonly InfoCodeParser infoParser;

        protected ContentTypeCodeParser(
            ContentTypeConfiguration configuration,
            InfoCodeParser infoParser
            ) 
            : base(configuration)
        {
            this.infoParser = infoParser;
        }

        public ContentType Parse(TypeDeclaration type)
        {
            var definition = CreateDefinition();
            infoParser.Parse(type, definition);
            return definition;
        }

        protected abstract ContentType CreateDefinition();
    }
}

using System.Collections.Generic;
using ICSharpCode.NRefactory.CSharp;
using Umbraco.CodeGen.Definitions;

namespace Umbraco.CodeGen.Parsers
{
    public abstract class ContentTypeCodeParser : ContentTypeCodeParserBase
    {
        private readonly IEnumerable<ContentTypeCodeParserBase> memberParsers;

        protected ContentTypeCodeParser(
            ContentTypeConfiguration configuration,
            params ContentTypeCodeParserBase[] memberParsers
            ) 
            : base(configuration)
        {
            this.memberParsers = memberParsers;
        }

        public ContentType Parse(AstNode node)
        {
            var type = CreateDefinition();
            Parse(node, type);
            return type;
        }

        public override void Parse(AstNode node, ContentType contentType)
        {
            foreach(var parser in memberParsers)
                parser.Parse(node, contentType);
        }

        protected abstract ContentType CreateDefinition();
    }
}

using ICSharpCode.NRefactory.CSharp;
using Umbraco.CodeGen.Definitions;

namespace Umbraco.CodeGen.Parsers
{
    public abstract class InfoCodeParser : ContentTypeCodeParserBase
    {
        protected InfoCodeParser(ContentTypeConfiguration configuration)
            : base(configuration)
        {
            
        }

        public void Parse(TypeDeclaration type, ContentType definition)
        {
            OnParseInfo(type, definition);
        }

        protected abstract void OnParseInfo(TypeDeclaration type, ContentType definition);
    }
}
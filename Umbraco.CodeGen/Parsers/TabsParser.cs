using System;
using System.Linq;
using ICSharpCode.NRefactory.CSharp;
using Umbraco.CodeGen.Definitions;

namespace Umbraco.CodeGen.Parsers
{
    public class TabsParser : ContentTypeCodeParserBase
    {
        public TabsParser(ContentTypeConfiguration configuration) : base(configuration)
        {
        }

        public override void Parse(AstNode node, ContentType contentType)
        {
            contentType.Tabs.AddRange(
                contentType.GenericProperties
                           .Select(p => p.Tab)
                           .Where(name => !String.IsNullOrWhiteSpace(name))
                           .Distinct()
                           .Select(name => new Tab
                           {
                               Caption = name
                           })
                );
        }
    }
}
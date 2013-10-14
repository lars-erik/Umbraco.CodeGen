using System.Collections.Generic;
using System.Linq;
using ICSharpCode.NRefactory.CSharp;
using NUnit.Framework;
using Umbraco.CodeGen.Configuration;
using Umbraco.CodeGen.Definitions;
using Umbraco.CodeGen.Parsers;

namespace Umbraco.CodeGen.Tests.Parsers
{
    [TestFixture]
    public class PropertiesParserTests : ContentTypeCodeParserTestBase
    {
        [Test]
        public void RunsPropertyParserForEachProperty()
        {
            const string code = @"
                public class AClass {
                    public string One {get;set;}
                    public string Two {get;set;}
                }";

            var tree = ParseType(code);
            var contentType = new MediaType();
            var propertyParser = new SpyPropertyParser(null);
            var parser = new PropertiesParser(
                new ContentTypeConfiguration(), 
                propertyParser
            );

            parser.Parse(tree, contentType);
            Assert.IsTrue(
                new[]{"One", "Two"}
                .SequenceEqual(propertyParser.PropertyNames)
            );
        }

        private class SpyPropertyParser : ContentTypeCodeParserBase
        {
            public readonly List<string> PropertyNames = new List<string>();

            public SpyPropertyParser(ContentTypeConfiguration configuration) : base(configuration)
            {
            }

            public override void Parse(AstNode node, ContentType contentType)
            {
                var propNode = (PropertyDeclaration) node;
                PropertyNames.Add(propNode.Name);
            }
        }
    }
}

using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Umbraco.CodeGen.Definitions;
using Umbraco.CodeGen.Parsers;

namespace Umbraco.CodeGen.Tests.Parsers
{
    [TestFixture]
    public class TabsParserTests
    {
        [Test]
        public void Parse_AddsTabForEachDistinctNonEmptyPropertyTab()
        {
            var contentType = new MediaType
            {
                GenericProperties = new List<GenericProperty>
                {
                    new GenericProperty {Tab = "A"},
                    new GenericProperty {Tab = "A"},
                    new GenericProperty {Tab = "B"},
                    new GenericProperty {Tab = ""},
                    new GenericProperty()
                }
            };
            var parser = new TabsParser(null);
            parser.Parse(null, contentType);
            Assert.That(
                new[]{"A","B"}
                .SequenceEqual(contentType.Tabs.Select(t => t.Caption))
                );
            Assert.AreEqual(0, contentType.Tabs.Sum(t => t.Id));
            // TODO: Tab sortorder
        }
    }
}

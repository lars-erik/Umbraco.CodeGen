using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Umbraco.CodeGen.Definitions;
using Umbraco.CodeGen.Parsers;

namespace Umbraco.CodeGen.Tests.Parsers
{
    public class PropertyParserTests : ContentTypeCodeParserTestBase
    {
        private const string SimplePropertyClass = @"
                public class AClass {
                    public string AProperty {get;set;}
                }";
        private GenericProperty property;

        [SetUp]
        public void SetUp()
        {
            var codeGenConfig = new CodeGeneratorConfiguration();
            Configuration = new ContentTypeConfiguration(codeGenConfig);
            ContentType = new MediaType();
            Parser = new PropertyParser(Configuration);
        }

        [Test]
        public void Parse_PropertyNode_AddsGenericProperty()
        {
            ParseProperty(SimplePropertyClass, "AProperty");
            Assert.AreEqual(1, ContentType.GenericProperties.Count);
        }

        [Test]
        public void Parse_Alias_IsPropertyNameInCamelCase()
        {
            ParseProperty(SimplePropertyClass, "AProperty");
            Assert.AreEqual("aProperty", property.Alias);
        }

        [Test]
        public void Parse_Name_WhenPureProperty_IsSplitPascalCase()
        {
            const string code = SimplePropertyClass;
            ParseProperty(code, "AProperty");
            Assert.AreEqual("A Property", property.Name);
        }

        private void ParseProperty(string code, string propertyName)
        {
            var type = ParseType(code);
            var prop = type.Members.SingleOrDefault(m => m.Name == propertyName);
            Parser.Parse(prop, ContentType);
            property = ContentType.GenericProperties.SingleOrDefault(p => p.Alias == propertyName.CamelCase());
        }
    }
}

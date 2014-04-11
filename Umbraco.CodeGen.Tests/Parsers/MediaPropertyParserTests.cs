using NUnit.Framework;
using Umbraco.CodeGen.Parsers;
using Umbraco.CodeGen.Tests.TestHelpers;

namespace Umbraco.CodeGen.Tests.Parsers
{
    [TestFixture]
    public class MediaPropertyParserTests : PropertyParserTestBase
    {
        protected override ContentTypeCodeParserBase CreateParser()
        {
            return new MediaPropertyParser(
                new CodeGen.Parsers.Annotated.PropertyParser(Configuration, DataTypeConfiguration),
                Configuration, DataTypeConfiguration
                );
        }

        [Test]
        public void Parse_Type_IsDataTypeGuid()
        {
            const string code = @"
                public class AClass {
                    [GenericProperty(Definition=""Richtext editor"")]
                    public string AProperty {get;set;}
                }";
            ParseProperty(code);
            Assert.AreEqual(TestDataTypeProvider.Richtexteditor.DataTypeGuid, Property.Type);
        }
    }
}

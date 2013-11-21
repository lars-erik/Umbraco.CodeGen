using System.IO;
using System.Linq;
using NUnit.Framework;
using Umbraco.CodeGen.Configuration;
using Umbraco.CodeGen.Definitions;
using Umbraco.CodeGen.Generators;
using Umbraco.CodeGen.Parsers;
using Umbraco.CodeGen.Tests.TestHelpers;

namespace Umbraco.CodeGen.Tests
{
    [TestFixture]
    public class CodeParserAcceptanceTests
    {
        [Test]
        public void Parse_ReturnsDocumentType()
        {
            TestParse("SomeDocumentType", "DocumentType", TestFactory.CreateExpectedDocumentType(), new DefaultParserFactory());
        }

        [Test] 
        public void Parse_ReturnsMediaType()
        {
            TestParse("SomeMediaType", "MediaType", TestFactory.CreateExpectedMediaType(), new DefaultParserFactory());
        }

        [Test]
        public void Parse_Annotated_ReturnsDocumentType()
        {
            TestParse("SomeAnnotatedDocumentType", "DocumentType", TestFactory.CreateExpectedDocumentType(), new AnnotatedParserFactory());
        }

        [Test]
        public void Parse_Annotated_ReturnsMediaType()
        {
            TestParse("SomeAnnotatedMediaType", "MediaType", TestFactory.CreateExpectedMediaType(), new AnnotatedParserFactory());
        }

        private static void TestParse<T>(string fileName, string contentTypeName, T expectedContentType, ParserFactory factory)
            where T : ContentType
        {
            string code;
            using (var inputReader = File.OpenText(@"..\..\TestFiles\" + fileName + ".cs"))
            {
                code = inputReader.ReadToEnd();
            }

            var config = new CodeGeneratorConfiguration().Get(contentTypeName);
            config.BaseClass = "DocumentTypeBase";

            var parser = new CodeParser(
                config,
                TestDataTypeProvider.All,
                factory
                );

            var contentType = parser.Parse(new StringReader(code)).SingleOrDefault();

            var expectedXml = SerializationHelper.BclSerialize(expectedContentType);
            var actualXml = SerializationHelper.BclSerialize((T) contentType);

            Assert.AreEqual(expectedXml, actualXml);
        }
    }
}

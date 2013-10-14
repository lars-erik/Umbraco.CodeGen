using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Umbraco.CodeGen.Configuration;
using Umbraco.CodeGen.Definitions;
using Umbraco.CodeGen.Parsers;
using Umbraco.CodeGen.Tests.Helpers;

namespace Umbraco.CodeGen.Tests
{
    [TestFixture]
    class CodeParserTests
    {
        [Test]
        public void Parse_ReturnsDocumentType()
        {
            TestParse("SomeDocumentType", "DocumentType", TestFactory.CreateExpectedDocumentType());
        }

        [Test] 
        public void Parse_ReturnsMediaType()
        {
            TestParse("SomeMediaType", "MediaType", TestFactory.CreateExpectedMediaType());
        }

        private static void TestParse<T>(string fileName, string contentTypeName, T expectedContentType)
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
                new DefaultParserFactory()
                );

            var contentType = parser.Parse(new StringReader(code)).SingleOrDefault();

            var expectedXml = SerializationHelper.BclSerialize(expectedContentType);
            var actualXml = SerializationHelper.BclSerialize((T) contentType);

            Assert.AreEqual(expectedXml, actualXml);
        }
    }
}

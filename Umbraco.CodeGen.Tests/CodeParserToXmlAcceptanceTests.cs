using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;
using Umbraco.CodeGen.Configuration;
using Umbraco.CodeGen.Definitions;
using Umbraco.CodeGen.Parsers;
using Umbraco.CodeGen.Tests.TestHelpers;

namespace Umbraco.CodeGen.Tests
{
    [TestFixture]
    public class CodeParserToXmlAcceptanceTests
    {
        [Test]
        public void Generate_ReturnsXmlForDocumentType()
        {
            TestGeneratedXml("SomeDocumentType", "SomeDocumentType", "DocumentType", new BclParserFactory());
        }

        [Test]
        public void Generate_ReturnsXmlForMediaType()
        {
            TestGeneratedXml("SomeMediaType", "SomeMediaType", "MediaType", new BclParserFactory());
        }

        [Test]
        public void Generate_Annotated_ReturnsXmlForDocumentType()
        {
            TestGeneratedXml("SomeAnnotatedDocumentType", "SomeDocumentType", "DocumentType", new AnnotatedParserFactory());
        }

        [Test]
        public void Generate_Annotated_ReturnsXmlForMediaType()
        {
            TestGeneratedXml("SomeAnnotatedMediaType", "SomeMediaType", "MediaType", new AnnotatedParserFactory());
        }

        private static void TestGeneratedXml(string classFileName, string xmlFileName, string contentTypeName, ParserFactory factory)
        {
            ContentType contentType;
            string expectedOutput;

            using (var goldReader = File.OpenText(@"..\..\TestFiles\" + xmlFileName + ".xml"))
            {
                expectedOutput = goldReader.ReadToEnd();
            }

            var contentTypeConfig = new CodeGeneratorConfiguration().Get(contentTypeName);
            contentTypeConfig.BaseClass = "Umbraco.Core.Models.TypedModelBase";

            using (var inputReader = File.OpenText(@"..\..\TestFiles\" + classFileName + ".cs"))
            {
                var codeParser = new CodeParser(contentTypeConfig, TestDataTypeProvider.All, factory);
                contentType = codeParser.Parse(inputReader).Single();
            }

            var serializer = new ContentTypeSerializer();
            var xml = serializer.Serialize(contentType);

            Console.WriteLine(xml);

            Assert.AreEqual(expectedOutput, xml);
        }
    }
}

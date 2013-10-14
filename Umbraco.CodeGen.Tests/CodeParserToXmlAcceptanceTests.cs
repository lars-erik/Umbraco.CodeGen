using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;
using Umbraco.CodeGen.Definitions;
using Umbraco.CodeGen.Parsers;

namespace Umbraco.CodeGen.Tests
{
	[TestFixture]
	public class CodeParserToXmlAcceptanceTests
	{
		[Test]
		public void Generate_ReturnsXmlForDocumentType()
		{
			const string fileName = "SomeDocumentType";
			const string contentTypeName = "DocumentType";

			TestGeneratedXml(fileName, contentTypeName);
		}

		[Test]
		public void Generate_ReturnsXmlForMediaType()
		{
			const string fileName = "SomeMediaType";
			const string contentTypeName = "MediaType";

			TestGeneratedXml(fileName, contentTypeName);
		}

		private static void TestGeneratedXml(string fileName, string contentTypeName)
		{
			ContentType contentType;
			var expectedOutput = "";
            using (var goldReader = File.OpenText(@"..\..\TestFiles\" + fileName + ".xml"))
			{
				expectedOutput = goldReader.ReadToEnd();
			}

			var configuration = new CodeGeneratorConfiguration
			{
				DefaultTypeMapping = "String"
			};
			var contentTypeConfig = new ContentTypeConfiguration(configuration)
			{
				ContentTypeName = contentTypeName,
				BaseClass = "DocumentTypeBase",
			};

            using (var inputReader = File.OpenText(@"..\..\TestFiles\" + fileName + ".cs"))
			{
                var codeParser = new CodeParser(contentTypeConfig, TestDataTypeProvider.All, new DefaultParserFactory());
			    contentType = codeParser.Parse(inputReader).Single();
			}
            
		    var serializer = new ContentTypeSerializer();
		    var xml = serializer.Serialize(contentType);

            Console.WriteLine(xml);

		    Assert.AreEqual(expectedOutput, xml);
		}
	}
}

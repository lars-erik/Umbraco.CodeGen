using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ICSharpCode.NRefactory.CSharp;
using NUnit.Framework;
using Umbraco.CodeGen.Definitions;
using Umbraco.CodeGen.Parsers;

namespace Umbraco.CodeGen.Tests
{
	[TestFixture]
	public class DocumentTypeXmlGeneratorTests
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
				TypeMappings = new Dictionary<string, string>(),
				DefaultTypeMapping = "string"
			};
			var contentTypeConfig = new ContentTypeConfiguration(configuration)
			{
				ContentTypeName = contentTypeName,
				BaseClass = "DocumentTypeBase",
			};

			var dataTypes = new List<DataTypeDefinition>
			{
				new DataTypeDefinition("RTE", "5e9b75ae-face-41c8-b47e-5f4b0fd82f83", "ca90c950-0aff-4e72-b976-a30b1ac57dad"),
				new DataTypeDefinition("Textstring", "ec15c1e5-9d90-422a-aa52-4f7622c63bea", "0cc0eba1-9960-42c9-bf9b-60e150b429ae"),
				new DataTypeDefinition("Numeric", "1413afcb-d19a-4173-8e9a-68288d2a73b8", "2e6d3631-066e-44b8-aec4-96f09099b2b5")
			};

            using (var inputReader = File.OpenText(@"..\..\TestFiles\" + fileName + ".cs"))
			{
                var codeParser = new CodeParser(contentTypeConfig, dataTypes, new DefaultParserFactory());
			    contentType = codeParser.Parse(inputReader).Single();
			}
            
		    var serializer = new ContentTypeSerializer();
		    var xml = serializer.Serialize(contentType);

            Console.WriteLine(xml);

		    Assert.AreEqual(expectedOutput, xml);
		}
	}
}

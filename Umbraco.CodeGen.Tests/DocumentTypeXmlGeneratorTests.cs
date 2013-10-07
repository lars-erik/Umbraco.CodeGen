using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NUnit.Framework;

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
			var code = "";
			var expectedOutput = "";
			using (var inputReader = File.OpenText(@"..\..\" + fileName + ".cs"))
			{
				code = inputReader.ReadToEnd();
			}
			using (var goldReader = File.OpenText(@"..\..\" + fileName + ".xml"))
			{
				expectedOutput = goldReader.ReadToEnd();
			}

			var configuration = new CodeGeneratorConfiguration
			{
				TypeMappings = new Dictionary<string, string>(),
				DefaultTypeMapping = "string"
			};
			configuration.DocumentTypes = new ContentTypeConfiguration(configuration)
			{
				ContentTypeName = contentTypeName,
				BaseClass = "DocumentTypeBase",
			};

			var dataTypeConfiguration = new List<DataTypeDefinition>
			{
				new DataTypeDefinition("RTE", "5e9b75ae-face-41c8-b47e-5f4b0fd82f83", "ca90c950-0aff-4e72-b976-a30b1ac57dad"),
				new DataTypeDefinition("Textstring", "ec15c1e5-9d90-422a-aa52-4f7622c63bea", "0cc0eba1-9960-42c9-bf9b-60e150b429ae")
			};

			var reader = new StringReader(code);
			var generator = new DocumentTypeXmlGenerator(configuration.DocumentTypes, dataTypeConfiguration);
			var doc = generator.Generate(reader).First();

			var sb = new StringBuilder();
			var writer = new StringWriter(sb);
			doc.Save(writer);
			writer.Flush();
			Console.WriteLine(sb.ToString());

			Assert.AreEqual(expectedOutput, sb.ToString());
		}
	}
}

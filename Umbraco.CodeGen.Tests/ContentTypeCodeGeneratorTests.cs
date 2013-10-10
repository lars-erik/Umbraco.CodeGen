using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.CSharp;
using NUnit.Framework;

namespace Umbraco.CodeGen.Tests
{
	[TestFixture]
	public class ContentTypeCodeGeneratorTests
	{
		[Test]
		public void BuildCode_GeneratesCodeForDocumentType()
		{
			TestBuildCode("SomeDocumentType", "DocumentType");
		}

		[Test]
		public void BuildCode_GeneratesCodeForMediaType()
		{
			TestBuildCode("SomeMediaType", "MediaType");
		}

		private static void TestBuildCode(string fileName, string contentTypeName)
		{
			var xml = "";
			var expectedOutput = "";
			using (var inputReader = File.OpenText(@"..\..\TestFiles\" + fileName + ".xml"))
			{
				xml = inputReader.ReadToEnd();
			}
            using (var goldReader = File.OpenText(@"..\..\TestFiles\" + fileName + ".cs"))
			{
				expectedOutput = goldReader.ReadToEnd();
			}

			var configuration = new CodeGeneratorConfiguration
			{
				TypeMappings = new Dictionary<string, string>
				{
					{"1413afcb-d19a-4173-8e9a-68288d2a73b8", "Int32"}
				},
				DefaultTypeMapping = "String",
			};
			var typeConfig = new ContentTypeConfiguration(configuration)
			{
				ContentTypeName = contentTypeName,
				BaseClass = "DocumentTypeBase",
				Namespace = "Umbraco.CodeGen.Models"
			};

			var sb = new StringBuilder();
			var writer = new StringWriter(sb);
			var generator = new ContentTypeCodeGenerator(typeConfig, XDocument.Parse(xml), new CSharpCodeProvider());
			generator.BuildCode(writer);
			writer.Flush();
			Console.WriteLine(sb.ToString());

			Assert.AreEqual(expectedOutput, sb.ToString());
		}
	}
}

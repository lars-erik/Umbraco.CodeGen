using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.CSharp;
using NUnit.Framework;
using Umbraco.CodeGen.Definitions;
using Umbraco.CodeGen.Generators;

namespace Umbraco.CodeGen.Tests
{
	[TestFixture]
	public class ContentTypeCodeGeneratorsAcceptanceTests
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
		    ContentType contentType;
			var expectedOutput = "";
			using (var inputReader = File.OpenText(@"..\..\TestFiles\" + fileName + ".xml"))
			{
                contentType = new ContentTypeSerializer().Deserialize(inputReader);
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

            var dataTypes = new List<DataTypeDefinition>
			{
				new DataTypeDefinition("RTE", "5e9b75ae-face-41c8-b47e-5f4b0fd82f83", "ca90c950-0aff-4e72-b976-a30b1ac57dad"),
				new DataTypeDefinition("Textstring", "ec15c1e5-9d90-422a-aa52-4f7622c63bea", "0cc0eba1-9960-42c9-bf9b-60e150b429ae"),
				new DataTypeDefinition("Numeric", "1413afcb-d19a-4173-8e9a-68288d2a73b8", "2e6d3631-066e-44b8-aec4-96f09099b2b5")
			};

            var options = new CodeGeneratorOptions
            {
                BlankLinesBetweenMembers = false,
                BracingStyle = "C"
            };

            // TODO: Wrap this

		    var sb = new StringBuilder();
			var writer = new StringWriter(sb);
			var generator = new DefaultCodeGeneratorFactory().Create(typeConfig, dataTypes);
		    var compileUnit = new CodeCompileUnit();
            generator.Generate(compileUnit, contentType);
		    var provider = new CSharpCodeProvider();
            provider.GenerateCodeFromCompileUnit(compileUnit, writer, options);
			writer.Flush();
			Console.WriteLine(sb.ToString());

			Assert.AreEqual(expectedOutput, sb.ToString());
		}
	}
}

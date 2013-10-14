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
				TypeMappings = new TypeMappings(new[]
				{
				    new TypeMapping { DataTypeId = "1413afcb-d19a-4173-8e9a-68288d2a73b8", Type = "Int32" }
				}){
				    DefaultType = "String"
                }
			};
			var typeConfig = new ContentTypeConfiguration(configuration)
			{
				ContentTypeName = contentTypeName,
				BaseClass = "DocumentTypeBase",
				Namespace = "Umbraco.CodeGen.Models"
			};

            var options = new CodeGeneratorOptions
            {
                BlankLinesBetweenMembers = false,
                BracingStyle = "C"
            };

            // TODO: Wrap this

		    var sb = new StringBuilder();
			var writer = new StringWriter(sb);
			var generator = new DefaultCodeGeneratorFactory().Create(typeConfig, TestDataTypeProvider.All);
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

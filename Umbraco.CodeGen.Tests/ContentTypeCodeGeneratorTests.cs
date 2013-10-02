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
		public void BuildCode_GeneratesCode()
		{
			var xml = "";
			var expectedOutput = "";
			using (var inputReader = File.OpenText(@"..\..\SomeDocumentType.xml"))
			{
				xml = inputReader.ReadToEnd();
			}
			using (var goldReader = File.OpenText(@"..\..\SomeDocumentType.cs"))
			{
				expectedOutput = goldReader.ReadToEnd();
			}

			var configuration = new CodeGeneratorConfiguration
			{
				BaseClass = "DocumentTypeBase",
				TypeMappings = new Dictionary<string, string>(),
				DefaultTypeMapping = "String",
				Namespace = "MyWeb.Models"
			};

			var sb = new StringBuilder();
			var writer = new StringWriter(sb);
			var generator = new ContentTypeCodeGenerator(configuration, XDocument.Parse(xml), new CSharpCodeProvider());
			generator.BuildCode(writer);
			writer.Flush();
			Console.WriteLine(sb.ToString());

			Assert.AreEqual(expectedOutput, sb.ToString());
		}

	}
}

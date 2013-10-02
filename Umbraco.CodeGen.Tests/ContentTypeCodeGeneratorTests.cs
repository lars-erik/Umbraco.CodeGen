using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Umbraco.CodeGen.Tests
{
	[TestFixture]
	public class ContentTypeCodeGeneratorTests
	{
		[Test]
		public void Generate_GeneratesCode()
		{
			var code = "";
			var expectedOutput = "";
			using (var inputReader = File.OpenText(@"..\..\SomeDocumentType.xml"))
			{
				code = inputReader.ReadToEnd();
			}
			using (var goldReader = File.OpenText(@"..\..\SomeDocumentType.cs"))
			{
				expectedOutput = goldReader.ReadToEnd();
			}

			var configuration = new CodeGeneratorConfiguration
			{
				BaseClass = "DocumentTypeBase",
				TypeMappings = new Dictionary<string, string>(),
				DefaultTypeMapping = "string"
			};



			var reader = new StringReader(code);
			var generator = new ContentTypeBuilder();

			Assert.Fail();
			//generator.Configure(configuration, );
			//var doc = generator.Generate(reader).First();

			//var sb = new StringBuilder();
			//var writer = new StringWriter(sb);
			//doc.Save(writer);
			//writer.Flush();
			//Console.WriteLine(sb.ToString());

			//Assert.AreEqual(expectedOutput, sb.ToString());

		}

	}
}

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
    public class DocumentTypeClassGenTests
    {
		[Test]
		public void BuildCode_GeneratesClasses()
		{
			TestBuildCode("classes");
		}

		[Test]
		public void BuildCode_WhenCustomBaseClass_DoesntGenerateBaseClass()
		{
			TestBuildCode("customBaseClass");
		}

		private static void TestBuildCode(string testFilesName)
		{
			var inputFilePath = Path.Combine(Environment.CurrentDirectory, @"..\..\", testFilesName + ".xml");
			var content = GetContent(inputFilePath);
			var classGen = new DocumentTypeClassGen();
			var builder = new StringWriter(new StringBuilder());

			classGen.SetInput(inputFilePath, content);
			classGen.SetNamespace("Some.Namespace");
			classGen.BuildCode(builder);

			var output = builder.ToString();

			Console.WriteLine(output);

			using (var gold = File.OpenText(Path.Combine(Environment.CurrentDirectory, @"..\..\", testFilesName + ".gold")))
			{
				Assert.AreEqual(gold.ReadToEnd(), output);
			}
		}

		private static string GetContent(string inputFilePath)
		{
			using (var reader = File.OpenText(inputFilePath))
			{
				return reader.ReadToEnd();
			}
		}
    }
}

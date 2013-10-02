using System;
using System.CodeDom.Compiler;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.CSharp;
using NUnit.Framework;

namespace Umbraco.CodeGen.Tests
{
	[TestFixture]
    public class DocumentTypeClassGenTests
    {
		[Test]
		public void BuildCode_GeneratesClasses()
		{
			TestGenerateCode("classes");
		}

		[Test]
		public void BuildCode_WhenCustomBaseClass_DoesntGenerateBaseClass()
		{
			TestGenerateCode("customBaseClass");
		}

		[Test]
		public void BuildCode_Classes_CanBeBuilt()
		{
			const string fakeCoreCode = @"
				namespace Umbraco.Core.Models
				{
					public interface IPublishedContent
					{
					}
				}
				namespace Umbraco.Web
				{
					using Umbraco.Core.Models;
					public static class PublishedContentExtensions
					{
						public static T GetPropertyValue<T>(this IPublishedContent content, string name)
						{
							return default(T);
						}
					}
				}
			";

			var generatedCode = GenerateCode("classes");

			var provider = new CSharpCodeProvider();
			var results = provider
				.CompileAssemblyFromSource(
					new CompilerParameters(new string[0]),
					new[] {fakeCoreCode, generatedCode}
				);

			Assert.AreEqual(0, results.Errors.Count, AggregateBuildErrors(results));
		}

		private static void TestGenerateCode(string testFilesName)
		{
			var output = GenerateCode(testFilesName);

			Console.WriteLine(output);

			using (var gold = File.OpenText(Path.Combine(Environment.CurrentDirectory, @"..\..\", testFilesName + ".xml")))
			{
				Assert.AreEqual(gold.ReadToEnd(), output);
			}
		}

		private static string GenerateCode(string testFilesName)
		{
			var inputFilePath = Path.Combine(Environment.CurrentDirectory, @"..\..\", testFilesName + ".xml");
			var content = GetContent(inputFilePath);
			var classGen = new DocumentTypeClassGen();
			var builder = new StringWriter(new StringBuilder());

			classGen.SetInput(inputFilePath, content);
			classGen.SetNamespace("Some.Namespace");
			classGen.GenerateCode(builder);

			var output = builder.ToString();
			return output;
		}

		private static string GetContent(string inputFilePath)
		{
			using (var reader = File.OpenText(inputFilePath))
			{
				return reader.ReadToEnd();
			}
		}

		private static string AggregateBuildErrors(CompilerResults results)
		{
			return results.Errors.Cast<CompilerError>().Aggregate(String.Empty, (s, e) => s += e.Line + ": " + e.ErrorText + Environment.NewLine);
		}
    }
}

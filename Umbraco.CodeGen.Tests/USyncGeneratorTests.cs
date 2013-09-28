using System;
using System.CodeDom;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.NRefactory.PatternMatching;
using Microsoft.CSharp;
using NUnit.Framework;

namespace Umbraco.CodeGen.Tests
{
	[TestFixture]
	public class USyncGeneratorTests
	{
		public string AProp
		{
			get; set;
		}

		[Test]
		public void CanReadSomeCode()
		{
			var code = @"
				namespace MyWeb.Models
				{
					using System;
					using System.ComponentModel;
					using System.ComponentModel.DataAnnotations;	
					using Umbraco.Core.Models;

					[DisplayName(""Some document type"")]
					[Description(""A description of some document type"")]
					public class SomeDocumentType : DocumentTypeBase
					{
						[DisplayName(""Some property"")]
						[Description(""A description"")]
						[Category(""A tab"")]
						[DataTypeAttribute(""BBBEB697-D751-4A19-8ACE-3A05DE2EEEF6"")]
						public string SomeProperty
						{
							get
							{
								return Content.GetPropertyValue<string>(""someProperty"");
							}
						}
					}
				}
			";

			const string expectedOutput = 
@"class SomeDocumentType : DocumentTypeBase
DisplayName - ""Some document type""
Description - ""A description of some document type""

string SomeProperty
DisplayName - ""Some property""
Description - ""A description""
Category - ""A tab""
DataTypeAttribute - ""BBBEB697-D751-4A19-8ACE-3A05DE2EEEF6""
";

			var reader = new StringReader(code);

			var parser = new CSharpParser();
			var tree = parser.Parse(reader);

			Assert.AreEqual(0, tree.Errors.Count, tree.Errors.Aggregate("", (s, e) => s += e.Region.BeginLine + ": " + e.Message + "\n"));

			var types = tree.Descendants.OfType<TypeDeclaration>();


			var outputBuilder = new StringBuilder();
			foreach (var type in types)
			{
				outputBuilder.AppendFormat("class {0} : {1}\r\n", type.Name, String.Join(", ", type.BaseTypes));

				outputBuilder.AppendFormat(type.Attributes.Aggregate("", (s, a) =>
					s + a.Attributes.Aggregate("", (s2, att) => 
						s2 + ((SimpleType)att.Type).Identifier + " - " +
						String.Join(", ", att.Arguments)
						) + "\r\n"
					) + "\r\n");

				foreach (var prop in type.Descendants.OfType<PropertyDeclaration>())
				{
					outputBuilder.AppendFormat("{0} {1}\r\n", prop.ReturnType, prop.Name);

					outputBuilder.AppendFormat(prop.Attributes.Aggregate("", (s, a) =>
						s + a.Attributes.Aggregate("", (s2, att) =>
							s2 + ((SimpleType)att.Type).Identifier + " - " +
							String.Join(", ", att.Arguments)
							) + "\r\n"
						));
				}
			}

			Console.WriteLine(outputBuilder.ToString());
			Assert.AreEqual(expectedOutput, outputBuilder.ToString());
		}
	}
}

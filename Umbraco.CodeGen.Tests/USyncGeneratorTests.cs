using System;
using System.CodeDom;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml.Linq;
using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.NRefactory.PatternMatching;
using Microsoft.CSharp;
using NUnit.Framework;
using Attribute = ICSharpCode.NRefactory.CSharp.Attribute;

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
			var code = "";
			var expectedOutput = "";
			using (var inputReader = File.OpenText(@"..\..\SomeDocumentType.cs"))
			{
				code = inputReader.ReadToEnd();
			}
			using (var goldReader = File.OpenText(@"..\..\SomeDocumentType.gold"))
			{
				expectedOutput = goldReader.ReadToEnd();
			}

			var reader = new StringReader(code);

			var parser = new CSharpParser();
			var tree = parser.Parse(reader);

			Assert.AreEqual(0, tree.Errors.Count, tree.Errors.Aggregate("", (s, e) => s += e.Region.BeginLine + ": " + e.Message + "\n"));

			var type = tree.Descendants.OfType<TypeDeclaration>().First();

			var doc = new XDocument();
			var root = new XElement("DocumentType");
			doc.Add(root);


			var info = new XElement("Info");
			root.Add(info);

			var displayNameAttribute = FindAttribute(type.Attributes, "DisplayName");
			var name = ElementFromAttribute("Name", displayNameAttribute, type.Name);

			info.Add(name);
			info.Add(new XElement("Alias", CamelCase(type.Name)));
			info.Add(new XElement("Icon", "folder.gif"));
			info.Add(new XElement("Thumbnail", "folder.png"));

			var descriptionAttribute = FindAttribute(type.Attributes, "Description");
			var description = ElementFromAttribute("Description", descriptionAttribute, "");
			info.Add(description);

			info.Add(new XElement("AllowAtRoot", "True"));
			info.Add(new XElement("Master"));
			info.Add(new XElement("AllowedTemplates"));
			info.Add(new XElement("DefaultTemplate"));

			root.Add(new XElement("Structure"));

			var props = new XElement("GenericProperties");
			root.Add(props);

			var tabNames = new List<string>();

			foreach (var prop in type.Descendants.OfType<PropertyDeclaration>())
			{
				var propElem = new XElement("GenericProperty");

				var propNameAtt = FindAttribute(prop.Attributes, "DisplayName");
				var propName = ElementFromAttribute("Name", propNameAtt, prop.Name);
				propElem.Add(propName);

				propElem.Add(new XElement("Alias", CamelCase(prop.Name)));

				var typeAtt = FindAttribute(prop.Attributes, "DataType");
				var typeId = ElementFromAttribute("Type", typeAtt, Guid.Empty.ToString());
				propElem.Add(typeId);

				propElem.Add(new  XElement("Definition", Guid.Empty.ToString()));

				var categoryAtt = FindAttribute(prop.Attributes, "Category");
				var tab = ElementFromAttribute("Tab", categoryAtt, "Properties");
				propElem.Add(tab);

				tabNames.Add(tab.Value);

				propElem.Add(new XElement("Mandatory", "False"));

				var validationAtt = FindAttribute(prop.Attributes, "RegularExpression");
				var validation = ElementFromAttribute("Validation", validationAtt, "");
				propElem.Add(validation);

				var propDescriptionAttribute = FindAttribute(prop.Attributes, "Description");
				var propDescription = CDataElementFromAttribute("Description", propDescriptionAttribute, "");
				propElem.Add(propDescription);

				props.Add(propElem);
			}

			tabNames = tabNames.Distinct().ToList();

			var tabs = new XElement("Tabs");
			root.Add(tabs);

			foreach (var tab in tabNames)
			{
				var tabElem = new XElement("Tab");
				tabElem.Add(new XElement("Id", "0"));
				tabElem.Add(new XElement("Name", tab));
				tabs.Add(tabElem);
			}

			var sb = new StringBuilder();
			var writer = new StringWriter(sb);
			doc.Save(writer);
			writer.Flush();
			Console.WriteLine(sb.ToString());

			Assert.AreEqual(expectedOutput, sb.ToString());
		}

		private static string CamelCase(string value)
		{
			return value.Substring(0, 1).ToLower() + value.Substring(1);
		}

		private static XElement ElementFromAttribute(string elementName, Attribute attribute, string defaultValue)
		{
			var value = AttributeValueOrDefault(attribute, defaultValue);
			return new XElement(elementName, value);
		}

		private static XElement CDataElementFromAttribute(string elementName, Attribute attribute, string defaultValue)
		{
			var value = AttributeValueOrDefault(attribute, defaultValue);
			return new XElement(elementName, new XCData(value));
		}

		private static string AttributeValueOrDefault(Attribute attribute, string defaultValue)
		{
			var value = attribute != null
				? attribute.Arguments.Select(arg => arg.GetText().Trim('"')).FirstOrDefault()
				: defaultValue;
			return value;
		}

		private static Attribute FindAttribute(IEnumerable<AttributeSection> attributeSections, string attributeName)
		{
			return attributeSections
				.SelectMany(att => att.Attributes)
				.Where(att => att.Type is SimpleType)
				.SingleOrDefault(att => ((SimpleType) att.Type).Identifier == attributeName);
		}
	}
}

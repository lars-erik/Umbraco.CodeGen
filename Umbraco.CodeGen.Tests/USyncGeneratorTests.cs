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
using Mono.CSharp;
using NUnit.Framework;
using Attribute = ICSharpCode.NRefactory.CSharp.Attribute;
using CSharpParser = ICSharpCode.NRefactory.CSharp.CSharpParser;
using Expression = ICSharpCode.NRefactory.CSharp.Expression;

namespace Umbraco.CodeGen.Tests
{
	[TestFixture]
	public class USyncGeneratorTests
	{
		private const StringComparison IgnoreCase = StringComparison.OrdinalIgnoreCase;

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

			const string fakeCode = @"
			namespace MyWeb.Models
			{
				public class SomeOtherDocType {}
			}
			";

			var reader = new StringReader(fakeCode + code);

			var parser = new CSharpParser();
			var tree = parser.Parse(reader);

			Assert.AreEqual(0, tree.Errors.Count, tree.Errors.Aggregate("", (s, e) => s += e.Region.BeginLine + ": " + e.Message + "\n"));

			var type = tree.Descendants.OfType<TypeDeclaration>().First(t => t.Name == "SomeDocumentType");

			var doc = new XDocument();
			var root = new XElement("DocumentType");
			doc.Add(root);

			var info = new XElement("Info");
			root.Add(info);

			var displayNameAttribute = FindAttribute(type.Attributes, "DisplayName");
			var name = ElementFromAttribute("Name", displayNameAttribute, type.Name);

			info.Add(name);
			info.Add(new XElement("Alias", CamelCase(type.Name)));

			var iconValue = FindStringFieldValue(type, "icon") ?? "Folder.gif";
			info.Add(new XElement("Icon", iconValue));

			var folderValue = FindStringFieldValue(type, "thumbnail") ?? "Folder.png";
			info.Add(new XElement("Thumbnail", folderValue));

			var descriptionAttribute = FindAttribute(type.Attributes, "Description");
			var description = ElementFromAttribute("Description", descriptionAttribute, "");
			info.Add(description);

			var allowAtRootValue = FindBoolFieldValue(type, "allowAtRoot");
			info.Add(new XElement("AllowAtRoot", allowAtRootValue.ToString().ProperCase()));
	
			info.Add(new XElement("Master"));

			var allowedTemplatesElement = new XElement("AllowedTemplates");
			info.Add(allowedTemplatesElement);

			var allowedTemplatesValue = FindStringArrayValue(type, "allowedTemplates");
			if (allowedTemplatesValue != null)
				foreach(var allowedTemplate in allowedTemplatesValue)
					allowedTemplatesElement.Add(new XElement("Template", allowedTemplate));

			var defaultTemplateValue = FindStringFieldValue(type, "defaultTemplate");
			info.Add(new XElement("DefaultTemplate", defaultTemplateValue));

			var structureElement = new XElement("Structure");
			root.Add(structureElement);

			var structureValue = FindTypeArrayValue(type, "structure");
			if (structureValue != null)
				foreach(var typeName in structureValue)
					structureElement.Add(new XElement("DocumentType", typeName));

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

		private static string FindStringFieldValue(TypeDeclaration type, string fieldName)
		{
			var fieldVariable = FindFieldVariable(type, fieldName);
			return WithInitializer(fieldVariable, ex => ex.GetText().Trim('"'));
		}

		private static string[] FindStringArrayValue(TypeDeclaration type, string fieldName)
		{
			var fieldVariable = FindFieldVariable(type, fieldName);
			return WithInitializer(fieldVariable, ex =>
				((ArrayCreateExpression) ex).Initializer.Elements
					.OfType<PrimitiveExpression>()
					.Select(e => e.Value as string)
					.ToArray()
			);
		}

		private static string[] FindTypeArrayValue(TypeDeclaration type, string fieldName)
		{
			var fieldVariable = FindFieldVariable(type, fieldName);
			return WithInitializer(fieldVariable, ex =>
				((ArrayCreateExpression) ex).Initializer.Elements
					.OfType<TypeOfExpression>()
					.Select(e => ((SimpleType)e.Type).Identifier)
					.ToArray()
			);
		}

		private static bool FindBoolFieldValue(TypeDeclaration type, string fieldName)
		{
			var fieldVariable = FindFieldVariable(type, fieldName);
			return WithInitializer(fieldVariable, ex => Convert.ToBoolean(ex.GetText()));
		}

		private static T WithInitializer<T>(VariableInitializer variable, Func<Expression, T> valueGetter)
		{
			if (variable == null || variable.Initializer == null)
				return default(T);
			return valueGetter(variable.Initializer);
		}

		private static VariableInitializer FindFieldVariable(TypeDeclaration type, string fieldName)
		{
			var fieldVariable =
				type.Members.OfType<FieldDeclaration>()
					.SelectMany(f => f.Variables)
					.SingleOrDefault(m => String.Compare(m.Name, fieldName, IgnoreCase) == 0);
			return fieldVariable;
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

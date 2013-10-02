using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using ICSharpCode.NRefactory.CSharp;
using Attribute = ICSharpCode.NRefactory.CSharp.Attribute;

namespace Umbraco.CodeGen
{

	public class DocumentTypeXmlGenerator
	{
		private const StringComparison IgnoreCase = StringComparison.OrdinalIgnoreCase;
		private readonly CodeGeneratorConfiguration configuration;
		private readonly IEnumerable<DataTypeDefinition> dataTypes;
		private readonly CSharpParser parser = new CSharpParser();

		public DocumentTypeXmlGenerator(CodeGeneratorConfiguration configuration, IEnumerable<DataTypeDefinition> dataTypes)
		{
			this.configuration = configuration;
			this.dataTypes = dataTypes;
		}

		public IEnumerable<XDocument> Generate(TextReader reader)
		{
			var tree = parser.Parse(reader);
			ValidateTree(tree);
			return FindTypes(tree).Select(Generate);
		}

		private static void ValidateTree(SyntaxTree tree)
		{
			if (tree.Errors.Count > 0)
				throw new AnalysisException(tree);
		}

		private static IEnumerable<TypeDeclaration> FindTypes(AstNode tree)
		{
			return tree.Descendants.OfType<TypeDeclaration>();
		}

		private XDocument Generate(TypeDeclaration type)
		{
			return new XDocument(
				new XElement(
					"DocumentType",
					new XElement("Info", GenerateInfo(type)),
					new XElement("Structure", GenerateStructure(type)),
					new XElement("GenericProperties", GenerateProperties(type)),
					new XElement("Tabs", GenerateTabs(FindTabNames(type)))
				)
			);
		}

		private IEnumerable<XElement> GenerateInfo(TypeDeclaration type)
		{
			return new[]
			{
				new XElement("Name", AttributeValue(type, "DisplayName", type.Name.PascalCase())),
				new XElement("Alias", type.Name.CamelCase()),
				new XElement("Icon", FindStringFieldValue(type, "icon", "Folder.gif")),
				new XElement("Thumbnail", FindStringFieldValue(type, "thumbnail", "Folder.png")),
				new XElement("Description", AttributeValue(type, "Description")),
				new XElement("AllowAtRoot", FindBoolFieldValue(type, "allowAtRoot")),
				new XElement("Master", FindMaster(type)),
				new XElement("AllowedTemplates", GenerateAllowedTemplates(type)),
				new XElement("DefaultTemplate", FindStringFieldValue(type, "defaultTemplate"))
			};
		}

		private IEnumerable<XElement> GenerateProperties(TypeDeclaration type)
		{
			return FindProperties(type).Select(GenerateProperty);
		}

		private XElement GenerateProperty(PropertyDeclaration prop)
		{
			return new XElement(
				"GenericProperty",
				new XElement("Name", AttributeValue(prop, "DisplayName")),
				new XElement("Alias", prop.Name.CamelCase()),
				new XElement("Type", FindDataTypeDefinitionId(prop)),
				new XElement("Definition", AttributeValue(prop, "DataType", Guid.Empty.ToString())),
				new XElement("Tab", AttributeValue(prop, "Category", "")),
				new XElement("Mandatory", (FindAttribute(prop.Attributes, "Required") != null).ToString().PascalCase()),
				new XElement("Validation", AttributeValue(prop, "RegularExpression")),
				new XElement("Description", new XCData(AttributeValue(prop, "Description")))
			);
		}

		private static IEnumerable<XElement> GenerateTabs(IEnumerable<string> tabNames)
		{
			return tabNames.Select(tab => 
				new XElement("Tab",
					new XElement("Id", "0"),
					new XElement("Caption", tab)
					)
				);
		}

		private static IEnumerable<XElement> GenerateStructure(TypeDeclaration type)
		{
			return FindTypeArrayValue(type, "structure").Select(typeName => 
				new XElement("DocumentType", typeName)
			);
		}

		private static IEnumerable<XElement> GenerateAllowedTemplates(TypeDeclaration type)
		{
			return FindStringArrayValue(type, "allowedTemplates").Select(template => 
				new XElement("Template", template)
			);
		}

		private string FindMaster(TypeDeclaration type)
		{
			var baseType = type.BaseTypes.FirstOrDefault() as SimpleType;
			if (baseType == null || baseType.Identifier == configuration.BaseClass)
				return null;
			return baseType.Identifier;
		}

		private string FindDataTypeDefinitionId(EntityDeclaration prop)
		{
			return dataTypes.Where(dt => dt.DefinitionId == AttributeValue(prop, "DataType", Guid.Empty.ToString())).Select(dt => dt.DataTypeId).SingleOrDefault();
		}

		private static IEnumerable<string> FindTabNames(TypeDeclaration type)
		{
			return FindProperties(type).Select(p => AttributeValue(p, "Category", "")).Distinct();
		}

		private static IEnumerable<PropertyDeclaration> FindProperties(TypeDeclaration type)
		{
			return type.Descendants.OfType<PropertyDeclaration>();
		}

		private static string FindStringFieldValue(TypeDeclaration type, string fieldName, string defaultValue = null)
		{
			var fieldVariable = FindFieldVariable(type, fieldName);
			return WithInitializer(fieldVariable, ex => ex.GetText().Trim('"'), defaultValue);
		}

		private static IEnumerable<string> FindStringArrayValue(TypeDeclaration type, string fieldName)
		{
			var fieldVariable = FindFieldVariable(type, fieldName);
			return WithInitializer(fieldVariable, ex =>
				((ArrayCreateExpression)ex).Initializer.Elements
					.OfType<PrimitiveExpression>()
					.Select(e => e.Value as string)
					.ToArray(),
				new string[0]
			);
		}

		private static IEnumerable<string> FindTypeArrayValue(TypeDeclaration type, string fieldName)
		{
			var fieldVariable = FindFieldVariable(type, fieldName);
			return WithInitializer(fieldVariable, ex =>
				((ArrayCreateExpression)ex).Initializer.Elements
					.OfType<TypeOfExpression>()
					.Select(e => ((SimpleType)e.Type).Identifier)
					.ToArray(),
				new string[0]
			);
		}

		private static string FindBoolFieldValue(TypeDeclaration type, string fieldName)
		{
			var fieldVariable = FindFieldVariable(type, fieldName);
			return WithInitializer(fieldVariable, ex => Convert.ToBoolean(ex.GetText()).ToString().PascalCase(), "False");
		}

		private static T WithInitializer<T>(VariableInitializer variable, Func<Expression, T> valueGetter, T defaultValue)
		{
			if (variable == null || variable.Initializer == null)
				return defaultValue;
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

		private static string AttributeValue(EntityDeclaration entity, string attributeName, string defaultValue = null)
		{
			var attribute = FindAttribute(entity.Attributes, attributeName);
			return AttributeValueOrDefault(attribute, defaultValue);
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
				.SingleOrDefault(att => ((SimpleType)att.Type).Identifier == attributeName);
		}
	}

	public class AnalysisException : Exception
	{
		public IEnumerable<string> Errors { get; private set; }

		public AnalysisException(SyntaxTree tree)
			: base("Errors in code analysis. See the Errors collection for details.")
		{
			Errors = tree.Errors.Select(e => e.Region.BeginLine + ": " + e.Message);
		}
	}
}
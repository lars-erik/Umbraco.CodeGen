using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using ICSharpCode.NRefactory.CSharp;
using Umbraco.CodeGen.Definitions;
using Umbraco.CodeGen.Parsers;

namespace Umbraco.CodeGen
{
	public class CodeParser : CodeParserBase
	{
		private const StringComparison IgnoreCase = StringComparison.OrdinalIgnoreCase;
		private readonly ContentTypeConfiguration configuration;
		private readonly IEnumerable<DataTypeDefinition> dataTypes;
	    private readonly ParserFactory parserFactory;
	    private readonly CSharpParser parser = new CSharpParser();

		public CodeParser(
            ContentTypeConfiguration configuration, 
            IEnumerable<DataTypeDefinition> dataTypes,
            ParserFactory parserFactory
        )
		{
			this.configuration = configuration;
			this.dataTypes = dataTypes;
		    this.parserFactory = parserFactory;
		}

		public IEnumerable<ContentType> Parse(TextReader reader)
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

		private ContentType Generate(TypeDeclaration type)
		{
		    ContentTypeCodeParser typedParser = parserFactory.Create(configuration);
		    ContentType contentType = typedParser.Parse(type);

		    return contentType;
		    //return new XDocument(
		    //    new XElement(
		    //        configuration.ContentTypeName,
		    //        new XElement("Info", GenerateInfo(type)),
		    //        new XElement("Structure", GenerateStructure(type)),
		    //        new XElement("GenericProperties", GenerateProperties(type)),
		    //        new XElement("Tabs", GenerateTabs(FindTabNames(type)))
		    //    )
		    //);
		}

		private IEnumerable<XElement> GenerateInfo(TypeDeclaration type)
		{
			var infoElements = new List<XElement>
			{
				new XElement("Name", AttributeValue(type, "DisplayName", type.Name.PascalCase())),
				new XElement("Alias", type.Name.CamelCase()),
				new XElement("Icon", StringFieldValue(type, "icon", "Folder.gif")),
				new XElement("Thumbnail", StringFieldValue(type, "thumbnail", "Folder.png")),
				new XElement("Description", AttributeValue(type, "Description")),
				new XElement("AllowAtRoot", BoolFieldValue(type, "allowAtRoot")),
				new XElement("Master", FindMaster(type, configuration)),
			};
			if (configuration.ContentTypeName == "DocumentType")
			{
				infoElements.AddRange(new[]{
					new XElement("AllowedTemplates", GenerateAllowedTemplates(type)),
					new XElement("DefaultTemplate", StringFieldValue(type, "defaultTemplate"))
				});
			}
			return infoElements;
		}

		private IEnumerable<XElement> GenerateProperties(TypeDeclaration type)
		{
			return FindProperties(type).Select(GenerateProperty);
		}

		private XElement GenerateProperty(PropertyDeclaration prop)
		{
			return new XElement(
				"GenericProperty",
				new XElement("Name", AttributeValue(prop, "DisplayName", prop.Name.PascalCase())),
				new XElement("Alias", prop.Name.CamelCase()),
				new XElement("Type", FindDataTypeDefinitionId(prop, dataTypes)),
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

		private IEnumerable<XElement> GenerateStructure(TypeDeclaration type)
		{
			return FindTypeArrayValue(type, "structure").Select(typeName => 
				new XElement(configuration.ContentTypeName, typeName)
			);
		}

		private static IEnumerable<XElement> GenerateAllowedTemplates(TypeDeclaration type)
		{
			return StringArrayValue(type, "allowedTemplates").Select(template => 
				new XElement("Template", template)
			);
		}

		private static IEnumerable<string> FindTabNames(TypeDeclaration type)
		{
			return FindProperties(type)
				.Select(p => AttributeValue(p, "Category", ""))
				.Where(name => !String.IsNullOrWhiteSpace(name))
				.Distinct();
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
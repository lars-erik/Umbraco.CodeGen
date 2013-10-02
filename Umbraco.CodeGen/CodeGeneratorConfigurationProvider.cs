using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Umbraco.CodeGen
{
	public class CodeGeneratorConfigurationProvider : IConfigurationProvider
	{
		private const string DefaultTypeMapping = "String";
		private readonly string inputFileContent;
		private CodeGeneratorConfiguration configuration;


		public CodeGeneratorConfigurationProvider(string inputFileContent)
		{
			this.inputFileContent = inputFileContent;
		}

		public CodeGeneratorConfiguration GetConfiguration()
		{
			if (configuration == null)
				configuration = LoadConfiguration();

			return configuration;
		}

		private CodeGeneratorConfiguration LoadConfiguration()
		{
			var doc = XDocument.Parse(inputFileContent);
			var root = doc.Element("CodeGenerator");
			if (root == null)
				throw new Exception("No CodeGenerator element at root");

			string defaultTypeMapping;
			Dictionary<string, string> typeMappings;

			var typeMappingNode = root.Element("TypeMappings");
			if (typeMappingNode != null)
			{
				defaultTypeMapping = typeMappingNode.Attributes("Default").Select(a => a.Value).SingleOrDefault() ?? DefaultTypeMapping;
				typeMappings = typeMappingNode.Descendants("TypeMapping")
					.Select(e => new {DataType = e.Attribute("DataTypeId").Value, Type = e.Attribute("Type").Value})
					.ToDictionary(a => a.DataType, a => a.Type);
			}
			else
			{
				defaultTypeMapping = DefaultTypeMapping;
				typeMappings = new Dictionary<string, string>();
			}

			return new CodeGeneratorConfiguration
			{
				BaseClass = AttributeValue(root, "BaseClass"),
				GenerateClasses = Convert.ToBoolean(AttributeValue(root, "GenerateClasses", "false")),
				GenerateXml = Convert.ToBoolean(AttributeValue(root, "GenerateXml", "false")),
				ModelPath = AttributeValue(root, "ModelPath", "Models"),
				OverwriteReadOnly = Convert.ToBoolean(AttributeValue(root, "OverwriteReadOnly", "false")),
				RemovePrefix = AttributeValue(root, "RemovePrefix"),
				DefaultTypeMapping = defaultTypeMapping,
				TypeMappings = typeMappings,
			};
		}

		private static string AttributeValue(XElement root, string attributeName, string defaultValue = null)
		{
			return root.Attributes(attributeName).Select(a => a.Value).DefaultIfEmpty(defaultValue).SingleOrDefault();
		}
	}
}
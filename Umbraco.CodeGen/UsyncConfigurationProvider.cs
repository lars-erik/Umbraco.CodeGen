using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Umbraco.CodeGen
{
	public class UsyncConfigurationProvider : IConfigurationProvider
	{
		private const string DefaultTypeMapping = "String";
		private readonly string inputFileContent;
		private string removePrefix;
		private XElement uSyncNode;
		private Dictionary<string, string> typeMappings;
		private string defaultTypeMapping;
		private CodeGeneratorConfiguration configuration;
		private string customBaseClass;


		public UsyncConfigurationProvider(string inputFileContent)
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
			var root = doc.Element("CodeGen");
			if (root == null)
				throw new Exception("No CodeGen element at root");

			uSyncNode = root.Element("USync");
			if (uSyncNode == null)
				throw new Exception("No USync node below CodeGen element");
			
			removePrefix = uSyncNode.Attributes("removePrefix").Select(a => a.Value).FirstOrDefault();
			customBaseClass = uSyncNode.Attributes("baseClass").Select(a => a.Value).FirstOrDefault();

			var typeMappingNode = root.Element("TypeMappings");
			if (typeMappingNode != null)
			{
				defaultTypeMapping = typeMappingNode.Attributes("default").Select(a => a.Value).SingleOrDefault() ?? DefaultTypeMapping;
				typeMappings = typeMappingNode.Descendants("TypeMapping")
					.Select(e => new {DataType = e.Attribute("dataType").Value, Type = e.Attribute("type").Value})
					.ToDictionary(a => a.DataType, a => a.Type);
			}
			else
			{
				defaultTypeMapping = DefaultTypeMapping;
				typeMappings = new Dictionary<string, string>();
			}

			return new CodeGeneratorConfiguration
			{
				RemovePrefix = removePrefix,
				DefaultTypeMapping = defaultTypeMapping,
				TypeMappings = typeMappings,
				CustomBaseClass = customBaseClass
			};
		}
	}

	public class CodeGeneratorConfiguration
	{
		public string RemovePrefix { get; set; }
		public string DefaultTypeMapping { get; set; }
		public Dictionary<string, string> TypeMappings { get; set; }
		public string CustomBaseClass { get; set; }

		public string GetTypeName(PropertyDefinition property)
		{
			var typeName = TypeMappings.ContainsKey(property.TypeId)
				? TypeMappings[property.TypeId]
				: DefaultTypeMapping;
			return typeName;
		}
	}
}
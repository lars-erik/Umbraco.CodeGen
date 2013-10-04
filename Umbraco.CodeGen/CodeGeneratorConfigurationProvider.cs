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
			var documentTypes = root.Element("DocumentTypes");
			if (documentTypes == null)
				throw new Exception("No CodeGenerator/DocumentTypes element");
			var mediaTypes = root.Element("MediaTypes");
			if (mediaTypes == null)
				throw new Exception("No CodeGenerator/MediaTypes element");

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

			var config = new CodeGeneratorConfiguration
			{
				OverwriteReadOnly = Convert.ToBoolean(AttributeValue(root, "OverwriteReadOnly", "false")),
				DefaultTypeMapping = defaultTypeMapping,
				TypeMappings = typeMappings,
			};
			config.DocumentTypes = GetContentTypeConfiguration(documentTypes, config, "DocumentType");
			config.MediaTypes = GetContentTypeConfiguration(mediaTypes, config, "MediaType");
			return config;
		}

		private ContentTypeConfiguration GetContentTypeConfiguration(
			XElement contentTypeConfiguration, 
			CodeGeneratorConfiguration config,
			string contentTypeName)
		{
			return new ContentTypeConfiguration(config)
			{
				BaseClass = AttributeValue(contentTypeConfiguration, "BaseClass"),
				GenerateClasses = Convert.ToBoolean(AttributeValue(contentTypeConfiguration, "GenerateClasses", "false")),
				GenerateXml = Convert.ToBoolean(AttributeValue(contentTypeConfiguration, "GenerateXml", "false")),
				ModelPath = AttributeValue(contentTypeConfiguration, "ModelPath", "Models"),
				Namespace = AttributeValue(contentTypeConfiguration, "Namespace"),
				RemovePrefix = AttributeValue(contentTypeConfiguration, "RemovePrefix"),
				ContentTypeName = contentTypeName
			};
		}

		private static string AttributeValue(XElement root, string attributeName, string defaultValue = null)
		{
			return root.Attributes(attributeName).Select(a => a.Value).DefaultIfEmpty(defaultValue).SingleOrDefault();
		}
	}
}
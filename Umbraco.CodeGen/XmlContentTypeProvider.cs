using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace Umbraco.CodeGen
{
	public class XmlContentTypeProvider : IContentTypeProvider
	{
		private readonly string inputFileContent;
		private readonly string path;
		private string uSyncPath;
		private string removePrefix;
		private string inputFolderPath;
		private XElement uSyncNode;
		private List<ContentTypeDefinition> contentTypeDefinitions;

		public XmlContentTypeProvider(string path, string inputFileContent)
		{
			this.path = path;
			this.inputFileContent = inputFileContent;
		}

		public IEnumerable<ContentTypeDefinition> GetContentTypeDefinitions()
		{
			if (contentTypeDefinitions != null)
				return contentTypeDefinitions;

			LoadInputConfiguration();
			var files = FindFiles();
			contentTypeDefinitions = new List<ContentTypeDefinition>();
			foreach(var nodeType in files.Keys)
				foreach(var file in files[nodeType])
					contentTypeDefinitions.Add(CreateContentType(nodeType, file));
			return contentTypeDefinitions;
		}

		private void LoadInputConfiguration()
		{
			var doc = XDocument.Parse(inputFileContent);
			var root = doc.Element("CodeGen");
			uSyncNode = root.Element("USync");
			uSyncPath = uSyncNode.Attributes("path").Single().Value;
			removePrefix = uSyncNode.Attributes("removePrefix").Select(a => a.Value).SingleOrDefault();
			inputFolderPath = Path.GetDirectoryName(path);
		}

		private Dictionary<string, string[]> FindFiles()
		{
			var nodeTypes = new[] { "DocumentType", "MediaType" };
			var files = new Dictionary<string, string[]>();
			foreach (var nodeType in nodeTypes)
			{
				var absoluteUSyncPath = Path.Combine(inputFolderPath, uSyncPath, nodeType);
				files.Add(nodeType, Directory.GetFiles(absoluteUSyncPath, "def.config", SearchOption.AllDirectories));
			}
			return files;
		}

		private ContentTypeDefinition CreateContentType(string nodeType, string file)
		{
			var typeNode = GetTypeNode(nodeType, file);
			var infoNode = typeNode.Element("Info");
			var className = infoNode.Element("Alias").Value;
			var baseClassName = infoNode.Elements("Master").Select(e => e.Value).SingleOrDefault();
			var properties = CreateProperties(typeNode);
			className = className.RemovePrefix(removePrefix).PascalCase();
			if (HasPropertyWithSameName(properties, className))
				className += "Class";

			return new ContentTypeDefinition
			{
				ClassName = className,
				BaseClassName = baseClassName,
				Properties = properties
			};
		}

		private List<PropertyDefinition> CreateProperties(XElement typeNode)
		{
			var propertiesNode = typeNode.Element("GenericProperties");
			var propertyNodes = propertiesNode.Elements("GenericProperty");
			return propertyNodes.Select(CreateProperty).ToList();
		}

		private PropertyDefinition CreateProperty(XElement propertyNode)
		{
			var name = propertyNode.Element("Alias").Value;
			var typeId = propertyNode.Element("Type").Value;
			return new PropertyDefinition
			{
				Name = name,
				TypeId = typeId
			};
		}

		private static bool HasPropertyWithSameName(IEnumerable<PropertyDefinition> properties, string className)
		{
			return properties.Any(p => String.Compare(p.Name, className, StringComparison.OrdinalIgnoreCase) == 0);
		}

		private static XElement GetTypeNode(string nodeType, string file)
		{
			var fileDoc = XDocument.Load(file);
			var typeNode = fileDoc
				.Element(nodeType);
			return typeNode;
		}
	}
}

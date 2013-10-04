using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml.Linq;
using System.Xml.XPath;
using jumps.umbraco.usync.helpers;
using Microsoft.CSharp;
using Umbraco.Core;

namespace Umbraco.CodeGen.Integration
{
	public class ApplicationEvents : IApplicationEventHandler
	{
		private IEnumerable<DataTypeDefinition> dataTypes;
		private CodeGeneratorConfiguration configuration;
		private USyncConfiguration uSyncConfiguration;
		private USyncDataTypeProvider dataTypesProvider;

		private Dictionary<string, string> paths = new Dictionary<string, string>(); 

		public void OnApplicationInitialized(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
		{
		}

		public void OnApplicationStarting(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
		{
			var uSyncConfigurationProvider = new USyncConfigurationProvider(HttpContext.Current.Server.MapPath("~/config/uSyncSettings.config"), new HttpContextPathResolver());
			var configurationProvider = new UmbracoCodeGeneratorConfigurationProvider(HttpContext.Current.Server.MapPath("~/config/CodeGen.config"));

			uSyncConfiguration = uSyncConfigurationProvider.GetConfiguration();
			
			dataTypesProvider = new USyncDataTypeProvider(uSyncConfiguration.USyncFolder);
	
			dataTypes = dataTypesProvider.GetDataTypes();
			configuration = configurationProvider.GetConfiguration();

			paths.Add("DocumentType", HttpContext.Current.Server.MapPath(configuration.DocumentTypes.ModelPath));
			paths.Add("MediaType", HttpContext.Current.Server.MapPath(configuration.MediaTypes.ModelPath));

			XmlDoc.Saved += OnDocumentTypeSaved;

			if (configuration.DocumentTypes.GenerateXml)
				GenerateXml(configuration.DocumentTypes);

			if (configuration.MediaTypes.GenerateXml)
				GenerateXml(configuration.MediaTypes);
		}

		private void GenerateXml(ContentTypeConfiguration contentTypeConfiguration)
		{
			var xmlGenerator = new DocumentTypeXmlGenerator(contentTypeConfiguration, dataTypes);
			var modelPath = HttpContext.Current.Server.MapPath(contentTypeConfiguration.ModelPath);
			var files = Directory.GetFiles(modelPath, "*.cs");
			var documents = new List<XDocument>();
			foreach(var file in files)
			{
				using (var reader = File.OpenText(file))
				{
					documents.AddRange(xmlGenerator.Generate(reader));
				}
			}

			var documentTypeRoot = Path.Combine(uSyncConfiguration.USyncFolder, contentTypeConfiguration.ContentTypeName);
			if (!Directory.Exists(documentTypeRoot))
				Directory.CreateDirectory(documentTypeRoot);
			WriteDocuments(contentTypeConfiguration, documents, documentTypeRoot, "");
		}

		private void WriteDocuments(
			ContentTypeConfiguration contentTypeConfiguration,
			IEnumerable<XDocument> documents, 
			string path, 
			string baseClass
			)
		{
			var contentTypeName = contentTypeConfiguration.ContentTypeName;
			var docsAtLevel = documents.Where(doc => doc.Element(contentTypeName).Element("Info").Element("Master").Value == baseClass);
			foreach (var doc in docsAtLevel)
			{
				var alias = doc.Element(contentTypeName).Element("Info").Element("Alias").Value;
				var directoryPath = Path.Combine(path, alias);
				if (!Directory.Exists(directoryPath))
					Directory.CreateDirectory(directoryPath);
				var defPath = Path.Combine(directoryPath, "def.config");

				if (configuration.OverwriteReadOnly && File.Exists(defPath))
					File.SetAttributes(defPath, File.GetAttributes(defPath) & ~FileAttributes.ReadOnly);

				doc.Save(defPath);

				WriteDocuments(contentTypeConfiguration, documents, directoryPath, alias);
			}
		}

		private void OnDocumentTypeSaved(XmlDocFileEventArgs e)
		{
			if (!e.Path.Contains("DocumentType") && !e.Path.Contains("MediaType"))
				return;

			var typeConfig = e.Path.Contains("DocumentType")
				? configuration.DocumentTypes
				: configuration.MediaTypes;

			if (!typeConfig.GenerateClasses)
				return;

			var codeDomProvider = new CSharpCodeProvider();
	
			var doc = XDocument.Load(e.Path);
			var classGenerator = new ContentTypeCodeGenerator(typeConfig, doc, codeDomProvider);
			var modelPath = paths[typeConfig.ModelPath];
			var path = Path.Combine(modelPath, doc.XPathSelectElement("//Info/Alias").Value.PascalCase() + ".cs");
				if (configuration.OverwriteReadOnly && File.Exists(path))
					File.SetAttributes(path, File.GetAttributes(path) & ~FileAttributes.ReadOnly);
			using (var stream = File.CreateText(path))
			{
				classGenerator.BuildCode(stream);
			}
		}

		public void OnApplicationStarted(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
		{
		}
	}

	public class HttpContextPathResolver : IRelativePathResolver
	{
		public string Resolve(string relativePath)
		{
			return HttpContext.Current.Server.MapPath(relativePath);
		}
	}
}

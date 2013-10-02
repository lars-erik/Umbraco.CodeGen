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
		private string modelPath;

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
			modelPath = HttpContext.Current.Server.MapPath(configuration.ModelPath);

			XmlDoc.Saved += OnDocumentTypeSaved;

			if (configuration.GenerateXml)
				GenerateXml();
		}

		private void GenerateXml()
		{
			var xmlGenerator = new DocumentTypeXmlGenerator(configuration, dataTypes);
			var modelPath = HttpContext.Current.Server.MapPath(configuration.ModelPath);
			var files = Directory.GetFiles(modelPath, "*.cs");
			var documents = new List<XDocument>();
			foreach(var file in files)
			{
				using (var reader = File.OpenText(file))
				{
					documents.AddRange(xmlGenerator.Generate(reader));
				}
			}

			var documentTypeRoot = Path.Combine(uSyncConfiguration.USyncFolder, "DocumentType");
			if (!Directory.Exists(documentTypeRoot))
				Directory.CreateDirectory(documentTypeRoot);
			WriteDocuments(documents, documentTypeRoot, "");
		}

		private void WriteDocuments(IEnumerable<XDocument> documents, string path, string baseClass)
		{
			var docsAtLevel = documents.Where(doc => doc.Element("DocumentType").Element("Info").Element("Master").Value == baseClass);
			foreach (var doc in docsAtLevel)
			{
				var alias = doc.Element("DocumentType").Element("Info").Element("Alias").Value;
				var directoryPath = Path.Combine(path, alias);
				if (!Directory.Exists(directoryPath))
					Directory.CreateDirectory(directoryPath);
				var defPath = Path.Combine(directoryPath, "def.config");

				if (configuration.OverwriteReadOnly && File.Exists(defPath))
					File.SetAttributes(defPath, File.GetAttributes(defPath) & ~FileAttributes.ReadOnly);

				doc.Save(defPath);

				WriteDocuments(documents, directoryPath, alias);
			}
		}

		private void OnDocumentTypeSaved(XmlDocFileEventArgs e)
		{
			if (!e.Path.Contains("DocumentType"))
				return;

			if (!configuration.GenerateClasses)
				return;

			var codeDomProvider = new CSharpCodeProvider();
	
			var doc = XDocument.Load(e.Path);
			var classGenerator = new ContentTypeCodeGenerator(configuration, doc, codeDomProvider);
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

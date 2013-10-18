using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml.Linq;
using umbraco.cms.businesslogic.datatype.controls;
using Umbraco.CodeGen.Configuration;
using Umbraco.CodeGen.Definitions;
using Umbraco.CodeGen.Generators;
using Umbraco.CodeGen.Parsers;
using jumps.umbraco.usync.helpers;
using Umbraco.Core;

namespace Umbraco.CodeGen.Integration
{
	public class ApplicationEvents : IApplicationEventHandler
	{
		private IEnumerable<DataTypeDefinition> dataTypes;
		private CodeGeneratorConfiguration configuration;
		private USyncConfiguration uSyncConfiguration;
		private USyncDataTypeProvider dataTypesProvider;

		private readonly Dictionary<string, string> paths = new Dictionary<string, string>();
	    private readonly ContentTypeSerializer serializer = new ContentTypeSerializer();
	    private CodeGeneratorFactory generatorFactory;
	    private ParserFactory parserFactory;

	    public void OnApplicationInitialized(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
		{
		}

		public void OnApplicationStarting(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
		{
			var uSyncConfigurationProvider = new USyncConfigurationProvider(HttpContext.Current.Server.MapPath("~/config/uSyncSettings.config"), new HttpContextPathResolver());
			var configurationProvider = new CodeGeneratorConfigurationFileProvider(HttpContext.Current.Server.MapPath("~/config/CodeGen.config"));

			uSyncConfiguration = uSyncConfigurationProvider.GetConfiguration();
			
			dataTypesProvider = new USyncDataTypeProvider(uSyncConfiguration.USyncFolder);
	
			dataTypes = dataTypesProvider.GetDataTypes();
			configuration = configurationProvider.GetConfiguration();

		    generatorFactory = CreateFactory<CodeGeneratorFactory>(configuration.GeneratorFactory);
		    parserFactory = CreateFactory<ParserFactory>(configuration.ParserFactory);

			paths.Add("DocumentType", HttpContext.Current.Server.MapPath(configuration.DocumentTypes.ModelPath));
			paths.Add("MediaType", HttpContext.Current.Server.MapPath(configuration.MediaTypes.ModelPath));

			XmlDoc.Saved += OnDocumentTypeSaved;

			if (configuration.DocumentTypes.GenerateXml)
				GenerateXml(configuration.DocumentTypes);
			if (configuration.MediaTypes.GenerateXml)
				GenerateXml(configuration.MediaTypes);
		}

	    private T CreateFactory<T>(string typeName)
	    {
	        try
	        {
	            var factoryType = Type.GetType(typeName);
                if (factoryType == null)
                    throw new Exception(String.Format("Type {0} not found", typeName));
	            return (T) Activator.CreateInstance(factoryType);
	        }
	        catch (Exception ex)
	        {
                throw new Exception(String.Format("Invalid factory '{0}'", typeName), ex);
	        }
	    }

	    private void GenerateXml(ContentTypeConfiguration contentTypeConfiguration)
		{
			var parser = new CodeParser(contentTypeConfiguration, dataTypes, parserFactory);
		    var modelPath = HttpContext.Current.Server.MapPath(contentTypeConfiguration.ModelPath);
			if (!Directory.Exists(modelPath))
				Directory.CreateDirectory(modelPath);
			var files = Directory.GetFiles(modelPath, "*.cs");
			var documents = new List<XDocument>();
			foreach(var file in files)
			{
				using (var reader = File.OpenText(file))
				{
				    var contentType = parser.Parse(reader).FirstOrDefault();
                    if (contentType != null)
					    documents.Add(XDocument.Parse(serializer.Serialize(contentType)));
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

		    ContentType contentType;
		    using (var reader = File.OpenText(e.Path))
			    contentType = serializer.Deserialize(reader);

			var modelPath = paths[typeConfig.ContentTypeName];
			if (!Directory.Exists(modelPath))
				Directory.CreateDirectory(modelPath);
			var path = Path.Combine(modelPath, contentType.Info.Alias.PascalCase() + ".cs");
				if (configuration.OverwriteReadOnly && File.Exists(path))
					File.SetAttributes(path, File.GetAttributes(path) & ~FileAttributes.ReadOnly);

            var classGenerator = new CodeGenerator(typeConfig, dataTypesProvider, generatorFactory);
            using (var stream = File.CreateText(path))
                classGenerator.Generate(contentType, stream);
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

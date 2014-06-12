using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml.Linq;
using jumps.umbraco.usync;
using umbraco.cms.businesslogic.datatype.controls;
using Umbraco.CodeGen.Configuration;
using Umbraco.CodeGen.Definitions;
using Umbraco.CodeGen.Generators;
using Umbraco.CodeGen.Integration.Api;
using Umbraco.CodeGen.Parsers;
using jumps.umbraco.usync.helpers;
using Umbraco.Core;
using Umbraco.Core.Logging;

namespace Umbraco.CodeGen.Integration
{
	public class ApplicationEvents : IApplicationEventHandler
	{
		private readonly Dictionary<string, string> paths = new Dictionary<string, string>();
	    private readonly ContentTypeSerializer serializer = new ContentTypeSerializer();
	    private CodeGeneratorFactory generatorFactory;
	    private ParserFactory parserFactory;

	    private DateTime globalStart;
	    private DateTime itemStart;

	    public void OnApplicationInitialized(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
		{
		}

		public void OnApplicationStarting(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
		{
            try
            {
                Configuration.Load();

                generatorFactory = CreateFactory<CodeGeneratorFactory>(Configuration.CodeGen.GeneratorFactory);
                parserFactory = CreateFactory<ParserFactory>(Configuration.CodeGen.ParserFactory);

                paths.Add("DocumentType", HttpContext.Current.Server.MapPath(Configuration.CodeGen.DocumentTypes.ModelPath));
                paths.Add("MediaType", HttpContext.Current.Server.MapPath(Configuration.CodeGen.MediaTypes.ModelPath));

			    XmlDoc.Saved += OnDocumentTypeSaved;

                if (Configuration.CodeGen.DocumentTypes.GenerateXml)
		        {
		            globalStart = DateTime.Now;
		            LogHelper.Info<CodeGenerator>(() => "Parsing typed documenttype models");
                    GenerateXml(Configuration.CodeGen.DocumentTypes);
                    LogHelper.Info<CodeGenerator>(() => String.Format("Parsing typed documenttype models done. Took {0}", DateTime.Now - globalStart));
		        }
                if (Configuration.CodeGen.MediaTypes.GenerateXml)
		        {
                    globalStart = DateTime.Now;
                    LogHelper.Info<CodeGenerator>(() => "Parsing typed mediatype models");
                    GenerateXml(Configuration.CodeGen.MediaTypes);
                    LogHelper.Info<CodeGenerator>(() => String.Format("Parsing typed mediatype models done. Took {0}", DateTime.Now - globalStart));
		        }

                PreviewController.RegisterMenu();
            }
            catch(Exception ex)
	        {
	            LogHelper.Error<CodeGenerator>("Initialization failed", ex);
    		}
		}

	    internal static T CreateFactory<T>(string typeName)
	    {
	        try
	        {
	            var factoryType = Type.GetType(typeName);
	            if (factoryType == null)
	                factoryType = Type.GetType(String.Format("{0}, Umbraco.CodeGen", typeName));
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
            var parser = new CodeParser(contentTypeConfiguration, Configuration.DataTypes, parserFactory);
		    var modelPath = HttpContext.Current.Server.MapPath(contentTypeConfiguration.ModelPath);
			if (!Directory.Exists(modelPath))
				Directory.CreateDirectory(modelPath);
			var files = Directory.GetFiles(modelPath, "*.cs");
			var documents = new List<XDocument>();
			foreach(var file in files)
			{
			    itemStart = DateTime.Now;
                LogHelper.Debug<CodeGenerator>(() => String.Format("Parsing file {0}", file));
				using (var reader = File.OpenText(file))
				{
				    var contentType = parser.Parse(reader).FirstOrDefault();
                    if (contentType != null)
					    documents.Add(XDocument.Parse(serializer.Serialize(contentType)));
				}
                LogHelper.Debug<CodeGenerator>(() => String.Format("Parsing file {0} done. Took {1}", file, DateTime.Now - itemStart));
            }

            var documentTypeRoot = Path.Combine(Configuration.USync.USyncFolder, contentTypeConfiguration.ContentTypeName);
			if (!Directory.Exists(documentTypeRoot))
				Directory.CreateDirectory(documentTypeRoot);
	        itemStart = DateTime.Now;
            LogHelper.Info<CodeGenerator>(() => "Writing uSync definitions");
			WriteDocuments(contentTypeConfiguration, documents, documentTypeRoot, "");
            LogHelper.Info<CodeGenerator>(() => String.Format("Writing uSync definitions done. Took {0}", DateTime.Now - itemStart));
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

				if (Configuration.CodeGen.OverwriteReadOnly && File.Exists(defPath))
					File.SetAttributes(defPath, File.GetAttributes(defPath) & ~FileAttributes.ReadOnly);

				doc.Save(defPath);

				WriteDocuments(contentTypeConfiguration, documents, directoryPath, alias);
			}
		}

		private void OnDocumentTypeSaved(XmlDocFileEventArgs e)
		{
            var inputPath = e.Path;
            try
            {
                if (inputPath.Contains("DocumentType") || inputPath.Contains("MediaType"))
                    GenerateModels(inputPath);

                if (inputPath.Contains("DataTypeDefinition"))
                    EnsureReloadDataTypes();
            }
            catch (Exception ex)
            {
                LogHelper.Error<CodeGenerator>("Generating typed model failed", ex);
            }
        }

	    private void EnsureReloadDataTypes()
	    {
            Configuration.DataTypesProvider.Reset();
        }

	    private void GenerateModels(string inputPath)
	    {
	        var typeConfig = inputPath.Contains("DocumentType")
	            ? Configuration.CodeGen.DocumentTypes
	            : Configuration.CodeGen.MediaTypes;

	        if (!typeConfig.GenerateClasses)
	            return;

	        itemStart = DateTime.Now;
	        LogHelper.Debug<CodeGenerator>(() => String.Format("Content type {0} saved, generating typed model", inputPath));

	        ContentType contentType;
	        using (var reader = File.OpenText(inputPath))
	            contentType = serializer.Deserialize(reader);

	        LogHelper.Info<CodeGenerator>(() => String.Format("Generating typed model for {0}", contentType.Alias));

	        var modelPath = paths[typeConfig.ContentTypeName];
	        if (!Directory.Exists(modelPath))
	            Directory.CreateDirectory(modelPath);
	        var path = Path.Combine(modelPath, contentType.Info.Alias.PascalCase() + ".cs");
	        if (Configuration.CodeGen.OverwriteReadOnly && File.Exists(path))
	            File.SetAttributes(path, File.GetAttributes(path) & ~FileAttributes.ReadOnly);

	        var classGenerator = new CodeGenerator(typeConfig, Configuration.DataTypesProvider, generatorFactory);
	        using (var stream = File.CreateText(path))
	            classGenerator.Generate(contentType, stream);

	        LogHelper.Debug<CodeGenerator>(
	            () => String.Format("Typed model for {0} generated. Took {1}", contentType.Alias, DateTime.Now - itemStart));
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

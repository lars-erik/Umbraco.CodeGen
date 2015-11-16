using System;
using System.IO;
using System.Text;
using System.Xml;
using NUnit.Framework;
using Umbraco.CodeGen.Configuration;

namespace Umbraco.CodeGen.Tests.Configuration
{
	[TestFixture]
	public class FileCodeGeneratorConfigurationProviderTests
	{
		[Test]
		public void GetConfiguration_ReturnsConfigurationFromDisk()
		{
		    var path = Path.Combine(Environment.CurrentDirectory, @"..\..\config\codegen.config");
		    var provider = new CodeGeneratorConfigurationFileProvider(path);
			var config = provider.GetConfiguration();

            Assert.AreEqual("Umbraco.CodeGen.Generators.DefaultCodeGeneratorFactory, Umbraco.CodeGen", config.GeneratorFactory); // set
            Assert.AreEqual("Umbraco.CodeGen.Parsers.DefaultParserFactory, Umbraco.CodeGen", config.ParserFactory); // default
            Assert.AreEqual("Umbraco.CodeGen.Generators.InterfaceGeneratorFactory, Umbraco.CodeGen", config.InterfaceFactory); // default
			Assert.AreEqual("SomeBaseClass", config.DocumentTypes.BaseClass);
			Assert.AreEqual("String", config.TypeMappings.DefaultType);
            Assert.AreEqual("0cc0eba1-9960-42c9-bf9b-60e150b429ae", config.TypeMappings.DefaultDefinitionId);
			Assert.AreEqual(true, config.DocumentTypes.GenerateClasses);
			Assert.AreEqual(true, config.DocumentTypes.GenerateXml);
			Assert.AreEqual("Models/DocumentTypes", config.DocumentTypes.ModelPath);
			Assert.AreEqual("MyWeb.Models", config.DocumentTypes.Namespace);
			Assert.AreEqual(false, config.OverwriteReadOnly);
			Assert.AreNotEqual(0, config.TypeMappings.Count);
			Assert.AreEqual("Int32", config.TypeMappings["1413afcb-d19a-4173-8e9a-68288d2a73b8"]);
		}

	    [Test]
	    public void SaveConfiguration_WritesXml()
	    {
            var path = Path.Combine(Environment.CurrentDirectory, @"..\..\config\codegen.config");
            var provider = new CodeGeneratorConfigurationFileProvider(path);
            var config = provider.GetConfiguration();
	        var content = "";

	        using (var reader = File.OpenText(path))
	        {
	            content = reader.ReadToEnd();
	        }

	        var builder = new StringBuilder();
	        var writer = new StringWriter(builder);
	        var xmlWriter = XmlWriter.Create(writer, new XmlWriterSettings {OmitXmlDeclaration = true, Encoding = Encoding.UTF8, Indent = true});
	        CodeGeneratorConfigurationProvider.SerializeConfiguration(config, xmlWriter);

            Console.WriteLine(builder.ToString());

            Assert.AreEqual(content, builder.ToString());
	    }
	}
}

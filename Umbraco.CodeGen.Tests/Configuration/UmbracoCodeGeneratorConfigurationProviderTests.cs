using System;
using System.IO;
using NUnit.Framework;
using Umbraco.CodeGen.Integration;

namespace Umbraco.CodeGen.Tests.Configuration
{
	[TestFixture]
	public class UmbracoCodeGeneratorConfigurationProviderTests
	{
		[Test]
		public void GetConfiguration_ReturnsConfigurationFromDisk()
		{
			var provider =
				new UmbracoCodeGeneratorConfigurationProvider(Path.Combine(Environment.CurrentDirectory,
					@"..\..\config\codegen.config"));
			var config = provider.GetConfiguration();
			Assert.AreEqual("SomeBaseClass", config.BaseClass);
			Assert.AreEqual("string", config.DefaultTypeMapping);
			Assert.AreEqual(true, config.GenerateClasses);
			Assert.AreEqual(true, config.GenerateXml);
			Assert.AreEqual("Models", config.ModelPath);
			Assert.AreEqual(false, config.OverwriteReadOnly);
			Assert.AreEqual("pfx", config.RemovePrefix);
			Assert.AreNotEqual(0, config.TypeMappings.Count);
			Assert.AreEqual("int", config.TypeMappings["1413AFCB-D19A-4173-8E9A-68288D2A73B8"]);
		}
	}
}

using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;
using Umbraco.CodeGen;

namespace Umbraco.CodeGen.Integration
{
	public class USyncConfigurationProvider
	{
		private readonly string uSyncConfigPath;
		private readonly IRelativePathResolver pathResolver;

		public USyncConfigurationProvider(string uSyncConfigPath, IRelativePathResolver pathResolver)
		{
			this.uSyncConfigPath = uSyncConfigPath;
			this.pathResolver = pathResolver;
		}

		public USyncConfiguration GetConfiguration()
		{
			var configuration = new USyncConfiguration();
			if (!File.Exists(uSyncConfigPath))
				return configuration;
			var doc = XDocument.Load(uSyncConfigPath);
			var relativePath = doc.XPathSelectElements("configuration/usync").Select(e => e.AttributeValue("folder")).SingleOrDefault();
			if (!String.IsNullOrEmpty(relativePath))
				configuration.USyncFolder = pathResolver.Resolve(relativePath);
			return configuration;
		}
	}

	public class USyncConfiguration
	{
		public string USyncFolder { get; set; }
	}

	public interface IRelativePathResolver
	{
		string Resolve(string relativePath);
	}
}

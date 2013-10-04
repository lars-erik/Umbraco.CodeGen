using System.Collections.Generic;

namespace Umbraco.CodeGen
{
	public class ContentTypeConfiguration
	{
		private readonly CodeGeneratorConfiguration config;
		public string ModelPath { get; set; }
		public string RemovePrefix { get; set; }
		public string BaseClass { get; set; }
		public bool GenerateClasses { get; set; }
		public bool GenerateXml { get; set; }
		public string Namespace { get; set; }
		public string ContentTypeName { get; set; }

		public string DefaultTypeMapping { get { return config.DefaultTypeMapping; } }
		public Dictionary<string, string> TypeMappings { get { return config.TypeMappings; } }

		public ContentTypeConfiguration(CodeGeneratorConfiguration config)
		{
			this.config = config;
		}
	}

	public class CodeGeneratorConfiguration
	{
		public ContentTypeConfiguration DocumentTypes { get; set; }
		public ContentTypeConfiguration MediaTypes { get; set; }
		public string DefaultTypeMapping { get; set; }
		public Dictionary<string, string> TypeMappings { get; set; }
		public bool OverwriteReadOnly { get; set; }
	}
}
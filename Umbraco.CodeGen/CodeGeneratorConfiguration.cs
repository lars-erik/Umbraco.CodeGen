using System.Collections.Generic;

namespace Umbraco.CodeGen
{
	public class CodeGeneratorConfiguration
	{
		public string ModelPath { get; set; }
		public string RemovePrefix { get; set; }
		public string BaseClass { get; set; }
		public bool GenerateClasses { get; set; }
		public bool GenerateXml { get; set; }
		public string DefaultTypeMapping { get; set; }
		public Dictionary<string, string> TypeMappings { get; set; }
		public bool OverwriteReadOnly { get; set; }

		public string GetTypeName(PropertyDefinition property)
		{
			var typeName = TypeMappings.ContainsKey(property.TypeId)
				? TypeMappings[property.TypeId]
				: DefaultTypeMapping;
			return typeName;
		}
	}
}
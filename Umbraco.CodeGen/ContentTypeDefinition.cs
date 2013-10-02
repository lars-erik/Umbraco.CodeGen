using System.Collections.Generic;

namespace Umbraco.CodeGen
{
	public class ContentTypeDefinition
	{
		public string ClassName { get; set; }
		public string BaseClassName { get; set; }
		public List<PropertyDefinition> Properties { get; set; }
	}
}
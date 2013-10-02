using System;

namespace Umbraco.CodeGen
{
	public class DataTypeDefinition
	{
		public string DefinitionId { get; set; }
		public string DataTypeId { get; set; }
		public string DataTypeName { get; set; }

		public DataTypeDefinition()
		{
		}

		public DataTypeDefinition(string name, string dataTypeId, string definitionId)
		{
			DataTypeName = name;
			DataTypeId = dataTypeId;
			DefinitionId = definitionId;
		}
	}
}

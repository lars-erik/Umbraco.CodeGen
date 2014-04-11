namespace Umbraco.CodeGen.Configuration
{
	public class DataTypeDefinition
	{
		public string DefinitionId { get; set; }
		public string DataTypeId { get; set; }
		public string DataTypeName { get; set; }
        public string DataTypeGuid { get; set; }

	    public DataTypeDefinition()
		{
		}

		public DataTypeDefinition(string name, string dataTypeId, string definitionId, string dataTypeGuid)
		{
			DataTypeName = name;
			DataTypeId = dataTypeId;
			DefinitionId = definitionId;
		    DataTypeGuid = dataTypeGuid;
		}
	}
}

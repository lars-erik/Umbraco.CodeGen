using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace Umbraco.CodeGen.Integration
{
	public class USyncDataTypeProvider : IDataTypeProvider
	{
		private readonly string uSyncPath;

		public USyncDataTypeProvider(string uSyncPath)
		{
			this.uSyncPath = uSyncPath;
		}

		public IEnumerable<DataTypeDefinition> GetDataTypes()
		{
			var dataTypesPath = Path.Combine(uSyncPath, "DataTypeDefinition");
			if (!Directory.Exists(dataTypesPath))
				return new List<DataTypeDefinition>();
			return Directory.GetFiles(dataTypesPath)
				.Select(CreateDefinition)
				.Where(def => def != null);
		}

		private static DataTypeDefinition CreateDefinition(string configPath)
		{
			var doc = XDocument.Load(configPath);
			var dataType = doc.Element("DataType");
			if (dataType == null) return null;
			var name = AttributeValue(dataType, "Name");
			var dataTypeId = AttributeValue(dataType, "Id");
			var definitionId = AttributeValue(dataType, "Definition");
			if (name == null || dataTypeId == null || definitionId == null)
				return null;
			return new DataTypeDefinition(name, dataTypeId, definitionId);
		}

		private static string AttributeValue(XElement dataType, string attributeName)
		{
			var attribute = dataType.Attribute(attributeName);
			if (attribute == null) return null;
			return attribute.Value;
		}
	}
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;
using Umbraco.CodeGen.Configuration;
using Umbraco.Core.PropertyEditors;
using Umbraco.Web;

namespace Umbraco.CodeGen.Integration
{
	public class USyncDataTypeProvider : IDataTypeProvider
	{
		private readonly string uSyncPath;
	    private IEnumerable<DataTypeDefinition> dataTypeDefinitions;

	    public USyncDataTypeProvider(string uSyncPath)
		{
			this.uSyncPath = uSyncPath;
		}

		public IEnumerable<DataTypeDefinition> GetDataTypes()
		{
		    if (dataTypeDefinitions == null)
		    {
		        var dataTypesPath = Path.Combine(uSyncPath, "DataTypeDefinition");
		        if (!Directory.Exists(dataTypesPath))
		            return new List<DataTypeDefinition>();
		        dataTypeDefinitions = Directory.GetFiles(dataTypesPath)
		            .Select(CreateDefinition)
		            .Where(def => def != null);
		    }
		    return dataTypeDefinitions;
		}

		private static DataTypeDefinition CreateDefinition(string configPath)
		{
			var doc = XDocument.Load(configPath);
		    var dataTypeNode = doc.XPathSelectElement("DataType");
			if (dataTypeNode == null) return null;
			var name = AttributeValue(dataTypeNode, "Name");
			var dataTypeId = AttributeValue(dataTypeNode, "Id");
			var definitionId = AttributeValue(dataTypeNode, "Definition");
			if (name == null || dataTypeId == null || definitionId == null)
				return null;

		    var legacyId = LegacyPropertyEditorIdToAliasConverter.GetLegacyIdFromAlias(
		        dataTypeId,
		        LegacyPropertyEditorIdToAliasConverter.NotFoundLegacyIdResponseBehavior.GenerateId
		        );

            return new DataTypeDefinition(name, dataTypeId, definitionId, legacyId.ToString());
		}

		private static string AttributeValue(XElement dataType, string attributeName)
		{
			var attribute = dataType.Attribute(attributeName);
			if (attribute == null) return null;
			return attribute.Value;
		}
	}
}

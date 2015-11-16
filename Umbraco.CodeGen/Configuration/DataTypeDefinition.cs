using System;

namespace Umbraco.CodeGen.Configuration
{
	public class DataTypeDefinition
	{
		public string PropertyEditorAlias { get; set; }
		public string DataTypeName { get; set; }
	    public Type ClrType { get; set; }

	    public DataTypeDefinition()
		{
		}

		public DataTypeDefinition(string name, string propertyEditorAlias, Type clrType)
		{
			DataTypeName = name;
			PropertyEditorAlias = propertyEditorAlias;
		    ClrType = clrType;
		}
	}
}

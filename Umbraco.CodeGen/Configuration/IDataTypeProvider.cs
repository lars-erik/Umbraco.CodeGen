using System.Collections.Generic;

namespace Umbraco.CodeGen.Configuration
{
	public interface IDataTypeProvider
	{
		IEnumerable<DataTypeDefinition> GetDataTypes();
	}
}
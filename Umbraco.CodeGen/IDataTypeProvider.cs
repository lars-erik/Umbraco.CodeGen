using System.Collections.Generic;

namespace Umbraco.CodeGen.Integration
{
	public interface IDataTypeProvider
	{
		IEnumerable<DataTypeDefinition> GetDataTypes();
	}
}
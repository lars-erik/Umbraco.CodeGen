using System.Collections.Generic;

namespace Umbraco.CodeGen
{
	public interface IContentTypeProvider
	{
		IEnumerable<ContentTypeDefinition> GetContentTypeDefinitions();
	}
}
using System.CodeDom;
using System.Collections.Generic;

namespace Umbraco.CodeGen
{
	public interface INamespaceMemberBuilder
	{
		void Configure(CodeGeneratorConfiguration configuration, IEnumerable<ContentTypeDefinition> contentTypes);
		void Build(CodeNamespace ns);
	}
}
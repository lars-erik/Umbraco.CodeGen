using System.CodeDom;
using System.Collections.Generic;

namespace Umbraco.CodeGen
{
	public class StandardImportsBuilder : INamespaceMemberBuilder
	{
		public void Configure(CodeGeneratorConfiguration configuration, IEnumerable<ContentTypeDefinition> contentTypes)
		{
		}

		public void Build(CodeNamespace ns)
		{
			ns.Imports.Add(new CodeNamespaceImport("System"));
			ns.Imports.Add(new CodeNamespaceImport("System.Reflection"));
			ns.Imports.Add(new CodeNamespaceImport("Umbraco.Core.Models"));
		}
	}
}
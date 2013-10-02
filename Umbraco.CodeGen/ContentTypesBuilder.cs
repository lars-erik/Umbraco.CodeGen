using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;

namespace Umbraco.CodeGen
{
	public class ContentTypesBuilder : INamespaceMemberBuilder
	{
		private CodeGeneratorConfiguration configuration;
		private IEnumerable<ContentTypeDefinition> contentTypes;

		public void Configure(CodeGeneratorConfiguration config, IEnumerable<ContentTypeDefinition> types)
		{
			configuration = config;
			contentTypes = types;
		}

		public void Build(CodeNamespace ns)
		{
			var builder = new ContentTypeBuilder();
			foreach(var type in contentTypes)
			{
				builder.Configure(configuration, type);
				builder.Build(ns);
			};
		}
	}
}
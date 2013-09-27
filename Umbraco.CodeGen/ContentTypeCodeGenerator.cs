using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Umbraco.CodeGen
{
	public class ContentTypeCodeGenerator
	{
		private readonly IEnumerable<ContentTypeDefinition> contentTypes;
		private readonly CodeDomProvider codeDomProvider;
		private readonly string nameSpace;
		private readonly CodeGeneratorConfiguration configuration;

		private CodeCompileUnit compileUnit;
		private CodeNamespace ns;
		private CodeTypeReference baseType;

		private readonly List<INamespaceMemberBuilder> builders = new List<INamespaceMemberBuilder>
		{
			new StandardImportsBuilder(),
			new BaseClassBuilder(),
			new ContentTypesBuilder()
		}; 

		public ContentTypeCodeGenerator(
			CodeGeneratorConfiguration configuration, 
			IEnumerable<ContentTypeDefinition> contentTypes,
			CodeDomProvider codeDomProvider,
			string nameSpace
			)
		{
			this.configuration = configuration;
			this.contentTypes = contentTypes;
			this.codeDomProvider = codeDomProvider;
			this.nameSpace = nameSpace;
		}

		public void BuildCode(StringWriter writer)
		{
			CreateCompileUnit();
			CreateNamespace();

			foreach (var builder in builders)
			{
				builder.Configure(configuration, contentTypes);
				builder.Build(ns);
			}

			compileUnit.Namespaces.Add(ns);
			WriteCode(writer, compileUnit);
			writer.Flush();
		}

		private void CreateCompileUnit()
		{
			compileUnit = new CodeCompileUnit();
		}

		private void CreateNamespace()
		{
			ns = new CodeNamespace(nameSpace);
		}


		private void WriteCode(StringWriter writer, CodeCompileUnit compileUnit)
		{
			var options = new CodeGeneratorOptions
			{
				BlankLinesBetweenMembers = false,
				BracingStyle = "C"
			};
			codeDomProvider.GenerateCodeFromCompileUnit(compileUnit, writer, options);
		}
	}
}

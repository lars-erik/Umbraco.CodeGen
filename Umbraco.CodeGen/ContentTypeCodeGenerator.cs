using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace Umbraco.CodeGen
{
	public class ContentTypeCodeGenerator
	{
		private readonly CodeDomProvider codeDomProvider;
		private readonly ContentTypeConfiguration configuration;
		private readonly ContentTypeBuilder contentTypeBuilder;

		private CodeCompileUnit compileUnit;
		private CodeNamespace ns;

		public ContentTypeCodeGenerator(
			ContentTypeConfiguration configuration, 
			XDocument contentTypeDefintion,
			CodeDomProvider codeDomProvider
			)
		{
			this.configuration = configuration;
			this.codeDomProvider = codeDomProvider;

			contentTypeBuilder = new ContentTypeBuilder(configuration, contentTypeDefintion);
		}

		public void BuildCode(TextWriter writer)
		{
			CreateCompileUnit();
			CreateNamespace();

			AddUsingStatements();
			contentTypeBuilder.Build(ns);

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
			ns = new CodeNamespace(configuration.Namespace);
		}

		private void AddUsingStatements()
		{
			ns.Imports.Add(new CodeNamespaceImport("System"));
			ns.Imports.Add(new CodeNamespaceImport("System.ComponentModel"));
			ns.Imports.Add(new CodeNamespaceImport("System.ComponentModel.DataAnnotations"));
			ns.Imports.Add(new CodeNamespaceImport("Umbraco.Core.Models"));
			ns.Imports.Add(new CodeNamespaceImport("Umbraco.Web"));
		}

		private void WriteCode(TextWriter writer, CodeCompileUnit compileUnit)
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

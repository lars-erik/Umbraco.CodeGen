using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ICSharpCode.NRefactory.CSharp;
using Umbraco.CodeGen.Configuration;
using Umbraco.CodeGen.Definitions;
using Umbraco.CodeGen.Parsers;

namespace Umbraco.CodeGen
{
	public class CodeParser
	{
		private readonly ContentTypeConfiguration configuration;
		private readonly IEnumerable<DataTypeDefinition> dataTypes;
	    private readonly DefaultParserFactory parserFactory;
	    private readonly CSharpParser parser = new CSharpParser();

		public CodeParser(
            ContentTypeConfiguration configuration, 
            IEnumerable<DataTypeDefinition> dataTypes,
            DefaultParserFactory parserFactory
        )
		{
			this.configuration = configuration;
			this.dataTypes = dataTypes;
		    this.parserFactory = parserFactory;
		}

		public IEnumerable<ContentType> Parse(TextReader reader)
		{
			var tree = parser.Parse(reader);
			ValidateTree(tree);
			return FindTypes(tree).Select(Generate);
		}

		private static void ValidateTree(SyntaxTree tree)
		{
			if (tree.Errors.Count > 0)
				throw new AnalysisException(tree);
		}

		private static IEnumerable<TypeDeclaration> FindTypes(AstNode tree)
		{
			return tree.Descendants.OfType<TypeDeclaration>();
		}

		private ContentType Generate(TypeDeclaration type)
		{
		    var typedParser = parserFactory.Create(configuration, dataTypes);
		    var contentType = typedParser.Parse(type);
		    return contentType;
		}
	}

	public class AnalysisException : Exception
	{
		public IEnumerable<string> Errors { get; private set; }

		public AnalysisException(SyntaxTree tree)
			: base("Errors in code analysis. See the Errors collection for details.")
		{
			Errors = tree.Errors.Select(e => e.Region.BeginLine + ": " + e.Message);
		}
	}
}
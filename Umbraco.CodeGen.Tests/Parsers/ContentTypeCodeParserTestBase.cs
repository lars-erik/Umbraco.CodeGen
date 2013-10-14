using System.Linq;
using ICSharpCode.NRefactory.CSharp;
using NUnit.Framework;
using Umbraco.CodeGen.Configuration;
using Umbraco.CodeGen.Definitions;
using Umbraco.CodeGen.Parsers;

namespace Umbraco.CodeGen.Tests.Parsers
{
    public class ContentTypeCodeParserTestBase
    {
        protected const string EmptyClass = @"public class AClass {}";
        protected ContentTypeConfiguration Configuration;
        protected ContentTypeCodeParserBase Parser;
        protected ContentType ContentType;
        protected Info Info;

        protected void Parse(string code)
        {
            var type = ParseType(code);
            Parser.Parse(type, ContentType);
        }

        protected static TypeDeclaration ParseType(string code)
        {
            var cSharpParser = new CSharpParser();
            var tree = cSharpParser.Parse(code);
            Assert.AreEqual(0, tree.Errors.Count, tree.Errors.Aggregate("", (s, e) => s + e.Message + "\r\n"));
            var type = tree.GetTypes().Single();
            return (TypeDeclaration)type;
        }

        protected object PropertyValue(string memberName)
        {
            return Info.GetType().GetProperty(memberName).GetValue(Info, null);
        }
    }
}
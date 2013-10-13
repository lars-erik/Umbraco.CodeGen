using System.CodeDom;
using System.CodeDom.Compiler;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.CSharp;
using NUnit.Framework;
using Umbraco.CodeGen.Definitions;
using Umbraco.CodeGen.Generators;

namespace Umbraco.CodeGen.Tests.Generators
{
    [TestFixture]
    public class PropertyBodyGeneratorTest
    {
        [Test]
        public void Generate_Body_GetsContentPropertyValueOfType()
        {
            var property = new GenericProperty {Alias = "aProperty"};
            var propNode = new CodeMemberProperty {Type = new CodeTypeReference("String")};
            var generator = new PropertyBodyGenerator(new ContentTypeConfiguration(null));
            
            generator.Generate(propNode, property);

            var ns = CodeGenerationHelper.CreateNamespaceWithTypeAndProperty(propNode);
            var builder = CodeGenerationHelper.GenerateCode(ns);

            var code = builder.ToString();
            var returnIndex = code.IndexOf("return");
            var endIndex = code.IndexOf(";", returnIndex);
            var body = code.Substring(returnIndex, endIndex - returnIndex + 1);

            Assert.AreEqual(
                "return Content.GetPropertyValue<String>(\"aProperty\");",
                body
                );
        }
    }
}

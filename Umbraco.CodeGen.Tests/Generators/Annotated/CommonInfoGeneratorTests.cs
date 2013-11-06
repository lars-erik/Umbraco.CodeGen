using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Umbraco.CodeGen.Configuration;
using Umbraco.CodeGen.Definitions;
using Umbraco.CodeGen.Generators.Annotated;

namespace Umbraco.CodeGen.Tests.Generators.Annotated
{
    [TestFixture]
    public class CommonInfoGeneratorTests : CodeGeneratorTestBase
    {
        private Info info;
        private ContentType contentType;
        private CodeTypeDeclaration type;
        private CodeAttributeDeclaration attribute;

        [SetUp]
        public void SetUp()
        {
            Configuration = new CodeGeneratorConfiguration().MediaTypes;
            contentType = new MediaType { Info = { Alias = "anEntity" } };
            Candidate = type = new CodeTypeDeclaration();
            info = contentType.Info;

            attribute = new CodeAttributeDeclaration("MediaType");
            type.CustomAttributes.Add(attribute);

            Generator = new CommonInfoGenerator(Configuration);
        }

        [Test]
        [ExpectedException(typeof(Exception), ExpectedMessage = "Common info generator must be used on an attribute declaration")]
        public void Generate_NotCodeTypeDeclaration_Throws()
        {
            Generator.Generate(new CodeMemberProperty(), contentType);;
        }

        [Test]
        public void Generate_Icon_WhenHasValue_IsAttributeArgumentWithValue()
        {
            info.Icon = "icon.gif";
            Generate();
            var attribute = Type.CustomAttributes.Cast<CodeAttributeDeclaration>().Single();
            var argument = attribute.Arguments.Cast<CodeAttributeArgument>().Single(arg => arg.Name == "Icon");
            var argValue = ((CodePrimitiveExpression) argument.Value).Value;
            Assert.AreEqual(info.Icon, argValue);
        }

        [Test]
        public void Generate_Icon_NoValue_OmitsIconArgument()
        {
            Generate();
            var attribute = Type.CustomAttributes.Cast<CodeAttributeDeclaration>().Single();
            Assert.That(attribute.Arguments.Cast<CodeAttributeArgument>().All(arg => arg.Name != "Icon"));
        }

        private void Generate()
        {
            Generator.Generate(Type, ContentType);
        }
    }
}
